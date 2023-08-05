using Model;
using System.Threading.Tasks;

namespace Sms
{
    public interface ISmsRepository
    {
        Task<SmsResponse> SmsSend(string ContentType, string smsOperator, string toNumber, string smsContent);
    }
}
