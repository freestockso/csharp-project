using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;

namespace GFTrader
{
    public partial class FormTrader : Form
    {
        private IWebDriver _driver;
        public FormTrader()
        {
            InitializeComponent();
            IWebDriver driver = new InternetExplorerDriver();
            _driver = driver;
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(@"http://hippo.gf.com.cn/#StockTrade/");
            LoginOperator lop = new LoginOperator(_driver);
            if (lop.Login("210100014580", "328077"))
            {
                //TradeOperator trp = new TradeOperator(_driver);
                MessageBox.Show("succeed.");
            }
            
        }      
    }
}
