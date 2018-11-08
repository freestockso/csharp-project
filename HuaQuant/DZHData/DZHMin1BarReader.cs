using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.DZH
{
    public class DZHMin1BarReader : DZHBarReader
    {
        public DZHMin1BarReader(string path)
            : base(path)
        {
            fileName = "MIN1.DAT";
            dataStartOffset = 266240;
            dataBlockSize = 16384;
            dataRecordSize = 32;
        }
    }
}