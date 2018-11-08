using NAudio.Wave;
using System;

namespace SpeechProcessing.Recorder
{
    class NAudioRecorder 
    {
        public delegate void UpdateProgress(int value);
        private UpdateProgress ups=null;
        private IWaveIn waveSource = null;
        private WaveFileWriter waveFile = null;
        private string fileName = string.Empty;
        public string FileName
        {
            get { return this.fileName; }
            set { this.fileName = value; }
        }
        private byte mode = 0;
        public NAudioRecorder(byte mode,UpdateProgress ups=null)
        {
            this.mode = mode;
            this.ups = ups;
        }
        /// <summary>
        /// 开始录音
        /// </summary>
        public void StartRec()
        {
            switch (this.mode)
            {
                case 0:
                    waveSource = new WaveIn();//麦克风录制
                    waveSource.WaveFormat = new WaveFormat(16000, 16, 1); // 16bit,16KHz,Mono的录音格式
                    break;
                case 1:
                    waveSource = new WasapiLoopbackCapture();//声卡录制,此模式下无法设置wave格式
                    break;
            }
            
            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);
            waveFile = new WaveFileWriter(fileName, waveSource.WaveFormat);
            waveSource.StartRecording();
        }

        /// <summary>
        /// 停止录音
        /// </summary>
        public void StopRec()
        {
            waveSource.StopRecording();

            // Close Wave(Not needed under synchronous situation)
            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }  
        }

        /// <summary>
        /// 开始录音回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                waveFile.Flush();
                int secondsRecorded = (int)(waveFile.Length / waveFile.WaveFormat.AverageBytesPerSecond);
                if (secondsRecorded >= 600)
                {
                    StopRec();
                }
                else
                {
                    if (this.ups != null) this.ups(secondsRecorded);
                }
            }
        }

        /// <summary>
        /// 录音结束回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }

            if (this.ups != null) this.ups(0);
        }
    }
}