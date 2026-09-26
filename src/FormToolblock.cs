using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.PMAlign;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace VisionDemo
{
    public partial class FormToolblock : Form
    {
        private FormMain main;
        public FormToolblock(FormMain main)
        {
            InitializeComponent();
            this.main = main;
        }

        private void FormToolblock_Load(object sender, EventArgs e)
        {
            base.Invoke(new Action(delegate
            {
                //无vpp文件时自动创建空工具链, 便于首次新建检测工具(单工具0.vpp)
                if (main.Tool == null)
                {
                    main.Tool = new CogToolBlock();
                }
                this.cogToolBlockEditV21.Subject = main.Tool;
            }));
        }

        private void btn_loadImage_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Training image(*.bmp;*.idb)|*.bmp;*.idb";
                ofd.ValidateNames = true;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.InitialDirectory = "%DESKTOP%";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    CogImageFile ImageFile = new CogImageFile();
                    ImageFile.Open(ofd.FileName, CogImageFileModeConstants.Read);
                    ICogImage img = ImageFile[0];
                    ImageFile.Close();
                    base.Invoke(new Action(delegate
                    {
                        //把图像写入ToolBlock的CogImage输入引脚(用户在vpp中添加的, L/R已接线到输入引脚), 供编辑L/R使用
                        SetBlockInputImage(this.cogToolBlockEditV21.Subject, img);
                    }));
                }
                else
                {
                    MessageBox.Show("Not select image！", "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void btn_SaveTool_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Save？", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            if (result == DialogResult.Yes)
            {
                String vpppath = Application.StartupPath + "\\data\\0.vpp";
                CogSerializer.SaveObjectToFile(this.cogToolBlockEditV21.Subject, vpppath);
                main.Tool = this.cogToolBlockEditV21.Subject;
                MessageBox.Show("Success", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                base.Close();
            }
        }

        private void btn_captur_Click(object sender, EventArgs e)
        {
            //调用主程序相机采集一帧新图像(编辑工具不新建相机实例, 避免与主程序抢相机0x80000203)
            ICogImage img = main.CaptureImageForEdit();
            if (img == null)
            {
                MessageBox.Show("未获取到相机图像！请确认相机已连接", "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            base.Invoke(new Action(delegate
            {
                SetBlockInputImage(this.cogToolBlockEditV21.Subject, img);
            }));
        }

        /// <summary>
        /// 把图像写入ToolBlock的CogImage输入引脚(用户在vpp中添加的), 同时兜底给所有PMAlign赋InputImage
        /// 用户在vpp内把L/R的InputImage接线到输入引脚后, 编辑L/R时图像从block输入引脚流入
        /// 按值类型匹配输入引脚(不依赖引脚名, CogToolBlockTerminal为internal类型故用dynamic), 兼容旧vpp无输入引脚/未接线的情况
        /// </summary>
        public static void SetBlockInputImage(CogToolBlock tb, ICogImage img)
        {
            if (tb == null || img == null) return;
            //1) 写入所有CogImage类型的输入引脚
            bool wrotePin = false;
            try
            {
                foreach (dynamic term in tb.Inputs)
                {
                    Type vt = term.ValueType as Type;
                    if (vt != null && typeof(ICogImage).IsAssignableFrom(vt))
                    {
                        term.Value = img;
                        wrotePin = true;
                    }
                }
            }
            catch { }
            //2) 兜底: 仍给所有PMAlign赋InputImage(兼容旧vpp无输入引脚/未接线的情况)
            foreach (ICogTool t in tb.Tools)
            {
                CogPMAlignTool pm = t as CogPMAlignTool;
                if (pm != null) pm.InputImage = img;
            }
        }

        //相机选择下拉(单工具模式仅相机1, 保留控件兼容)
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //单工具: 固定使用0.vpp, 无需切换
        }
    }
}
