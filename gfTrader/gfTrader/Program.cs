using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;

using OpenQA.Selenium;
using OpenQA.Selenium.IE;
//using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace GFTrader
{
    class Program
    {
        protected static IWebDriver _driver;

        //public static string verifyCodeBuild(IWebElement codeImage)
        //{
        //    byte[] imageBytes=((ITakesScreenshot)_driver).GetScreenshot().AsByteArray;
        //    //读入MemoryStream对象
        //    MemoryStream memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length);
        //    memoryStream.Write(imageBytes, 0, imageBytes.Length);
        //    //转成源图片
        //    Bitmap source =(Bitmap)Image.FromStream(memoryStream);
        //    //裁剪到目标图片
        //    Point pos = codeImage.Location;
        //    int width = codeImage.Size.Width;
        //    int height = codeImage.Size.Height;
        //    Bitmap dest = new Bitmap(width, height);
        //    Rectangle destRect = new Rectangle(0, 0, width, height);//矩形容器
        //    Rectangle srcRect = new Rectangle(pos.X, pos.Y, width, height);
        //    Graphics graph=Graphics.FromImage(dest);
        //    graph.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
        //    UnCodebase uc = new UnCodebase(dest);
        //    uc.GrayByPixels();
        //    int dgDray = uc.GetDgGrayValue();
        //    uc.ClearNoise(dgDray, 4);
        //    uc.ClearNoise(dgDray);
        //    //uc.GetPicValidByValue(dgDray,5);
        //    dest = uc.bmpobj;
        //    dest.Save("verifycode.bmp", ImageFormat.Bmp);
        //    Common.StartProcess("cmd.exe", new string[] { String.Format(@"tesseract.exe {0}\verifycode.bmp {0}\result -l eng", @".\"), "exit" });
        //    Thread.Sleep(1000);
        //    FileStream file = new FileStream(@".\result.txt", FileMode.Open);
        //    StreamReader sr = new StreamReader(file);
        //    string code = sr.ReadLine();
        //    file.Close();
        //    if (code == null) code = "88888";
        //    return code;
        //    //return Console.ReadLine();
        //}
        public static string verifyCodeBuild(IWebElement codeImage)
        {
            return Console.ReadLine();
        }
        static void Main(string[] args)
        {
            IWebDriver driver = new InternetExplorerDriver();
            _driver = driver;
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(@"https://trade.gf.com.cn/");
            LoginOperator lop = new LoginOperator(driver);

            if (lop.Login("210100014580", "328077", verifyCodeBuild, 5))
            {
                TradeOperator trp = new TradeOperator(driver);
            }
            //driver.Quit();
        }
    }
}
