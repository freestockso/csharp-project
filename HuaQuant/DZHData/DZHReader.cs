using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HuaQuant.Data.DZH
{
    public class DZHReader:IDisposable
    {
        protected struct FileHandle{
            public FileStream stream;
            public BinaryReader reader;
        };
        protected string dataPath;
        protected string fileName;
        protected long indexStartOffset, dataStartOffset;
        protected int indexRecordSize, dataRecordSize, dataBlockSize;
        protected string market;

        protected uint securityCount;
        protected long securityCountOffset;

        protected Dictionary<string, Dictionary<string, long>> indexBuffer = new Dictionary<string, Dictionary<string, long>>();
        protected Dictionary<string, FileHandle> fileHandleBuffer = new Dictionary<string, FileHandle>();
        
        protected DateTime date19700101 = new DateTime(1970, 1, 1);
        protected FileStream fileStream;
        protected BinaryReader reader;

        private bool disposed=false;
        //构造函数
        protected DZHReader(string path)
        {
            dataPath = path.Trim();
            if (dataPath != "" && !dataPath.EndsWith(@"\"))
            {
                dataPath += @"\";
            }
        }
        ~DZHReader()
        {
            Dispose(false);
        }
        //设置市场
        protected void SetMarket(string market)
        {
            this.market = market.Trim();
            string file = (dataPath + market + @"\" + fileName).ToUpper();
            if ((fileStream == null) || (file != fileStream.Name))  FileChanged(file);  
        }
        //当文件发生改变，打开文件，创建文件流和二进制读取器,创建索引容器
        protected void FileChanged(string file)
        {
            if (fileHandleBuffer.ContainsKey(this.market))
            {
                FileHandle fileHandle = fileHandleBuffer[market];
                fileStream = fileHandle.stream;
                reader = fileHandle.reader;
            }
            else
            {
                if (!File.Exists(file)) throw new FileNotFoundException("The file: " + file + " is not exists.");
                try
                {
                    fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    reader = new BinaryReader(fileStream);
                    fileHandleBuffer[this.market] = new FileHandle { stream = fileStream, reader = reader };
                    if (!indexBuffer.ContainsKey(this.market))
                    {
                        Dictionary<string, long> indexs = new Dictionary<string, long>();
                        indexBuffer.Add(this.market, indexs);
                    }  
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            OnFileChanged();
        }

        protected virtual void OnFileChanged()
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing) //托管资源释放
                {
                    indexBuffer = null;
                }
                //非托管资源释放
                foreach (KeyValuePair<string, FileHandle> kvp in fileHandleBuffer)
                {
                    if (kvp.Value.stream != null) kvp.Value.stream.Close();
                    if (kvp.Value.reader != null) kvp.Value.reader.Close();
                }
                disposed = true;
            }
        }
    }
}
