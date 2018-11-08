namespace GFTrader
{
    partial class CodeInput
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textCode = new System.Windows.Forms.TextBox();
            this.Ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "验证码：";
            // 
            // textCode
            // 
            this.textCode.Location = new System.Drawing.Point(97, 13);
            this.textCode.Name = "textCode";
            this.textCode.Size = new System.Drawing.Size(204, 25);
            this.textCode.TabIndex = 1;
            this.textCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textCode_KeyDown);
            // 
            // Ok
            // 
            this.Ok.Location = new System.Drawing.Point(319, 13);
            this.Ok.Name = "Ok";
            this.Ok.Size = new System.Drawing.Size(75, 25);
            this.Ok.TabIndex = 2;
            this.Ok.Text = "确定";
            this.Ok.UseVisualStyleBackColor = true;
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // CodeInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 55);
            this.Controls.Add(this.Ok);
            this.Controls.Add(this.textCode);
            this.Controls.Add(this.label1);
            this.Name = "CodeInput";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "请输入验证码";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textCode;
        private System.Windows.Forms.Button Ok;
    }
}