using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;

namespace LogLog4Net
{
    public class LoggerRepository : ILoggerRepository
    {
        private static readonly ILog _errorLog = LogManager.GetLogger("ErrorLog");
        private static readonly ILog _licenseLog = LogManager.GetLogger("LogLicense");

        public void LogError(string projectName, string functionName, string errorMessage)
        {
            var dirname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            XmlConfigurator.Configure(new FileInfo(string.Format("{0}{1}", dirname, @"\log4net.config")));
            string sLogFormat = $"{DateTime.Now} === Project Name: {projectName} === Function Name: {functionName} === Message: {errorMessage} {Environment.NewLine}";
            _errorLog.InfoFormat(sLogFormat);
        }

        public void LogLicenseExpired(string projectName, string location, string expiredMessage)
        {
            string directory = $"{location}\\{DateTime.Now.Year}-{DateTime.Now:MM}-{DateTime.Now.Day}.tsv";

            if (File.Exists(directory))
            {
                return;
            }
            else
            {
                var dirname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                XmlConfigurator.Configure(new FileInfo(string.Format("{0}{1}", dirname, @"\log4net.config")));
                string sLogFormat = $"{DateTime.Now} === Project Name: {projectName} === Message: {expiredMessage} {Environment.NewLine}";
                _licenseLog.InfoFormat(sLogFormat);
            }
        }







    }
}
