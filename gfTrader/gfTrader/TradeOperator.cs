using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;

using OpenQA.Selenium;
using OpenQA.Selenium.IE;
//using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace GFTrader
{
    public class TradeOperator:WebOperator
    {
        private IWebElement _tradeRegion;
        private IWebElement _topRegion;
        private IWebElement _leftRegion;
        private IWebElement _rightRegion;
        private IWebElement _bottomRegion;
        private IWebElement _tipsRegion;
        private IWebElement _stockMenuItem;
        private IWebElement _noticeForm;
        private IWebElement _noticeCloseButton;

        private IWebElement _leftMenuTree;
        private ReadOnlyCollection<IWebElement> _leftMenuItems;

        private IWebElement _entrustBuyMenuItem;
        private IWebElement _entrustSellMenuItem;
        private IWebElement _entrustCancelMenuItem;
        private IWebElement _stockQueryMenuItem;
        private IWebElement _queryFundMenuItem;
        private IWebElement _queryTodayEntrustMenuItem;
        private IWebElement _queryTodayDealMenuItem;
        private ReadOnlyCollection<IWebElement> _fundViewTableCells;
        private IWebElement _nextPageForStock;
        private ReadOnlyCollection<IWebElement> _stockListTableRecords;
        private IWebElement _buyInputForm;
        private IWebElement _stockCodeInput;
        private IWebElement _buyPriceInput;
        private IWebElement _buyAmountInput;
        private IWebElement _buySubmit;

        public TradeOperator(IWebDriver driver)
            : base(driver)
        {
            this.WaitForPageLoaded();
            _tradeRegion = this.GetElement(By.Id("ext-gen44"));
            _topRegion = this.GetElement(_tradeRegion, By.Id("ext-gen3"));
            _leftRegion=this.GetElement(_tradeRegion, By.Id("ext-gen5"));
            _rightRegion=this.GetElement(_tradeRegion, By.Id("ext-gen6"));
            _bottomRegion=this.GetElement(_tradeRegion, By.Id("ext-gen4"));
            //_tipsRegion = this.GetElement(By.Id("ext-comp-1001"));
            
            _noticeForm = this.GetElement(By.ClassName("x-window"));
            if (_noticeForm!=null&&_noticeForm.Displayed)
            {
                _noticeCloseButton = this.GetElement(_noticeForm, By.ClassName("x-tool-close"));
                _noticeCloseButton.Click();
            }
            _stockMenuItem = this.GetElement(_topRegion, By.CssSelector(".icon-menu-stock"));
            _stockMenuItem.Click();
            Thread.Sleep(1000);
            _leftMenuTree = this.GetElement(_leftRegion, By.CssSelector(@".x-tree-root-ct"));
            _leftMenuItems = this.GetElements(_leftMenuTree, By.CssSelector(@".x-tree-node div"));
            Actions action = new Actions(_driver);
            _entrustBuyMenuItem = _leftMenuItems[0];
            _entrustSellMenuItem = _leftMenuItems[1];
            _entrustCancelMenuItem = _leftMenuItems[2];
            _stockQueryMenuItem = _leftMenuItems[3];
            _entrustBuyMenuItem.Click();
            _buyInputForm = this.GetElement(_rightRegion, By.CssSelector(@"#EntrustbuyView .x-form"), 15);
            _stockCodeInput = this.GetElement(_buyInputForm, By.Name("stock_code"));
            _buyPriceInput = this.GetElement(_buyInputForm, By.Name("stock_price"));
            _buyAmountInput = this.GetElement(_buyInputForm, By.Name("stock_amount"));
            _buySubmit = this.GetElement(_buyInputForm, By.Id("ext-comp-1627"));
            _entrustSellMenuItem.Click();
            Thread.Sleep(3000);
            _entrustCancelMenuItem.Click();
            Thread.Sleep(3000);
            action.MoveToElement(_stockQueryMenuItem).DoubleClick().Build().Perform();
            //_queryFundMenuItem = this.GetElement(_leftRegion, By.CssSelector(@".x-tree-node div[ext:tree-node-id='FundView']"));
            //_queryTodayDealMenuItem = this.GetElement(_leftRegion, By.CssSelector(@".x-tree-node div[ext:tree-node-id='DrcjView']"));
            //_queryTodayEntrustMenuItem = this.GetElement(_leftRegion, By.CssSelector(@".x-tree-node div[ext:tree-node-id='DrwtView']"));

            
            //_fundViewTableCells = this.GetElements(_rightRegion, By.CssSelector(@"#FundView #ext-gen1894 table tbody tr td"));
            //_nextPageForStock = this.GetElement(_rightRegion, By.CssSelector(@"#FundView #ext-comp-1418"));
            ////_stockListTableRecords=
            

        }
    }
}
