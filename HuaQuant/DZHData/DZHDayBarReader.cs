using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.DZH
{
    public class DZHDayBarReader : DZHBarReader
    {
        public DZHDayBarReader(string path)
            : base(path)
        {
            fileName = "DAY.DAT";
            dataStartOffset = 266240;
            dataBlockSize = 8192;
            dataRecordSize = 32;
        }
    }
}
