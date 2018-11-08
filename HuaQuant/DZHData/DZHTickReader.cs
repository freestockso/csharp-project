using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HuaQuant.Data.DZH
{
    public class DZHTickReader:DZHBlockReader
    {
       
        public DZHTickReader(string path)
            : base(path)
        {
            fileName = "REPORT.DAT";
            dataStartOffset = 266240;
            dataBlockSize = 12272;
            dataRecordSize = 52;
        }
        public List<DZHTick> RequestLastTicks(DZHSymbol symbol)
        {
            if (symbol.Equals(currentSymbol))
            {
                long pos = indexBuffer[symbol.Market][symbol.Code];
                fileStream.Position = pos + 10;
                recordCount = reader.ReadInt32();
                if (recordCount > recordCountReaded)//有新记录
                {
                    int recordsPerBlock = dataBlockSize / dataRecordSize;
                    if (recordCount / recordsPerBlock> blockNumberReaded)//有新块号
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
                   
                    List<DZHTick> results = new List<DZHTick>();

                    while (iBlock < 25 && blocks[iBlock] != -1)
                    {
                        int r = iRecord % recordsPerBlock;//块内记录号
                        while (iRecord < recordCount && r < recordsPerBlock)
                        {
                            results.Add(ReadARecord(dataStartOffset + blocks[iBlock] * dataBlockSize + r * dataRecordSize));
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
                return RequestTicks(symbol);
            }
        }
        public List<DZHTick> RequestTicks(DZHSymbol symbol)
        {
           return this.RequestTicks(symbol, null,null);
        }
        public List<DZHTick> RequestTicks(DZHSymbol symbol, DateTime? startTime,DateTime? endTime)
        {
            SetMarket(symbol.Market);
            SetSymbol(symbol);

            currentSymbol = symbol;
            int iRecord = 0;//记录
            int iBlock = 0;//第iBlock块
            int recordsPerBlock = dataBlockSize / dataRecordSize;
            int iEndRecord = recordCount-1;

            if (startTime.HasValue)
            {
                iRecord = GetStartIRecord(startTime.Value);
                iBlock = iRecord / recordsPerBlock;
            }
            if (endTime.HasValue)
            {
                iEndRecord = GetEndIRecord(endTime.Value);
            }
           
            List<DZHTick> results = new List<DZHTick>();

            while (iBlock < 25 && blocks[iBlock] != -1)
            {
                int r = iRecord % recordsPerBlock;//块内记录号
                while (iRecord <=iEndRecord && r < recordsPerBlock)
                {
                    results.Add(ReadARecord(dataStartOffset + blocks[iBlock] * dataBlockSize + r * dataRecordSize));
                    r = r + 1;
                    iRecord = iRecord + 1;
                }
                iBlock = iBlock + 1;
            }
           
            recordCountReaded = iEndRecord+1;
            blockNumberReaded = iBlock;
            //Debug.WriteLine(results.Count.ToString());
            return results;
        }
        private DZHTick ReadARecord(long pos)
        {
            DZHTick aTick = new DZHTick();
            fileStream.Position = pos;
            uint val = reader.ReadUInt32();
            aTick.Time = date19700101.AddSeconds(val);//时间
            aTick.Price = reader.ReadSingle();
            aTick.Volume = reader.ReadSingle();
            aTick.Amount = reader.ReadSingle();
            val= reader.ReadUInt16();

            //三个标志位用于后面的计算
            Byte flag18, flag19, flag20;
            flag18 = reader.ReadByte();
            flag19 = reader.ReadByte();
            flag20 = reader.ReadByte();
            aTick.Number = val+(flag20&0xF0)*0x1000;
           
            //内外盘标志
            Byte inout = reader.ReadByte();
            if ((inout == 0x80) || (inout == 0xE0)) aTick.Side = 'B';//外盘
            else if ((inout == 0xC0) || (inout == 0xA0)) aTick.Side= 'S';//内盘
            else aTick.Side= 'N';
            //五个买量
            val = reader.ReadUInt16();
            if ((flag18 & 1) == 1) val = val * 32;
            aTick.Bid1Vol = val;
            val = reader.ReadUInt16();
            if ((flag18 & 16) == 16) val = val * 32;
            aTick.Bid2Vol = val;

            val = reader.ReadUInt16();
            if ((flag19 & 1) == 1) val = val * 32;
            aTick.Bid3Vol = val;
            val = reader.ReadUInt16();
            if ((flag19 & 16) == 16) val = val * 32;
            aTick.Bid4Vol = val;

            val = reader.ReadUInt16();
            if ((flag20 & 1) == 1) val = val * 32;
            aTick.Bid5Vol = val;
            //五个卖量
            val = reader.ReadUInt16();
            if ((flag18 & 2) == 2) val = val * 32;
            aTick.Ask1Vol = val;
            val = reader.ReadUInt16();
            if ((flag18 & 32) == 32) val = val * 32;
            aTick.Ask2Vol = val;

            val = reader.ReadUInt16();
            if ((flag19 & 2) == 2) val = val * 32;
            aTick.Ask3Vol = val;
            val = reader.ReadUInt16();
            if ((flag19 & 32) == 32) val = val * 32;
            aTick.Ask4Vol = val;

            val = reader.ReadUInt16();
            if ((flag20 & 2) == 2) val = val * 32;
            aTick.Ask5Vol = val;

            float t;
            if ((inout == 0xE0) || (inout == 0xA0)) t = 0.001F;//E0或A0代表是权证之类的，价格是小数三位
            else t = 0.01F;

            aTick.Bid1 = aTick.Price + reader.ReadSByte() * t;
            aTick.Bid2 = aTick.Price + reader.ReadSByte() * t;
            aTick.Bid3 = aTick.Price + reader.ReadSByte() * t;
            aTick.Bid4 = aTick.Price + reader.ReadSByte() * t;
            aTick.Bid5 = aTick.Price + reader.ReadSByte() * t;
            aTick.Ask1 = aTick.Price + reader.ReadSByte() * t;
            aTick.Ask2 = aTick.Price + reader.ReadSByte() * t;
            aTick.Ask3 = aTick.Price + reader.ReadSByte() * t;
            aTick.Ask4 = aTick.Price + reader.ReadSByte() * t;
            aTick.Ask5 = aTick.Price + reader.ReadSByte() * t;
            return aTick;
        }
    }
}
