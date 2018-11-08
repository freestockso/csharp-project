using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace AutoExam
{
    public class WebOperator
    {
        protected IWebDriver _driver;

        public WebOperator(IWebDriver driver)
        {
            this._driver = driver;
        }
        protected static bool isPageLoaded(IWebDriver driver)
        {
            return ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");
        }
        public void WaitForPageLoaded()
        {
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 30));
            wait.Until(isPageLoaded);
        }
        public IWebElement GetElement(By selector, int seconds = 5)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, seconds));
                return wait.Until(ExpectedConditions.ElementExists(selector));
            }
            catch (Exception ex)
            {
                LogHelper.LogError("To get element by " + selector.ToString() + "--" + ex.Message);
                return null;
            }
        }
        public IWebElement GetElement(IWebElement parentEle, By selector, int seconds = 5)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, seconds));
                return wait.Until(delegate (IWebDriver driver) {
                    return parentEle.FindElement(selector);
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogError("To get element by " + selector.ToString() + "--" + ex.Message);
                return null;
            }
        }
        public ReadOnlyCollection<IWebElement> GetElements(By selector, int seconds = 5)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, seconds));
                return wait.Until(delegate (IWebDriver driver) {
                    return _driver.FindElements(selector);
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogError("To get elements by " + selector.ToString() + "--" + ex.Message);
                return null;
            }
        }
        public ReadOnlyCollection<IWebElement> GetElements(IWebElement parentEle, By selector, int seconds = 15)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, seconds));
                return wait.Until(delegate (IWebDriver driver)
                {
                    return parentEle.FindElements(selector);
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogError("To get elements by " + selector.ToString() + "--" + ex.Message);
                return null;
            }
        }
        public void ImplicitClick(IWebElement ele)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", ele);
        }
        public void ImplicitScrollToView(IWebElement ele)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", ele);
        }
    }
}
