using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Cognex.VisionPro;
using System.Collections;
using Cognex.VisionPro.ToolBlock;
using System.Runtime.InteropServices;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.Implementation;
using System.IO;
using Cognex.VisionPro.ImageFile;
using System.Threading.Tasks;
using Cognex.VisionPro.PMAlign;
using MvCameraControl;

namespace VisionDemo
{
    public partial class FormMain : Form
    {
        //组件------------------------------------------------------------------------------------------
        private MvNetCamera cam;              //迈德威视GigE相机采集(MV-CS200-10GM, IP直连192.168.1.100, MvCameraControl.Net SDK)
        private CogToolBlock tool;             //康耐视VisionPro检测工具(ToolBlock, 内含L/R两个PMAlign检测)
        private CogRecordDisplay cogDisp;         //图像显示(单显示)
        private FormSplash spl;                   //启动窗口
        private IIOBoard ioBoard;                 //IO卡(4入4出): 网口IO卡(真实通讯)
        private LabelPrinter labelPrinter;        //标签打印(OK时打印一张, 配置见sys.ini [打印]节)
        //常量------------------------------------------------------------------------------------------
        private const bool debug = false;         //真机模式: 接相机+真实IO卡(模拟模式改true)
        //检测状态(严格单次触发: 触发一次→检测一次→完成等待)
        private readonly object inspectLock = new object();   //检测锁(防并发重入)
        private bool inspecting = false;          //是否正在检测(防重入)
        private DateTime lastTriggerTime = DateTime.MinValue;  //上次触发时间(防抖: IO卡电平抖动连续触发)
        //本地变量---------------------------------------------------------------------------------------
        private bool 拍照OK = false;              //拍照OK
        private bool 拍照NG = false;              //拍照NG
        //统计信息
        private uint total = 0;
        private uint pass = 0;
        private double inspecttime;
        CogStopwatch RunTimes;
        //启动停止
        private bool stop = false;
        //日志显示
        private delegate void AddLogListboxDlg(string msg, bool isornot);
        private AddLogListboxDlg dlgAddLog;
        //调试用
        private int debugImageIndex = 0;
        public FormMain()
        {
            InitializeComponent();
            this.spl = new FormSplash();
        }
        //窗体加载
        private void FormMain_Load(object sender, EventArgs e)
        {
            base.Hide();
            Thread Thr_splash = new Thread(new ThreadStart(this.ThreadSplash));
            Thr_splash.Start();
            //1 初始化
            this.setText("启动中");
            this.setVal(20);
            tool = null;
            cogDisp = cogRecordDisplay1;
            this.dlgAddLog = new FormMain.AddLogListboxDlg(this.AddLog);
            //打开迈德威视GigE相机(MV-CS200-10GM, IP直连, 程序采集图像喂给vpp内L/R PMAlign)
            this.setText("打开相机");
            this.setVal(30);
            if (!Debug)
            {
                //相机IP/电脑网卡IP从data\sys.ini的[相机]节读取(默认相机192.168.1.100, 电脑192.168.1.20)
                String camfile = Application.StartupPath + "\\data\\sys.ini";
                string camIp = IniFile.ReadString(camfile, "相机", "相机IP", "192.168.1.100");
                string netIp = IniFile.ReadString(camfile, "相机", "电脑IP", "192.168.1.20");
                long exposure = IniFile.ReadInt(camfile, "相机", "曝光", 70000);   //曝光时间微秒, 从参数加载默认70000
                cam = new MvNetCamera(camIp, netIp, exposure);
                int camCount = cam.ScanCameras();
                SaveLogFile("启动步骤: 扫描到相机 " + camCount + " 台, 曝光=" + exposure + "us");
                if (camCount >= 1 && cam.OpenCameraByIp())
                {
                    lb相机.BackColor = Color.Green;
                    SaveLogFile("启动步骤: 相机打开成功 (" + camIp + " 直连)");
                }
                else
                {
                    MessageBox.Show("相机连接失败!\r\n已自动重试2分钟仍未连上(" + camIp + ")。\r\n可能原因: 相机刚上电/网线松动/上一次程序异常退出未释放相机。\r\n请检查相机IP(" + camIp + ")与网卡IP(" + netIp + ")配置, 或重启相机电源后重试。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
            //加载检测工具(VisionPro ToolBlock)
            this.setText("加载工具");
            this.setVal(50);
            SaveLogFile("启动步骤: 开始加载检测工具");
            vppLoading();
            SaveLogFile("启动步骤: 工具加载已启动");

            //初始化IO卡(4入4出: IN1触发检测 IN2计数复位, OUT1绿/OUT2红/OUT3黄/OUT4蜂鸣器)
            //使用网口IO卡(Modbus-RTU over TCP, 默认192.168.1.30:23, 可在sys.ini的[IO]节配置)
            String iofile = Application.StartupPath + "\\data\\sys.ini";
            string ioIp = IniFile.ReadString(iofile, "IO", "IP", "192.168.1.30");
            int ioPort = IniFile.ReadInt(iofile, "IO", "Port", 23);
            NetIOBoard net = new NetIOBoard(ioIp, ioPort);
            net.ConnectedChanged += ioBoard_ConnectedChanged;
            net.Start();
            ioBoard = net;
            ioBoard.InputChanged += ioBoard_InputChanged;
            ioBoard.OutputChanged += ioBoard_OutputChanged_UI;
            SaveLogFile("启动步骤: IO卡初始化完成");
            //初始化标签打印(读sys.ini [打印]节: PrinterPath/PrinterName/色号/序号)
            labelPrinter = new LabelPrinter(Application.StartupPath + "\\data\\sys.ini");
            SaveLogFile("启动步骤: 标签打印初始化完成 (打印机:" + labelPrinter.PrinterName + ", 色号:" + labelPrinter.ColorCode + ", 序号:" + labelPrinter.CurrentSeq + ")");
            //显示统计信息
            String inifile = Application.StartupPath + "\\data\\sys.ini";
            total = (uint)IniFile.ReadInt(inifile, "统计", "总数", 0);
            pass = (uint)IniFile.ReadInt(inifile, "统计", "合格数量", 0);
            this.ShowStatistics(total, pass, Convert.ToDouble(0));
            //不启动检测线程: 严格单次触发(按一下检测一下)
            SaveLogFile("启动步骤: 初始化完成, 等待触发(开始按钮/IO IN1/拍照按钮)");
            this.setText("加载完成");
            this.setVal(100);

            //完成启动: 关闭启动画面(必须在启动画面线程自身执行Close, 否则跨线程InvalidOperationException)
            CloseSplash();
            //等待启动画面线程自然结束(超时3秒,不Abort避免ThreadAbortException)
            if (Thr_splash.IsAlive)
            {
                Thr_splash.Join(3000);
            }
            base.Show();
            SaveLogFile("启动步骤: 界面已显示");
            //不自动检测: 由用户按"开始"按钮或IO卡IN1触发
            SaveLogFile("启动步骤: 等待触发(开始按钮/IO IN1/拍照按钮)");
        }
        //显示启动条
        private void ThreadSplash()
        {
            this.spl.ShowDialog();
        }
        //关闭启动画面(线程安全): 若在非创建线程调用, 自动切到启动画面线程执行Close
        private void CloseSplash()
        {
            try
            {
                if (this.spl == null || this.spl.IsDisposed) return;
                if (this.spl.InvokeRequired)
                {
                    this.spl.BeginInvoke(new Action(CloseSplash));
                }
                else
                {
                    this.spl.Close();
                }
            }
            catch { }
        }
        //显示启动信息(非阻塞)
        private void setText(string nowText)
        {
            try
            {
                if (this.spl.labelInfo.InvokeRequired)
                {
                    this.spl.labelInfo.BeginInvoke(new Action<string>(delegate (string text)
                    {
                        this.spl.labelInfo.Text = text;
                    }), new object[]
                    {
                        nowText
                    });
                }
                else
                {
                    this.spl.labelInfo.Text = nowText;
                }
            }
            catch { }
        }
        //设置进度条百分比(非阻塞)
        private void setVal(int nowValue)
        {
            try
            {
                if (this.spl.progressBar1.InvokeRequired)
                {
                    this.spl.progressBar1.BeginInvoke(new Action<int>(delegate (int value)
                    {
                        this.spl.progressBar1.Value = value;
                    }), new object[]
                    {
                        nowValue
                    });
                }
                else
                {
                    this.spl.progressBar1.Value = nowValue;
                }
            }
            catch { }
        }
        //主任务
        /// <summary>
        /// 单次触发检测(按一下检测一下): 加锁防重入, 检测一次完成后等待下次触发
        /// 触发源: 开始按钮/IO卡IN1/拍照按钮按下
        /// </summary>
        private void TriggerInspect()
        {
            lock (inspectLock)
            {
                //防抖: 距上次触发不足1秒忽略(防止IO卡电平抖动/轮询误判连续触发)
                if ((DateTime.Now - lastTriggerTime).TotalMilliseconds < 1000) return;
                lastTriggerTime = DateTime.Now;
                if (inspecting) return;   //正在检测中, 忽略本次触发(防重入)
                inspecting = true;
            }
            try
            {
                RunTimes = new CogStopwatch();
                RunTimes.Start();
                //检测开始: 清空所有IO输出(混合三色灯+蜂鸣器全灭)
                if (ioBoard != null)
                {
                    ioBoard.ResetOutputs();
                }
                bool ok = CheckVpp();
                inspecttime = RunTimes.MicroSeconds / 1000;
                RunTimes.Stop();
                //统计
                total += 1u;
                if (ok) pass += 1u;
                base.Invoke(new Action(delegate
                {
                    this.ShowStatistics(total, pass, inspecttime);
                }));
                String inifile = Application.StartupPath + "\\data\\sys.ini";
                IniFile.WriteInt(inifile, "统计", "总数", (int)total);
                IniFile.WriteInt(inifile, "统计", "合格数量", (int)pass);
                //结果驱动IO: OK亮绿灯, NG亮红灯+蜂鸣器1秒
                if (ioBoard != null)
                {
                    ioBoard.SetRunningOutput(false);
                    ioBoard.SetResultOutput(ok);
                    if (!ok) AutoOffBuzzer(1000);   //蜂鸣器响1秒后自动关闭
                }
                if (ok)
                {
                    base.Invoke(this.dlgAddLog, new object[] { "检测完成 OK", true });
                    base.Invoke(new Action(delegate
                    {
                        btnResult.Text = "OK";
                        btnResult.BackColor = Color.Green;
                    }));
                    //OK打印标签(异步不阻塞检测, 序号自动+1)
                    if (labelPrinter != null)
                    {
                        string printedSeq = labelPrinter.CurrentSeq;
                        System.Threading.Tasks.Task.Run(delegate
                        {
                            bool pr = labelPrinter.PrintLabel();
                            base.Invoke(this.dlgAddLog, new object[]
                            {
                                pr ? "OK标签打印完成: " + labelPrinter.ColorCode + " 序号" + printedSeq : "OK标签打印失败, 见日志",
                                true
                            });
                        });
                    }
                }
                else
                {
                    base.Invoke(this.dlgAddLog, new object[] { "检测完成 NG", true });
                    base.Invoke(new Action(delegate
                    {
                        btnResult.Text = "NG";
                        btnResult.BackColor = Color.Red;
                        SaveNgPic();
                    }));
                }
            }
            catch (Exception ex)
            {
                SaveLogFile("检测异常: " + ex.Message);
            }
            finally
            {
                lock (inspectLock) { inspecting = false; }
            }
        }
        //VPP工具加载(VisionPro ToolBlock, 单工具)
        private void vppLoading()
        {
            String vppfile = Application.StartupPath + "\\data\\0.vpp";
            try
            {
                if (File.Exists(vppfile))
                {
                    tool = (CogToolBlock)CogSerializer.LoadObjectFromFile(vppfile);
                    SaveLogFile("VisionPro工具加载成功: " + vppfile);
                    //记录工具链结构(确认相机采集工具CogAcqFifoTool存在)
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("工具链工具(" + tool.Tools.Count + "个): ");
                        foreach (ICogTool t in tool.Tools)
                        {
                            sb.Append(t.Name + "[" + t.GetType().Name + "] ");
                        }
                        SaveLogFile(sb.ToString());
                    }
                    catch { }
                }
                else
                {
                    //无vpp文件时置空, 检测时提示配置工具, 不阻塞启动
                    tool = null;
                    SaveLogFile("VisionPro工具文件未找到, 请用[工具设置->检测工具设置]创建并保存: " + vppfile);
                }
            }
            catch (Exception ex)
            {
                tool = null;
                SaveLogFile("VisionPro工具加载失败: " + ex.Message);
            }
        }
        //显示日志
        private void AddLog(string msg, bool show)
        {
            object o = new object();
            lock (o)
            {
                string str = string.Concat(new string[]
                {
                    "[",
                    DateTime.Now.ToString("HH:mm:ss"),
                    "]",
                    msg,
                    "\n"
                });
                SaveLogFile(str);
                if (show)
                {
                    if (this.rTB_log.Lines.LongLength > 27L)
                    {
                        this.rTB_log.Clear();
                    }
                    this.rTB_log.AppendText(str);
                    this.rTB_log.ScrollToCaret();
                }
            }
        }
        //保存日志到文件
        public void SaveLogFile(string strMessage)
        {
            string logPath = Environment.CurrentDirectory + "\\Log";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            string fullPath = logPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            FileStream fs = new FileStream(fullPath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine(strMessage);
            sw.Flush();
            fs.Close();
        }
        //VPP检测(单工具)
        public bool CheckVpp() {

            if (tool == null)
            {
                //VisionPro工具未配置(无vpp文件): 标红提示, 不抛异常
                ShowCameraInfo(cogDisp, CogColorConstants.Red, 0.0, 0.0, "检测工具未配置");
                return false;
            }
            if (!Debug)
            {
                //真机模式: 程序采集(迈德威视CS200-10GM)图像 → 赋给vpp内L/R两个PMAlign检测工具
                //(vpp内CogAcqFifoTool1采集取向失败, 不运行它, 只运行L/R PMAlign)
                if (cam == null)
                {
                    ShowCameraInfo(cogDisp, CogColorConstants.Red, 0.0, 0.0, "相机未打开");
                    return false;
                }
                bool grabOk = cam.Run();
                int pmCount = 0;
                foreach (ICogTool t in tool.Tools)
                {
                    CogPMAlignTool pm = t as CogPMAlignTool;
                    if (pm != null) { pm.InputImage = cam.OutputImageRgb24; pmCount++; }
                }
                //把图像同时写入vpp的CogImage输入引脚(用户在vpp中添加, L/R已接线到输入引脚), 新接线方式下检测同样生效
                FormToolblock.SetBlockInputImage(tool, cam.OutputImageRgb24);
                SaveLogFile("真机模式: 采集" + (grabOk ? "成功" : "失败") + ", 图像" + (cam.OutputImageRgb24 != null ? "有效(" + cam.OutputImageRgb24.Width + "x" + cam.OutputImageRgb24.Height + ")" : "null") + ", 已赋给" + pmCount + "个PMAlign工具(L/R)");
            }
            else
            {
                String imageath = Application.StartupPath + "\\Image\\CAM1\\";
                if (!Directory.Exists(imageath))
                {
                    ShowCameraInfo(cogDisp, CogColorConstants.Red, 0.0, 0.0, "图像目录不存在: " + imageath);
                    return false;
                }
                string[] files = Directory.GetFiles(imageath, "*.bmp", SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    ShowCameraInfo(cogDisp, CogColorConstants.Red, 0.0, 0.0, "图像目录无bmp文件");
                    return false;
                }
                int idx = debugImageIndex % files.Length;   //防止越界
                CogImageFile ImageFile = new CogImageFile();
                ImageFile.Open(files[idx], CogImageFileModeConstants.Read);
                //模拟模式: 把本地bmp赋给所有内部PMAlign检测工具(L/R)的InputImage(不走相机采集)
                int pmCount = 0;
                foreach (ICogTool t in tool.Tools)
                {
                    CogPMAlignTool pm = t as CogPMAlignTool;
                    if (pm != null) { pm.InputImage = ImageFile[0]; pmCount++; }
                }
                //把图像同时写入vpp的CogImage输入引脚(用户在vpp中添加, L/R已接线到输入引脚)
                FormToolblock.SetBlockInputImage(tool, ImageFile[0]);
                SaveLogFile("模拟模式: 已给" + pmCount + "个PMAlign工具(L/R)赋值本地图像");
                ImageFile.Close();
            }
            //执行检测:
            //新vpp(有CogImage输入引脚): 运行整个block链(Input引脚→CogImageConvertTool1→L/R), 与训练时链路一致, 转换工具参与
            //旧vpp(无输入引脚, 含CogAcqFifoTool1): 只单独运行L/R PMAlign(避开采集工具, 图像已由程序采集喂入)
            //L/R任一NG则整体NG
            bool hasInputPin = false;
            try { foreach (dynamic term in tool.Inputs) { hasInputPin = true; break; } } catch { }
            if (hasInputPin)
            {
                try
                {
                    tool.Run();
                    SaveLogFile("检测: 按block完整链路运行(输入引脚→转换→L/R)");
                }
                catch (Exception ex)
                {
                    SaveLogFile("检测: block链路运行异常(" + ex.Message + "), 回退单独运行PMAlign");
                    hasInputPin = false;
                }
            }
            bool lOk = false, rOk = false;
            int lMatch = 0, rMatch = 0;
            double lScore = 0, rScore = 0;
            foreach (ICogTool t in tool.Tools)
            {
                CogPMAlignTool pm = t as CogPMAlignTool;
                if (pm == null) continue;
                if (!hasInputPin) pm.Run();   //旧vpp: 单独运行PMAlign(避开采集工具)
                //判定规则(用户明确): L和R各自匹配数量>0才算合格(不按Accept/Reject状态)
                bool ok = false;
                int cnt = 0;
                double sc = 0;
                try
                {
                    CogPMAlignResults pr = pm.Results;
                    if (pr != null)
                    {
                        cnt = pr.Count;
                        if (cnt > 0) sc = pr[0].Score;
                    }
                    ok = cnt > 0;
                }
                catch { ok = false; }
                if (t.Name == "L") { lOk = ok; lMatch = cnt; lScore = sc; }
                else if (t.Name == "R") { rOk = ok; rMatch = cnt; rScore = sc; }
                else { lOk = lOk || ok; rOk = rOk || ok; }
            }
            SaveLogFile("检测: L匹配=" + lMatch + "(分" + lScore.ToString("0.0") + ") R匹配=" + rMatch + "(分" + rScore.ToString("0.0") + ") => " + ((lOk && rOk) ? "OK" : "NG"));
            bool 检测OK = lOk && rOk;
            //显示图像: 显示相机采集图像(当前帧), 每次检测先清空上次残留图形
            //L/R分开画区域框: 各自OK=绿色框, NG=红色框(用训练区域坐标, 与训练时一致, 相机固定同尺寸直接套用)
            try
            {
                Cognex.VisionPro.ICogImage dispImg = GetCameraImage();
                if (dispImg != null)
                {
                    cogDisp.Image = dispImg;
                    cogDisp.Fit();
                    cogDisp.InteractiveGraphics.Clear();   //清掉上次检测残留图形
                    foreach (ICogTool t in tool.Tools)
                    {
                        CogPMAlignTool pm = t as CogPMAlignTool;
                        if (pm == null || pm.Pattern == null) continue;
                        CogRectangleAffine trainRect = pm.Pattern.TrainRegion as CogRectangleAffine;
                        if (trainRect == null) continue;
                        bool thisOk = (t.Name == "L") ? lOk : (t.Name == "R") ? rOk : (lOk && rOk);
                        CogRectangleAffine rect = new CogRectangleAffine();
                        rect.CenterX = trainRect.CenterX;
                        rect.CenterY = trainRect.CenterY;
                        rect.SideXLength = trainRect.SideXLength;
                        rect.SideYLength = trainRect.SideYLength;
                        rect.Rotation = trainRect.Rotation;
                        rect.SelectedSpaceName = trainRect.SelectedSpaceName;   //与训练区域同空间
                        rect.Color = thisOk ? CogColorConstants.Green : CogColorConstants.Red;   //L/R分开: OK绿, NG红
                        rect.LineWidthInScreenPixels = 2;
                        try { cogDisp.InteractiveGraphics.Add(rect, t.Name, false); }
                        catch
                        {
                            //空间名与显示图像不匹配时, 改用空像素空间重试(训练图与运行图同尺寸同相机, 数值坐标一致)
                            try { rect.SelectedSpaceName = ""; cogDisp.InteractiveGraphics.Add(rect, t.Name, false); } catch { }
                        }
                    }
                }
            }
            catch { }

            if (检测OK)
            {
                ShowCameraInfo(cogDisp, CogColorConstants.Green, 0.0, 0.0, "OK");
            }
            else
            {
                ShowCameraInfo(cogDisp, CogColorConstants.Red, 0.0, 0.0, "NG");
            }
            return 检测OK;
        }
        //启动按钮
        private void btn_StartAuto_Click(object sender, EventArgs e)
        {

            //开始按钮: 按一下检测一下(严格单次触发, 不循环)
            SaveLogFile("开始按钮: 触发一次检测");
            TriggerInspect();
            btn_StartAuto.BackColor = Color.Green;
            btn_StopAuto.BackColor = Color.Gray;
        }
        //停止按钮(复位状态灯)
        private void btn_StopAuto_Click(object sender, EventArgs e)
        {
            base.Invoke(this.dlgAddLog, new object[]
                              {
                                    "停止检测",
                                    true
                              });
            btn_StartAuto.BackColor = Color.Gray;
            btn_StopAuto.BackColor = Color.Red;
        }
        //计数复位
        private void btn_DataReset_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure to reset camera  statistics？", "Alarm！", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                ResetStatistics();
            }
        }
        //复位统计并复位IO输出
        private void ResetStatistics()
        {
            base.Invoke(this.dlgAddLog, new object[]
            {
                "复位统计数据!",
                true
            });
            total = 0;
            pass = 0;
            String inifile = Application.StartupPath + "\\data\\sys.ini";
            IniFile.WriteInt(inifile, "统计", "总数", (int)total);
            IniFile.WriteInt(inifile, "统计", "合格数量", (int)pass);
            this.ShowStatistics(0u, 0u, 0.0);
            if (ioBoard != null) ioBoard.ResetOutputs();   //三色灯/蜂鸣器全部熄灭
        }
        //界面关闭
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            base.Invoke(this.dlgAddLog, new object[]
                                   {
                                        "系统退出！",
                                        true
                                   });
            SaveLogFile("关闭触发堆栈: " + Environment.StackTrace);
            try
            {
                //关闭相机
                if (cam != null) cam.Close();
            }
            catch (System.Exception err)
            {

            }
            NetIOBoard netIO = ioBoard as NetIOBoard;
            if (netIO != null) netIO.Stop();   //停止网口IO卡轮询并断开连接
            Environment.Exit(0);
        }
        //退出按钮
        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //IO卡输出变化: 更新主界面状态灯(与硬件映射一致: OUT1红灯NG/OUT2黄灯/OUT3绿灯OK/OUT4蜂鸣器)
        private void ioBoard_OutputChanged_UI(int index, bool value)
        {
            if (this.IsDisposed) return;
            try
            {
                this.BeginInvoke(new Action(delegate
                {
                    switch (index)
                    {
                        case 1: light_NG.SetOn(value); break;     //OUT1 红灯(NG)
                        case 2: light_Run.SetOn(value); break;    //OUT2 黄灯(不用)
                        case 3: light_OK.SetOn(value); break;     //OUT3 绿灯(OK)
                        case 4: light_Buzzer.SetOn(value); break; //OUT4 蜂鸣器
                    }
                }));
            }
            catch { }
        }
        //IO卡输入变化: IN1上升沿触发一次检测(严格单次), IN2上升沿计数复位
        private void ioBoard_InputChanged(int index, bool value)
        {
            if (!value) return;   //只处理上升沿
            if (index == 1)
            {
                base.Invoke(this.dlgAddLog, new object[] { "IO卡IN1触发检测", true });
                TriggerInspect();   //单次触发
            }
            else if (index == 2)
            {
                base.Invoke(new Action(delegate
                {
                    ResetStatistics();
                }));
                base.Invoke(this.dlgAddLog, new object[] { "IO卡IN2计数复位", true });
            }
        }
        //网口IO卡连接状态变化: 更新IO测试按钮与日志(断线自动重连)
        private void ioBoard_ConnectedChanged(bool connected)
        {
            if (this.IsDisposed) return;
            try
            {
                this.BeginInvoke(new Action(delegate
                {
                    NetIOBoard net = ioBoard as NetIOBoard;
                    string addr = net != null ? (net.Host + ":" + net.Port) : "";
                    if (connected)
                    {
                        btn_IOTest.Text = "IO已连接";
                        AddLog("网口IO卡已连接 " + addr, true);
                    }
                    else
                    {
                        btn_IOTest.Text = "IO未连接";
                        AddLog("网口IO卡连接断开 " + addr + ", 自动重连中...", false);
                    }
                }));
            }
            catch { }
        }
        //蜂鸣器延时自动关闭(毫秒)
        private void AutoOffBuzzer(int delayMs)
        {
            Task.Run(async delegate
            {
                await Task.Delay(delayMs);
                if (ioBoard != null) ioBoard.SetOutput(4, false);
            });
        }
        //打开网口IO卡通讯测试页面(独立连接, 不影响主程序ioBoard)
        private void btn_IOTest_Click(object sender, EventArgs e)
        {
            IOBoardTestForm frm = new IOBoardTestForm();
            frm.Show();
            frm.BringToFront();
        }
        //工具设置
        private void toolStripMenuTool_Click(object sender, EventArgs e)
        {
           
        }
        //显示相机信息
        private static void ShowCameraInfo(CogRecordDisplay cogRecordDisplay, CogColorConstants color, double x, double y, string info)
        {
            try
            {
                CogGraphicLabel lbl = new CogGraphicLabel();
                lbl.LineWidthInScreenPixels = 3;
                lbl.Font = new Font("", 16f, FontStyle.Bold);
                lbl.Color = CogColorConstants.Black;
                lbl.BackgroundColor = color;
                lbl.Alignment = CogGraphicLabelAlignmentConstants.TopLeft;
                lbl.SetXYText(x, y, info);
                lbl.SelectedSpaceName = "#";
                cogRecordDisplay.StaticGraphics.Add(lbl, "");
            }
            catch (System.Exception er)
            {
            }
        }
        //统计信息
        private void ShowStatistics(uint total, uint pass, double time)
        {
            base.Invoke(new Action(delegate
            {
                this.tb_Cam1Total.Text = total.ToString();
                this.tb_Cam1Pass.Text = pass.ToString();
                this.tb_Cam1Time.Text = string.Format("{0:0.00}", time);
                if (total == 0u)
                {
                    this.tb_Cam1Yeild.Text = string.Format("{0:0.00}", 0);
                }
                else
                {
                    this.tb_Cam1Yeild.Text = string.Format("{0:0.00}", 1.0 * pass / total * 100.0);
                }
            }));
        }
        
