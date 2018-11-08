using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.DZH
{
    public class DZHMin5BarReader : DZHBarReader
    {
        public DZHMin5BarReader(string path)
            : base(path)
        {
            fileName = "MIN.DAT";
            dataStartOffset = 266240;
            dataBlockSize = 8192;
            dataRecordSize = 32;
        }
    }
}
