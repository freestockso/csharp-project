using NAudio.Wave;
using SpeechProcessing.Model;
using System;
using System.IO;

namespace SpeechProcessing
{
    class ClassUtils
    {
        /// <summary>
        /// 将当前时间转为秒
        /// </summary>
        /// <returns>秒数</returns>
        public static long CurrentTime2Second()
        {
            string currentTime = DateTime.Now.ToString();
            DateTime dt = new DateTime(1970, 1, 1);
            TimeSpan d = DateTime.Parse(currentTime) - dt;
            long totalSeconds = d.Ticks / 10000000; // turn current time to seconds

            return totalSeconds;
        }

        public static string checkAudio(string fileName)
        {
            WavInfo wav = ClassUtils.GetWavInfo(fileName);

            //数据量 = (采样频率 × 采样位数 × 声道数 × 时间) / 8
            // 非8k/16k, 16bit 位深, 单声道的，进行格式转换
            if (fileName.EndsWith(".mp3", StringComparison.CurrentCultureIgnoreCase)
                || int.Parse(wav.dwsamplespersec.ToString()) != 16000
                || int.Parse(wav.wbitspersample.ToString()) != 16
                || int.Parse(wav.wchannels.ToString()) != 1)
            {
                fileName = ClassUtils.Convert2Wav(fileName); // convert audio file to 16k，16bit wav
            }
            return fileName;
        }
        /// <summary>
        /// 将.mp3或者其他.wav文件转为16kHz，16bit的.wav（by NAudio）
        /// </summary>
        /// <param name="filePath">转换前音频文件的路径</param>
        /// <returns>转换后音频文件的路径</returns>
        public static string Convert2Wav(string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            string tempDir = directoryName + "\\temp" + DateTime.Now.ToString("yyyyMMddHHmmss") + "\\";

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            if (filePath.EndsWith(".wav", StringComparison.CurrentCultureIgnoreCase))
            {
                //using (WaveFileReader reader = new WaveFileReader(filePath))
                using (MediaFoundationReader reader = new MediaFoundationReader(filePath))
                {
                    var newFormat = new WaveFormat(16000, 16, 1); // 16kHz, 16bit
                    using (var conversionStream = new WaveFormatConversionStream(newFormat, reader))
                    {
                        WaveFileWriter.CreateWaveFile(tempDir + fileName, conversionStream);
                    }
                }
            }
            else if (filePath.EndsWith(".mp3", StringComparison.CurrentCultureIgnoreCase))
            {
                using (Mp3FileReader reader = new Mp3FileReader(filePath))
                {
                    var newFormat = new WaveFormat(16000, 16, 1); // 16kHz, 16bit
                    using (var conversionStream = new WaveFormatConversionStream(newFormat, reader))
                    {
                        WaveFileWriter.CreateWaveFile(tempDir + fileName, conversionStream);
                    }
                }
            }

            return tempDir + fileName;
        }

        /// <summary>
        /// 取出WAV头信息
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static WavInfo GetWavInfo(string filePath)
        {
            WavInfo wavInfo = new WavInfo();
            FileInfo fi = new FileInfo(filePath);
            using (System.IO.FileStream fs = fi.OpenRead())
            {
                if (fs.Length >= 44)
                {
                    byte[] bInfo = new byte[44];
                    fs.Read(bInfo, 0, 44);
                    System.Text.Encoding.Default.GetString(bInfo, 0, 4);
                    if (System.Text.Encoding.Default.GetString(bInfo, 0, 4) == "RIFF" && System.Text.Encoding.Default.GetString(bInfo, 8, 4) == "WAVE" && System.Text.Encoding.Default.GetString(bInfo, 12, 4) == "fmt ")
                    {
                        wavInfo.groupid = System.Text.Encoding.Default.GetString(bInfo, 0, 4);
                        System.BitConverter.ToInt32(bInfo, 4);
                        wavInfo.filesize = System.BitConverter.ToInt32(bInfo, 4);
                        //wavInfo.filesize = Convert.ToInt64(System.Text.Encoding.Default.GetString(bInfo,4,4));
                        wavInfo.rifftype = System.Text.Encoding.Default.GetString(bInfo, 8, 4);
                        wavInfo.chunkid = System.Text.Encoding.Default.GetString(bInfo, 12, 4);
                        wavInfo.chunksize = System.BitConverter.ToInt32(bInfo, 16);
                        wavInfo.wformattag = System.BitConverter.ToInt16(bInfo, 20);
                        wavInfo.wchannels = System.BitConverter.ToUInt16(bInfo, 22);
                        wavInfo.dwsamplespersec = System.BitConverter.ToUInt32(bInfo, 24);
                        wavInfo.dwavgbytespersec = System.BitConverter.ToUInt32(bInfo, 28);
                        wavInfo.wblockalign = System.BitConverter.ToUInt16(bInfo, 32);
                        wavInfo.wbitspersample = System.BitConverter.ToUInt16(bInfo, 34);
                        wavInfo.datachunkid = System.Text.Encoding.Default.GetString(bInfo, 36, 4);
                        wavInfo.datasize = System.BitConverter.ToInt32(bInfo, 40);
                    }
                }
            }

            return wavInfo;
        }


        public static void TrimWavFile(string inPath, string outPath, TimeSpan cutFromStart, TimeSpan cutFromEnd)
        {
            using (WaveFileReader reader = new WaveFileReader(inPath))
            {
                using (WaveFileWriter writer = new WaveFileWriter(outPath, reader.WaveFormat))
                {
                    int bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000;
                    int startPos = (int)cutFromStart.TotalMilliseconds * bytesPerMillisecond;
                    startPos = startPos - startPos % reader.WaveFormat.BlockAlign;
                    int endPos = (int)cutFromEnd.TotalMilliseconds * bytesPerMillisecond;
                    endPos = endPos - endPos % reader.WaveFormat.BlockAlign;
                    TrimWavFile(reader, writer, startPos, endPos);
                }
            }
        }
        private static void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
        {
            reader.Position = startPos;
            byte[] buffer = new byte[1024];
            while (reader.Position < endPos)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }
}