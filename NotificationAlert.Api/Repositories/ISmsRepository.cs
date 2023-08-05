using System.Threading.Tasks;

namespace NotificationAlert.Api.Repositories
{
    public interface ISmsRepository
    {
        Task UflSmsSendApiAsync(string smsContent, string toNumber, string accountNo);
        Task CustomUflSmsSendApiAsync(string smsContent, string toNumber, string accountNo);
        Task CustomSslApiHttpClientAsync(string smsContent, string toNumber, string accountNo);
        Task SendSmsAsync(string smsContent, string toNumber, string smsOperator, string accountNo);
        Task CustomSslApiBothHttpClientAsync(string smsContent, string toNumber, string accountNo);

    }
}
