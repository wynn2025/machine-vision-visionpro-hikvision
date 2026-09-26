using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using MvCameraControl;
using Cognex.VisionPro;

namespace VisionDemo
{
    /// <summary>
    /// 迈德威视 GigE 相机驱动(基于新版 MvCameraControl.Net SDK, IP直连)
    /// 相机: MV-CS200-10GM (192.168.1.100)
    /// 旧版 MVCAMSDK_X64.dll 的 CameraEnumerateDevice 不兼容此相机(返回-16), 必须用新版SDK
    /// 采集流程: SDKSystem.Initialize → DeviceFactory.CreateDeviceByIp → device.Open → StreamGrabber.StartGrabbing → GetImageBuffer → 转RGB24
    /// </summary>
    public class MvNetCamera
    {
        private IDevice device;              //相机设备
        private CogImage24PlanarColor _OutputImageRgb24; //输出图像(供VisionPro显示/检测)
        private bool sdkInited = false;
        private readonly string cameraIp;    //相机IP
        private readonly string netExportIp; //电脑网卡IP
        private readonly long exposureUs;    //曝光时间(微秒, 从参数加载, 默认70000)

        public MvNetCamera(string cameraIp, string netExportIp, long exposureUs = 70000)
        {
            this.cameraIp = cameraIp;
            this.netExportIp = netExportIp;
            this.exposureUs = exposureUs;
            //确保程序目录加入DLL搜索路径(MvCameraControl.dll的ConvertPixelType需加载运行环境内的转换插件)
            try
            {
                SetDllDirectory(System.Windows.Forms.Application.StartupPath);
            }
            catch { }
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        //输出RGB24图像(VisionPro CogImage24PlanarColor)
        public CogImage24PlanarColor OutputImageRgb24 { get { return _OutputImageRgb24; } }

        //相机是否已打开
        public bool IsOpen { get { return device != null; } }

        //初始化SDK(幂等)
        private void EnsureSdkInit()
        {
            if (!sdkInited)
            {
                int r = SDKSystem.Initialize();
                sdkInited = true;
            }
        }

        /// <summary>
        /// 扫描相机(枚举GigE+USB), 返回数量
        /// </summary>
        public int ScanCameras()
        {
            try
            {
                EnsureSdkInit();
                List<IDeviceInfo> list;
                int ret = DeviceEnumerator.EnumDevices(DeviceTLayerType.MvGigEDevice | DeviceTLayerType.MvUsbDevice, out list);
                if (ret != 0) return 0;
                return list == null ? 0 : list.Count;
            }
            catch { return 0; }
        }

        /// <summary>
        /// 打开相机(按IP直连), 带重试
        /// 常见失败原因: 上一个进程被强杀后相机残留会话未释放(0x80000203 设备已打开, 相机侧GigE心跳约1.5-2分钟才超时), 相机刚上电未就绪等
        /// 自动重试最多40次(间隔3秒, 共约2分钟), 覆盖残留会话释放窗口, 避免直接弹窗"连接失败"
        /// </summary>
        public bool OpenCameraByIp()
        {
            EnsureSdkInit();
            for (int attempt = 1; attempt <= 40; attempt++)
            {
                if (attempt > 1)
                {
                    SaveLogFile("MvNetCamera: 相机打开失败, 3秒后重试(第" + attempt + "次/共40次)");
                    System.Threading.Thread.Sleep(3000);
                }
                try
                {
                    device = DeviceFactory.CreateDeviceByIp(cameraIp, netExportIp);
                    if (device == null)
                    {
                        SaveLogFile("MvNetCamera: CreateDeviceByIp返回null, 第" + attempt + "次");
                        continue;
                    }
                    int r = device.Open();
                    if (r != 0)
                    {
                        SaveLogFile("MvNetCamera: device.Open失败 0x" + r.ToString("x8") + ", 第" + attempt + "次");
                        try { device.Dispose(); } catch { }
                        device = null;
                        continue;
                    }
                    //GigE相机优化包大小
                    if (device is IGigEDevice)
                    {
                        int packetSize;
                        if ((device as IGigEDevice).GetOptimalPacketSize(out packetSize) == 0 && packetSize > 0)
                        {
                            device.Parameters.SetIntValue("GevSCPSPacketSize", packetSize);
                        }
                    }
                    //采集模式Continuous + 触发模式off(自由连续采集)
                    device.Parameters.SetEnumValueByString("AcquisitionMode", "Continuous");
                    device.Parameters.SetEnumValue("TriggerMode", 0);
                    //曝光时间从参数加载(默认70000微秒=70ms, 可在sys.ini [相机]节 曝光= 修改)
                    //注意: ExposureTime是Float类型, 必须用SetFloatValue
                    try
                    {
                        device.Parameters.SetFloatValue("ExposureTime", (float)exposureUs);
                        SaveLogFile("MvNetCamera: 曝光已设置为 " + exposureUs + " us");
                    }
                    catch (Exception ex)
                    {
                        SaveLogFile("MvNetCamera: 曝光设置失败(" + exposureUs + "us): " + ex.Message);
                    }
                    //开始采集(失败也视为打开失败, 重新走重试)
                    int sr = device.StreamGrabber.StartGrabbing();
                    if (sr != 0)
                    {
                        SaveLogFile("MvNetCamera: StartGrabbing失败 0x" + sr.ToString("x8") + ", 第" + attempt + "次");
                        Close();
                        continue;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    SaveLogFile("MvNetCamera: 打开相机异常(" + ex.Message + "), 第" + attempt + "次");
                    try { if (device != null) device.Dispose(); } catch { }
                    device = null;
                }
            }
            return false;
        }

        /// <summary>
        /// 采集一帧图像并转换为RGB24(VisionPro CogImage24PlanarColor)
        /// </summary>
        public bool Run()
        {
            if (device == null) return false;
            try
            {
                IFrameOut frame;
                int ret = device.StreamGrabber.GetImageBuffer(3000, out frame);
                if (ret != 0 || frame == null)
                {
                    SaveLogFile("MvNetCamera.Run: GetImageBuffer失败 ret=0x" + ret.ToString("x8"));
                    return false;
                }
                try
                {
                    int width = (int)frame.Image.Width;
                    int height = (int)frame.Image.Height;
                    //转RGB8格式(PixelTypeConverter) - 必须转换成功才能按RGB读像素(原图为Mono12_Packed)
                    IImage outImage;
                    int cr = device.PixelTypeConverter.ConvertPixelType(frame.Image, out outImage, MvGvspPixelType.PixelType_Gvsp_RGB8_Packed);
                    if (cr != 0 || outImage == null)
                    {
                        SaveLogFile("MvNetCamera.Run: ConvertPixelType失败 cr=0x" + cr.ToString("x8"));
                        return false;
                    }
                    //确保输出图像尺寸
                    if (_OutputImageRgb24 == null || _OutputImageRgb24.Width != width || _OutputImageRgb24.Height != height)
                    {
                        _OutputImageRgb24 = new CogImage24PlanarColor(width, height);
                    }
                    //复制像素到CogImage(转换后的RGB8, 每行4字节对齐)
                    CopyPixelsToCogImage(outImage.PixelDataPtr, width, height);
                    return true;
                }
                finally
                {
                    device.StreamGrabber.FreeImageBuffer(frame);
                }
            }
            catch (Exception ex) 
            {
                SaveLogFile("MvNetCamera.Run异常: " + ex.Message);
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
                System.IO.File.AppendAllText(fullPath, "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + strMessage + "\r\n", System.Text.Encoding.UTF8);
            }
            catch { }
        }

        //把RGB24像素复制到CogImage24PlanarColor(参考SwiftCopyRgb24, 用Scan0)
        private unsafe void CopyPixelsToCogImage(IntPtr srcPtr, int width, int height)
        {
            CogImage24PlanarColor img = _OutputImageRgb24 as CogImage24PlanarColor;
            if (img == null) return;
            ICogImage8PixelMemory[] channels = new ICogImage8PixelMemory[3];
            img.Get24PlanarColorPixelMemory(CogImageDataModeConstants.ReadWrite, 0, 0, 0, 0, out channels[0], out channels[1], out channels[2]);
            byte* pr = (byte*)channels[0].Scan0.ToPointer();
            byte* pg = (byte*)channels[1].Scan0.ToPointer();
            byte* pb = (byte*)channels[2].Scan0.ToPointer();
            byte* psrc = (byte*)srcPtr.ToPointer();
            int srcStride = ((width * 3 + 3) / 4) * 4;  //RGB8_Packed每行4字节对齐
            for (int i = 0; i < height; i++)
            {
                byte* srow = psrc + i * srcStride;
                byte* r = pr + i * (int)channels[0].Stride;
                byte* g = pg + i * (int)channels[1].Stride;
                byte* b = pb + i * (int)channels[2].Stride;
                for (int j = 0; j < width; j++)
                {
                    r[j] = srow[j * 3 + 0];
                    g[j] = srow[j * 3 + 1];
                    b[j] = srow[j * 3 + 2];
                }
            }
        }

        /// <summary>
        /// 关闭相机
        /// </summary>
        public void Close()
        {
            try
            {
                if (device != null)
                {
                    try { device.StreamGrabber.StopGrabbing(); } catch { }
                    device.Close();
                    device.Dispose();
                    device = null;
                }
            }
            catch { device = null; }
        }
    }
}
