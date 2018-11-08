namespace DzhSample
{
    partial class Form1
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

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.finToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.dynToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topmostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.showToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(870, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveAsTextToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.fileToolStripMenuItem.Text = "文件";
            // 
            // SaveAsTextToolStripMenuItem
            // 
            this.SaveAsTextToolStripMenuItem.Name = "SaveAsTextToolStripMenuItem";
            this.SaveAsTextToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.SaveAsTextToolStripMenuItem.Text = "保存到文本...";
            this.SaveAsTextToolStripMenuItem.Click += new System.EventHandler(this.SaveAsTextToolStripMenuItem_Click);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timeToolStripMenuItem,
            this.tickToolStripMenuItem,
            this.finToolStripMenuItem,
            this.toolStripSeparator1,
            this.dynToolStripMenuItem,
            this.topmostToolStripMenuItem});
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.showToolStripMenuItem.Text = "显示";
            // 
            // timeToolStripMenuItem
            // 
            this.timeToolStripMenuItem.Name = "timeToolStripMenuItem";
            this.timeToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.timeToolStripMenuItem.Text = "时间序列";
            this.timeToolStripMenuItem.Click += new System.EventHandler(this.timeToolStripMenuItem_Click);
            // 
            // tickToolStripMenuItem
            // 
            this.tickToolStripMenuItem.Name = "tickToolStripMenuItem";
            this.tickToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.tickToolStripMenuItem.Text = "分笔";
            this.tickToolStripMenuItem.Click += new System.EventHandler(this.tickToolStripMenuItem_Click);
            // 
            // finToolStripMenuItem
            // 
            this.finToolStripMenuItem.Name = "finToolStripMenuItem";
            this.finToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.finToolStripMenuItem.Text = "财务";
            this.finToolStripMenuItem.Click += new System.EventHandler(this.finToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(139, 6);
            // 
            // dynToolStripMenuItem
            // 
            this.dynToolStripMenuItem.Name = "dynToolStripMenuItem";
            this.dynToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.dynToolStripMenuItem.Text = "动态刷新数据";
            this.dynToolStripMenuItem.Click += new System.EventHandler(this.dynToolStripMenuItem_Click);
            // 
            // topmostToolStripMenuItem
            // 
            this.topmostToolStripMenuItem.Name = "topmostToolStripMenuItem";
            this.topmostToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.topmostToolStripMenuItem.Text = "窗口在最前端";
            this.topmostToolStripMenuItem.Click += new System.EventHandler(this.topmostToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 383);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveAsTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tickToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dynToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topmostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem finToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

    }
}