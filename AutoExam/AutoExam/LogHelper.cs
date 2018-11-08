using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace AutoExam
{
    public class LogHelper
    {
        public static void LogError(string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogHelper));
            log.Error(msg);
        }
        public static void LogInfo(string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogHelper));
            log.Info(msg);
        }
    }
}
