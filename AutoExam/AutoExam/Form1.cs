using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using System.Data.OleDb;
using System.Threading;


namespace AutoExam
{
    public partial class Form1 : Form
    {
        private Dictionary<string, int> genderEnum = new Dictionary<string, int>();
        private Dictionary<string, int> gradeEnum = new Dictionary<string, int>();
        private Dictionary<string, int> classesEnum = new Dictionary<string, int>();
        private List<string> questionList = new List<string>();
        private List<string> answerList = new List<string>();
        private List<int> quesHashList = new List<int>();
        public Form1()
        {
            InitializeComponent();
            this.genderEnum.Add("男", 0);
            this.genderEnum.Add("女", 1);
            this.gradeEnum.Add("一年级",0);
            this.gradeEnum.Add("二年级",1);
            this.gradeEnum.Add("三年级",2);
            this.gradeEnum.Add("四年级",3);
            this.gradeEnum.Add("五年级",4);
            this.gradeEnum.Add("六年级",5);
            this.gradeEnum.Add("七年级",6);
            this.gradeEnum.Add("八年级",7);
            this.gradeEnum.Add("九年级",8);
            this.gradeEnum.Add("高一",9);
            this.gradeEnum.Add("高二",10);
            this.gradeEnum.Add("高三",11);
            for(int i=0;i<50;i++)
            {
                string str = (i + 1).ToString() + "班";
                this.classesEnum.Add(str, i);
            }
                
        }

