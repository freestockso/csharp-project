using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PlateMonitor
{
    class PlateDataTable:DataTable
    {
        public PlateDataTable()
        {
            this.Columns.Add(new DataColumn("ID", typeof(int)));
            this.Columns.Add(new DataColumn("PlateName", typeof(string)));
            this.Columns.Add(new DataColumn("UpLimitCount", typeof(int)));
            this.Columns.Add(new DataColumn("NPercentCount", typeof(int)));
            this.Columns.Add(new DataColumn("UpCount", typeof(int)));
            this.Columns.Add(new DataColumn("SecurityCount",typeof(int)));
            this.Columns.Add(new DataColumn("Weight", typeof(float)));
        }
    }
}
