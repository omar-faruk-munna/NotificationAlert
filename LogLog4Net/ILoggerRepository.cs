namespace LogLog4Net
{
    public interface ILoggerRepository
    {
        void LogError(string projectName, string functionName, string errorMessage);
        void LogLicenseExpired(string projectName, string location, string expiredMessage);
    }
}
