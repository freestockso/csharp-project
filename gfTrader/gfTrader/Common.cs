using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GFTrader
{
    public class Common
    {

        #region 功能描述：静态成员 C#里运行命令行里的命令
        /// <summary>  
        /// 功能描述：静态成员 C#里运行命令行里的命令  
        /// </summary>  
        /// <param name="startFileName">要启动的应用程序</param>  
        /// <param name="commands">命令行数组</param>  
        /// <returns></returns>  
        public static string StartProcess(string startFileName, string[] commands)
        {
            //实例一个Process类，启动一个独立进程  
            Process p = new Process();

            //Process类有一个StartInfo属性，这个是ProcessStartInfo类，包括了一些属性和方法，  

            //下面我们用到了他的几个属性：  

            //设定程序名  
            p.StartInfo.FileName = startFileName;

            //关闭Shell的使用  
            p.StartInfo.UseShellExecute = false;

            //重定向标准输入  
            p.StartInfo.RedirectStandardInput = true;

            //重定向标准输出  
            p.StartInfo.RedirectStandardOutput = true;

            //重定向错误输出  
            p.StartInfo.RedirectStandardError = true;

            //设置不显示窗口  
            p.StartInfo.CreateNoWindow = true;

            //上面几个属性的设置是比较关键的一步。  

            //既然都设置好了那就启动进程吧  
            p.Start();

            //输入要执行的命令  
            foreach (string command in commands)
            {
                p.StandardInput.WriteLine(command);
            }

            //从输出流获取命令执行结果  
            return p.StandardOutput.ReadToEnd();
        }
        #endregion
    }  
}
