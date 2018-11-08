using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.DZH
{
    public class DZHBarReader:DZHBlockReader
    {
        protected DZHBarReader(string path)
            : base(path)
        {
        }

        public List<DZHBar> RequestLastBars(DZHSymbol symbol)
        {
            if (symbol.Equals(currentSymbol))
            {
                long pos = indexBuffer[symbol.Market][symbol.Code];
                fileStream.Position = pos + 10;
                recordCount = reader.ReadInt32();
                if (recordCount > recordCountReaded)//有新记录
                {
                    int recordsPerBlock = dataBlockSize / dataRecordSize;
                    if (recordCount / recordsPerBlock > blockNumberReaded)//有新块号
                    {
                        fileStream.Position += blockNumberReaded * 2;
                        for (int j = blockNumberReaded; j < 25; j++)
                        {
                            blocks[j] = reader.ReadInt16();
                        }
                    }
                    int iRecord = 0;//记录
                    int iBlock = 0;//第iBlock块
                    iRecord = recordCountReaded;
                    iBlock = iRecord / recordsPerBlock;

                    List<DZHBar> results = new List<DZHBar>();

                    while (iBlock < 25 && blocks[iBlock] != -1)
                    {
                        int r = iRecord % recordsPerBlock;//块内记录号
                        while (iRecord < recordCount && r < recordsPerBlock)
                        {
                            results.Add(readARecord(dataStartOffset + blocks[iBlock] * dataBlockSize + r * dataRecordSize));
                            r = r + 1;
                            iRecord = iRecord + 1;
                        }
                        iBlock = iBlock + 1;
                    }
                    recordCountReaded = recordCount;
                    blockNumberReaded = iBlock;
                    return results;
                }
                else return null;
            }
            else
            {
                return RequestBars(symbol);
            }
        }

        public List<DZHBar> RequestBars(DZHSymbol symbol)
        {
            return this.RequestBars(symbol, null, null);
        }

        public List<DZHBar> RequestBars(DZHSymbol symbol, DateTime? startTime, DateTime? endTime)
        {
            SetMarket(symbol.Market);
            SetSymbol(symbol);

            int iRecord = 0;//记录
            int iBlock = 0;//第iBlock块
            int recordsPerBlock = dataBlockSize / dataRecordSize;
            int iEndRecord = recordCount - 1;

            if (startTime.HasValue)
            {
                iRecord = GetStartIRecord(startTime.Value);
                iBlock = iRecord / recordsPerBlock;
            }
            if (endTime.HasValue)
            {
                iEndRecord = GetEndIRecord(endTime.Value);
            }

            List<DZHBar> results = new List<DZHBar>();

            while (iBlock < 25 && blocks[iBlock] != -1)
            {
                int r = iRecord % recordsPerBlock;//块内记录号
                while (iRecord <= iEndRecord && r < recordsPerBlock)
                {

                    results.Add(readARecord(dataStartOffset + blocks[iBlock] * dataBlockSize + r * dataRecordSize));
                    r = r + 1;
                    iRecord = iRecord + 1;
                }
                iBlock = iBlock + 1;
            }
            //Debug.WriteLine(results.Count.ToString());
            recordCountReaded = iEndRecord + 1;
            blockNumberReaded = iBlock;
            results.Sort();//日线或5分线有可能时间顺序错乱，故而排序了下
            return results;
        }

        private DZHBar readARecord(long pos)
        {
            DZHBar aBar = new DZHBar();
            fileStream.Position = pos;
            uint val = reader.ReadUInt32();
            aBar.Time = date19700101.AddSeconds(val);
            aBar.Open = reader.ReadSingle();
            aBar.High = reader.ReadSingle();
            aBar.Low = reader.ReadSingle();
            aBar.Close = reader.ReadSingle();
            aBar.Volume = reader.ReadSingle();
            aBar.Amount = reader.ReadSingle();
            return aBar;
        }
    }
}
