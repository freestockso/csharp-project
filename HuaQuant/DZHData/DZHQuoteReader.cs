using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.DZH
{
    public class DZHQuoteReader : DZHStripReader
    {

        public DZHQuoteReader(string path)
            : base(path)
        {
        }

        public DZHQuote RequestQuote(DZHSymbol symbol)
        {
            SetMarket(symbol.Market);
            long pos = GetIndex(symbol);
            if (pos == -1) return null;
            else return ReadARecord(pos);
        }

        public Dictionary<DZHSymbol, DZHQuote> RequestQuotes(List<DZHSymbol> symbols)
        {
            Dictionary<DZHSymbol, DZHQuote> results = new Dictionary<DZHSymbol, DZHQuote>();
            foreach (DZHSymbol symbol in symbols)
            {
                results.Add(symbol, RequestQuote(symbol));
            }
            return results;
        }

        protected DZHQuote ReadARecord(long pos)
        {
            DZHQuote aQuote = new DZHQuote();
            fileStream.Position = pos + 60;
            aQuote.Time = date19700101.AddSeconds(reader.ReadUInt32());
            fileStream.Position = pos + 68;
            aQuote.LastClose = reader.ReadSingle();
            aQuote.Open = reader.ReadSingle();
            aQuote.High = reader.ReadSingle();
            aQuote.Low = reader.ReadSingle(); ;
            aQuote.Price = reader.ReadSingle();
            aQuote.TotalVolume= reader.ReadSingle();
            aQuote.TotalAmount = reader.ReadSingle();
            aQuote.Volume = reader.ReadSingle();
            fileStream.Position = pos + 100;
            aQuote.Bid1 = reader.ReadSingle();
            aQuote.Bid2 = reader.ReadSingle();
            aQuote.Bid3 = reader.ReadSingle();
            aQuote.Bid4 = reader.ReadSingle();
            aQuote.Bid5 = reader.ReadSingle();
            aQuote.Bid1Vol = reader.ReadSingle();
            aQuote.Bid2Vol = reader.ReadSingle();
            aQuote.Bid3Vol = reader.ReadSingle();
            aQuote.Bid4Vol = reader.ReadSingle();
            aQuote.Bid5Vol = reader.ReadSingle();
            aQuote.Ask1 = reader.ReadSingle();
            aQuote.Ask2 = reader.ReadSingle();
            aQuote.Ask3 = reader.ReadSingle();
            aQuote.Ask4 = reader.ReadSingle();
            aQuote.Ask5 = reader.ReadSingle();
            aQuote.Ask1Vol = reader.ReadSingle();
            aQuote.Ask2Vol = reader.ReadSingle();
            aQuote.Ask3Vol = reader.ReadSingle();
            aQuote.Ask4Vol = reader.ReadSingle();
            aQuote.Ask5Vol = reader.ReadSingle();
            return aQuote;
        }

        protected long GetIndex(DZHSymbol symbol)
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

                while (pos + indexRecordSize < indexStartOffset + securityCount * indexRecordSize)
                {
                    if (pos <= fileStream.Length)
                    {
                        fileStream.Position = pos;
                        //大智慧用10个字节保存代码，一般用8个字节
                        string code0 = System.Text.Encoding.Default.GetString(reader.ReadBytes(10));
                        code0 = code0.Replace("\0", "");
                        indexs.Add(code0, pos);
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

            if (find) return pos;
            else return -1;
        }
    }
}
