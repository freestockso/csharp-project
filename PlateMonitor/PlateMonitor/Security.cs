using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMSDK;

namespace PlateMonitor
{
    class Security
    {
        public string Symbol;
        public string Name;
        public List<Plate> Plates = new List<Plate>();
        public double Price = 0.0;
        public double IncPercent = 0.0;
        public bool UpLimited = false;
        public DateTime FirstUpLimitTime = new DateTime();//首次触板时间
        public DateTime LastUpLimitTime = new DateTime();//最后封板时间
        public bool Matched = false;
        public int HotPlateCount = 0;
        public List<DailyBar> DailyBars = new List<DailyBar>();
    }
}
