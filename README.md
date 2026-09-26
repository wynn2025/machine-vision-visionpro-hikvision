# 机器视觉检测上位机（海康 GigE 相机 + VisionPro ToolBlock）

从真实产线视觉检测项目整理的 WinForms 上位机源码，全链路打通：海康 MVS 新版 SDK 采图 → Cognex VisionPro ToolBlock 检测 → 网络 IO 板联动 → 标签打印。全部中文注释，一行不删。

核心亮点：IP 直连打开相机（不走枚举，绕开旧版 SDK 枚举返回 -16 的硬伤）、像素级转换写入 CogImage24PlanarColor（处理 stride 对齐）、新旧两种 vpp 结构双兼容、相机打开自动重试 40 次 × 3 秒覆盖残留会话窗口。

## 目录结构

```
.
├── README.md              # 本文件
├── 采图检测流程说明.md      # 采图→检测→IO→打印全流程讲解
└── src/
    ├── 主工程 csproj（WinForms）+ app.config
    ├── Program.cs / Settings.cs / IniFile.cs / UIHelper.cs
    ├── FormMain.cs / FormToolblock.cs（内嵌 CogToolBlockEditV21 现场调工具）/ FormSplash.cs
    ├── MvNetCamera.cs     # 海康新版 MvCameraControl.Net 封装（IP 直连 + 像素转换）
    ├── NetIOBoard.cs / IIOBoard.cs / IOBoardTestForm.cs   # Modbus-RTU over TCP（手写 CRC16）
    ├── LabelPrinter.cs    # 标签打印
    └── Properties/
```

检测工具块（vpp）现场可编辑保存，程序加载后逐工具解析 PMAlign 结果（匹配数量 + 首个匹配分数）。

## FAQ

1. **支持哪些相机？** 海康 MV-CS 系列 GigE 相机，使用新版 MvCameraControl.Net SDK（SDKSystem.Initialize → DeviceFactory.CreateDeviceByIp → StreamGrabber）。
2. **旧版 SDK 枚举相机返回 -16 怎么办？** 不用枚举。本源码按 ini 配置 IP 直连打开相机，换现场只改配置不改代码。
3. **打开相机报 0x80000203（设备已打开）？** 进程被强杀后相机侧会话残留约 1.5–2 分钟才释放，程序已内置自动重试 40 次 × 3 秒覆盖该窗口，无需人工干预。
4. **检测工具块（vpp）怎么改？** 界面内嵌 CogToolBlockEditV21 编辑控件，现场直接训练 PMAlign、改参数、看结果并保存；新旧两种 vpp 结构均兼容。
5. **相机原始图像格式怎么进 VisionPro？** Mono12_Packed 等工业格式先经 PixelTypeConverter 转 RGB8_Packed，再逐行拆 R/G/B 通道写入 CogImage24PlanarColor，含每行 4 字节对齐的 stride 处理。

## 样章导引

本 repo 为精简展示版。完整源码与《采图检测流程说明》全文见购买指引所指渠道；样章（采图检测流程摘录）随 repo 一并可见。

## 购买指引

- 面包多（全套源码 + 逐模块讲解）：`【待回填：面包多 S2 对应商品页链接】`
- CSDN（相关博文/资源）：https://blog.csdn.net/csdngouwei/article/details/166601561（海康相机故障排查实战）
