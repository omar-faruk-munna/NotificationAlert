using Dapper;
using Helper;
using LogLog4Net;
using Microsoft.Extensions.Configuration;
using Model;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Database
{
    public class DbRepository : IDbRepository
    {
        private readonly Connection _connection;
        private readonly IConfiguration _configuration;
        private readonly ILoggerRepository _error;
        private readonly string _connectionString;


        public DbRepository(IConfiguration configuration, ILoggerRepository error)
        {
            this._configuration = configuration;
            this._error = error;
            _connection = new Connection();
            _connectionString = this._configuration.GetValue<string>("ConnectionStrings:DbConnString");
        }

        public async Task CreateRecord(int fncId, string productId, string branchId, string accountNo, string custId, int smsFlag, string toNumber, string toSmsContent, int smsRespFlag, string smsReasonForFail, int emailFlag, string toEmail, string emailSubject, string toEmailContent, int emailRespFlag, string emailReasonForFail, int pushFlag, int alertFlag)
        {
            const string query = "ALERT_SERVICE.alert_svc_rec_custom_i";
            using IDbConnection con = _connection.GetConnection(_connectionString);
            con.Open();

            OracleDynamicParameters parameters = new OracleDynamicParameters();
            parameters.Add("pnas_fnc_id", fncId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("pproduct_id", productId, OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("pbranch_id", branchId, OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("paccount_no", accountNo, OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("pcustomer_id", custId, OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("psms_sent_flag", smsFlag, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("pmobile_operator", "", OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("pmobile_number", toNumber, OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("psms_text", toSmsContent, OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("pemail_sent_flag", emailFlag, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("pemail_id", toEmail, OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("pemail_body", toEmailContent, OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("palert_try_no", 1, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("psms_provider_message_id", "", OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("psms_reason_for_fail", smsReasonForFail, OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("palert_flag", alertFlag, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("ppush_flag", pushFlag, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("pemail_reason_for_fail", emailReasonForFail, OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("psms_resp_flag", smsRespFlag, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("pemail_resp_flag", emailRespFlag, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("pbangla_flag", 0, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("pemail_subject", emailSubject, OracleDbType.NVarchar2, ParameterDirection.Input);

            await con.QueryAsync(query, parameters, commandType: CommandType.StoredProcedure);

            con.Close();
            con.Dispose();
        }

        public IEnumerable<dynamic> GetCustomerContact(int isMultiple, string branchId, string accountNo)
        {
            const string query = "ALERT_SERVICE.customer_contact_ga";
            using IDbConnection con = _connection.GetConnection(_connectionString);
            con.Open();

            OracleDynamicParameters parameters = new OracleDynamicParameters();
            parameters.Add("pis_multiple", isMultiple, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("pbranch_id", branchId, OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("paccount_no", accountNo, OracleDbType.NVarchar2, ParameterDirection.Input);
            parameters.Add("presult_cursor", null, OracleDbType.RefCursor, ParameterDirection.Output);

            IEnumerable<dynamic> result = con.QueryAsync(query, parameters, commandType: CommandType.StoredProcedure).Result;

            con.Close();
            con.Dispose();

            return result;
        }

        public IEnumerable<dynamic> GetCustomerTest()
        {
            const string query = "ALERT_SERVICE.alert_test_ga";
            using IDbConnection con = _connection.GetConnection(_connectionString);
            con.Open();

            OracleDynamicParameters parameters = new OracleDynamicParameters();
            parameters.Add("presult_cursor", null, OracleDbType.RefCursor, ParameterDirection.Output);

            IEnumerable<dynamic> result = con.QueryAsync(query, parameters, commandType: CommandType.StoredProcedure).Result;

            con.Close();
            con.Dispose();

            return result;
        }

    }
}
