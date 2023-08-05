using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace NasWorker.Helpers
{
    public class Connection
    {
        public IDbConnection GetConnection(string connectionString)
        {
            OracleConnection conn = new OracleConnection(connectionString);
            return conn;
        }
    }
}
