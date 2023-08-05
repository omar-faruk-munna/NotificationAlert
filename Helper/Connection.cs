using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Helper
{
    public class Connection
    {
        public IDbConnection GetConnection(string connectionString)
        {
            OracleConnection con = new OracleConnection(connectionString);
            return con;
        }
    }
}
