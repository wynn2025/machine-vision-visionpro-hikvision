namespace VisionDemo
{
    partial class IOBoardTestForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox_Conn = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel_Conn = new System.Windows.Forms.TableLayoutPanel();
            this.label_IP = new System.Windows.Forms.Label();
            this.txt_IP = new System.Windows.Forms.TextBox();
            this.label_Port = new System.Windows.Forms.Label();
            this.txt_Port = new System.Windows.Forms.TextBox();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.btn_Disconnect = new System.Windows.Forms.Button();
            this.label_State = new System.Windows.Forms.Label();
            this.light_State = new System.Windows.Forms.Label();
            this.groupBox_IN = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel_IN = new System.Windows.Forms.TableLayoutPanel();
            this.btn_IN4 = new System.Windows.Forms.Button();
            this.btn_IN3 = new System.Windows.Forms.Button();
            this.btn_IN2 = new System.Windows.Forms.Button();
            this.btn_IN1 = new System.Windows.Forms.Button();
            this.label_IN4 = new System.Windows.Forms.Label();
            this.label_IN3 = new System.Windows.Forms.Label();
            this.label_IN2 = new System.Windows.Forms.Label();
            this.label_IN1 = new System.Windows.Forms.Label();
            this.groupBox_OUT = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel_OUT = new System.Windows.Forms.TableLayoutPanel();
            this.btn_OUT4 = new System.Windows.Forms.Button();
            this.btn_OUT3 = new System.Windows.Forms.Button();
            this.btn_OUT2 = new System.Windows.Forms.Button();
            this.btn_OUT1 = new System.Windows.Forms.Button();
            this.label_OUT4 = new System.Windows.Forms.Label();
            this.label_OUT3 = new System.Windows.Forms.Label();
            this.label_OUT2 = new System.Windows.Forms.Label();
            this.label_OUT1 = new System.Windows.Forms.Label();
            this.btn_ResetAll = new System.Windows.Forms.Button();
            this.richTextBox_log = new System.Windows.Forms.RichTextBox();
            this.label_protocol = new System.Windows.Forms.Label();
            this.timer_Refresh = new System.Windows.Forms.Timer(this.components);
            this.groupBox_Conn.SuspendLayout();
            this.tableLayoutPanel_Conn.SuspendLayout();
            this.groupBox_IN.SuspendLayout();
            this.tableLayoutPanel_IN.SuspendLayout();
            this.groupBox_OUT.SuspendLayout();
            this.tableLayoutPanel_OUT.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_Conn
            // 
            this.groupBox_Conn.Controls.Add(this.tableLayoutPanel_Conn);
            this.groupBox_Conn.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.groupBox_Conn.ForeColor = System.Drawing.Color.White;
            this.groupBox_Conn.Location = new System.Drawing.Point(18, 18);
            this.groupBox_Conn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox_Conn.Name = "groupBox_Conn";
            this.groupBox_Conn.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox_Conn.Size = new System.Drawing.Size(864, 144);
            this.groupBox_Conn.TabIndex = 0;
            this.groupBox_Conn.TabStop = false;
            this.groupBox_Conn.Text = "连接配置 (网口IO卡, Modbus-RTU over TCP)";
            // 
            // tableLayoutPanel_Conn
            // 
            this.tableLayoutPanel_Conn.ColumnCount = 6;
            this.tableLayoutPanel_Conn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tableLayoutPanel_Conn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24F));
            this.tableLayoutPanel_Conn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tableLayoutPanel_Conn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel_Conn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tableLayoutPanel_Conn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel_Conn.Controls.Add(this.label_IP, 0, 0);
            this.tableLayoutPanel_Conn.Controls.Add(this.txt_IP, 1, 0);
            this.tableLayoutPanel_Conn.Controls.Add(this.label_Port, 2, 0);
            this.tableLayoutPanel_Conn.Controls.Add(this.txt_Port, 3, 0);
            this.tableLayoutPanel_Conn.Controls.Add(this.btn_Connect, 4, 0);
            this.tableLayoutPanel_Conn.Controls.Add(this.btn_Disconnect, 5, 0);
            this.tableLayoutPanel_Conn.Controls.Add(this.label_State, 0, 1);
            this.tableLayoutPanel_Conn.Controls.Add(this.light_State, 1, 1);
            this.tableLayoutPanel_Conn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Conn.Location = new System.Drawing.Point(4, 36);
            this.tableLayoutPanel_Conn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel_Conn.Name = "tableLayoutPanel_Conn";
            this.tableLayoutPanel_Conn.RowCount = 2;
            this.tableLayoutPanel_Conn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Conn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Conn.Size = new System.Drawing.Size(856, 104);
            this.tableLayoutPanel_Conn.TabIndex = 0;
            // 
            // label_IP
            // 
            this.label_IP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_IP.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label_IP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_IP.Location = new System.Drawing.Point(4, 0);
            this.label_IP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_IP.Name = "label_IP";
            this.label_IP.Size = new System.Drawing.Size(94, 52);
            this.label_IP.TabIndex = 0;
            this.label_IP.Text = "IP地址:";
            this.label_IP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_IP
            // 
            this.txt_IP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_IP.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.txt_IP.Location = new System.Drawing.Point(106, 4);
            this.txt_IP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_IP.Name = "txt_IP";
            this.txt_IP.Size = new System.Drawing.Size(197, 34);
            this.txt_IP.TabIndex = 1;
            this.txt_IP.Text = "192.168.1.30";
            // 
            // label_Port
            // 
            this.label_Port.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Port.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label_Port.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_Port.Location = new System.Drawing.Point(311, 0);
            this.label_Port.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Port.Name = "label_Port";
            this.label_Port.Size = new System.Drawing.Size(94, 52);
            this.label_Port.TabIndex = 2;
            this.label_Port.Text = "端口:";
            this.label_Port.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_Port
            // 
            this.txt_Port.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_Port.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.txt_Port.Location = new System.Drawing.Point(413, 4);
            this.txt_Port.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_Port.Name = "txt_Port";
            this.txt_Port.Size = new System.Drawing.Size(111, 34);
            this.txt_Port.TabIndex = 3;
            this.txt_Port.Text = "23";
            // 
            // btn_Connect
            // 
            this.btn_Connect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(90)))), ((int)(((byte)(100)))));
            this.btn_Connect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Connect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Connect.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.btn_Connect.ForeColor = System.Drawing.Color.White;
            this.btn_Connect.Location = new System.Drawing.Point(534, 4);
            this.btn_Connect.Margin = new System.Windows.Forms.Padding(6, 4, 3, 4);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(145, 44);
            this.btn_Connect.TabIndex = 0;
            this.btn_Connect.Text = "连接";
            this.btn_Connect.UseVisualStyleBackColor = false;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // btn_Disconnect
            // 
            this.btn_Disconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(90)))), ((int)(((byte)(100)))));
            this.btn_Disconnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Disconnect.Enabled = false;
            this.btn_Disconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Disconnect.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.btn_Disconnect.ForeColor = System.Drawing.Color.White;
            this.btn_Disconnect.Location = new System.Drawing.Point(685, 4);
            this.btn_Disconnect.Margin = new System.Windows.Forms.Padding(3, 4, 6, 4);
            this.btn_Disconnect.Name = "btn_Disconnect";
            this.btn_Disconnect.Size = new System.Drawing.Size(165, 44);
            this.btn_Disconnect.TabIndex = 1;
            this.btn_Disconnect.Text = "断开";
            this.btn_Disconnect.UseVisualStyleBackColor = false;
            this.btn_Disconnect.Click += new System.EventHandler(this.btn_Disconnect_Click);
            // 
            // label_State
            // 
            this.label_State.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_State.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label_State.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_State.Location = new System.Drawing.Point(4, 52);
            this.label_State.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_State.Name = "label_State";
            this.label_State.Size = new System.Drawing.Size(94, 52);
            this.label_State.TabIndex = 4;
            this.label_State.Text = "连接状态:";
            this.label_State.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // light_State
            // 
            this.light_State.Dock = System.Windows.Forms.DockStyle.Fill;
            this.light_State.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.light_State.ForeColor = System.Drawing.Color.White;
            this.light_State.Location = new System.Drawing.Point(106, 52);
            this.light_State.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.light_State.Name = "light_State";
            this.light_State.Size = new System.Drawing.Size(197, 52);
            this.light_State.TabIndex = 5;
            this.light_State.Text = "未连接";
            this.light_State.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox_IN
            // 
            this.groupBox_IN.Controls.Add(this.tableLayoutPanel_IN);
            this.groupBox_IN.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.groupBox_IN.ForeColor = System.Drawing.Color.White;
            this.groupBox_IN.Location = new System.Drawing.Point(18, 180);
            this.groupBox_IN.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox_IN.Name = "groupBox_IN";
            this.groupBox_IN.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox_IN.Size = new System.Drawing.Size(420, 360);
            this.groupBox_IN.TabIndex = 1;
            this.groupBox_IN.TabStop = false;
            this.groupBox_IN.Text = "输入 IN1~IN4 (实时状态)";
            // 
            // tableLayoutPanel_IN
            // 
            this.tableLayoutPanel_IN.ColumnCount = 2;
            this.tableLayoutPanel_IN.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_IN.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_IN.Controls.Add(this.btn_IN4, 0, 3);
            this.tableLayoutPanel_IN.Controls.Add(this.btn_IN3, 0, 2);
            this.tableLayoutPanel_IN.Controls.Add(this.btn_IN2, 0, 1);
            this.tableLayoutPanel_IN.Controls.Add(this.btn_IN1, 0, 0);
            this.tableLayoutPanel_IN.Controls.Add(this.label_IN4, 1, 3);
            this.tableLayoutPanel_IN.Controls.Add(this.label_IN3, 1, 2);
            this.tableLayoutPanel_IN.Controls.Add(this.label_IN2, 1, 1);
            this.tableLayoutPanel_IN.Controls.Add(this.label_IN1, 1, 0);
            this.tableLayoutPanel_IN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_IN.Location = new System.Drawing.Point(4, 36);
            this.tableLayoutPanel_IN.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel_IN.Name = "tableLayoutPanel_IN";
            this.tableLayoutPanel_IN.RowCount = 4;
            this.tableLayoutPanel_IN.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel_IN.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel_IN.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel_IN.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel_IN.Size = new System.Drawing.Size(412, 320);
            this.tableLayoutPanel_IN.TabIndex = 0;
            // 
            // btn_IN4
            // 
            this.btn_IN4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_IN4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_IN4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_IN4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.btn_IN4.ForeColor = System.Drawing.Color.White;
            this.btn_IN4.Location = new System.Drawing.Point(4, 244);
            this.btn_IN4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_IN4.Name = "btn_IN4";
            this.btn_IN4.Size = new System.Drawing.Size(198, 72);
            this.btn_IN4.TabIndex = 3;
            this.btn_IN4.Text = "IN4 OFF";
            this.btn_IN4.UseVisualStyleBackColor = false;
            // 
            // btn_IN3
            // 
            this.btn_IN3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_IN3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_IN3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_IN3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.btn_IN3.ForeColor = System.Drawing.Color.White;
            this.btn_IN3.Location = new System.Drawing.Point(4, 164);
            this.btn_IN3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_IN3.Name = "btn_IN3";
            this.btn_IN3.Size = new System.Drawing.Size(198, 72);
            this.btn_IN3.TabIndex = 2;
            this.btn_IN3.Text = "IN3 OFF";
            this.btn_IN3.UseVisualStyleBackColor = false;
            // 
            // btn_IN2
            // 
            this.btn_IN2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_IN2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_IN2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_IN2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.btn_IN2.ForeColor = System.Drawing.Color.White;
            this.btn_IN2.Location = new System.Drawing.Point(4, 84);
            this.btn_IN2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_IN2.Name = "btn_IN2";
            this.btn_IN2.Size = new System.Drawing.Size(198, 72);
            this.btn_IN2.TabIndex = 1;
            this.btn_IN2.Text = "IN2 OFF";
            this.btn_IN2.UseVisualStyleBackColor = false;
            // 
            // btn_IN1
            // 
            this.btn_IN1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_IN1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_IN1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_IN1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.btn_IN1.ForeColor = System.Drawing.Color.White;
            this.btn_IN1.Location = new System.Drawing.Point(4, 4);
            this.btn_IN1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_IN1.Name = "btn_IN1";
            this.btn_IN1.Size = new System.Drawing.Size(198, 72);
            this.btn_IN1.TabIndex = 0;
            this.btn_IN1.Text = "IN1 OFF";
            this.btn_IN1.UseVisualStyleBackColor = false;
            // 
            // label_IN4
            // 
            this.label_IN4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_IN4.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label_IN4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_IN4.Location = new System.Drawing.Point(210, 240);
            this.label_IN4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_IN4.Name = "label_IN4";
            this.label_IN4.Size = new System.Drawing.Size(198, 80);
            this.label_IN4.TabIndex = 4;
            this.label_IN4.Text = "预留";
            this.label_IN4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_IN3
            // 
            this.label_IN3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_IN3.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label_IN3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_IN3.Location = new System.Drawing.Point(210, 160);
            this.label_IN3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_IN3.Name = "label_IN3";
            this.label_IN3.Size = new System.Drawing.Size(198, 80);
            this.label_IN3.TabIndex = 5;
            this.label_IN3.Text = "预留";
            this.label_IN3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_IN2
            // 
            this.label_IN2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_IN2.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label_IN2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_IN2.Location = new System.Drawing.Point(210, 80);
            this.label_IN2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_IN2.Name = "label_IN2";
            this.label_IN2.Size = new System.Drawing.Size(198, 80);
            this.label_IN2.TabIndex = 6;
            this.label_IN2.Text = "计数复位";
            this.label_IN2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_IN1
            // 
            this.label_IN1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_IN1.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label_IN1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_IN1.Location = new System.Drawing.Point(210, 0);
            this.label_IN1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_IN1.Name = "label_IN1";
            this.label_IN1.Size = new System.Drawing.Size(198, 80);
            this.label_IN1.TabIndex = 7;
            this.label_IN1.Text = "触发检测";
            this.label_IN1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox_OUT
            // 
            this.groupBox_OUT.Controls.Add(this.tableLayoutPanel_OUT);
            this.groupBox_OUT.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.groupBox_OUT.ForeColor = System.Drawing.Color.White;
            this.groupBox_OUT.Location = new System.Drawing.Point(462, 180);
            this.groupBox_OUT.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox_OUT.Name = "groupBox_OUT";
            this.groupBox_OUT.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox_OUT.Size = new System.Drawing.Size(420, 360);
            this.groupBox_OUT.TabIndex = 2;
            this.groupBox_OUT.TabStop = false;
            this.groupBox_OUT.Text = "输出 OUT1~OUT4 (点击测试)";
            // 
            // tableLayoutPanel_OUT
            // 
            this.tableLayoutPanel_OUT.ColumnCount = 2;
            this.tableLayoutPanel_OUT.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_OUT.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_OUT.Controls.Add(this.btn_OUT4, 0, 3);
            this.tableLayoutPanel_OUT.Controls.Add(this.btn_OUT3, 0, 2);
            this.tableLayoutPanel_OUT.Controls.Add(this.btn_OUT2, 0, 1);
            this.tableLayoutPanel_OUT.Controls.Add(this.btn_OUT1, 0, 0);
            this.tableLayoutPanel_OUT.Controls.Add(this.label_OUT4, 1, 3);
            this.tableLayoutPanel_OUT.Controls.Add(this.label_OUT3, 1, 2);
            this.tableLayoutPanel_OUT.Controls.Add(this.label_OUT2, 1, 1);
            this.tableLayoutPanel_OUT.Controls.Add(this.label_OUT1, 1, 0);
            this.tableLayoutPanel_OUT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_OUT.Location = new System.Drawing.Point(4, 36);
            this.tableLayoutPanel_OUT.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel_OUT.Name = "tableLayoutPanel_OUT";
            this.tableLayoutPanel_OUT.RowCount = 4;
            this.tableLayoutPanel_OUT.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel_OUT.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel_OUT.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel_OUT.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel_OUT.Size = new System.Drawing.Size(412, 320);
            this.tableLayoutPanel_OUT.TabIndex = 0;
            // 
            // btn_OUT4
            // 
            this.btn_OUT4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(90)))), ((int)(((byte)(100)))));
            this.btn_OUT4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_OUT4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_OUT4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.btn_OUT4.ForeColor = System.Drawing.Color.White;
            this.btn_OUT4.Location = new System.Drawing.Point(4, 244);
            this.btn_OUT4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_OUT4.Name = "btn_OUT4";
            this.btn_OUT4.Size = new System.Drawing.Size(198, 72);
            this.btn_OUT4.TabIndex = 3;
            this.btn_OUT4.Text = "OUT4 OFF";
            this.btn_OUT4.UseVisualStyleBackColor = false;
            this.btn_OUT4.Click += new System.EventHandler(this.btn_OUT_Click);
            // 
            // btn_OUT3
            // 
            this.btn_OUT3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(90)))), ((int)(((byte)(100)))));
            this.btn_OUT3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_OUT3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_OUT3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.btn_OUT3.ForeColor = System.Drawing.Color.White;
            this.btn_OUT3.Location = new System.Drawing.Point(4, 164);
            this.btn_OUT3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_OUT3.Name = "btn_OUT3";
            this.btn_OUT3.Size = new System.Drawing.Size(198, 72);
            this.btn_OUT3.TabIndex = 2;
            this.btn_OUT3.Text = "OUT3 OFF";
            this.btn_OUT3.UseVisualStyleBackColor = false;
            this.btn_OUT3.Click += new System.EventHandler(this.btn_OUT_Click);
            // 
            // btn_OUT2
            // 
            this.btn_OUT2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(90)))), ((int)(((byte)(100)))));
            this.btn_OUT2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_OUT2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_OUT2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.btn_OUT2.ForeColor = System.Drawing.Color.White;
            this.btn_OUT2.Location = new System.Drawing.Point(4, 84);
            this.btn_OUT2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_OUT2.Name = "btn_OUT2";
            this.btn_OUT2.Size = new System.Drawing.Size(198, 72);
            this.btn_OUT2.TabIndex = 1;
            this.btn_OUT2.Text = "OUT2 OFF";
            this.btn_OUT2.UseVisualStyleBackColor = false;
            this.btn_OUT2.Click += new System.EventHandler(this.btn_OUT_Click);
            // 
            // btn_OUT1
            // 
            this.btn_OUT1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(90)))), ((int)(((byte)(100)))));
            this.btn_OUT1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_OUT1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_OUT1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.btn_OUT1.ForeColor = System.Drawing.Color.White;
            this.btn_OUT1.Location = new System.Drawing.Point(4, 4);
            this.btn_OUT1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_OUT1.Name = "btn_OUT1";
            this.btn_OUT1.Size = new System.Drawing.Size(198, 72);
            this.btn_OUT1.TabIndex = 0;
            this.btn_OUT1.Text = "OUT1 OFF";
            this.btn_OUT1.UseVisualStyleBackColor = false;
            this.btn_OUT1.Click += new System.EventHandler(this.btn_OUT_Click);
            // 
            // label_OUT4
            // 
            this.label_OUT4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_OUT4.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label_OUT4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_OUT4.Location = new System.Drawing.Point(210, 240);
            this.label_OUT4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_OUT4.Name = "label_OUT4";
            this.label_OUT4.Size = new System.Drawing.Size(198, 80);
            this.label_OUT4.TabIndex = 4;
            this.label_OUT4.Text = "蜂鸣器 (NG响)";
            this.label_OUT4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_OUT3
            // 
            this.label_OUT3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_OUT3.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label_OUT3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_OUT3.Location = new System.Drawing.Point(210, 160);
            this.label_OUT3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_OUT3.Name = "label_OUT3";
            this.label_OUT3.Size = new System.Drawing.Size(198, 80);
            this.label_OUT3.TabIndex = 5;
            this.label_OUT3.Text = "黄灯 (运行中)";
            this.label_OUT3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_OUT2
            // 
            this.label_OUT2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_OUT2.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label_OUT2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_OUT2.Location = new System.Drawing.Point(210, 80);
            this.label_OUT2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_OUT2.Name = "label_OUT2";
            this.label_OUT2.Size = new System.Drawing.Size(198, 80);
            this.label_OUT2.TabIndex = 6;
            this.label_OUT2.Text = "红灯 (NG)";
            this.label_OUT2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_OUT1
            // 
            this.label_OUT1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_OUT1.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label_OUT1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_OUT1.Location = new System.Drawing.Point(210, 0);
            this.label_OUT1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_OUT1.Name = "label_OUT1";
            this.label_OUT1.Size = new System.Drawing.Size(198, 80);
            this.label_OUT1.TabIndex = 7;
            this.label_OUT1.Text = "绿灯 (OK)";
            this.label_OUT1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_ResetAll
            // 
            this.btn_ResetAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(80)))), ((int)(((byte)(60)))));
            this.btn_ResetAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ResetAll.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Bold);
            this.btn_ResetAll.ForeColor = System.Drawing.Color.White;
            this.btn_ResetAll.Location = new System.Drawing.Point(462, 549);
            this.btn_ResetAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_ResetAll.Name = "btn_ResetAll";
            this.btn_ResetAll.Size = new System.Drawing.Size(420, 54);
            this.btn_ResetAll.TabIndex = 3;
            this.btn_ResetAll.Text = "全部输出复位";
            this.btn_ResetAll.UseVisualStyleBackColor = false;
            this.btn_ResetAll.Click += new System.EventHandler(this.btn_ResetAll_Click);
            // 
            // richTextBox_log
            // 
            this.richTextBox_log.BackColor = System.Drawing.Color.Black;
            this.richTextBox_log.Font = new System.Drawing.Font("Consolas", 10F);
            this.richTextBox_log.ForeColor = System.Drawing.Color.Lime;
            this.richTextBox_log.Location = new System.Drawing.Point(18, 624);
            this.richTextBox_log.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.richTextBox_log.Name = "richTextBox_log";
            this.richTextBox_log.ReadOnly = true;
            this.richTextBox_log.Size = new System.Drawing.Size(862, 268);
            this.richTextBox_log.TabIndex = 4;
            this.richTextBox_log.Text = "";
            // 
            // label_protocol
            // 
            this.label_protocol.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label_protocol.ForeColor = System.Drawing.Color.Gray;
            this.label_protocol.Location = new System.Drawing.Point(18, 903);
            this.label_protocol.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_protocol.Name = "label_protocol";
            this.label_protocol.Size = new System.Drawing.Size(864, 45);
            this.label_protocol.TabIndex = 5;
            this.label_protocol.Text = "协议(Modbus-RTU over TCP): 读输入 01 02 00 00 00 08 | 写线圈 01 05 00 0n FF 00/00 00 | CR" +
    "C16低字节在前";
            // 
            // timer_Refresh
            // 
            this.timer_Refresh.Interval = 200;
            this.timer_Refresh.Tick += new System.EventHandler(this.timer_Refresh_Tick);
            // 
            // IOBoardTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(900, 960);
            this.Controls.Add(this.label_protocol);
            this.Controls.Add(this.richTextBox_log);
            this.Controls.Add(this.btn_ResetAll);
            this.Controls.Add(this.groupBox_OUT);
            this.Controls.Add(this.groupBox_IN);
            this.Controls.Add(this.groupBox_Conn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "IOBoardTestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "网口IO卡通讯测试 (网口IO板)";
            this.groupBox_Conn.ResumeLayout(false);
            this.tableLayoutPanel_Conn.ResumeLayout(false);
            this.tableLayoutPanel_Conn.PerformLayout();
            this.groupBox_IN.ResumeLayout(false);
            this.tableLayoutPanel_IN.ResumeLayout(false);
            this.groupBox_OUT.ResumeLayout(false);
            this.tableLayoutPanel_OUT.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_Conn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Conn;
        private System.Windows.Forms.Label label_IP;
        private System.Windows.Forms.TextBox txt_IP;
        private System.Windows.Forms.Label label_Port;
        private System.Windows.Forms.TextBox txt_Port;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.Button btn_Disconnect;
        private System.Windows.Forms.Label label_State;
        private System.Windows.Forms.Label light_State;
        private System.Windows.Forms.GroupBox groupBox_IN;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_IN;
        private System.Windows.Forms.Button btn_IN1;
        private System.Windows.Forms.Button btn_IN2;
        private System.Windows.Forms.Button btn_IN3;
        private System.Windows.Forms.Button btn_IN4;
        private System.Windows.Forms.Label label_IN1;
        private System.Windows.Forms.Label label_IN2;
        private System.Windows.Forms.Label label_IN3;
        private System.Windows.Forms.Label label_IN4;
        private System.Windows.Forms.GroupBox groupBox_OUT;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_OUT;
        private System.Windows.Forms.Button btn_OUT1;
        private System.Windows.Forms.Button btn_OUT2;
        private System.Windows.Forms.Button btn_OUT3;
        private System.Windows.Forms.Button btn_OUT4;
        private System.Windows.Forms.Label label_OUT1;
        private System.Windows.Forms.Label label_OUT2;
        private System.Windows.Forms.Label label_OUT3;
        private System.Windows.Forms.Label label_OUT4;
        private System.Windows.Forms.Button btn_ResetAll;
        private System.Windows.Forms.RichTextBox richTextBox_log;
        private System.Windows.Forms.Label label_protocol;
        private System.Windows.Forms.Timer timer_Refresh;
    }
}
