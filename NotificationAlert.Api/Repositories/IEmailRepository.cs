using Oracle.ManagedDataAccess.Types;
using System.Threading.Tasks;

namespace NotificationAlert.Api.Repositories
{
    public interface IEmailRepository
    {
        Task SendEmailAsync(OracleClob clob, string toEmail, string accountNo);
        Task SendEmailAsync(string clob, string toEmail, string accountNo, string msg_fnc_nm, string content);
    }
}
