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
    //public delegate string VerifyCodeBuildDelegate(IWebElement codeImage = null);
    public delegate string VerifyCodeBuildDelegate(IWebElement codeImage);
    public class LoginOperator:WebOperator
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        private IWebElement _loginContainerElement;
        private IWebElement _customIDElement;//客户编码
        private IWebElement _passwordElement;//密码
        private IWebElement _verifyCodeElement;//验证码
        private IWebElement _codeImageElement;
        private IWebElement _loginSubmitElement;//提交按钮
        private IWebElement _loginTipElement;//登录错误
        
        public LoginOperator(IWebDriver driver)
            : base(driver)
        {
            this.WaitForPageLoaded();
            _loginContainerElement = this.GetElement(By.CssSelector(@"#loginContainer"));
            _customIDElement = this.GetElement(_loginContainerElement, By.CssSelector(@"div.accountContainer input.userInput"));
            _passwordElement = this.GetElement(_loginContainerElement, By.CssSelector(@"#pwddiv div.pwdContainer #SecurePassword"));
            _verifyCodeElement = this.GetElement(_loginContainerElement, By.CssSelector(@"input.verificationCode"));
            _codeImageElement = this.GetElement(_loginContainerElement, By.CssSelector(@"div.verificationCodeImage"));
            _loginSubmitElement = this.GetElement(_loginContainerElement, By.CssSelector(@"a.login"));
            _loginTipElement = this.GetElement(_loginContainerElement, By.CssSelector(@"p.tip"));
            
        }
        
        public bool Login(string customID, string password, VerifyCodeBuildDelegate verifyCodeBuild, int verifyCodeLength)
        {
             
            do
            {
                _customIDElement.Clear();
                _customIDElement.SendKeys(customID);
                Actions action = new Actions(_driver);
                action.SendKeys(Keys.Tab).Build().Perform();
                foreach (char c in password)
                {
                    keybd_event(Convert.ToByte(c), 0, 0, 0);
                }
                string verifyCode = verifyCodeBuild(_codeImageElement);
                if (verifyCode.Length > verifyCodeLength) verifyCode = verifyCode.Substring(0, verifyCodeLength);
                _verifyCodeElement.Clear();
                _verifyCodeElement.SendKeys(verifyCode);
                _loginSubmitElement.Click();
                Thread.Sleep(2000);
                string info;
                try
                {
                    info = _loginTipElement.Text;
                }
                catch (StaleElementReferenceException ex)
                {
                    LogHelper.LogInfo("登录成功.");
                    return true;
                }

                if (info!= "")
                {
                    LogHelper.LogError("登录失败,原因是:" + info);
                    if (_loginTipElement.Text == "验证码输入不正确!")
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    LogHelper.LogInfo("登录成功.");
                    return true;
                }
            } while (true);
        }
    }
}
