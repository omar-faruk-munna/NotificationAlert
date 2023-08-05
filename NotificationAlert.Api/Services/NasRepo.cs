using Dapper;
using Microsoft.Extensions.Configuration;
using NotificationAlert.Api.Helpers;
using NotificationAlert.Api.Repositories;
using Oracle.ManagedDataAccess.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationAlert.Api.Services
{
    public class NasRepo : INasRepo
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly Connection _connection;
        private readonly ILogRepository _errorLog;
        private readonly IEmailRepository _emailRepo;
        private readonly ISmsRepository _smsRepo;
        private readonly string _bankName;
        #endregion

        public NasRepo(IConfiguration configuration, ILogRepository errorLog, IEmailRepository emailRepo, ISmsRepository smsRepo)
        {
            this._configuration = configuration;
            this._connection = new Connection();
            this._errorLog = errorLog;
            this._emailRepo = emailRepo;
            this._smsRepo = smsRepo;
            this._bankName = _configuration.GetSection("BankName").Value;
        }

        public async Task NasAsync(string dbCon)
        {
            try
            {
                //ufl:31-12-2030, mmbl:5-4-2021, lbfl:5-4-2021
                if (DateTime.Now.Ticks > new DateTime(2030, 12, 31).Ticks)
                {
                    _errorLog.LogLicenseExpired("License Expired on 31-12-2030").Wait();
                    return;
                }

                OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();
                dynamicParameters.Add("pname_value_list", 0, OracleDbType.Int32, ParameterDirection.Input);
                dynamicParameters.Add("pmsg_fnc_id", null, OracleDbType.NVarchar2, ParameterDirection.Input);
                dynamicParameters.Add("presult_set_cur", null, OracleDbType.RefCursor, ParameterDirection.Output);
                //const string query = "NOTIFICATION_ALERT_SERVICE.nas_alert_msg_fnc_ga";
                const string query = "ALERT_SERVICE.ALERT_SVC_msg_fnc_ga";

                using IDbConnection con = _connection.GetConnection(dbCon);
                con.Open();

                try
                {
                    IEnumerable<dynamic> result = await con.QueryAsync<dynamic>(query, dynamicParameters, commandType: CommandType.StoredProcedure);
                    result = result.Cast<IDictionary<string, object>>();
                    Parallel.ForEach(result, async rows =>
                    {
                        if (!(rows is IDictionary<string, object> fields))
                        {
                            return;
                        }

                        int delay = Convert.ToInt32(fields["TIMER_SCAN_INTRV"]);
                        string processName = Convert.ToString(fields["PROCESS_NAME"]);
                        string freq = Convert.ToString(fields["PROCESS_FREQ"]);
                        DateTime lastExec = Convert.ToDateTime(Convert.ToString(fields["PROCESS_LAST_EXEC"]));
                        string msg_fnc_nm = Convert.ToString(fields["MSG_FNC_NM"]);

                        await SendAlertAsync(freq, processName, lastExec, delay, msg_fnc_nm, dbCon);
                        await Task.Delay(10);
                    });
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    _errorLog.LogError("Connection Exeption").Wait();
                }

                con.Close();
                con.Dispose();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                _errorLog.LogError(e.Message).Wait();
            }
        }
        public async Task SendAlertAsync(string frequency, string process, DateTime lastExecution, int delay, string msg_fnc_nm, string dbCon)
        {
            try
            {
                DateTime today = DateTime.Today;
                TimeSpan sinceLastExec = today.Subtract(lastExecution);
                IEnumerable<dynamic> result = null;
                OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();
                dynamicParameters.Add("pauth_status_id", "A", OracleDbType.NVarchar2, ParameterDirection.Input);
                dynamicParameters.Add("presult_set_cur", null, OracleDbType.RefCursor, ParameterDirection.Output);

                using IDbConnection con = _connection.GetConnection(dbCon);
                try
                {
                    con.Open();

                    if (frequency == "D" && sinceLastExec.Days >= 1)
                    {
                        try
                        {
                            result = await con.QueryAsync(process, dynamicParameters, commandType: CommandType.StoredProcedure);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                            _errorLog.LogError(e.Message).Wait();
                        }
                    }
                    else if (frequency == "M" && today.Day >= 1 && sinceLastExec.Days >= 1)
                    {
                        try
                        {
                            result = await con.QueryAsync(process, dynamicParameters, commandType: CommandType.StoredProcedure);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                            _errorLog.LogError(e.Message).Wait();
                        }
                    }
                    else if (frequency == "T")
                    {
                        try
                        {
                            Task.Run(async () =>
                            {
                                result = await con.QueryAsync(process, dynamicParameters, commandType: CommandType.StoredProcedure);
                            }).Wait(TimeSpan.FromMilliseconds(delay));
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                            _errorLog.LogError(e.Message).Wait();
                        }
                    }

                    result = result?.Cast<IDictionary<string, object>>();

                    if (result != null)
                    {
                        Parallel.ForEach(result, async rows =>
                        {
                            if (rows is IDictionary<string, object> fields)
                            {
                                int emailFlag = Convert.ToInt32(fields["EMAIL_SENT_FLAG"]);
                                int smsFlag = Convert.ToInt32(fields["SMS_SENT_FLAG"]);
                                string smsContent = Convert.ToString(fields["SMS_TEXT"]);
                                string accountNo = Convert.ToString(fields["ACCOUNT_NO"]);

                                if (emailFlag == 1)
                                {
                                    #region Email
                                    string emailBlob = Convert.ToString(fields["EMAIL_BODY"]);
                                    string toEmail = Convert.ToString(fields["EMAIL_ID"]);
                                    //OracleClob emailBlob = (OracleClob)fields["EMAIL_BODY"];

                                    switch (_bankName.ToUpper())
                                    {
                                        case "LBFL"://lanka bangla
                                            await _emailRepo.SendEmailAsync(emailBlob, toEmail, accountNo, msg_fnc_nm, smsContent);
                                            break;
                                        case "MMBL"://modhu moti
                                            await _emailRepo.SendEmailAsync(emailBlob, toEmail, accountNo, msg_fnc_nm, smsContent);
                                            break;
                                        case "UFL"://united finance
                                            await _emailRepo.SendEmailAsync(emailBlob, toEmail, accountNo, msg_fnc_nm, smsContent);
                                            break;
                                        case "UBL"://uttara bank
                                            await _emailRepo.SendEmailAsync(emailBlob, toEmail, accountNo, msg_fnc_nm, smsContent);
                                            break;
                                        case "MBL"://Meghna Bank
                                            await _emailRepo.SendEmailAsync(emailBlob, toEmail, accountNo, msg_fnc_nm, smsContent);
                                            break;
                                        case "FSIBL"://First Security
                                            await _emailRepo.SendEmailAsync(emailBlob, toEmail, accountNo, msg_fnc_nm, smsContent);
                                            break;
                                        case "SEBL"://South East
                                            await _emailRepo.SendEmailAsync(emailBlob, toEmail, accountNo, msg_fnc_nm, smsContent);
                                            break;
                                        case "DBL"://Dhaka Bank
                                            await _emailRepo.SendEmailAsync(emailBlob, toEmail, accountNo, msg_fnc_nm, smsContent);
                                            break;
                                        default:
                                            break;
                                    }
                                    #endregion
                                }

                                if (smsFlag == 1)
                                {
                                    #region Sms
                                    string toNumber = Convert.ToString(fields["MOBILE_NUMBER"]);
                                    string smsOperator = Convert.ToString(fields["MOBILE_OPERATOR"]);

                                    switch (_bankName.ToUpper())
                                    {
                                        case "LBFL"://lanka bangla
                                            await _smsRepo.SendSmsAsync(smsContent, toNumber, smsOperator, accountNo);
                                            break;
                                        case "MMBL"://modhu moti
                                            await _smsRepo.SendSmsAsync(smsContent, toNumber, smsOperator, accountNo);
                                            break;
                                        case "UFL"://united finance
                                            await _smsRepo.UflSmsSendApiAsync(smsContent, toNumber, accountNo);
                                            break;
                                        case "UBL"://uttara bank
                                            await _smsRepo.SendSmsAsync(smsContent, toNumber, smsOperator, accountNo);
                                            break;
                                        case "MBL"://Meghna Bank
                                            await _smsRepo.SendSmsAsync(smsContent, toNumber, smsOperator, accountNo);
                                            break;
                                        case "FSIBL"://First Security
                                            await _smsRepo.SendSmsAsync(smsContent, toNumber, smsOperator, accountNo);
                                            break;
                                        case "SEBL"://South East
                                            await _smsRepo.SendSmsAsync(smsContent, toNumber, smsOperator, accountNo);
                                            break;
                                        case "DBL"://Dhaka Bank
                                            await _smsRepo.SendSmsAsync(smsContent, toNumber, smsOperator, accountNo);
                                            break;
                                        default:
                                            break;
                                    }
                                    #endregion
                                }
                            }
                        });
                    }

                    //const string query = "NOTIFICATION_ALERT_SERVICE.set_last_execution";
                    const string query = "ALERT_SERVICE.set_last_execution";
                    OracleDynamicParameters parameters = new OracleDynamicParameters();
                    parameters.Add("pprocess_name", process, OracleDbType.NVarchar2, ParameterDirection.Input);
                    await con.QueryAsync(query, parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    _errorLog.LogError(e.Message).Wait();
                }

                con.Close();
                con.Dispose();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                _errorLog.LogError(e.Message).Wait();
            }
        }
    }
}
