using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HuaQuant.Data.DZH;
using System.Diagnostics;


namespace HuaQuant
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = @"H:\全推大智慧\data";
            DZHSymbol sy = new DZHSymbol("SH", "603828");
            /*测试日线*/
            //DZHDayBarReader dr = new DZHDayBarReader(path);
            //List<DZHBar> data = dr.RequestBars(sy);
            /*测试5分线*/
            //DZHMin5BarReader dr = new DZHMin5BarReader(path);
            //List<DZHBar> data = dr.RequestBars(sy);

            /*测试报价*/
            //DZHQuoteReader dr = new DZHQuoteReader(path);
            //List<DZHQuote> data = new List<DZHQuote>();
            //DZHQuote aQuote = dr.RequestQuote(sy);
            //data.Add(aQuote);
            //sy = new DZHSymbol("SZ", "000001");
            //aQuote = dr.RequestQuote(sy);
            //data.Add(aQuote);
            //sy = new DZHSymbol("SH", "600000");
            //aQuote = dr.RequestQuote(sy);
            //data.Add(aQuote);
            /*测试分笔*/
            //DZHTickReader dr = new DZHTickReader(path);
            //List<DZHTick> data = dr.RequestTicks(sy);
            //dr.Dispose();
            /*测试读取证券*/
            //DZHSymbolReader dr = new DZHSymbolReader(path);
            //List<DZHSymbol> data = dr.GetSymbols("SZ");
            /*测试财务数据*/
            DZHFinanceReader dr = new DZHFinanceReader(path);
            DZHFinance aFinance = dr.RequestFinance(sy);
            List<DZHFinance> data = new List<DZHFinance>();
            data.Add(aFinance);
            /*测试除权数据*/
            //DZHFinanceReader dr = new DZHFinanceReader(path);
            //List<DZHExDividend> data = dr.RequestExDividends(sy);
            /*测试日线复权*/
            //DZHDayBarReader dr = new DZHDayBarReader(path);
            //List<DZHBar> data = dr.RequestBars(sy);
            //DZHFinanceReader dr1 = new DZHFinanceReader(path);
            //List<DZHExDividend> ex = dr1.RequestExDividends(sy);
            //dr1.ForwardAdjustedPrice(data, ex);
            /*因为数据类使用了publice字段而不是属性了，所以datagriview无法直接从list中获取数据显示，得动态添加*/

            this.ShowData(data.OfType<object>().ToList());
            //this.dataGridView1.DataSource = data;

        }
        private void ShowData(List<object> data)
        {
            if (data.Count > 0)
            {
                Type tmpClass = data[0].GetType();
                int i = 0;
                object[] cols;
                int mode;
                if (tmpClass.GetProperties().Count() > 0) {
                    cols = tmpClass.GetProperties();
                    mode = 0;
                }
                else {
                    cols = tmpClass.GetFields();
                    mode = 1;
                }
                

                foreach (var p in cols)
                {
                    DataGridViewTextBoxColumn dgc = new DataGridViewTextBoxColumn();
                    dgc.Name = (mode == 0) ? ((System.Reflection.PropertyInfo)p).Name : ((System.Reflection.FieldInfo)p).Name;
                    this.dataGridView1.Columns.Add(dgc);
                    i++;
                }

                //Debug.WriteLine(tmpClass.GetProperties().Count().ToString());
                foreach(object aa in data) {
                    object[] values=new object[i];
                    int j=0;
                    foreach (var p in cols)
                    {
                        values[j] = (mode == 0) ? ((System.Reflection.PropertyInfo)p).GetValue(aa, null).ToString() : ((System.Reflection.FieldInfo)p).GetValue(aa).ToString();
                        j++;
                    }
                    this.dataGridView1.Rows.Add(values);
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string path = @"i:\dzh2\data\";
            DZHSymbol sy = new DZHSymbol("SH", "600759");
            DZHQuoteReader dr = new DZHQuoteReader(path);
            DZHQuote aQuote = dr.RequestQuote(sy);
            DZHTickReader dr1 = new DZHTickReader(path);
            List<DZHTick> ticks = dr1.RequestTicks(sy);
            DZHTick tick = new DZHTick();
            tick.Price = aQuote.LastClose;
            ticks.Insert(0, tick);
            Single totalBuyAmount=0, totalSellAmount=0;
            for (int i = 1; i < ticks.Count();i++ )
            {
                if (ticks[i].Price > ticks[i - 1].Price) totalBuyAmount += ticks[i].Amount - ticks[i - 1].Amount;
                if (ticks[i].Price < ticks[i - 1].Price) totalSellAmount += ticks[i].Amount - ticks[i - 1].Amount;
            }
            this.textBox1.Text = totalBuyAmount.ToString();
            this.textBox2.Text = totalSellAmount.ToString();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string path = @"I:\全推大智慧\data";
            DZHSymbolReader dsr=new DZHSymbolReader(path);
            DZHQuoteReader dqr = new DZHQuoteReader(path);
            DZHTickReader dtr = new DZHTickReader(path);
            DZHFinanceReader dfr = new DZHFinanceReader(path);
            List<DZHFinance> data = new List<DZHFinance>();

            List<DZHSymbol> sys = dsr.GetSymbols("SH");
            
            foreach (DZHSymbol sy in sys)
            {
                if (sy.SecurityType == DZHSymbol.SecurityTypes.Stock && sy.StockType == DZHSymbol.StockTypes.A)
                {
                    Debug.WriteLine(sy.Code);
                    DZHFinance aFinance = dfr.RequestFinance(sy);            
                    data.Add(aFinance);
                }
            }
            data.Sort(CompareByAShares);
            this.ShowData(data.OfType<object>().ToList());
        }
        public static int CompareByAShares(DZHFinance x, DZHFinance y)//从大到小排序器  
       {  
           if (x == null)  
           {  
               if (y == null)  
               {  
                   return 0;  
               }  
  
               return 1;  
  
           }  
           if (y == null)  
           {  
               return -1;  
           }
           int retval = y.AShares.CompareTo(x.AShares);  
           return retval;  
       }  
    }
}
