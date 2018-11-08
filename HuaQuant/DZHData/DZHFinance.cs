using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace HuaQuant.Data.DZH
{
    public class DZHFinance
    {
        public DateTime UpdateDate;
        public DateTime ReportDate;
        public DateTime ListingDate;
        public Single EarningsPerShare;//每股收益
        public Single NetAssetsPerShare;//每股净资产
        public Single RateOfReturnOnNetAssets;//净资产收益率
        public Single OperatingCashPerShare;//每股经营现金
        public Single PerShareFund;//每股公积金
        public Single NotDividePerShare;//每股未分配
        public Single TheEquityRatio;//股东权益比
        public Single NetProfitGrow;//净利润同比
        public Single MainBusinessIncomeGrow;//主营收入同比
        public Single SalesGrossMargin;//销售毛利率
        public Single AdjustedNetAssetsPerShare;//调整每股净资产
        public Single TotalAssets;//总资产
        public Single CurrentAssets;//流动资产
        public Single FixedAssets;//固定资产
        public Single IntangibleAssets;//无形资产
        public Single CurrentLiabilities;//流动负债
        public Single LongTermLiabilities;//长期负债
        public Single TotalLiabilities;//总负债
        public Single ShareholdersEquity;//股东权益
        public Single TheCapitalFund;//资本公积金
        public Single OperatingCashFlow;//经营现金流量
        public Single InvestmentCashFlow;//投资现金流量
        public Single FinancingCashFlows;//筹资现金流量
        public Single CashIncreased;//现金增加额
        public Single MainBusinessIncome;//主营收入
        public Single TheMainProfit;//主营利润
        public Single OperatingProfit;//营业利润
        public Single InvestmentReturns;//投资收益
        public Single NonoperatingIncome_Expense;//营业外收支
        public Single TotalProfit;//利润总额
        public Single NetProfit;//净利润
        public Single NotDivideProfit;//未分配利润
        public Single TheTotalShareCapital;//总股本
        public Single TotalUnlimitedSaleOFShares;//不限售股本合计
        public Single AShares;//A股
        public Single BShares;//B股
        public Single OverseasListedShares;//境外上市股
        public Single OtherTradableShares;//其他流通股
        public Single TotalLimitedSaleOFShares;//限售股本合计
        public Single TheStateOwnershipShares;//国家持股
        public Single StateOwnedLegalPersonShares;//国有法人股
        public Single DomesticLegalPersonShares;//境内法人股
        public Single DomesticNaturalPersonShares;//境内自然人股
        public Single OtherSponsorsShares;//其他发起人股
        public Single RaisedLegalPersonShares;//募集法人股
        public Single ForeignLegalPersonShares;//境外法人股
        public Single ForeignNaturalPersonShares;//境外自然人股
        public Single PreferredSharesOrOther;//优先股和其他
    }
}
