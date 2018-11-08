using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using SpeechProcessing;
using SpeechProcessing.Recorder;

namespace SpeechToText
{
    public partial class MainForm : Form
    {
        private NAudioRecorder recorder=null;
        public MainForm()
        {
            InitializeComponent();
        }

        private void cmｄOpen_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "wav文件|*.wav|mp3文件|*.mp3|所有文件|*.*";
            this.openFileDialog1.RestoreDirectory = true;
            DialogResult ret=this.openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                this.textFileName.Text = this.openFileDialog1.FileName;
            }
        }

        private void cmdTrans_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.RunWorkerAsync();
            this.pictureBox1.Show();
            this.label2.Show();
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (this.radSoundCard.Checked) mode = 1;
            this.recorder = new NAudioRecorder(mode,this.updateProgress);
            string path=Application.StartupPath+@"\Capture\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            this.recorder.FileName=path+ String.Format("Audio {0:yyy-MM-dd HH-mm-ss}.wav", DateTime.Now);
            this.recorder.StartRec();
            this.btnStopCapture.Enabled= true;
            this.btnCapture.Enabled = false;
        }

        private void btnStopCapture_Click(object sender, EventArgs e)
        {
            if (this.recorder != null)
            {
                this.recorder.StopRec();
                this.btnCapture.Enabled = true;
                this.btnStopCapture.Enabled = false;
                this.textFileName.Text = this.recorder.FileName;
                this.recorder = null;
            }
        }
        private delegate void RefreshProgressBarDelegate(ProgressBar pb, int value);
        private delegate void RefreshLabelDelegate(Label lbl, string text);
        private delegate void RefreshTextBoxDelegate(TextBox tb, string text);
        private void refreshProgressBar(ProgressBar pb,int value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RefreshProgressBarDelegate(this.refreshProgressBar), pb, value);
            }
            else
            {
                pb.Value = value;
            }
        }
        private void refreshLabel(Label lbl ,string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RefreshLabelDelegate(this.refreshLabel), lbl, text);
            }else
            {
                lbl.Text = text;
            }
        }
        private void updateProgress(int value)
        {
            this.refreshProgressBar(this.progressBar1, value);
            this.refreshLabel(this.lblProgress, "当前已录制：" + value+" 秒");
        }
        private void refreshTextBox(TextBox tb, string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RefreshTextBoxDelegate(refreshTextBox), tb, text);
            }else
            {
                tb.Text = text;
            }
        }
        private void updateResultText(string result)
        {
            this.refreshTextBox(this.textResult, result);
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // 设置APPID/AK/SK
            var APP_ID = "14590738";
            var API_KEY = "UvxeakvvBlXuwGGrnD3ebUc0";
            var SECRET_KEY = "RI5QTsSSsNxqLjNMDsFYuXdDIuQ0jgC9";
            var client = new Baidu.Aip.Speech.Asr(APP_ID, API_KEY, SECRET_KEY);
            string fileName = ClassUtils.checkAudio(this.textFileName.Text);
            var data = File.ReadAllBytes(fileName);
            client.Timeout = 120000; // 若语音较长，建议设置更大的超时时间. ms
            var result = client.Recognize(data, "pcm", 16000);
            if ((int)result.GetValue("err_no") != 0)
            {
                MessageBox.Show(result.GetValue("err_msg").ToString(), "错误提示");
            }
            else
            {
                updateResultText(result.GetValue("result")[0].ToString());
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.pictureBox1.Hide();
            this.label2.Hide();
        }
    }
}