        private void btnGetAnswers_Click(object sender, EventArgs e)
        {
            DataTable stus = this.getStudentInfos();
            if (stus.Rows.Count > 0)
            {
                IWebDriver driver = new ChromeDriver();
                driver.Manage().Window.Maximize();
                WebOperator webOperator = new WebOperator(driver);
                int times = stus.Rows.Count < 10 ? stus.Rows.Count : 10;
                for (int k = 0; k < times; k++)
                {
                    DataRow astu = stus.Rows[k];
                    driver.Navigate().GoToUrl(@"http://exam.jxeduyun.com/login");
                    //等待页面加载，并输入身份证号登陆
                    webOperator.WaitForPageLoaded();
                    IWebElement loginForm = webOperator.GetElement(By.Id(@"login_form"), 15);
                    if (loginForm == null)
                    {
                        LogHelper.LogInfo("没有发现登录窗体！");
                        continue;
                    }
                    IWebElement idInput = webOperator.GetElement(loginForm, By.Id("id_card"));
                    IWebElement submitButton = webOperator.GetElement(loginForm, By.Id("id_submit"));
                    idInput.SendKeys(astu["身份证号"].ToString().Trim());
                    submitButton.Click();
                    //响应是否超时对话框
                    webOperator.WaitForPageLoaded();
                    Thread.Sleep(3000);
                   
                    ////输入姓名，学校代码，性别，年级，班级
                    IWebElement profileForm = webOperator.GetElement(By.Id(@"profile_form"));
                    if (profileForm != null)
                    {
                        IWebElement nameInput = webOperator.GetElement(profileForm, By.Name("name"));
                        IWebElement schoollInput = webOperator.GetElement(profileForm, By.Name("organization_code"));
                        ReadOnlyCollection<IWebElement> genderCheckboxs = webOperator.GetElements(profileForm, By.Name("gender"));
                        ReadOnlyCollection<IWebElement> gradeSelects = webOperator.GetElements(profileForm, By.CssSelector(@"#grade~div  .item"));
                        ReadOnlyCollection<IWebElement> classesSelects = webOperator.GetElements(profileForm, By.CssSelector(@"#classes~div  .item"));
                        IWebElement nextStepButton = webOperator.GetElement(profileForm, By.Id("next_step"));

                        nameInput.SendKeys(astu["姓名"].ToString().Trim());
                        schoollInput.SendKeys(astu["学校代码"].ToString().Trim());
                        string gender = astu["性别"].ToString().Trim();
                        webOperator.ImplicitClick(genderCheckboxs[this.genderEnum[gender]]);
                        string grade = astu["年级"].ToString().Trim();
                        webOperator.ImplicitClick(gradeSelects[this.gradeEnum[grade]]);
                        string classes = astu["班级"].ToString().Trim();
                        webOperator.ImplicitClick(classesSelects[this.classesEnum[classes]]);
                        webOperator.ImplicitClick(nextStepButton);
                        Thread.Sleep(3000);
                        IWebElement tipsModalBox = webOperator.GetElement(By.Id(@"tips_modal"));
                        if (tipsModalBox.Displayed)
                        {
                            IWebElement confirmButton = webOperator.GetElement(tipsModalBox, By.CssSelector(@"button.submit"));
                            webOperator.ImplicitClick(confirmButton);
                        }

                        ////等待出现试卷，抓取题目
                        webOperator.WaitForPageLoaded();
                        Thread.Sleep(3000);
                    }
                    //如果出现错误提示框
                    IWebElement errorModalBox = webOperator.GetElement(By.Id(@"error_modal"));
                    if (errorModalBox.Displayed)
                    {
                        IWebElement handButton = webOperator.GetElement(errorModalBox, By.CssSelector(@"button.positive"));
                        webOperator.ImplicitClick(handButton);
                    }
                    //在之前已输入学生信息后会直接出现答卷
                    ReadOnlyCollection<IWebElement> questionSegments = webOperator.GetElements(By.CssSelector(@"#question_loader div.segment.question"));
                    if (questionSegments == null || questionSegments.Count < 1)
                    {
                        LogHelper.LogInfo("没有发现题目：原因可能是网络慢，数据没有生成！");
                        continue;
                    }
                    //每个选A，点击卷
                    foreach (IWebElement questionSeg in questionSegments)
                    {
                        ReadOnlyCollection<IWebElement> optionLis = webOperator.GetElements(questionSeg, By.CssSelector(".card-action  div.field"));
                        if (optionLis.Count > 0) webOperator.ImplicitClick(optionLis[0].FindElement(By.CssSelector("label")));
                    }
                    IWebElement handExamButton = webOperator.GetElement(By.Id("submit"));
                    webOperator.ImplicitClick(handExamButton);
                    ////响应交卷对话框
                    Thread.Sleep(3000);
                    IWebElement submitModalBox = webOperator.GetElement(By.Id(@"submit_modal"));
                    if (submitModalBox.Displayed)
                    {
                        IWebElement handButton = webOperator.GetElement(submitModalBox, By.CssSelector(@"button.positive"));
                        webOperator.ImplicitClick(handButton);
                    }
                    ////响应交卷后查看成绩对话框
                    Thread.Sleep(3000);
                    IWebElement scoreModalBox = webOperator.GetElement(By.Id(@"score_modal"));
                    if (scoreModalBox.Displayed)
                    {
                        IWebElement viewAnswerButton = webOperator.GetElement(scoreModalBox, By.CssSelector(@"button.positive"));
                        webOperator.ImplicitClick(viewAnswerButton);
                    }
                    ////等待答案出现并抓取
                    webOperator.WaitForPageLoaded();
                    questionSegments = webOperator.GetElements(By.CssSelector(@"#question_loader div.segment.question"));
                    if (questionSegments == null || questionSegments.Count < 1)
                    {
                        LogHelper.LogInfo("没有发现题目：原因可能是网络慢，数据没有生成！");
                        continue;
                    }
                    ////MessageBox.Show(questionSegments.Count.ToString());
                    foreach (IWebElement questionSeg in questionSegments)
                    {
                        IWebElement questionEle = webOperator.GetElement(questionSeg, By.CssSelector(".card-content p"));
                        string[] temp = questionEle.Text.Split('.');//去掉问题的题号
                        int la = temp.Length > 1 ? 1 : 0;
                        string question = temp[la].Trim();
                        IWebElement answerEle = webOperator.GetElement(questionSeg, By.CssSelector(".card-action div.field.right label"));
                        temp = answerEle.Text.Split('.');//去掉答案的编号
                        la = temp.Length > 1 ? 1 : 0;
                        string answer = temp[la].Trim();

                        int hashcode = (question + ':' + answer).GetHashCode();//将题目与答案合并后哈希，有题目相同但答案不一样的情形，故而合并后哈希
                        if (!this.quesHashList.Contains(hashcode))
                        {
                            this.questionList.Add(question);
                            this.answerList.Add(answer);
                            this.quesHashList.Add(hashcode);
                        }
                    }
                    Thread.Sleep(1000);
                }
                this.saveQuestionAndAnswers();
                driver.Quit();
            }      
            //IWebDriver driver = new InternetExplorerDriver();
        }

