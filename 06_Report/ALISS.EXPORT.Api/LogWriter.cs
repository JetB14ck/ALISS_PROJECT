using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;

namespace ALISS.EXPORT.Api
{
    public class LogWriter
    {
        private string m_exePath = string.Empty;
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }
        public void LogWrite(string logMessage)
        {
            m_exePath = @"C:\ALISS_ProcessFile\ANTIBIOTREND" + "\\" + "log_export" + "\\" + DateTime.Now.ToString("yyyyMMdd");

            if (!System.IO.Directory.Exists(m_exePath))
            {
                System.IO.Directory.CreateDirectory(m_exePath);
            }

            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                //txtWriter.Write("\r\nLog Entry : ");
                txtWriter.Write("Log Entry : ");
                txtWriter.WriteLine("{0} {1} : {2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), logMessage);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
