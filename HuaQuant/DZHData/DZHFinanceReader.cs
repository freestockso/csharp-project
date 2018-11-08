using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HuaQuant.Data.DZH
{
    public class DZHFinanceReader: DZHReader
    {
        public DZHFinanceReader(string path)
            : base(path)
        {
            securityCountOffset = 8;
            indexStartOffset = 28;
            fileName = "INFOEX.DAT";
        }
        protected override void OnFileChanged()
        {
            fileStream.Position = securityCountOffset;
            securityCount = reader.ReadUInt32();
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
                if (indexs.Count == 0) pos = indexStartOffset;
                else
                {
                    KeyValuePair<string, long> lastVP = indexs.Last<KeyValuePair<string, long>>();
                    pos = lastVP.Value;
                }

                while (indexs.Count <= securityCount)
                {
                    if (pos + 11 < fileStream.Length)
                    {
                        fileStream.Position = pos;
                        //这文件中好像只用8个字节表示代码
                        string code0 = System.Text.Encoding.Default.GetString(reader.ReadBytes(8));
                        code0 = code0.Replace("\0", "");
                        if (!indexs.ContainsKey(code0)) indexs.Add(code0, pos);
                        if (symbol.Code == code0)
                        {
                            find = true;
                            break;
                        }
                        else
                        {
                            fileStream.Position = pos + 11;
                            Byte flag = reader.ReadByte();
                            byte x;
                            switch (flag)
                            {
                                case 0x80:
                                    pos = pos + 216;
                                    break;
                                case 0xc0:
                                    fileStream.Position = pos + 10;
                                    x = reader.ReadByte();
                                    pos = pos + 216 + 32 + x * 20;
                                    break;
                                case 0x40:
                                    fileStream.Position = pos + 10;
                                    x = reader.ReadByte();
                                    pos = pos + 12+32 + x * 20;
                                    break;
                                case 0x00:
                                    pos = pos + 12;
                                    break;
                                default:
                                    //Debug.WriteLine(pos.ToString());
                                    throw new Exception("invalid flag in reading finance data:" + flag.ToString("D"));
                            }
                        }
                    }
                    else break;
                }
            }

            if (find) return pos;
            else return -1;
        }
        public List<DZHExDividend> RequestExDividends(DZHSymbol symbol)
        {
            SetMarket(symbol.Market);
            long pos = GetIndex(symbol);

            if (pos == -1) return null;
            else
            {
                List<DZHExDividend> results = new List<DZHExDividend>();
                fileStream.Position = pos + 10;
                byte x = reader.ReadByte();
                if (x > 0)
                {
                    for (int i = 0; i < x; i++)
                    {
                        fileStream.Position = pos + 248 + i * 20;
                        DZHExDividend aEx = new DZHExDividend();
                        aEx.Time = date19700101.AddSeconds(reader.ReadUInt32());
                        aEx.RateForPresented = reader.ReadSingle() * 10;
                        aEx.RateForPlacement = reader.ReadSingle() * 10;
                        aEx.PriceForPlacement = reader.ReadSingle();
                        aEx.Dividend = reader.ReadSingle() * 10;
                        results.Add(aEx);
                    }
                }
                results.Sort();//大智慧除权除息数据是倒序放置的，故而排序了下
                return results;
            }
        }
        public DZHFinance RequestFinance(DZHSymbol symbol)
        {
            SetMarket(symbol.Market);
            long pos = GetIndex(symbol);
            if (pos == -1) return null;
            else return ReadARecord(pos);
        }
        public void ForwardAdjustedPrice(List<DZHBar> bars,List<DZHExDividend> exDividends)
        {
            foreach (DZHExDividend ex in exDividends)
            {
                Single cx = ex.Dividend/10;//除息
                Single sgl = ex.RateForPresented / 10;//送股率
                Single pgl = ex.RateForPlacement / 10;//配股率
                Single pgj = ex.PriceForPlacement;//配股价
                /**
                 * 采用前复权
                 * 复权后价＝（复权前价－除息+配股价*配股率）/(1+送股率+配股率）
                 * 复权后量＝复权前量*（1+送股率+配股率）
                 */
                int num= bars.Count;
                int i = 0;
                while ((i < num) && (bars[i].Time < ex.Time))
                {
                    bars[i].Close = (bars[i].Close - cx + pgj *pgl) / (1 + sgl + pgl);
                    bars[i].High = (bars[i].High - cx + pgj * pgl) / (1 + sgl + pgl);
                    bars[i].Open = (bars[i].Open - cx + pgj * pgl) / (1 + sgl + pgl);
                    bars[i].Low = (bars[i].Low - cx + pgj * pgl) / (1 + sgl + pgl);
                    bars[i].Volume=bars[i].Volume*(1 + sgl + pgl);
                    i++;
                }
            }
        } 
        protected DZHFinance ReadARecord(long pos)
        {   
            fileStream.Position = pos + 11;
            Byte flag = reader.ReadByte();
            if (flag == 0x80 || flag == 0xc0)
            {
                DZHFinance finance = new DZHFinance();
                //这里的日期时间不是以date19700101加上多少秒的，而是直接以数字写入
                uint val = reader.ReadUInt32();
                finance.UpdateDate= DateTime.ParseExact(val.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                val = reader.ReadUInt32();
                finance.ReportDate= DateTime.ParseExact(val.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                val = reader.ReadUInt32();
                finance.ListingDate = (val!=0)?DateTime.ParseExact(val.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture):DateTime.MaxValue;
                finance.EarningsPerShare = reader.ReadSingle();
                finance.NetAssetsPerShare = reader.ReadSingle();
                finance.RateOfReturnOnNetAssets = reader.ReadSingle();
                finance.OperatingCashPerShare = reader.ReadSingle(); 
                finance.PerShareFund = reader.ReadSingle(); 
                finance.NotDividePerShare = reader.ReadSingle(); 
                finance.TheEquityRatio = reader.ReadSingle(); 
                finance.NetProfitGrow = reader.ReadSingle();
                finance.MainBusinessIncomeGrow = reader.ReadSingle();
                finance.SalesGrossMargin=reader.ReadSingle();
                finance.AdjustedNetAssetsPerShare = reader.ReadSingle();
                finance.TotalAssets = reader.ReadSingle();
                finance.CurrentAssets = reader.ReadSingle();
                finance.FixedAssets = reader.ReadSingle();
                finance.IntangibleAssets = reader.ReadSingle();
                finance.CurrentLiabilities = reader.ReadSingle();
                finance.LongTermLiabilities = reader.ReadSingle();
                finance.TotalLiabilities = reader.ReadSingle();
                finance.ShareholdersEquity = reader.ReadSingle();
                finance.TheCapitalFund = reader.ReadSingle();
                finance.OperatingCashFlow = reader.ReadSingle();
                finance.InvestmentCashFlow = reader.ReadSingle();
                finance.FinancingCashFlows = reader.ReadSingle();
                finance.CashIncreased = reader.ReadSingle();
                finance.MainBusinessIncome = reader.ReadSingle();
                finance.TheMainProfit = reader.ReadSingle();
                finance.OperatingProfit = reader.ReadSingle();
                finance.InvestmentReturns = reader.ReadSingle();
                finance.NonoperatingIncome_Expense = reader.ReadSingle();
                finance.TotalProfit = reader.ReadSingle();
                finance.NetProfit = reader.ReadSingle();
                finance.NotDivideProfit = reader.ReadSingle();
                finance.TheTotalShareCapital = reader.ReadSingle();
                finance.TotalUnlimitedSaleOFShares = reader.ReadSingle();
                finance.AShares = reader.ReadSingle();
                finance.BShares = reader.ReadSingle();
                finance.OverseasListedShares = reader.ReadSingle();//境外上市股
                finance.OtherTradableShares = reader.ReadSingle();//其他流通股
                finance.TotalLimitedSaleOFShares = reader.ReadSingle();//限售股本合计
                finance.TheStateOwnershipShares = reader.ReadSingle();//国家持股
                finance.StateOwnedLegalPersonShares = reader.ReadSingle();//国有法人股
                finance.DomesticLegalPersonShares = reader.ReadSingle();//境内法人股
                finance.DomesticNaturalPersonShares = reader.ReadSingle();//境内自然人股
                finance.OtherSponsorsShares = reader.ReadSingle();//其他发起人股
                finance.RaisedLegalPersonShares = reader.ReadSingle();//募集法人股
                finance.ForeignLegalPersonShares = reader.ReadSingle();//境外法人股
                finance.ForeignNaturalPersonShares = reader.ReadSingle();//境外自然人股
                finance.PreferredSharesOrOther = reader.ReadSingle();//优先股和其他
                return finance;
            }
            else
            {
                return null;
            }  
        }
    }
}