        private void saveQuestionAndAnswers()
        {
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=题库.xlsx;" + "Extended Properties='Excel 12.0;HDR=Yes';";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            OleDbCommand myCommand = new OleDbCommand("",conn);
            for (int i = 0; i < this.questionList.Count; i++)
            {
                int j = i + 1;
                string strSql = "insert into [sheet1$] values(" + j.ToString() + ",\'" + this.questionList[i] + "\',\'" + this.answerList[i] + "\')";
                myCommand.CommandText = strSql;
                myCommand.ExecuteNonQuery();
            }
            conn.Close();
        }

        private DataTable getQuestionAndAnswers()
        {
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=题库.xlsx;" + "Extended Properties='Excel 12.0;HDR=Yes';";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "select * from [sheet1$]";
            OleDbDataAdapter myAdapter = new OleDbDataAdapter(strExcel, conn);
            DataSet ds = new DataSet();
            myAdapter.Fill(ds, "table1");
            conn.Close();
            return ds.Tables[0];
        }

        private DataTable getStudentInfos()
        {
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=考生信息.xlsx;" + "Extended Properties='Excel 12.0;HDR=Yes';";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "select * from [sheet1$]";
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(strExcel, strConn);
            DataSet ds = new DataSet();
            myDataAdapter.Fill(ds, "table1");
            conn.Close();
            return ds.Tables[0];
        }

