using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using Cognex.VisionPro;
using System.IO;

namespace VisionDemo
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //全局异常捕获,写入启动目录crash.log
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => { WriteCrashLog(e.Exception); };
            AppDomain.CurrentDomain.UnhandledException += (s, e) => { WriteCrashLog(e.ExceptionObject as Exception); };
            //程序目录加入PATH(确保MvCameraControl.dll的ConvertPixelType能加载运行环境内的转换插件ippi/MediaProcess等)
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                string path = Environment.GetEnvironmentVariable("PATH") ?? "";
                if (!path.Contains(dir)) Environment.SetEnvironmentVariable("PATH", dir + ";" + path);
                //MVS运行时目录(标准安装路径, 含完整SDK依赖)
                string mvs = @"C:\Program Files (x86)\Common Files\MVS\Runtime\Win64_x64";
                if (System.IO.Directory.Exists(mvs) && !path.Contains(mvs))
                    Environment.SetEnvironmentVariable("PATH", mvs + ";" + Environment.GetEnvironmentVariable("PATH"));
            }
            catch { }

            bool createNew;
            using (new Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FormMain());
                }
                else
                {
                    MessageBox.Show("视觉检测系统已启动！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        //写崩溃日志
        private static void WriteCrashLog(Exception ex)
        {
            try
            {
                if (ex == null) return;
                File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "crash.log"),
                    "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]\r\n" + ex.ToString() + "\r\n\r\n");
            }
            catch { }
        }
    }
}
