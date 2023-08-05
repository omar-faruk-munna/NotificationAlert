using System.Data;
using System.Data.SqlClient;

namespace GdicNasWorker.Helpers
{
    public class Connection
    {
        public IDbConnection GetConnection(string connectionString)
        {
            SqlConnection con = new SqlConnection(connectionString);
            return con;
        }
    }
}
