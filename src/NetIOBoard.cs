using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace VisionDemo
{
    /// <summary>
    /// 网口开关量采集/继电器控制板
    /// 4路开关量输入 X0-X3 + 4路继电器输出 Y0-Y3
    /// 通讯: 标准 Modbus-RTU over TCP (RTU帧带CRC16, 低字节在前)
    ///   出厂: IP 192.168.1.30 端口23 TCP Server模式, 站号=01 (上位机做TCP Client)
    ///   功能码: 02读输入开关量 / 01读输出线圈 / 05写单个线圈 / 0F写多线圈
    ///   读输入X: 发 01 02 00 00 00 08 CRC → 收 01 02 01 XX CRC (XX bit0=X0...bit7=X7)
    ///   写线圈Y: 发 01 05 00 0n FF 00 CRC(吸合) / 01 05 00 0n 00 00 CRC(断开) → 回显同帧
    /// 参考: 《网口IO板产品说明书》
    /// 4入4出, 后台线程每50ms轮询读输入, 检测上升沿触发InputChanged事件
    /// </summary>
    public class NetIOBoard : IIOBoard
    {
        //---- 协议常量(Modbus-RTU over TCP) ----
        private const byte SLAVE = 0x01;            //设备站号(出厂1)
        private const byte FN_READ_OUT = 0x01;      //读输出线圈
        private const byte FN_READ_IN = 0x02;       //读输入开关量
        private const byte FN_WRITE_COIL = 0x05;    //写单个线圈
        private const ushort START_ADDR = 0x0000;   //X/Y起始地址
        private const ushort POINT_COUNT = 0x0008;  //读8位(X0-X7/Y0-Y7)

        //---- TCP ----
        private readonly string host;
        private readonly int port;
        private TcpClient client;
        private NetworkStream stream;
        private readonly object tcpLock = new object();   //TCP事务锁(请求-响应严格串行)

        //---- 状态 ----
        private readonly object stateLock = new object();
        private bool[] inputs = new bool[4];
        private bool[] outputs = new bool[4];
        private bool connected;

        //---- 轮询 ----
        private Thread pollThread;
        private volatile bool running;

        //输入变化事件(上升沿/下降沿均触发)
        public event Action<int, bool> InputChanged;
        //输出变化事件
        public event Action<int, bool> OutputChanged;
        //连接状态变化事件(true=已连接)
        public event Action<bool> ConnectedChanged;
        //帧收发日志事件(测试页面/调试用): 参数为 TX:/RX: 前缀的HEX帧文本, 主程序不订阅则零开销
        public event Action<string> FrameLog;

        public bool IsConnected
        {
            get { lock (stateLock) { return connected; } }
        }
        public string Host { get { return host; } }
        public int Port { get { return port; } }

        /// <summary>默认参数对应说明书出厂配置: 192.168.1.30:23</summary>
        public NetIOBoard(string host = "192.168.1.30", int port = 23)
        {
            this.host = string.IsNullOrEmpty(host) ? "192.168.1.30" : host.Trim();
            this.port = port <= 0 ? 23 : port;
        }

        /// <summary>启动后台输入轮询(断线自动重连)</summary>
        public void Start()
        {
            if (running) return;
            running = true;
            pollThread = new Thread(PollLoop);
            pollThread.IsBackground = true;
            pollThread.Start();
        }

        /// <summary>停止轮询并关闭连接</summary>
        public void Stop()
        {
            running = false;
            CloseConnection();
        }

        //---- IIOBoard 接口实现 ----

        public bool GetInput(int index)
        {
            if (index < 1 || index > 4) return false;
            lock (stateLock) { return inputs[index - 1]; }
        }

        public bool GetOutput(int index)
        {
            if (index < 1 || index > 4) return false;
            lock (stateLock) { return outputs[index - 1]; }
        }

        //写入输出: 功能码05写单线圈(Y0-Y3), 设备回显同帧即写入成功
        public void SetOutput(int index, bool value)
        {
            if (index < 1 || index > 4) return;
            bool changed;
            lock (stateLock)
            {
                changed = outputs[index - 1] != value;
                outputs[index - 1] = value;
            }
            bool ok = WriteCoil(index - 1, value);
            //写入成功且状态变化时才通知UI(灯与硬件一致); 失败保持原状态, 由日志提示
            if (ok && changed)
            {
                Action<int, bool> h = OutputChanged;
                if (h != null) h(index, value);
            }
        }

        //批量设置输出(用于检测结果: ok=true亮绿灯, ok=false亮红灯+蜂鸣器)
        //三色灯排列: 红黄绿(混合三色灯, 同一时间只亮1个灯), 蜂鸣器与红灯同时响
        //输出映射: OUT1=红灯(NG) OUT2=黄灯(不用) OUT3=绿灯(OK) OUT4=蜂鸣器(NG)
        public void SetResultOutput(bool ok)
        {
            SetOutput(1, !ok);     //OUT1 红灯: NG亮
            SetOutput(2, false);   //OUT2 黄灯: 不用
            SetOutput(3, ok);      //OUT3 绿灯: OK亮
            SetOutput(4, !ok);     //OUT4 蜂鸣器: NG响(与红灯同时)
        }

        //运行状态指示(混合三色灯同时只亮1个灯, 检测中不亮灯, 无操作)
        public void SetRunningOutput(bool running)
        {
            //混合三色灯: 检测中不亮黄灯, 仅检测结果亮绿/红
        }

        //全部输出复位
        public void ResetOutputs()
        {
            for (int i = 1; i <= 4; i++) SetOutput(i, false);
        }

        //调试命令(与虚拟卡ASCII协议兼容, 仅支持查询; SET:OUT写硬件)
        public string ExecuteCommand(string command)
        {
            string cmd = (command ?? "").Trim().ToUpperInvariant();
            if (cmd == "READ:IN")
            {
                lock (stateLock)
                {
                    return "IN:" + (inputs[0] ? "1" : "0") + (inputs[1] ? "1" : "0") +
                           (inputs[2] ? "1" : "0") + (inputs[3] ? "1" : "0");
                }
            }
            if (cmd == "READ:OUT")
            {
                lock (stateLock)
                {
                    return "OUT:" + (outputs[0] ? "1" : "0") + (outputs[1] ? "1" : "0") +
                           (outputs[2] ? "1" : "0") + (outputs[3] ? "1" : "0");
                }
            }
            if (cmd.StartsWith("SET:OUT:"))
            {
                string[] parts = cmd.Split(':');
                int n;
                if (parts.Length >= 4 && int.TryParse(parts[2], out n) &&
                    (parts[3] == "1" || parts[3] == "0"))
                {
                    if (n >= 1 && n <= 4)
                    {
                        SetOutput(n, parts[3] == "1");
                        return "OK";
                    }
                }
                return "ERR";
            }
            return "ERR";
        }

        //---- 内部: 输入轮询 ----
        private void PollLoop()
        {
            while (running)
            {
                try
                {
                    byte[] resp = Transact(BuildFrame(SLAVE, FN_READ_IN, START_ADDR, POINT_COUNT), 6);
                    if (resp != null && resp.Length >= 6 && resp[0] == SLAVE &&
                        resp[1] == FN_READ_IN && (resp[1] & 0x80) == 0)
                    {
                        SetConnected(true);
                        byte st = resp[3];
                        for (int i = 0; i < 4; i++)
                        {
                            bool v = (st & (1 << i)) != 0;
                            bool prev;
                            lock (stateLock) { prev = inputs[i]; inputs[i] = v; }
                            if (prev != v)
                            {
                                Action<int, bool> h = InputChanged;
                                if (h != null) h(i + 1, v);
                            }
                        }
                    }
                    else
                    {
                        SetConnected(false);
                    }
                }
                catch
                {
                    SetConnected(false);
                }
                Thread.Sleep(50);   //50ms轮询, 满足IN1触发响应需求
            }
        }

        //---- 内部: 单线圈写入(功能码05) ----
        private bool WriteCoil(int coilIndex, bool on)
        {
            byte[] frame = BuildFrame(SLAVE, FN_WRITE_COIL, (ushort)coilIndex, on ? (ushort)0xFF00 : (ushort)0x0000);
            byte[] resp = Transact(frame, 8);
            return resp != null && resp.Length >= 8 && resp[0] == SLAVE &&
                   resp[1] == FN_WRITE_COIL && (resp[1] & 0x80) == 0;
        }

        //---- 内部: Modbus-RTU帧构造(站号+功能码+地址+数据+CRC16低字节在前) ----
        private static byte[] BuildFrame(byte slave, byte fn, ushort addr, ushort value)
        {
            byte[] data = new byte[6];
            data[0] = slave;
            data[1] = fn;
            data[2] = (byte)(addr >> 8);
            data[3] = (byte)(addr & 0xFF);
            data[4] = (byte)(value >> 8);
            data[5] = (byte)(value & 0xFF);
            ushort crc = Crc16(data, 6);
            byte[] frame = new byte[8];
            Buffer.BlockCopy(data, 0, frame, 0, 6);
            frame[6] = (byte)(crc & 0xFF);
            frame[7] = (byte)(crc >> 8);
            return frame;
        }

        //---- 内部: Modbus CRC16 (poly 0xA001, init 0xFFFF) ----
        private static ushort Crc16(byte[] buf, int len)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < len; i++)
            {
                crc ^= buf[i];
                for (int j = 0; j < 8; j++)
                {
                    crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                }
            }
            return crc;
        }

        //---- 内部: 事务(锁内发送请求→读取完整响应, 失败返回null) ----
        private byte[] Transact(byte[] request, int respLen)
        {
            lock (tcpLock)
            {
                try
                {
                    if (!EnsureConnectedLocked()) return null;
                    stream.Write(request, 0, request.Length);
                    stream.Flush();
                    LogFrame("TX: " + ToHex(request));
                    byte[] resp = new byte[respLen];
                    int got = 0;
                    while (got < respLen)
                    {
                        int n = stream.Read(resp, got, respLen - got);
                        if (n <= 0)
                        {
                            CloseConnection();
                            return null;
                        }
                        got += n;
                    }
                    LogFrame("RX: " + ToHex(resp));
                    return resp;
                }
                catch
                {
                    CloseConnection();
                    return null;
                }
            }
        }

        //---- 内部: 帧日志(HEX, 空格分隔) ----
        private void LogFrame(string text)
        {
            Action<string> h = FrameLog;
            if (h != null) h(text);
        }

        //---- 内部: 字节数组转HEX文本(空格分隔) ----
        private static string ToHex(byte[] buf)
        {
            if (buf == null || buf.Length == 0) return "";
            StringBuilder sb = new StringBuilder(buf.Length * 3);
            for (int i = 0; i < buf.Length; i++)
            {
                if (i > 0) sb.Append(' ');
                sb.Append(buf[i].ToString("X2"));
            }
            return sb.ToString();
        }

        //---- 内部: 确保TCP已连接(调用方须持有tcpLock) ----
        private bool EnsureConnectedLocked()
        {
            if (client != null && client.Connected) return true;
            CloseConnection();
            try
            {
                client = new TcpClient();
                client.NoDelay = true;
                client.ReceiveTimeout = 500;
                client.SendTimeout = 500;
                IAsyncResult ar = client.BeginConnect(host, port, null, null);
                if (!ar.AsyncWaitHandle.WaitOne(1000))
                {
                    CloseConnection();
                    return false;
                }
                client.EndConnect(ar);
                stream = client.GetStream();
                if (stream != null) stream.ReadTimeout = 500;
                SetConnected(true);
                return true;
            }
            catch
            {
                CloseConnection();
                return false;
            }
        }

        //---- 内部: 关闭连接(可重入) ----
        private void CloseConnection()
        {
            try { if (stream != null) stream.Close(); } catch { }
            try { if (client != null) client.Close(); } catch { }
            stream = null;
            client = null;
            SetConnected(false);
        }

        //---- 内部: 连接状态变化(防抖, 仅变化时触发事件) ----
        private void SetConnected(bool value)
        {
            bool changed;
            lock (stateLock) { changed = connected != value; connected = value; }
            if (changed)
            {
                Action<bool> h = ConnectedChanged;
                if (h != null) h(value);
            }
        }
    }
}
