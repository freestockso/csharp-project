using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HuaQuant.Data.DZH
{
    public class DZHTick
    {
        public DateTime Time;//时间
        public Single Price;//成交价格
        public Single Volume;//累计成交量
        public Single Amount;//累计成效额
        public long Number;//累计成交笔数
        public char Side;//买卖盘

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
