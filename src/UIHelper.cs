using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VisionDemo
{
    /// <summary>
    /// 扁平圆角按钮: 悬停提亮/按下变暗, 支持圆角半径与自定义颜色
    /// </summary>
    public class FlatButton : Button
    {
        private int radius = 8;
        private bool hover = false;
        private bool pressed = false;

        public int Radius
        {
            get { return radius; }
            set { radius = value; Invalidate(); }
        }

        public FlatButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.BackColor = Color.FromArgb(30, 110, 190);
            this.ForeColor = Color.White;
            this.Font = new Font("微软雅黑", 12F, FontStyle.Bold);
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            hover = true; Invalidate(); base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            hover = false; pressed = false; Invalidate(); base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            pressed = true; Invalidate(); base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            pressed = false; Invalidate(); base.OnMouseUp(mevent);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            //底色: 按下变暗, 悬停提亮
            Color baseColor = BackColor;
            if (pressed) baseColor = ControlPaint.Dark(BackColor, 0.2f);
            else if (hover) baseColor = ControlPaint.Light(BackColor, 0.15f);

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = GetRoundedPath(rect, radius))
            using (SolidBrush brush = new SolidBrush(baseColor))
            {
                g.FillPath(brush, path);
            }
            //边框
            using (GraphicsPath path = GetRoundedPath(rect, radius))
            using (Pen pen = new Pen(Color.FromArgb(60, 255, 255, 255)))
            {
                g.DrawPath(pen, path);
            }

            //文字居中
            TextRenderer.DrawText(g, Text, Font, rect, Enabled ? ForeColor : Color.Gray,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int r)
        {
            GraphicsPath path = new GraphicsPath();
            int d = r * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// 圆形指示灯: IsOn=true 亮起OnColor, 否则 OffColor; 可带发光效果
    /// </summary>
    public class CircleLight : Label
    {
        private Color onColor = Color.LimeGreen;
        private Color offColor = Color.FromArgb(50, 50, 50);
        private bool isOn = false;

        public Color OnColor { get { return onColor; } set { onColor = value; Invalidate(); } }
        public Color OffColor { get { return offColor; } set { offColor = value; Invalidate(); } }
        public bool IsOn { get { return isOn; } set { isOn = value; Invalidate(); } }

        public CircleLight()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.Size = new Size(24, 24);
        }

        public void SetOn(bool value) { isOn = value; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color c = isOn ? onColor : offColor;

            int size = Math.Min(Width, Height) - 4;
            Rectangle rect = new Rectangle((Width - size) / 2, (Height - size) / 2, size, size);

            //发光光晕
            if (isOn)
            {
                using (SolidBrush glow = new SolidBrush(Color.FromArgb(60, c)))
                {
                    Rectangle glowRect = new Rectangle(rect.X - 4, rect.Y - 4, rect.Width + 8, rect.Height + 8);
                    g.FillEllipse(glow, glowRect);
                }
            }
            using (SolidBrush b = new SolidBrush(c))
            {
                g.FillEllipse(b, rect);
            }
            using (Pen p = new Pen(Color.FromArgb(160, 255, 255, 255), 1.5f))
            {
                g.DrawEllipse(p, rect);
            }
        }
    }
}
