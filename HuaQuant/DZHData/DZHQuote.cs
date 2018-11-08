using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.DZH
{
    public class DZHQuote
    {
        public DateTime Time;//更新时间
        public Single LastClose;//昨收
        public Single Open;//开盘
        public Single Price;//最新价格
        public Single High;//最高价
        public Single Low;//最低价
        public Single TotalVolume;//累计成交量
        public Single TotalAmount;//累计成交金额
        public Single Volume;//当前成交量

        public Single Ask1;//卖价一
        public Single Ask1Vol;//卖价一量
        public Single Ask2;//卖价二
        public Single Ask2Vol;//卖价二量
        public Single Ask3;//卖价三
        public Single Ask3Vol;//卖价三量
        public Single Ask4;//卖价四
        public Single Ask4Vol;//卖价四量
        public Single Ask5;//卖价五
        public Single Ask5Vol;//卖价五量
        public Single Bid1;//买价一
        public Single Bid1Vol;//买价一量
        public Single Bid2;//买价二
        public Single Bid2Vol;//买价二量
        public Single Bid3;//买价三
        public Single Bid3Vol;//买价三量
        public Single Bid4;//买价四
        public Single Bid4Vol;//买价四量
        public Single Bid5;//买价五
        public Single Bid5Vol;//买价五量
    }
}
