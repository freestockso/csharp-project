using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HuaQuant.Data.DZH
{
    public class DZHSymbolReader : DZHStripReader
    {
        public DZHSymbolReader(string path)
            : base(path)
        {
        }
        public List<DZHSymbol> GetSymbols(string market)
        {
            SetMarket(market);
            List<DZHSymbol> results = new List<DZHSymbol>();
            long pos = indexStartOffset;
            while (pos + indexRecordSize < indexStartOffset + securityCount * indexRecordSize)
            {
                if (pos <= fileStream.Length)
                {
                    fileStream.Position = pos;
                    //大智慧用10个字节保存代码，一般用8个字节
                    string code0 = System.Text.Encoding.Default.GetString(reader.ReadBytes(10));
                    code0 = code0.Replace("\0", "");
                    if (code0 == "") break;
                    string name = System.Text.Encoding.Default.GetString(reader.ReadBytes(10));
                    results.Add(new DZHSymbol(market, code0, name));
                    pos += indexRecordSize;
                }
            }
            return results;
        }
    }
}

