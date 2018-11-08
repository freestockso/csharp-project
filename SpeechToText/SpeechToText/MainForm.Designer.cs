namespace SpeechToText
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textFileName = new System.Windows.Forms.TextBox();
            this.cmｄOpen = new System.Windows.Forms.Button();
            this.textResult = new System.Windows.Forms.TextBox();
            this.cmdTrans = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.radMaike = new System.Windows.Forms.RadioButton();
            this.radSoundCard = new System.Windows.Forms.RadioButton();
            this.btnCapture = new System.Windows.Forms.Button();
            this.btnStopCapture = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // textFileName
            // 
            this.textFileName.Location = new System.Drawing.Point(59, 10);
            this.textFileName.Name = "textFileName";
            this.textFileName.Size = new System.Drawing.Size(460, 21);
            this.textFileName.TabIndex = 0;
            // 
            // cmｄOpen
            // 
            this.cmｄOpen.AutoSize = true;
            this.cmｄOpen.Location = new System.Drawing.Point(538, 9);
            this.cmｄOpen.Name = "cmｄOpen";
            this.cmｄOpen.Size = new System.Drawing.Size(87, 23);
            this.cmｄOpen.TabIndex = 1;
            this.cmｄOpen.Text = "打开音频文件";
            this.cmｄOpen.UseVisualStyleBackColor = true;
            this.cmｄOpen.Click += new System.EventHandler(this.cmｄOpen_Click);
            // 
            // textResult
            // 
            this.textResult.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textResult.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.textResult.Location = new System.Drawing.Point(12, 55);
            this.textResult.Multiline = true;
            this.textResult.Name = "textResult";
            this.textResult.Size = new System.Drawing.Size(706, 241);
            this.textResult.TabIndex = 2;
            // 
            // cmdTrans
            // 
            this.cmdTrans.Location = new System.Drawing.Point(643, 9);
            this.cmdTrans.Name = "cmdTrans";
            this.cmdTrans.Size = new System.Drawing.Size(75, 23);
            this.cmdTrans.TabIndex = 3;
            this.cmdTrans.Text = "识别文字";
            this.cmdTrans.UseVisualStyleBackColor = true;
            this.cmdTrans.Click += new System.EventHandler(this.cmdTrans_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "文件：";
            // 
            // radMaike
            // 
            this.radMaike.AutoSize = true;
            this.radMaike.Location = new System.Drawing.Point(743, 55);
            this.radMaike.Name = "radMaike";
            this.radMaike.Size = new System.Drawing.Size(59, 16);
            this.radMaike.TabIndex = 5;
            this.radMaike.Text = "麦克风";
            this.radMaike.UseVisualStyleBackColor = true;
            // 
            // radSoundCard
            // 
            this.radSoundCard.AutoSize = true;
            this.radSoundCard.Checked = true;
            this.radSoundCard.Location = new System.Drawing.Point(743, 88);
            this.radSoundCard.Name = "radSoundCard";
            this.radSoundCard.Size = new System.Drawing.Size(47, 16);
            this.radSoundCard.TabIndex = 6;
            this.radSoundCard.TabStop = true;
            this.radSoundCard.Text = "声卡";
            this.radSoundCard.UseVisualStyleBackColor = true;
            // 
            // btnCapture
            // 
            this.btnCapture.Location = new System.Drawing.Point(743, 165);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(75, 23);
            this.btnCapture.TabIndex = 7;
            this.btnCapture.Text = "开始录音";
            this.btnCapture.UseVisualStyleBackColor = true;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // btnStopCapture
            // 
            this.btnStopCapture.Enabled = false;
            this.btnStopCapture.Location = new System.Drawing.Point(743, 214);
            this.btnStopCapture.Name = "btnStopCapture";
            this.btnStopCapture.Size = new System.Drawing.Size(75, 23);
            this.btnStopCapture.TabIndex = 8;
            this.btnStopCapture.Text = "停止录音";
            this.btnStopCapture.UseVisualStyleBackColor = true;
            this.btnStopCapture.Click += new System.EventHandler(this.btnStopCapture_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 331);
            this.progressBar1.Maximum = 600;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(706, 23);
            this.progressBar1.TabIndex = 9;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(12, 313);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(77, 12);
            this.lblProgress.TabIndex = 10;
            this.lblProgress.Text = "当前已录制：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(611, 313);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "最长录制时间6分钟";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SpeechToText.Properties.Resources.timg;
            this.pictureBox1.Location = new System.Drawing.Point(743, 273);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(75, 52);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(759, 342);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "请等待...";
            this.label2.UseWaitCursor = true;
            this.label2.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 367);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnStopCapture);
            this.Controls.Add(this.btnCapture);
            this.Controls.Add(this.radSoundCard);
            this.Controls.Add(this.radMaike);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdTrans);
            this.Controls.Add(this.textResult);
            this.Controls.Add(this.cmｄOpen);
            this.Controls.Add(this.textFileName);
            this.Name = "MainForm";
            this.Text = "语音转写";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textFileName;
        private System.Windows.Forms.Button cmｄOpen;
        private System.Windows.Forms.TextBox textResult;
        private System.Windows.Forms.Button cmdTrans;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radMaike;
        private System.Windows.Forms.RadioButton radSoundCard;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Button btnStopCapture;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
    }
}

