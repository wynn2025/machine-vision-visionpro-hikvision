using System;

namespace VisionDemo
{
    /// <summary>
    /// IO卡统一接口(4入4出): 真实网口IO卡(NetIOBoard)
    /// 输入: IN1=触发检测 IN2=计数复位 IN3/IN4=预留
    /// 输出(三色灯排列红黄绿, 同一时间只亮1个灯): OUT1=红灯(NG) OUT2=黄灯(不用) OUT3=绿灯(OK) OUT4=蜂鸣器(NG)
    /// </summary>
    public interface IIOBoard
    {
        //输入变化事件(上升沿/下降沿均触发)
        event Action<int, bool> InputChanged;
        //输出变化事件
        event Action<int, bool> OutputChanged;

        //读取输入状态
        bool GetInput(int index);
        //读取输出状态
        bool GetOutput(int index);
        //写入输出(控制指示灯/蜂鸣器)
        void SetOutput(int index, bool value);
        //批量设置输出(检测结果): ok=true亮绿灯, ok=false亮红灯+蜂鸣器
        void SetResultOutput(bool ok);
        //运行状态指示(检测中亮黄灯)
        void SetRunningOutput(bool running);
        //全部输出复位
        void ResetOutputs();
        //调试命令收发(网口板状态查询/写硬件)
        string ExecuteCommand(string command);
    }
}
