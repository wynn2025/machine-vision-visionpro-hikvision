using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisionDemo
{
    /// <summary>
    /// 网口IO卡(网口IO板)通讯测试页面: 独立连接(不影响主程序ioBoard)
    /// 功能: 连接/断开(IP端口可编辑) + 输入实时状态 + 输出手动开关测试 + 收发帧日志
    /// 协议: Modbus-RTU over TCP, 见 NetIOBoard.cs 与《网口IO板产品说明书》
    /// </summary>
    public partial class IOBoardTestForm : Form
    {
        //测试用独立IO板实例(与主程序ioBoard互不干扰)
        private NetIOBoard board;
        //输入ON时按钮底色
        private static readonly Color InOnColor = Color.LimeGreen;
        private static readonly Color InOffColor = Color.FromArgb(64, 64, 64);
        //输出ON时按钮底色
        private static readonly Color OutOnColor = Color.FromArgb(40, 120, 60);
        private static readonly Color OutOffColor = Color.FromArgb(69, 90, 100);

        public IOBoardTestForm()
        {
            InitializeComponent();
            //默认连接参数从sys.ini的[IO]节读取(与主程序一致), 不存在时用出厂默认192.168.1.30:23
            String inifile = Application.StartupPath + "\\data\\sys.ini";
            string ip = IniFile.ReadString(inifile, "IO", "IP", "192.168.1.30");
            int port = IniFile.ReadInt(inifile, "IO", "Port", 23);
            txt_IP.Text = ip;
            txt_Port.Text = port.ToString();
            //定时刷新输入/输出状态(IN为状态指示不可点击, 连接后每200ms刷新真实输入电平)
            timer_Refresh.Start();
            AppendLog("测试页面就绪, 默认参数来自 sys.ini [IO]: " + ip + ":" + port);
        }

        //---- 连接 ----
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            if (board != null) return;   //已连接
            string ip = txt_IP.Text.Trim();
            int port;
            if (!int.TryParse(txt_Port.Text.Trim(), out port) || port <= 0 || port > 65535)
            {
                AppendLog("端口无效: " + txt_Port.Text);
                MessageBox.Show("端口无效, 请输入1-65535", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                //创建独立连接并启动输入轮询
                board = new NetIOBoard(ip, port);
                board.FrameLog += Board_FrameLog;
                board.InputChanged += Board_InputChanged;
                board.OutputChanged += Board_OutputChanged;
                board.ConnectedChanged += Board_ConnectedChanged;
                board.Start();
                SetConnUi(true);
                AppendLog("正在连接 " + ip + ":" + port + " ...");
            }
            catch (Exception ex)
            {
                AppendLog("连接失败: " + ex.Message);
                board = null;
            }
        }

        //---- 断开 ----
        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            if (board == null) return;
            board.Stop();
            board.FrameLog -= Board_FrameLog;
            board.InputChanged -= Board_InputChanged;
            board.OutputChanged -= Board_OutputChanged;
            board.ConnectedChanged -= Board_ConnectedChanged;
            board = null;
            SetConnUi(false);
            AppendLog("已断开连接");
        }

        //---- 输出开关测试 ----
        private void btn_OUT_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || board == null)
            {
                AppendLog("未连接, 无法控制输出");
                return;
            }
            int index = int.Parse(btn.Name.Replace("btn_OUT", ""));
            bool next = !board.GetOutput(index);
            AppendLog("手动控制 OUT" + index + " -> " + (next ? "ON" : "OFF"));
            //网络I/O放后台线程, 避免UI线程阻塞卡死
            Task.Run(delegate
            {
                board.SetOutput(index, next);
            });
        }

        //---- 全部输出复位 ----
        private void btn_ResetAll_Click(object sender, EventArgs e)
        {
            if (board == null)
            {
                AppendLog("未连接, 无法复位输出");
                return;
            }
            AppendLog("全部输出已复位 (OUT1-4 OFF)");
            //网络I/O放后台线程, 避免UI线程阻塞卡死
            Task.Run(delegate
            {
                board.ResetOutputs();
            });
        }

        //---- 定时刷新输入/输出显示 ----
        private void timer_Refresh_Tick(object sender, EventArgs e)
        {
            if (board == null) return;
            //输入状态
            for (int i = 1; i <= 4; i++) UpdateInButton(i, board.GetInput(i));
            //输出状态
            for (int i = 1; i <= 4; i++) UpdateOutButton(i, board.GetOutput(i));
            //连接状态
            bool conn = board.IsConnected;
            light_State.Text = conn ? "已连接" : "未连接";
            light_State.ForeColor = conn ? Color.LimeGreen : Color.OrangeRed;
            btn_Disconnect.Enabled = conn;
        }

        //---- 输入状态按钮显示(只读指示, 不可点击) ----
        private void UpdateInButton(int index, bool value)
        {
            Button btn = GetInButton(index);
            if (btn == null) return;
            btn.Text = "IN" + index + (value ? " ON" : " OFF");
            btn.BackColor = value ? InOnColor : InOffColor;
        }

        //---- 输出状态按钮显示 ----
        private void UpdateOutButton(int index, bool value)
        {
            Button btn = GetOutButton(index);
            if (btn == null) return;
            btn.Text = "OUT" + index + (value ? " ON" : " OFF");
            btn.BackColor = value ? OutOnColor : OutOffColor;
        }

        private Button GetInButton(int index)
        {
            switch (index)
            {
                case 1: return btn_IN1;
                case 2: return btn_IN2;
                case 3: return btn_IN3;
                case 4: return btn_IN4;
                default: return null;
            }
        }

        private Button GetOutButton(int index)
        {
            switch (index)
            {
                case 1: return btn_OUT1;
                case 2: return btn_OUT2;
                case 3: return btn_OUT3;
                case 4: return btn_OUT4;
                default: return null;
            }
        }

        //---- 连接/断开按钮可用性切换 ----
        private void SetConnUi(bool connecting)
        {
            btn_Connect.Enabled = !connecting;
            btn_Disconnect.Enabled = connecting;
            txt_IP.Enabled = !connecting;
            txt_Port.Enabled = !connecting;
        }

        //---- 事件(轮询线程触发, 需Invoke到UI线程) ----

        //帧收发日志
        private void Board_FrameLog(string text)
        {
            if (this.IsDisposed) return;
            try
            {
                this.BeginInvoke(new Action(delegate { AppendLog(text); }));
            }
            catch { }
        }

        //输入变化
        private void Board_InputChanged(int index, bool value)
        {
            if (this.IsDisposed) return;
            try
            {
                this.BeginInvoke(new Action(delegate
                {
                    UpdateInButton(index, value);
                    AppendLog("输入变化: IN" + index + "=" + (value ? "1(ON)" : "0(OFF)"));
                }));
            }
            catch { }
        }

        //输出变化
        private void Board_OutputChanged(int index, bool value)
        {
            if (this.IsDisposed) return;
            try
            {
                this.BeginInvoke(new Action(delegate
                {
                    UpdateOutButton(index, value);
                    AppendLog("输出变化: OUT" + index + "=" + (value ? "1(ON)" : "0(OFF)"));
                }));
            }
            catch { }
        }

        //连接状态变化
        private void Board_ConnectedChanged(bool connected)
        {
            if (this.IsDisposed) return;
            try
            {
                this.BeginInvoke(new Action(delegate
                {
                    light_State.Text = connected ? "已连接" : "未连接";
                    light_State.ForeColor = connected ? Color.LimeGreen : Color.OrangeRed;
                    AppendLog(connected ? "连接成功 (通讯正常)" : "连接断开, 自动重连中...");
                }));
            }
            catch { }
        }

        //---- 日志 ----
        private void AppendLog(string msg)
        {
            if (richTextBox_log.Lines.LongLength > 200)
            {
                richTextBox_log.Clear();
            }
            richTextBox_log.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + msg + "\n");
            richTextBox_log.ScrollToCaret();
        }

        //关闭时释放独立连接(不影响主程序ioBoard)
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (board != null)
            {
                board.Stop();
                board = null;
            }
            base.OnFormClosing(e);
        }
    }
}
