using System.Threading.Tasks;

namespace NotificationAlert.Api.Repositories
{
    public interface INasRepo
    {
        Task NasAsync(string dbCon);
    }
}
