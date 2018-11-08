namespace PlateMonitor
{
    partial class MainForm
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
            //中断更新线程
            if (this.updateThread != null && this.updateThread.ThreadState != System.Threading.ThreadState.Stopped)
            {
                this.updateThread.Abort();
            }
            //中断计算线程
            if (this.calculateThread != null && this.calculateThread.ThreadState != System.Threading.ThreadState.Stopped)
            {
                this.calculateThread.Abort();
            }
            //中断保存线程
            if (this.saveThread != null && this.saveThread.ThreadState != System.Threading.ThreadState.Stopped)
            {
                this.saveThread.Abort();
            }
            //保存应用程序设置
            global::PlateMonitor.Properties.Settings.Default.Save();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dgvIndustries = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgvConcepts = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dgvAreas = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.tbInterval = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.掘金量化行情配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.东方财富板块路径配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbUpdating = new System.Windows.Forms.CheckBox();
            this.lblPlateName = new System.Windows.Forms.Label();
            this.btnShowAll = new System.Windows.Forms.Button();
            this.dgvStocks = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.tssLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIndustries)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConcepts)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAreas)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStocks)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 31);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(476, 364);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgvIndustries);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(468, 338);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "行业板块";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgvIndustries
            // 
            this.dgvIndustries.AllowUserToAddRows = false;
            this.dgvIndustries.AllowUserToDeleteRows = false;
            this.dgvIndustries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIndustries.Location = new System.Drawing.Point(3, 3);
            this.dgvIndustries.Name = "dgvIndustries";
            this.dgvIndustries.ReadOnly = true;
            this.dgvIndustries.RowHeadersVisible = false;
            this.dgvIndustries.RowTemplate.Height = 23;
            this.dgvIndustries.Size = new System.Drawing.Size(460, 330);
            this.dgvIndustries.TabIndex = 0;
            this.dgvIndustries.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIndustries_CellClick);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgvConcepts);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(468, 338);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "概念板块";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvConcepts
            // 
            this.dgvConcepts.AllowUserToAddRows = false;
            this.dgvConcepts.AllowUserToDeleteRows = false;
            this.dgvConcepts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConcepts.Location = new System.Drawing.Point(3, 3);
            this.dgvConcepts.Name = "dgvConcepts";
            this.dgvConcepts.ReadOnly = true;
            this.dgvConcepts.RowHeadersVisible = false;
            this.dgvConcepts.RowTemplate.Height = 23;
            this.dgvConcepts.Size = new System.Drawing.Size(460, 330);
            this.dgvConcepts.TabIndex = 0;
            this.dgvConcepts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvConcepts_CellClick);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dgvAreas);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(468, 338);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "地区板块";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgvAreas
            // 
            this.dgvAreas.AllowUserToAddRows = false;
            this.dgvAreas.AllowUserToDeleteRows = false;
            this.dgvAreas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAreas.Location = new System.Drawing.Point(3, 3);
            this.dgvAreas.Name = "dgvAreas";
            this.dgvAreas.ReadOnly = true;
            this.dgvAreas.RowHeadersVisible = false;
            this.dgvAreas.RowTemplate.Height = 23;
            this.dgvAreas.Size = new System.Drawing.Size(460, 330);
            this.dgvAreas.TabIndex = 0;
            this.dgvAreas.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAreas_CellClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(121, 402);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "间隔秒数：";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // tbInterval
            // 
            this.tbInterval.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::PlateMonitor.Properties.Settings.Default, "tbInterval", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, null, "N0"));
            this.tbInterval.Location = new System.Drawing.Point(192, 399);
            this.tbInterval.Name = "tbInterval";
            this.tbInterval.Size = new System.Drawing.Size(61, 21);
            this.tbInterval.TabIndex = 5;
            this.tbInterval.Text = "10";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.设置ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(993, 25);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.保存ToolStripMenuItem,
            this.toolStripMenuItem2});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 保存ToolStripMenuItem
            // 
            this.保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            this.保存ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.保存ToolStripMenuItem.Text = "保存";
            this.保存ToolStripMenuItem.Click += new System.EventHandler(this.保存ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItem2.Text = "退出";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // 设置ToolStripMenuItem
            // 
            this.设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.掘金量化行情配置ToolStripMenuItem,
            this.东方财富板块路径配置ToolStripMenuItem});
            this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            this.设置ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.设置ToolStripMenuItem.Text = "设置";
            // 
            // 掘金量化行情配置ToolStripMenuItem
            // 
            this.掘金量化行情配置ToolStripMenuItem.Name = "掘金量化行情配置ToolStripMenuItem";
            this.掘金量化行情配置ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.掘金量化行情配置ToolStripMenuItem.Text = "掘金量化行情配置";
            this.掘金量化行情配置ToolStripMenuItem.Click += new System.EventHandler(this.掘金量化行情配置ToolStripMenuItem_Click);
            // 
            // 东方财富板块路径配置ToolStripMenuItem
            // 
            this.东方财富板块路径配置ToolStripMenuItem.Name = "东方财富板块路径配置ToolStripMenuItem";
            this.东方财富板块路径配置ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.东方财富板块路径配置ToolStripMenuItem.Text = "东方财富板块路径配置";
            this.东方财富板块路径配置ToolStripMenuItem.Click += new System.EventHandler(this.东方财富板块路径配置ToolStripMenuItem_Click);
            // 
            // cbUpdating
            // 
            this.cbUpdating.AutoSize = true;
            this.cbUpdating.Checked = global::PlateMonitor.Properties.Settings.Default.cbUpdating;
            this.cbUpdating.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUpdating.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::PlateMonitor.Properties.Settings.Default, "cbUpdating", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbUpdating.Location = new System.Drawing.Point(19, 401);
            this.cbUpdating.Name = "cbUpdating";
            this.cbUpdating.Size = new System.Drawing.Size(96, 16);
            this.cbUpdating.TabIndex = 3;
            this.cbUpdating.Text = "是否实时更新";
            this.cbUpdating.UseVisualStyleBackColor = true;
            // 
            // lblPlateName
            // 
            this.lblPlateName.AutoSize = true;
            this.lblPlateName.Location = new System.Drawing.Point(496, 37);
            this.lblPlateName.Name = "lblPlateName";
            this.lblPlateName.Size = new System.Drawing.Size(65, 12);
            this.lblPlateName.TabIndex = 9;
            this.lblPlateName.Text = "当前板块：";
            // 
            // btnShowAll
            // 
            this.btnShowAll.Location = new System.Drawing.Point(903, 28);
            this.btnShowAll.Name = "btnShowAll";
            this.btnShowAll.Size = new System.Drawing.Size(75, 23);
            this.btnShowAll.TabIndex = 10;
            this.btnShowAll.Text = "显示全部";
            this.btnShowAll.UseVisualStyleBackColor = true;
            this.btnShowAll.Click += new System.EventHandler(this.btnShowAll_Click);
            // 
            // dgvStocks
            // 
            this.dgvStocks.AllowUserToAddRows = false;
            this.dgvStocks.AllowUserToDeleteRows = false;
            this.dgvStocks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStocks.Location = new System.Drawing.Point(498, 56);
            this.dgvStocks.Name = "dgvStocks";
            this.dgvStocks.ReadOnly = true;
            this.dgvStocks.RowHeadersVisible = false;
            this.dgvStocks.RowTemplate.Height = 23;
            this.dgvStocks.Size = new System.Drawing.Size(480, 330);
            this.dgvStocks.TabIndex = 8;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssLabel,
            this.tssLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 426);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(993, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssLabel
            // 
            this.tssLabel.Name = "tssLabel";
            this.tssLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // btnCalculate
            // 
            this.btnCalculate.Location = new System.Drawing.Point(832, 397);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(145, 23);
            this.btnCalculate.TabIndex = 12;
            this.btnCalculate.Text = "计算各股是否符合条件";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // tssLabel2
            // 
            this.tssLabel2.Name = "tssLabel2";
            this.tssLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(993, 448);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnShowAll);
            this.Controls.Add(this.lblPlateName);
            this.Controls.Add(this.dgvStocks);
            this.Controls.Add(this.tbInterval);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbUpdating);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "板块监控";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvIndustries)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvConcepts)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAreas)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStocks)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dgvIndustries;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dgvConcepts;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dgvAreas;
        private System.Windows.Forms.CheckBox cbUpdating;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbInterval;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 掘金量化行情配置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 东方财富板块路径配置ToolStripMenuItem;
        private System.Windows.Forms.Label lblPlateName;
        private System.Windows.Forms.Button btnShowAll;
        private System.Windows.Forms.DataGridView dgvStocks;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssLabel;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.ToolStripStatusLabel tssLabel2;
    }
}

