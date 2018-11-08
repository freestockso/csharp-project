using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlateMonitor
{
    class Plate
    {
        public int ID;
        public string Name;
        public int Type;
        public List<Security> Securities = new List<Security>();
        public int UpLimitCount=0;
        public int NPercentCount = 0;
        public int UpCount = 0;
        public float Weight = 0;
    }
}
