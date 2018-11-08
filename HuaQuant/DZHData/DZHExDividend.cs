using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.DZH
{
    public class DZHExDividend : IComparable<DZHExDividend>
    {
        public DateTime Time;
        public Single RateForPresented;//送股
        public Single RateForPlacement;//配股
        public Single PriceForPlacement;//配股价
        public Single Dividend;//分红

        public int CompareTo(DZHExDividend other)//比较接口，用于排序
        {
            return this.Time.CompareTo(other.Time);
        }

    }
}