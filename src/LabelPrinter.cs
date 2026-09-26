using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace VisionDemo
{
    /// <summary>
    /// 标签打印(调用 标签打印程序 Printer.exe, CODESOFT 7 打印链路)
    /// 配置在 sys.ini [打印] 节:
    ///   PrinterPath = Printer.exe 完整路径(默认取程序目录 Printer\Printer.exe)
    ///   PrinterName = Windows打印机名(默认 ZD421, 无打印机可填 "Microsoft Print to PDF" 验证)
    ///   色号        = IP01/IH14/IA08/IG03(默认 IP01)
    ///   序号        = 生产序号(00001起递增, 由本类维护写回)
    /// 调用格式: Printer.exe 打印机名 色号 生产序号 Z序号 日 月 年
    /// </summary>
    public class LabelPrinter
    {
        private readonly string inifile;
        private readonly string printerPath;
        private readonly string printerName;
        private readonly string colorCode;
        private int seq;   //当前生产序号(1起), 打印一张后+1写回

        public LabelPrinter(string iniPath)
        {
            inifile = iniPath;
            printerPath = IniFile.ReadString(inifile, "打印", "PrinterPath", Path.Combine(Application.StartupPath, "VisionDemo.Printer.exe"));
            printerName = IniFile.ReadString(inifile, "打印", "PrinterName", "ZD421");
            colorCode = IniFile.ReadString(inifile, "打印", "色号", "IP01");
            seq = IniFile.ReadInt(inifile, "打印", "序号", 1);
        }

        //打印机名
        public string PrinterName => printerName;
        //色号
        public string ColorCode => colorCode;
        //当前生产序号(5位字符串)
        public string CurrentSeq => seq.ToString("D5");

        /// <summary>
        /// 打印一张标签(序号+1并写回), 返回是否成功
        /// </summary>
        public bool PrintLabel()
        {
            int useSeq = seq;
            seq++;
            IniFile.WriteInt(inifile, "打印", "序号", seq);
            return RunPrinter(useSeq);
        }

        /// <summary>
        /// 打印指定序号(用于测试/补打, 不递增序号), 返回是否成功
        /// </summary>
        public bool PrintLabel(int fixedSeq)
        {
            return RunPrinter(fixedSeq);
        }

        /// <summary>
        /// 调用 Printer.exe 打印 (Printer.exe 打印机名 色号 生产序号 000 日 月 年)
        /// </summary>
        private bool RunPrinter(int seqNo)
        {
            try
            {
                if (!File.Exists(printerPath))
                {
                    SaveLogFile("标签打印失败: Printer.exe 不存在 " + printerPath + " (请在 sys.ini [打印] 节配置 PrinterPath)");
                    return false;
                }
                string args = string.Format("\"{0}\" {1} {2} 000 {3} {4} {5}",
                    printerName, colorCode, seqNo.ToString("D5"),
                    DateTime.Now.ToString("dd"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("yy"));
                SaveLogFile("标签打印: " + Path.GetFileName(printerPath) + " " + args);
                Process p = new Process();
                p.StartInfo.FileName = printerPath;
                p.StartInfo.Arguments = args;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.WaitForExit(30000);   //最多等30秒(CODESOFT打开/打印)
                if (p.HasExited && p.ExitCode != 0)
                {
                    SaveLogFile("标签打印失败: 退出码 " + p.ExitCode);
                    return false;
                }
                SaveLogFile("标签打印成功: " + colorCode + " 序号 " + seqNo.ToString("D5"));
                return true;
            }
            catch (Exception ex)
            {
                SaveLogFile("标签打印异常: " + ex.Message);
                return false;
            }
        }

        //写日志(与主程序同格式, 追加 Log\yyyy-MM-dd.log)
        private void SaveLogFile(string strMessage)
        {
            try
            {
                string logPath = Environment.CurrentDirectory + "\\Log";
                if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
                string fullPath = logPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                File.AppendAllText(fullPath, "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + strMessage + "\r\n", System.Text.Encoding.UTF8);
            }
            catch { }
        }
    }
}
