using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.DZH
{
    public class DZHBar : IComparable<DZHBar>
    {
        public DateTime Time;//时间
        public Single Open;//开盘价
        public Single High;//最高价
        public Single Low;//最低价
        public Single Close;//收盘价
        public Single Volume;//成交量
        public Single Amount;//成交金额

        public int CompareTo(DZHBar other)//比较接口，用于排序
        {
            return this.Time.CompareTo(other.Time);
        }
    }
}
