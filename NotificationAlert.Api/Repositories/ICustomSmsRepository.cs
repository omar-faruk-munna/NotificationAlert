using NotificationAlert.Api.Models;
using System.Threading.Tasks;

namespace NotificationAlert.Api.Repositories
{
    public interface ICustomSmsRepository
    {
        Task CustomSmsSendAsync(SmsModelWrap model);
    }
}
