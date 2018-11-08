using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Data.OleDb;



namespace SalaryAccounting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "EXCEL文件(*.xlsx)|*.xlsx";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = fileDialog.FileName;           
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "")
            {
                MessageBox.Show("工资表文件名不能为空!", "错误");
            }else
            {
                try
                {
                    string filename = this.textBox1.Text.Trim();
                    string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source="+filename+";" + "Extended Properties='Excel 12.0;HDR=Yes';";
                    OleDbConnection conn = new OleDbConnection(strConn);
                    conn.Open();
                    string strExcel = "select * from [员工表$]";
                    OleDbDataAdapter myAdapter = new OleDbDataAdapter(strExcel, conn);
                    DataSet ds = new DataSet();
                    myAdapter.Fill(ds, "employees");
                    strExcel = "select * from [产品项目表$]";
                    myAdapter = new OleDbDataAdapter(strExcel, conn);
                    myAdapter.Fill(ds, "products");
                    strExcel = "select * from [员工工作量化明细表$]";
                    myAdapter = new OleDbDataAdapter(strExcel, conn);
                    myAdapter.Fill(ds, "details");
                    
                    Dictionary<string, double> priceDict = new Dictionary<string, double>();
                    DataRowCollection products = ds.Tables["products"].Rows;
                    for (int i = 0; i < products.Count; i++)
                    {
                        DataRow aProduct = products[i];
                        if (aProduct["产品项目名称"] is DBNull) throw new Exception("【产品项目表】中（除标题行外）的第" + (i + 1).ToString() + "行的<产品项目名称>为空，请输入数据，或是删除此行！");
                        string productName = Convert.ToString(aProduct["产品项目名称"]);
                        if (productName == "") throw new Exception("【产品项目表】中（除标题行外）的第" + (i + 1).ToString() + "行的<产品项目名称>为空，请输入数据，或是删除此行！");
                        if (priceDict.ContainsKey(productName))
                            throw new Exception("【产品项目表】中（除标题行外）的第" + (i + 1).ToString() + "行的<产品项目名称>与前面的重复，请修改数据，或是删除此行！");
                        priceDict.Add(productName, Convert.ToDouble(aProduct["完成单价"]));

                    }
      
                    Dictionary<string, double> salaryDict = new Dictionary<string, double>();
                    DataRowCollection employees = ds.Tables["employees"].Rows;
                    for(int i = 0; i < employees.Count; i++)
                    {
                        DataRow aEmployee = employees[i];
                        if (aEmployee["员工姓名"] is DBNull) throw new Exception("【员工表】中（除标题行外）的第" + (i + 1).ToString() + "行的<员工姓名>为空，请输入数据，或是删除此行！");
                        string employeeName = Convert.ToString(aEmployee["员工姓名"]);
                        if (employeeName == "") throw new Exception("【员工表】中（除标题行外）的第" + (i + 1).ToString() + "行的<员工姓名>为空，请输入数据，或是删除此行！");
                        if (salaryDict.ContainsKey(employeeName))
                            throw new Exception("【员工表】中（除标题行外）的第" + (i + 1).ToString() + "行的<员工姓名>与前面的重复，请修改数据，或是删除此行！");
                        salaryDict.Add(employeeName, 0);
                    }

                    DataRowCollection details = ds.Tables["details"].Rows;
                    for(int i = 0; i < details.Count; i++)
                    {
                        DataRow aDetail = details[i];
                        if (aDetail["员工"] is DBNull) throw new Exception("【员工工作量化明细表】中（除标题行外）的第" + (i + 1).ToString() + "行的<员工>为空，请输入数据，或是删除此行！");
                        string name = Convert.ToString(aDetail["员工"]);
                        if (name == "") throw new Exception("【员工工作量化明细表】中（除标题行外）的第" + (i + 1).ToString() + "行的<员工>为空，请输入数据，或是删除此行！");
                        if (aDetail["产品项目"] is DBNull) throw new Exception("【员工工作量化明细表】中（除标题行外）的第" + (i + 1).ToString() + "行的<产品项目>为空，请输入数据，或是删除此行！");
                        string product = Convert.ToString(aDetail["产品项目"]);
                        if (product == "") throw new Exception("【员工工作量化明细表】中（除标题行外）的第" + (i + 1).ToString() + "行的<产品项目>为空，请输入数据，或是删除此行！");
                        if (!salaryDict.ContainsKey(name))
                            throw new Exception("【员工工作量化明细表】中（除标题行外）的第" + (i + 1).ToString() + "行的员工："+name+"，在【员工表】中不存在，请仔细核对！");
                        if (!priceDict.ContainsKey(product))
                            throw new Exception("【员工工作量化明细表】中（除标题行外）的第" + (i + 1).ToString() + "行的产品项目：" + product + "，在【产品项目表】中不存在，请仔细核对！");
                        salaryDict[name] += Convert.ToDouble(aDetail["完成件数"]) * priceDict[product];
                    }
                    
                    OleDbCommand insertCommand = new OleDbCommand();
                    insertCommand.Connection = conn;
                    int no = 0;
                    foreach (KeyValuePair<string,double> salaryKvp in salaryDict)
                    {
                        no++;
                        string name = salaryKvp.Key;
                        double salary = salaryKvp.Value;
                        
                        insertCommand.CommandText = "insert into [员工工资表$](编号,员工,量化工资) values(" + no + ",\'" + name + "\'," + salary + ")";
                        insertCommand.ExecuteNonQuery();
                    }
                    conn.Close();
                    MessageBox.Show("工资核算完毕，你可以打开工资表查阅了。");
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误");
                }

            }
        }
    }
}