        //拍照按钮: 按下触发一次检测(严格单次)
        private void Trig_MouseUp(object sender, MouseEventArgs e)
        {
            debugImageIndex++;
        }
        //拍照按钮: 按下触发一次检测(严格单次)
        private void Trig_MouseDown(object sender, MouseEventArgs e)
        {
            TriggerInspect();
        }
        //保存OK图像
        private void btnSaveOk_Click(object sender, EventArgs e)
        {
            SaveOkPic();
            MessageBox.Show("Success", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        //保存NG图像
        private void btnSaveNg_Click(object sender, EventArgs e)
        {
            SaveNgPic();
            MessageBox.Show("Success", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        //保存OK图像
        private void SaveOkPic()
        {
            if (Debug) return;
            CogImageFileTool cogFile = new CogImageFileTool();
            String ipath = Application.StartupPath + "\\CAM1\\";
            String ifile = ipath + DateTime.Now.ToString("yyyyMMddHHmmss") + "OK.bmp";
            if (!Directory.Exists(ipath)) //若此文件夹不存在
            {
                Directory.CreateDirectory(ipath); //创建此文件夹
            }

            cogFile.Operator.Open(ifile, CogImageFileModeConstants.Write);
            cogFile.InputImage = GetCameraImage();
            cogFile.Run();
            cogFile.Operator.Close();
        }
        //保存NG图像
        private void SaveNgPic()
        {
            if (Debug) return;
            try
            {
                CogImageFileTool cogFile = new CogImageFileTool();
                String ipath = Application.StartupPath + "\\CAM1\\";
                String ifile = ipath + DateTime.Now.ToString("yyyyMMddHHmmss") + "NG.bmp";
                if (!Directory.Exists(ipath)) //若此文件夹不存在
                {
                    Directory.CreateDirectory(ipath); //创建此文件夹
                }
                ICogImage img = GetCameraImage();
                if (img == null)
                {
                    SaveLogFile("SaveNgPic: GetCameraImage返回null, 跳过保存");
                    return;
                }
                cogFile.Operator.Open(ifile, CogImageFileModeConstants.Write);
                cogFile.InputImage = img;
                cogFile.Run();
                cogFile.Operator.Close();
                SaveLogFile("NG图像已保存: " + ifile);
            }
            catch (Exception ex)
            {
                SaveLogFile("SaveNgPic异常: " + ex.Message);
            }
        }
        //获取康耐视工具(单工具)
        public CogToolBlock Tool { get => tool; set => tool = value; }
        //在ToolBlock工具链中查找相机采集工具(CogAcqFifoTool), 未找到返回null
        private static CogAcqFifoTool FindAcqTool(CogToolBlock tb)
        {
            if (tb == null) return null;
            foreach (ICogTool t in tb.Tools)
            {
                CogAcqFifoTool acq = t as CogAcqFifoTool;
                if (acq != null) return acq;
            }
            return null;
        }
        //获取当前相机图像(迈德威视相机最近一次输出, 保存OK/NG图/工具预览用)
        public ICogImage GetCameraImage()
        {
            if (cam == null) return null;
            try { return cam.OutputImageRgb24; } catch { return null; }
        }
        //编辑工具"采集图像"专用: 用主程序相机采集一帧新图像(不新建相机实例, 与检测互斥锁防并发采集)
        public ICogImage CaptureImageForEdit()
        {
            try
            {
                if (cam == null || !cam.IsOpen) return null;
                lock (inspectLock)
                {
                    if (!cam.Run()) return null;
                }
                return cam.OutputImageRgb24;
            }
            catch { return null; }
        }

        public static bool Debug => debug;

        private void Trig_Click(object sender, EventArgs e)
        {

        }

        private void vPP设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
                FormToolblock frmtb = new FormToolblock(this);
                frmtb.Show();
        
        }

        //标签打印测试(工具设置菜单): 打印一张当前序号标签验证打印链路(序号不递增)
        private void 标签打印测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (labelPrinter == null) return;
            string msg = string.Format("打印机: {0}\n色号: {1}\n当前序号: {2}\n\n确定打印一张测试标签吗? (序号不递增)\n(需已安装CODESOFT 7并连接打印机)",
                labelPrinter.PrinterName, labelPrinter.ColorCode, labelPrinter.CurrentSeq);
            if (MessageBox.Show(msg, "标签打印测试", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            bool ok = labelPrinter.PrintLabel(Convert.ToInt32(labelPrinter.CurrentSeq));
            base.Invoke(this.dlgAddLog, new object[]
            {
                ok ? "标签打印测试完成: " + labelPrinter.ColorCode + " 序号" + labelPrinter.CurrentSeq : "标签打印测试失败, 见日志",
                true
            });
            MessageBox.Show(ok ? "打印成功!" : "打印失败! 详见 Log 目录日志", "标签打印测试", MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }

    }
}
