using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using NotificationAlert.Api.Repositories;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NotificationAlert.Api.Services
{
    public class LogService : ILogRepository
    {
        private readonly IConfiguration _configuration;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _nasLogLoc;
        private readonly string _nasErrorLogLoc;
        private readonly string _nasLic;

        public LogService(IConfiguration configuration)
        {
            _configuration = configuration;
            _nasLogLoc = _configuration.GetSection("NasLogLocation").Value;
            _nasErrorLogLoc = _configuration.GetSection("ErrorDir").Value;
            _nasLic = _configuration.GetSection("NasLic").Value;
        }

        #region Helpers Function
        private async Task WriteTextAsync(string filePath, string errorMessage)
        {
            try
            {
                string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " <===========> ";
                byte[] encodedText = Encoding.Unicode.GetBytes(sLogFormat + errorMessage);

                using (FileStream sourceStream = new FileStream(filePath,
                    FileMode.Append, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                    sourceStream.Dispose();
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
        //private void CreateErrorTsv(string path, string message)
        //{
        //    try
        //    {
        //        StringBuilder tsvcontent = new StringBuilder();
        //        string messageFormat = $"{DateTime.Now.ToString()}\t{message}";
        //        tsvcontent.AppendLine(messageFormat);
        //        System.IO.File.AppendAllText(path, tsvcontent.ToString());
        //    }
        //    catch (Exception e)
        //    {
        //        LogError(e.Message).Wait();
        //    }
        //}
        private void CreateSmsSuccessHeader(string path, string headFormat)
        {
            try
            {
                StringBuilder tsvcontent = new StringBuilder();
                tsvcontent.AppendLine(headFormat);
                System.IO.File.AppendAllText(path, tsvcontent.ToString());
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        private void CreateSmsFailHeader(string path, string headFormat)
        {
            try
            {
                StringBuilder tsvcontent = new StringBuilder();
                tsvcontent.AppendLine(headFormat);
                System.IO.File.AppendAllText(path, tsvcontent.ToString());
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        private void CreateEmailSuccessHeader(string path, string headFormat)
        {
            try
            {
                StringBuilder tsvcontent = new StringBuilder();
                tsvcontent.AppendLine(headFormat);
                System.IO.File.AppendAllText(path, tsvcontent.ToString());
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        private void CreateEmailFailHeader(string path, string headFormat)
        {
            try
            {
                StringBuilder tsvcontent = new StringBuilder();
                tsvcontent.AppendLine(headFormat);
                System.IO.File.AppendAllText(path, tsvcontent.ToString());
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        private void CustomSmsSuccessHeader(string path, string headFormat)
        {
            try
            {
                StringBuilder tsvcontent = new StringBuilder();
                tsvcontent.AppendLine(headFormat);
                System.IO.File.AppendAllText(path, tsvcontent.ToString());
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        private void CustomSmsFailHeader(string path, string headFormat)
        {
            try
            {
                StringBuilder tsvcontent = new StringBuilder();
                tsvcontent.AppendLine(headFormat);
                System.IO.File.AppendAllText(path, tsvcontent.ToString());
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        private void CreateSmsSuccessBody(string path, string mobileNo, string message, string accountNo, string status)
        {
            try
            {
                string productId = string.Empty;
                StringBuilder tsvcontent = new StringBuilder();

                if (!string.IsNullOrEmpty(accountNo))
                {
                    productId = accountNo.Substring(0, 3);
                }

                string messageFormat = $"{DateTime.Now}\t{productId}\t{accountNo}\t{mobileNo}\t{message}\t{status}";
                tsvcontent.AppendLine(messageFormat);
                System.IO.File.AppendAllText(path, tsvcontent.ToString());
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        private void CreateSmsFailBody(string path, string mobileNo, string message, string accountNo, string status)
        {
            try
            {
                string productId = string.Empty;
                StringBuilder tsvcontent = new StringBuilder();

                if (!string.IsNullOrEmpty(accountNo))
                {
                    productId = accountNo.Substring(0, 3);
                }

                string messageFormat = $"{DateTime.Now}\t{productId}\t{accountNo}\t{mobileNo}\t{message}\t{status}";
                tsvcontent.AppendLine(messageFormat);
                System.IO.File.AppendAllText(path, tsvcontent.ToString());
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        private void CreateEmailSuccessBody(string path, string email, string message, string accountNo, string status)
        {
            try
            {
                string productId = string.Empty;
                StringBuilder tsvcontent = new StringBuilder();

                if (!string.IsNullOrEmpty(accountNo))
                {
                    productId = accountNo.Substring(0, 3);
                }

                string messageFormat = $"{DateTime.Now}\t{productId}\t{accountNo}\t{email}\t{message}\t{status}";
                tsvcontent.AppendLine(messageFormat);
                System.IO.File.AppendAllText(path, tsvcontent.ToString());
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        private void CreateEmailFailBody(string path, string email, string message, string accountNo, string status)
        {
            try
            {
                string productId = string.Empty;
                StringBuilder tsvcontent = new StringBuilder();

                if (!string.IsNullOrEmpty(accountNo))
                {
                    productId = accountNo.Substring(0, 3);
                }

                string messageFormat = $"{DateTime.Now}\t{productId}\t{accountNo}\t{email}\t{message}\t{status}";
                tsvcontent.AppendLine(messageFormat);
                System.IO.File.AppendAllText(path, tsvcontent.ToString());
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        private void CustomSmsSuccessBody(string path, string mobileNo, string message, string accountNo, string status)
        {
            try
            {
                string productId = string.Empty;
                StringBuilder tsvcontent = new StringBuilder();

                if (!string.IsNullOrEmpty(accountNo))
                {
                    productId = accountNo.Substring(0, 3);
                }

                string messageFormat = $"{DateTime.Now}\t{productId}\t{accountNo}\t{mobileNo}\t{message}\t{status}";
                tsvcontent.AppendLine(messageFormat);
                System.IO.File.AppendAllText(path, tsvcontent.ToString());
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        private void CustomSmsFailBody(string path, string mobileNo, string message, string accountNo, string status)
        {
            try
            {
                string productId = string.Empty;
                StringBuilder tsvcontent = new StringBuilder();

                if (!string.IsNullOrEmpty(accountNo))
                {
                    productId = accountNo.Substring(0, 3);
                }

                string messageFormat = $"{DateTime.Now}\t{productId}\t{accountNo}\t{mobileNo}\t{message}\t{status}";
                tsvcontent.AppendLine(messageFormat);
                System.IO.File.AppendAllText(path, tsvcontent.ToString());
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }

        #endregion

        public void LogErrorByLog4Net(string errorMessage)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log.Info(errorMessage);
        }
        public Task LogError(string errorMessage)
        {
            //if (_errorLogRead == "1")
            //{
            string filePath = _nasErrorLogLoc + "\\NAS_ERROR_LOG.txt";
            return WriteTextAsync(filePath, errorMessage);
            //}
            //else
            //{
            //    return Task.FromResult<object>(null);
            //}
        }
        public Task LogSmsSuccess(string message)
        {
            string filePath = _nasLogLoc + "\\NAS_SMS_SUCCESS_LOG.txt";
            return WriteTextAsync(filePath, message);
        }
        public Task LogSmsFailed(string message)
        {
            string filePath = _nasLogLoc + "\\NAS_SMS_FAILED_LOG.txt";
            return WriteTextAsync(filePath, message);
        }
        public Task LogEmailSuccess(string message)
        {
            string filePath = _nasLogLoc + "\\NAS_EMAIL_SUCCESS_LOG.txt";
            return WriteTextAsync(filePath, message);
        }
        public Task LogEmailFailed(string message)
        {
            string filePath = _nasLogLoc + "\\NAS_EMAIL_FAILED_LOG.txt";
            return WriteTextAsync(filePath, message);
        }
        public Task LogLicenseExpired(string message)
        {
            try
            {
                string directory = $@"{_nasLic}\{DateTime.Now.Year}-{DateTime.Now:MMM}-{DateTime.Now.Day}";

                if (Directory.Exists(directory))
                {
                    return Task.FromResult(0);
                }
                else
                {
                    Directory.CreateDirectory(directory);
                    string path = $@"{directory}\LICENSE_EXPIRED.txt";
                    return WriteTextAsync(path, message);
                }
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
                return Task.FromResult(0);
            }


        }
        public void LogSmsSuccessTsv(string smsDirectory, string headFormat, string phoneNo, string message, string accountNo, string status)
        {
            try
            {
                string fileNameFormat = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.tsv";
                string directory = $@"{smsDirectory}\{DateTime.Now.Year}-{DateTime.Now:MMM}-{DateTime.Now.Day}";
                string path = $@"{directory}\{fileNameFormat}";

                if (Directory.Exists(directory))
                {
                    CreateSmsSuccessBody(path, phoneNo, message, accountNo, status);
                }
                else
                {
                    Directory.CreateDirectory(directory);
                    CreateSmsSuccessHeader(path, headFormat);
                    CreateSmsSuccessBody(path, phoneNo, message, accountNo, status);
                }
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        public void LogSmsFailTsv(string smsDirectory, string headFormat, string phoneNo, string message, string accountNo, string status)
        {
            try
            {
                string fileNameFormat = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.tsv";
                string directory = $@"{smsDirectory}\{DateTime.Now.Year}-{DateTime.Now:MMM}-{DateTime.Now.Day}";
                string path = $@"{directory}\{fileNameFormat}";

                if (Directory.Exists(directory))
                {
                    CreateSmsFailBody(path, phoneNo, message, accountNo, status);
                }
                else
                {
                    Directory.CreateDirectory(directory);
                    CreateSmsFailHeader(path, headFormat);
                    CreateSmsFailBody(path, phoneNo, message, accountNo, status);
                }
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        public void LogEmailSuccessTsv(string emailDirectory, string headFormat, string email, string message, string accountNo, string status)
        {
            try
            {
                string fileNameFormat = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.tsv";
                string directory = $@"{emailDirectory}\{DateTime.Now.Year}-{DateTime.Now:MMM}-{DateTime.Now.Day}";
                string path = $@"{directory}\{fileNameFormat}";

                if (Directory.Exists(directory))
                {
                    CreateEmailSuccessBody(path, email, message, accountNo, status);
                }
                else
                {
                    Directory.CreateDirectory(directory);
                    CreateEmailSuccessHeader(path, headFormat);
                    CreateEmailSuccessBody(path, email, message, accountNo, status);
                }
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        public void LogEmailFailTsv(string emailDirectory, string headFormat, string email, string message, string accountNo, string status)
        {
            try
            {
                string fileNameFormat = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.tsv";
                string directory = $@"{emailDirectory}\{DateTime.Now.Year}-{DateTime.Now:MMM}-{DateTime.Now.Day}";
                string path = $@"{directory}\{fileNameFormat}";

                if (Directory.Exists(directory))
                {
                    CreateEmailFailBody(path, email, message, accountNo, status);
                }
                else
                {
                    Directory.CreateDirectory(directory);
                    CreateEmailFailHeader(path, headFormat);
                    CreateEmailFailBody(path, email, message, accountNo, status);
                }
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        public void LogCustomSmsSuccessTsv(string smsDirectory, string headFormat, string phoneNo, string message, string accountNo, string status)
        {
            try
            {
                string fileNameFormat = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.tsv";
                string directory = $@"{smsDirectory}\{DateTime.Now.Year}-{DateTime.Now:MMM}-{DateTime.Now.Day}";
                string path = $@"{directory}\{fileNameFormat}";

                if (Directory.Exists(directory))
                {
                    CustomSmsSuccessBody(path, phoneNo, message, accountNo, status);
                }
                else
                {
                    Directory.CreateDirectory(directory);
                    CustomSmsSuccessHeader(path, headFormat);
                    CustomSmsSuccessBody(path, phoneNo, message, accountNo, status);
                }
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }
        public void LogCustomSmsFailTsv(string smsDirectory, string headFormat, string phoneNo, string message, string accountNo, string status)
        {
            try
            {
                string fileNameFormat = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.tsv";
                string directory = $@"{smsDirectory}\{DateTime.Now.Year}-{DateTime.Now:MMM}-{DateTime.Now.Day}";
                string path = $@"{directory}\{fileNameFormat}";

                if (Directory.Exists(directory))
                {
                    CustomSmsFailBody(path, phoneNo, message, accountNo, status);
                }
                else
                {
                    Directory.CreateDirectory(directory);
                    CustomSmsFailHeader(path, headFormat);
                    CustomSmsFailBody(path, phoneNo, message, accountNo, status);
                }
            }
            catch (Exception e)
            {
                LogError(e.Message).Wait();
            }
        }

    }
}
