using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HuaQuant.Data.DZH
{
    //大智慧证券符号类
    public class DZHSymbol
    {
        public enum SecurityTypes { Index, Stock, Bond, Fund, Warrants, Other };//指数，股票，债券，基金，权证，其他
        public enum StockTypes { A, B, Board2, Board3, unknow };//A股，B股，创业板，三板

        public string Code;//代码
        public string Name;//名称
        public string Market;//市场
        public SecurityTypes SecurityType;//证券类型
        public StockTypes StockType;//股票类型

        public DZHSymbol(string market, string code, string name)
        {
            this.Market = market.Trim().ToUpper();
            this.Code = code.Trim().ToUpper();
            this.Name = name;
            this.SetSymbolType();
        }

        public DZHSymbol(string market, string code)
            : this(market, code, "")
        {
        }

        public override string ToString()
        {
            return this.Market + '.' + this.Code;
        }

        public static DZHSymbol Parse(string symbolString)
        {
            int i = symbolString.IndexOf('.');
            return new DZHSymbol(symbolString.Substring(0, i), symbolString.Substring(i + 1));
        }
        //设置证券类型
        private void SetSymbolType()
        {
            string symbolString = this.ToString();
            this.StockType = StockTypes.unknow;

            if (Regex.IsMatch(symbolString, @"(SH.000\d{3})|(SZ.399\d{3})") == true)
            {
                this.SecurityType = SecurityTypes.Index;
            }
            else if (Regex.IsMatch(symbolString, @"(SH.60[0-8]\d{3})|(SZ.00[01256789]\d{3})") == true)
            {
                this.SecurityType = SecurityTypes.Stock;
                this.StockType = StockTypes.A;
            }
            else if (Regex.IsMatch(symbolString, @"(SH.90\d{4})|(SZ.20\d{4})") == true)
            {
                this.SecurityType = SecurityTypes.Stock;
                this.StockType = StockTypes.B;
            }
            else if (Regex.IsMatch(symbolString, @"(SZ.300\d{3})") == true)
            {
                this.SecurityType = SecurityTypes.Stock;
                this.StockType = StockTypes.Board2;
            }
            else if (Regex.IsMatch(symbolString, @"(SZ.4[023]\d{4})") == true)
            {
                this.SecurityType = SecurityTypes.Stock;
                this.StockType = StockTypes.Board3;
            }
            else if (Regex.IsMatch(symbolString, @"(SH.[012]\d{5})|(SZ.1[0123]\d{4})") == true)
            {
                this.SecurityType = SecurityTypes.Bond;
            }
            else if (Regex.IsMatch(symbolString, @"(SH.5[01]\d{4})|(SZ.184\d{3})|(SZ.1[56]\d{4})") == true)
            {
                this.SecurityType = SecurityTypes.Fund;
            }
            else if (Regex.IsMatch(symbolString, @"(SH.58\d{4})|(SZ.03\d{4})") == true)
            {
                this.SecurityType = SecurityTypes.Warrants;
            }
            else
            {
                this.SecurityType = SecurityTypes.Other;
            }
        }
        //以下两个重写的方法是为了symbol能在字典上做主键，实现主键的查找
        public override bool Equals(object obj)
        {
            if (obj is DZHSymbol)
            {
                DZHSymbol other = (DZHSymbol)obj;
                return (this.Market == other.Market && this.Code == other.Code) ? true : false;
            }
            else return false;
        }

        public override int GetHashCode()
        {
            string s = this.ToString();
            long n = 0;
            for (int i = 0; i < s.Length; i++)
            {
                n = (n + s[i]) * 31 & 0xFFFFFFFF;
            }
            return (int)n;
        }
       
    }
}