        private void btnAutoExam_Click(object sender, EventArgs e)
        {
            DataTable stus = this.getStudentInfos();
            //下面三个列表用于将无法在题库中找到的题目与答案暂存，便于后面存储
            this.questionList.Clear();
            this.answerList.Clear();
            this.quesHashList.Clear();

            if (stus.Rows.Count > 0)
            {
                DataTable questionAndAnswers = this.getQuestionAndAnswers();
                List<string> questionList = new List<string>();
                List<string> answerList = new List<string>();
                List<int> quesHashList = new List<int>();
                foreach(DataRow quesAnswer in questionAndAnswers.Rows)
                {
                    string question = quesAnswer["题目"].ToString().Trim();
                    questionList.Add(question);
                    answerList.Add(quesAnswer["答案"].ToString().Trim());
                    quesHashList.Add(question.GetHashCode());//此处只将题目进行哈希，因为是依靠题目去找答案，对于相同题目有不同答案的情形，只能搜索多次
                }
                ChromeOptions option = new ChromeOptions();
                option.AddArguments("-test-type");
                IWebDriver driver = new ChromeDriver(option);

                driver.Manage().Window.Maximize();
                WebOperator webOperator = new WebOperator(driver);
                for(int k = 0; k < stus.Rows.Count; k++)
                {
                    DataRow astu = stus.Rows[k];
                    driver.Navigate().GoToUrl(@"http://exam.jxeduyun.com/login");
                    //等待页面加载，并输入身份证号登陆
                    webOperator.WaitForPageLoaded();
                    IWebElement loginForm = webOperator.GetElement(By.Id(@"login_form"), 15);
                    if (loginForm == null)
                    {
                        LogHelper.LogInfo("没有发现登录窗体！");
                        continue;
                    }
                    IWebElement idInput = webOperator.GetElement(loginForm, By.Id("id_card"));
                    IWebElement submitButton = webOperator.GetElement(loginForm, By.Id("id_submit"));
                    idInput.SendKeys(astu["身份证号"].ToString().Trim());
                    submitButton.Click();
                    //响应是否超时对话框
                    webOperator.WaitForPageLoaded();
                    Thread.Sleep(3000);

                    ////输入姓名，学校代码，性别，年级，班级
                    IWebElement profileForm = webOperator.GetElement(By.Id(@"profile_form"));
                    if (profileForm != null)
                    {
                        IWebElement nameInput = webOperator.GetElement(profileForm, By.Name("name"));
                        IWebElement schoollInput = webOperator.GetElement(profileForm, By.Name("organization_code"));
                        ReadOnlyCollection<IWebElement> genderCheckboxs = webOperator.GetElements(profileForm, By.Name("gender"));
                        ReadOnlyCollection<IWebElement> gradeSelects = webOperator.GetElements(profileForm, By.CssSelector(@"#grade~div  .item"));
                        ReadOnlyCollection<IWebElement> classesSelects = webOperator.GetElements(profileForm, By.CssSelector(@"#classes~div  .item"));
                        IWebElement nextStepButton = webOperator.GetElement(profileForm, By.Id("next_step"));

                        nameInput.SendKeys(astu["姓名"].ToString().Trim());
                        schoollInput.SendKeys(astu["学校代码"].ToString().Trim());
                        string gender = astu["性别"].ToString().Trim();
                        webOperator.ImplicitClick(genderCheckboxs[this.genderEnum[gender]]);
                        string grade = astu["年级"].ToString().Trim();
                        webOperator.ImplicitClick(gradeSelects[this.gradeEnum[grade]]);
                        string classes = astu["班级"].ToString().Trim();
                        webOperator.ImplicitClick(classesSelects[this.classesEnum[classes]]);
                        webOperator.ImplicitClick(nextStepButton);
                        Thread.Sleep(3000);
                        IWebElement tipsModalBox = webOperator.GetElement(By.Id(@"tips_modal"));
                        if (tipsModalBox.Displayed)
                        {
                            IWebElement confirmButton = webOperator.GetElement(tipsModalBox, By.CssSelector(@"button.submit"));
                            webOperator.ImplicitClick(confirmButton);
                        }

                        ////等待出现试卷，抓取题目
                        webOperator.WaitForPageLoaded();
                        Thread.Sleep(3000);
                    }
                    //如果出现错误提示框
                    IWebElement errorModalBox = webOperator.GetElement(By.Id(@"error_modal"));
                    if (errorModalBox.Displayed)
                    {
                        IWebElement handButton = webOperator.GetElement(errorModalBox, By.CssSelector(@"button.positive"));
                        webOperator.ImplicitClick(handButton);
                    }
                    //在之前已输入学生信息后会直接出现答卷
                    ReadOnlyCollection<IWebElement> questionSegments = webOperator.GetElements(By.CssSelector(@"#question_loader div.segment.question"));
                    if (questionSegments == null || questionSegments.Count < 1)
                    {
                        LogHelper.LogInfo("没有发现题目：原因可能是网络慢，数据没有生成！");
                        continue;
                    }
                    //MessageBox.Show(questionSegments.Count.ToString());
                    List<IWebElement> noAnswerQuestionSegments = new List<IWebElement>();//没有找到答案的题目段
                    foreach (IWebElement questionSeg in questionSegments)
                    {
                        IWebElement questionEle = webOperator.GetElement(questionSeg, By.CssSelector(".card-content p"));
                        string[] temp = questionEle.Text.Split('.');//去掉问题的题号
                        int la = temp.Length > 1 ? 1 : 0;
                        string question = temp[la].Trim();
                        int quesHash = question.GetHashCode();
                        int index = -1;
                        bool answered = false;
                        ReadOnlyCollection<IWebElement> optionLis = webOperator.GetElements(questionSeg, By.CssSelector(".card-action div.field"));
                        while (!answered)
                        {
                            index = quesHashList.IndexOf(quesHash,index+1);
                            if (index ==-1) break;
                            
                            foreach(IWebElement optionLi in optionLis)
                            {
                                temp = optionLi.Text.Split('.');
                                la = temp.Length > 1 ? 1 : 0;
                                string optionText= temp[la].Trim();
                                if (optionText == answerList[index])
                                {
                                    webOperator.ImplicitClick(optionLi.FindElement(By.CssSelector("label")));
                                    answered = true;
                                }
                            }
                        }
                        if (!answered)//找不到答案的情形,选A并将题目段记录下来
                        {
                            webOperator.ImplicitClick(optionLis[0].FindElement(By.CssSelector("label")));
                            noAnswerQuestionSegments.Add(questionSeg);
                        }            
                    }
                    //交卷
                    IWebElement handExamButton = webOperator.GetElement(By.Id("submit"));
                    webOperator.ImplicitClick(handExamButton);
                    //响应交卷对话框
                    Thread.Sleep(3000);
                    IWebElement submitModalBox = webOperator.GetElement(By.Id(@"submit_modal"));
                    if (submitModalBox.Displayed)
                    {
                        IWebElement handButton = webOperator.GetElement(submitModalBox, By.CssSelector(@"button.positive"));
                        webOperator.ImplicitClick(handButton);
                    }
                    //响应交卷后查看成绩对话框
                    Thread.Sleep(3000);
                    IWebElement scoreModalBox = webOperator.GetElement(By.Id(@"score_modal"));
                    if (scoreModalBox.Displayed)
                    {
                        IWebElement scoreText = webOperator.GetElement(scoreModalBox, By.CssSelector(@".content .description p:nth-child(4) span"));
                        astu["得分"] =int.Parse(scoreText.Text);
                        astu["答对题数"] = questionSegments.Count - noAnswerQuestionSegments.Count;

                        IWebElement viewAnswerButton = webOperator.GetElement(scoreModalBox, By.CssSelector(@"button.positive"));
                        webOperator.ImplicitClick(viewAnswerButton);
                    }
                    if (noAnswerQuestionSegments.Count > 0)//有未找到答案的情形时，交卷后找出答案，将题目与答案暂存
                    {
                        foreach (IWebElement questionSeg in noAnswerQuestionSegments)
                        {
                            IWebElement questionEle = webOperator.GetElement(questionSeg, By.CssSelector(".card-content p"));
                            string[] temp = questionEle.Text.Split('.');//去掉问题的题号
                            int la = temp.Length > 1 ? 1 : 0;
                            string question = temp[la].Trim();
                            IWebElement answerEle = webOperator.GetElement(questionSeg, By.CssSelector(".card-action div.field.right label"));
                            temp = answerEle.Text.Split('.');//去掉答案的编号
                            la = temp.Length > 1 ? 1 : 0;
                            string answer = temp[la].Trim();

                            int hashcode = (question + ':' + answer).GetHashCode();//将题目与答案合并后哈希，有题目相同但答案不一样的情形，故而合并后哈希
                            if (!this.quesHashList.Contains(hashcode))
                            {
                                //用于题目与答案暂存，便于以后写入excel题库文件中
                                this.questionList.Add(question);
                                this.answerList.Add(answer);
                                this.quesHashList.Add(hashcode);
                                //添加到当前使用中的题库里，便于后面的考生能查找到题目
                                questionList.Add(question);
                                answerList.Add(answer);
                                quesHashList.Add(question.GetHashCode());
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
                if (this.questionList.Count > 0)//有新的题目与答案，写入excel文件中
                {
                    this.saveQuestionAndAnswers();
                }
                //更新考生成绩
                this.updateScore(stus);
                driver.Quit();
            }
        }

        private void updateScore(DataTable scoreTable)
        {
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=考生信息.xlsx;" + "Extended Properties='Excel 12.0;HDR=Yes';";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            OleDbCommand myCommand = new OleDbCommand("", conn);
            foreach(DataRow aRow in scoreTable.Rows)
            {
                if (aRow["得分"] == null || aRow["得分"].ToString() == String.Empty) aRow["得分"] = "0";
                if (aRow["答对题数"] == null || aRow["答对题数"].ToString() == String.Empty) aRow["答对题数"] = "0";
                myCommand.CommandText = "update [sheet1$] set 得分=" + aRow["得分"] + ",答对题数="+aRow["答对题数"]+" where 序号=" + aRow["序号"];
                myCommand.ExecuteNonQuery();
            }
            conn.Close();
        }
    }
}
