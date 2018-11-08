using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using OpenQA.Selenium;
using OpenQA.Selenium.IE;
//using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace GFTrader
{
    public delegate string VerifyCodeBuildDelegate(IWebElement codeImage);
    public class LoginOperator:WebOperator
    {
        private IWebElement _loginForm;
        private IWebElement _customIDElement;//客户编码
        private IWebElement _passwordElement;//密码
        private IWebElement _wrongTipElement;//登录错误提示
        
        public LoginOperator(IWebDriver driver)
            : base(driver)
        {
            this.WaitForPageLoaded();
            _loginForm = this.GetElement(By.CssSelector(@".TradeLoginPopup .LoginForm form"));
            if (_loginForm != null)
            {
                _customIDElement = this.GetElement(_loginForm, By.Name("user_id"));
                _passwordElement = this.GetElement(_loginForm, By.Name("password"));
            }
        }

        public bool Login(string customID, string password)
        {

            if (_loginForm != null)
            {
                _customIDElement.Clear();
                _customIDElement.SendKeys(customID);
                _passwordElement.Clear();
                _passwordElement.SendKeys(password);
                _loginForm.Submit();
                Thread.Sleep(2000);
                try
                {
                    _wrongTipElement = _loginForm.FindElement(By.Id("wrongcue"));
                }
                catch (StaleElementReferenceException ex)
                {
                    LogHelper.LogInfo("登录成功.");
                    return true;
                }
                if (_wrongTipElement != null)
                {
                    LogHelper.LogError("登录失败,原因是:"+_wrongTipElement.Text);
                    
                }else
                {
                    LogHelper.LogError("登录失败,原因未知.");
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
