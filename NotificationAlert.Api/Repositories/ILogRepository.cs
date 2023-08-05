using System.Threading.Tasks;

namespace NotificationAlert.Api.Repositories
{
    public interface ILogRepository
    {
        Task LogError(string message);
        Task LogSmsSuccess(string message);
        Task LogSmsFailed(string message);
        Task LogEmailSuccess(string message);
        Task LogEmailFailed(string message);
        Task LogLicenseExpired(string message);
        void LogErrorByLog4Net(string message);
        void LogSmsSuccessTsv(string smsDirectory, string headFormat, string toNumber, string smsContent, string accountNo, string status);
        void LogSmsFailTsv(string smsDirectory, string headFormat, string toNumber, string smsContent, string accountNo, string status);
        void LogEmailSuccessTsv(string emailDirectory, string headFormat, string toEmail, string emailContent, string accountNo, string status);
        void LogEmailFailTsv(string emailDirectory, string headFormat, string toEmail, string emailContent, string accountNo, string status);
        void LogCustomSmsSuccessTsv(string smsDirectory, string headFormat, string toNumber, string smsContent, string accountNo, string status);
        void LogCustomSmsFailTsv(string smsDirectory, string headFormat, string toNumber, string smsContent, string accountNo, string status);

    }
}
