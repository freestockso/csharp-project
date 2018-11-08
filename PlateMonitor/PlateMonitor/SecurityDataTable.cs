using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PlateMonitor
{
    class SecurityDataTable:DataTable
    {
        public SecurityDataTable()
        {
            this.Columns.Add(new DataColumn("Symbol", typeof(string)));
            this.Columns.Add(new DataColumn("Name", typeof(string)));
            this.Columns.Add(new DataColumn("IncPercent", typeof(double)));
            this.Columns.Add(new DataColumn("Price", typeof(double)));
            this.Columns.Add(new DataColumn("UpLimited", typeof(string)));
            this.Columns.Add(new DataColumn("HotPlateCount", typeof(int)));
            this.Columns.Add(new DataColumn("Matched", typeof(string)));
        }
    }
}
