namespace AutoExam
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnGetAnswers = new System.Windows.Forms.Button();
            this.btnAutoExam = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGetAnswers
            // 
            this.btnGetAnswers.Location = new System.Drawing.Point(31, 12);
            this.btnGetAnswers.Name = "btnGetAnswers";
            this.btnGetAnswers.Size = new System.Drawing.Size(138, 42);
            this.btnGetAnswers.TabIndex = 0;
            this.btnGetAnswers.Text = "获取题目和答案";
            this.btnGetAnswers.UseVisualStyleBackColor = true;
            this.btnGetAnswers.Click += new System.EventHandler(this.btnGetAnswers_Click);
            // 
            // btnAutoExam
            // 
            this.btnAutoExam.Location = new System.Drawing.Point(31, 85);
            this.btnAutoExam.Name = "btnAutoExam";
            this.btnAutoExam.Size = new System.Drawing.Size(138, 42);
            this.btnAutoExam.TabIndex = 1;
            this.btnAutoExam.Text = "自动考试";
            this.btnAutoExam.UseVisualStyleBackColor = true;
            this.btnAutoExam.Click += new System.EventHandler(this.btnAutoExam_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 174);
            this.Controls.Add(this.btnAutoExam);
            this.Controls.Add(this.btnGetAnswers);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGetAnswers;
        private System.Windows.Forms.Button btnAutoExam;
    }
}

