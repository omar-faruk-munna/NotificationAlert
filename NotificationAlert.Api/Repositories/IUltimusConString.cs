using System.Threading.Tasks;

namespace NotificationAlert.Api.Repositories
{
    public interface IUltimusConString
    {
        Task<string> GetUltimusConString();
    }
}
