using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HuaQuant.Data.DZH
{
    public class DZHBlockReader:DZHReader
    {
        protected int recordCount = 0;//记录数
        protected int recordCountReaded = 0;//已读记录数
        protected short[] blocks = new short[25];//块号数组
        protected int blockNumberReaded = 0;//已读块数
        protected DZHSymbol currentSymbol = null;

         protected DZHBlockReader(string path)
            : base(path)
        {
            securityCountOffset = 12;
            indexStartOffset= 24;
            indexRecordSize = 64;
        }

         protected override void OnFileChanged()
         {
             fileStream.Position = securityCountOffset;
             securityCount = reader.ReadUInt32();
         }

         protected void OnSymbolChanged()
         {
             GetBlocks(currentSymbol);
         }

         protected void SetSymbol(DZHSymbol symbol)
         {
             if ((currentSymbol == null) || (!currentSymbol.Equals(symbol)))
             {
                 currentSymbol = symbol;
                 OnSymbolChanged();
             }
         }

         protected bool GetBlocks(DZHSymbol symbol)
         {
             Dictionary<string, long> indexs = indexBuffer[symbol.Market];

             long pos = 0;
             bool find = false;

             if (indexs.ContainsKey(symbol.Code))
             {
                 pos = indexs[symbol.Code];
                 find = true;
             }
             else
             {
                 int indexCount = indexs.Count;
                 pos = indexStartOffset + indexCount * indexRecordSize;

                 while (pos + indexRecordSize < indexStartOffset + securityCount* indexRecordSize)
                 {
                     if (pos < fileStream.Length)
                     {
                         fileStream.Position = pos;
                         //大智慧用10个字节保存代码，一般用8个字节
                         string code0 = System.Text.Encoding.Default.GetString(reader.ReadBytes(10));
                         code0 = code0.Replace("\0", "");
                         if (!indexs.ContainsKey(code0)) indexs.Add(code0, pos);
                         if (symbol.Code == code0)
                         {
                             find = true;
                             break;
                         }
                         else
                         {
                             pos += indexRecordSize;
                         }
                     }
                 }
             }

             if (!find)
             {
                 for (int j = 0; j < 25; j++) blocks[j] = -1;//块号记录清空
                 return false;
             }
             else
             {
                 fileStream.Position = pos + 10;
                 recordCount = reader.ReadInt32();
                 for (int j = 0; j < 25; j++)
                 {
                     blocks[j] = reader.ReadInt16();     
                 }
                 return true;
             }
         }

         protected int GetStartIRecord(DateTime startTime)
         {

             int iBlock = 0;//第iBlock块
             int recordsPerBlock = dataBlockSize / dataRecordSize;
             bool find = false;

             while (iBlock < 25 && blocks[iBlock] != -1)
             {
                 if (iBlock < (recordCount - 1) / recordsPerBlock)//不是在最后一块，则是满块，取块内最后一记录的时间
                 {
                     fileStream.Position = dataStartOffset + (blocks[iBlock] + 1) * dataBlockSize - dataRecordSize;
                 }
                 else//在最后一块时，因为不是满块，取整个最后记录的时间
                 {
                     fileStream.Position = dataStartOffset + blocks[iBlock] * dataBlockSize + ((recordCount - 1) % recordsPerBlock) * dataRecordSize;
                 }

                 DateTime LastTime = date19700101.AddSeconds(reader.ReadUInt32());
                 if (startTime <= LastTime)
                 {
                     find = true;
                     break;
                 }
                 iBlock++;
             }

             if (!find) return recordCount;//开始时间大于最后一记录，则返回最后一记录号加１
             for (int r = 0; r < recordsPerBlock; r++)
             {
                 fileStream.Position = dataStartOffset + blocks[iBlock] * dataBlockSize + r * dataRecordSize;
                 DateTime curTime = date19700101.AddSeconds(reader.ReadUInt32());
                 if (curTime >= startTime) return iBlock * recordsPerBlock + r;
             }
             return 0;//此处本不会取到，但为程序的语法需要加上
         }

         protected int GetEndIRecord(DateTime endTime)
         {
             int iBlock = 0;//第iBlock块
             int recordsPerBlock = dataBlockSize / dataRecordSize;
             bool find = false;

             while (iBlock < 25 && blocks[iBlock] != -1)
             {
                 if (iBlock < (recordCount - 1) / recordsPerBlock)//不是在最后一块，则是满块，取块内最后一记录的时间
                 {
                     fileStream.Position = dataStartOffset + (blocks[iBlock] + 1) * dataBlockSize - dataRecordSize;
                 }
                 else//在最后一块时，因为不是满块，取整个最后记录的时间
                 {
                     fileStream.Position = dataStartOffset + blocks[iBlock] * dataBlockSize + ((recordCount - 1) % recordsPerBlock) * dataRecordSize;
                 }

                 DateTime LastTime = date19700101.AddSeconds(reader.ReadUInt32());
                 if (endTime <= LastTime)
                 {
                     find = true;
                     break;
                 }
                 iBlock++;
             }

             if (!find) return recordCount - 1;//结束时间大于最后一条记录的时间则，返回最后一记录号
             for (int r = 0; r < recordsPerBlock; r++)
             {
                 fileStream.Position = dataStartOffset + blocks[iBlock] * dataBlockSize + r * dataRecordSize;
                 DateTime curTime = date19700101.AddSeconds(reader.ReadUInt32());

                 if (curTime == endTime) return iBlock * recordsPerBlock + r;
                 else if (curTime > endTime) return iBlock * recordsPerBlock + r - 1;
             }
             return 0;//此处本不会取到，但为程序的语法需要加上
         }

    }
}
