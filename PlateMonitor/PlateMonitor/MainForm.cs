using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;


namespace PlateMonitor
{
    public partial class MainForm : Form
    {
        private Operator _op = new Operator();
        private int curPlateID = -1;
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this._op.GetActiveSymbols();
            this._op.ReadPlatesAndSecurities();
            this.update();
        }
        private delegate void RefreshDataDelegate(DataGridView dgv, DataTable dt);
        private delegate void RefreshLabelDelegate(Label lbl, string text);
        private void setCurPlateID(int plateID)
        {
            this.curPlateID = plateID;
            List<Security> securities = new List<Security>();
            string lblText = "当前板块是：";
            if (!this._op.PlateDict.ContainsKey(plateID))
            {
                List<Security> temp = new List<Security>();
                foreach (Plate plate in this._op.PlateDict.Values)
                {
                    temp.AddRange(plate.Securities);
                }
                securities.AddRange(temp.Distinct());
                lblText += "全部";
            }else
            {
                Plate plate = this._op.PlateDict[plateID];
                securities = plate.Securities;
                lblText += plate.Name;
            }
            DataTable dtSecurities = new SecurityDataTable();
            foreach(Security secur in securities)
            {
                DataRow dr = dtSecurities.NewRow();
                dr["Symbol"] = secur.Symbol;
                dr["Name"] = secur.Name;
                dr["IncPercent"] = string.Format("{0:N2}", secur.IncPercent);
                dr["Price"] = string.Format("{0:N2}", secur.Price);
                if (secur.UpLimited)
                {
                    dr["UpLimited"] = "是";
                }else
                {
                    dr["UpLimited"] = "";
                }
                dr["HotPlateCount"] = secur.HotPlateCount;
                if (secur.Matched)
                {
                    dr["Matched"] = "是";
                }else
                {
                    dr["Matched"] = "";
                }
                dtSecurities.Rows.Add(dr); 
            }
            dtSecurities.DefaultView.Sort="IncPercent DESC";
           
            this.RefreshLabel(this.lblPlateName, lblText);
            this.RefreshStockData(dgvStocks, dtSecurities);
        }
        private void RefreshLabel(Label lbl,string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RefreshLabelDelegate(this.RefreshLabel), lbl, text);
            }else
            {
                lbl.Text = text;
            }
        }
        private void RefreshPlateData(DataGridView dgv, DataTable dt)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RefreshDataDelegate(this.RefreshPlateData), dgv, dt);
            }
            else
            {
                dgv.DataSource = dt;
                dgv.Columns[0].HeaderText = "编号";
                dgv.Columns[0].Visible = false;
                dgv.Columns[1].HeaderText = "名称";
                dgv.Columns[2].HeaderText = "涨停数";
                dgv.Columns[3].HeaderText = "N%以上计数";
                dgv.Columns[4].HeaderText = "上涨数";
                dgv.Columns[5].HeaderText = "证券数";
                dgv.Columns[6].HeaderText = "权值";
                dgv.Columns[2].Width = 70;
                dgv.Columns[3].Width = 70;
                dgv.Columns[4].Width = 70;
                dgv.Columns[5].Width = 70;
                dgv.Columns[6].Width = 70;
            }
        }
        private void RefreshStockData(DataGridView dgv, DataTable dt)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RefreshDataDelegate(this.RefreshStockData), dgv, dt);
            }
            else
            {
                dgv.DataSource = dt;
                dgv.Columns[0].HeaderText = "证券";
                dgv.Columns[1].HeaderText = "名称";
                dgv.Columns[2].HeaderText = "涨幅(%)";
                dgv.Columns[3].HeaderText = "价格";
                dgv.Columns[4].HeaderText = "是否涨停";
                dgv.Columns[5].HeaderText = "热点板块数";
                dgv.Columns[6].HeaderText = "符合条件";
                dgv.Columns[2].Width = 60;
                dgv.Columns[3].Width = 60;
                dgv.Columns[4].Width = 60;
                dgv.Columns[5].Width = 60;
                dgv.Columns[6].Width = 60;
            }
        }
        private void update()
        {
            this._op.UpdatePlateStatistic();
            DataTable dtIndustryPlates = new PlateDataTable();
            DataTable dtConceptPlates = new PlateDataTable();
            DataTable dtAreaPlates = new PlateDataTable();

            foreach (Plate curPlate in this._op.PlateDict.Values)
            {
                DataRow dr = null;
                switch (curPlate.Type)
                {
                    case 1:
                        dr = dtAreaPlates.NewRow();
                        break;
                    case 2:
                        dr = dtIndustryPlates.NewRow();
                        break;
                    case 3:
                        dr = dtConceptPlates.NewRow();
                        break;
                }
                dr["ID"] = curPlate.ID;
                dr["PlateName"] = curPlate.Name;
                dr["UpLimitCount"] = curPlate.UpLimitCount;
                dr["NPercentCount"] = curPlate.NPercentCount;
                dr["UpCount"] = curPlate.UpCount;
                dr["SecurityCount"] = curPlate.Securities.Count;
                dr["Weight"] = curPlate.Weight;
                switch (curPlate.Type)
                {
                    case 1:
                        dtAreaPlates.Rows.Add(dr);
                        break;
                    case 2:
                        dtIndustryPlates.Rows.Add(dr);
                        break;
                    case 3:
                        dtConceptPlates.Rows.Add(dr);
                        break;
                }
            }
            DataRow dr1 = dtAreaPlates.NewRow();
            dr1["ID"] = -1;
            dr1["PlateName"] = "全部";
            dr1["UpLimitCount"] = this._op.AllUpLimitCount;
            dr1["NPercentCount"] = this._op.AllNPercentCount;
            dr1["UpCount"] = this._op.AllUpCount;
            dr1["SecurityCount"] = this._op.AllSecurityCount;
            dr1["Weight"] = (this._op.AllUpLimitCount * 0.5 + this._op.AllNPercentCount * 0.3 + this._op.AllUpCount * 0.2) * 100 / this._op.AllSecurityCount;
            dtAreaPlates.Rows.Add(dr1);
            dtAreaPlates.DefaultView.Sort = "UpLimitCount DESC";
            dr1 = dtIndustryPlates.NewRow();
            dr1["ID"] = -1;
            dr1["PlateName"] = "全部";
            dr1["UpLimitCount"] = this._op.AllUpLimitCount;
            dr1["NPercentCount"] = this._op.AllNPercentCount;
            dr1["UpCount"] = this._op.AllUpCount;
            dr1["SecurityCount"] = this._op.AllSecurityCount;
            dr1["Weight"] = (this._op.AllUpLimitCount * 0.5 + this._op.AllNPercentCount * 0.3 + this._op.AllUpCount * 0.2) * 100 / this._op.AllSecurityCount;
            dtIndustryPlates.Rows.Add(dr1);
            dtIndustryPlates.DefaultView.Sort = "UpLimitCount DESC";
            dr1 = dtConceptPlates.NewRow();
            dr1["ID"] = -1;
            dr1["PlateName"] = "全部";
            dr1["UpLimitCount"] = this._op.AllUpLimitCount;
            dr1["NPercentCount"] = this._op.AllNPercentCount;
            dr1["UpCount"] = this._op.AllUpCount;
            dr1["SecurityCount"] = this._op.AllSecurityCount;
            dr1["Weight"] = (this._op.AllUpLimitCount * 0.5 + this._op.AllNPercentCount * 0.3 + this._op.AllUpCount * 0.2) * 100 / this._op.AllSecurityCount;
            dtConceptPlates.Rows.Add(dr1);
            dtConceptPlates.DefaultView.Sort = "UpLimitCount DESC";

            this.RefreshPlateData(this.dgvAreas, dtAreaPlates);
            this.RefreshPlateData(this.dgvIndustries, dtIndustryPlates);
            this.RefreshPlateData(this.dgvConcepts, dtConceptPlates);
            this.setCurPlateID(this.curPlateID);
            string plateString = "热点板块：";
            foreach(Plate plate in this._op.HotPlates)
            {
                plateString += plate.Name+" ";
            }
            this.tssLabel2.Text = plateString;
        }
        private int timeSpan = 0;
        private Thread updateThread = null;
        private DateTime beginTime = DateTime.Today.Add(new TimeSpan(9, 20, 0));
        private DateTime endTime = DateTime.Today.Add(new TimeSpan(15, 1, 0));
        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.cbUpdating.Checked)
            {
                if (DateTime.Now < this.beginTime || DateTime.Now > this.endTime) return;//非交易时间不刷新
                this.timeSpan += this.timer.Interval;
                int interval = Properties.Settings.Default.tbInterval;//刷新时间间隔
                if (this.timeSpan >= interval * 1000)
                {
                    if (this.updateThread == null || this.updateThread.ThreadState == ThreadState.Stopped)
                    {
                        this.updateThread = new Thread(new ThreadStart(this.update));
                        this.updateThread.Start();
                    }
                    this.timeSpan = 0;
                }
            }else
            {
                this.timeSpan = 0;
            }
        }

        private Thread calculateThread = null;
        private Thread saveThread = null;
        private void doCalculate()
        {
            this.tssLabel.Text = "正在计算是否满足待选条件...";
            this._op.DoCalculate();
            this.tssLabel.Text = "计算完毕。";
        }
        private void calculate()
        {
            if (this.calculateThread == null || this.calculateThread.ThreadState == ThreadState.Stopped)
            {
                this.calculateThread = new Thread(new ThreadStart(this.doCalculate));
                this.calculateThread.Start();
            }
        }
        private void doSave()
        {
            this.tssLabel.Text = "正在保存中，请勿关闭...";
            this._op.SavePlateStatistic();
            this.tssLabel.Text = "保存完毕。";
        }
        private void save()
        {
            if (this.saveThread == null || this.saveThread.ThreadState == ThreadState.Stopped)
            {
                this.saveThread = new Thread(new ThreadStart(this.doSave));
                this.saveThread.Start();
            }
            
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.save();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 掘金量化行情配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form con = new QuantConfig();
            con.Show();
        }

        private void 东方财富板块路径配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form con = new EasyMoneyConfig();
            con.Show();
        }

        private void dgvIndustries_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0 && i < dgvIndustries.Rows.Count)
            {
                int id = (int)dgvIndustries.Rows[i].Cells["ID"].Value;
                this.setCurPlateID(id);
            }
        }

        private void dgvConcepts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0 && i < dgvConcepts.Rows.Count)
            {
                int id = (int)dgvConcepts.Rows[i].Cells["ID"].Value;
                this.setCurPlateID(id);
            }
        }

        private void dgvAreas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0 && i < dgvAreas.Rows.Count)
            {
                int id = (int)dgvAreas.Rows[i].Cells["ID"].Value;
                this.setCurPlateID(id);
            }
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            this.setCurPlateID(-1);
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            this.calculate();
        }
    }
}
