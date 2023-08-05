using Dapper;
using HtmlAgilityPack;
//using LeadSoft.VASConn;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GdicNasWorker.Helpers;
using Newtonsoft.Json;
//using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GdicNasWorker.Entities;

namespace GdicNasWorker
{
    public class Worker : BackgroundService
    {
        #region Fields
        const string fncRecGetQuery = "ALERT_SERVICE.ALERT_SVC_msg_fnc_ga";
        const string setExeQuery = "ALERT_SERVICE.set_last_execution";
        const string nasRecGetQuery = "ALERT_SERVICE.alert_svc_rec_ga";
        const string recUpdateQuery = "ALERT_SERVICE.alert_svc_rec_u";
        //const string fncRecGetQuery = "ALERT_SERVICE.alert_test_ga";
        private static Timer applicationTimer = null;
        private static Timer apiTimer = null;
        private readonly int _executePeriod;
        private readonly int _alertTryPeriod;
        private readonly Connection _connection;
        private readonly ILogger<Worker> _logger;
        private static readonly ILog _errorLog = LogManager.GetLogger("ErrorLog");
        private static readonly ILog _logLicense = LogManager.GetLogger("LogLicense");
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _nasLic;
        private readonly string _appMood;
        private readonly string _toEmail;
        private readonly string _toNumber;
        private readonly string _bankName;
        private readonly string _numberPrefix;
        private readonly string _isApplicationStart;
        private readonly string _isSmsStart;
        private readonly string _isEmailStart;
        private readonly string _encryptEnable;
        private readonly string _encryptUserId;
        private readonly string _encryptPass;
        private readonly string _encryptConId;
        private readonly string _encryptUri;
        private readonly string _connectionString;
        private readonly string _sslEnable;
        private readonly string _sslSid;
        private readonly string _sslUser;
        private readonly string _sslPass;
        private readonly string _sslUri;
        private readonly string _infobipEnable;
        private readonly string _infobipFrom;
        private readonly string _infobipUserName;
        private readonly string _infobipPass;
        private readonly string _infobipUri;
        private readonly string _metrotelEnable;
        private readonly string _metrotelApiKey;
        private readonly string _metrotelType;
        private readonly string _metrotelSenderId;
        private readonly string _metrotelUri;
        private readonly string _uflEnable;
        private readonly string _uflApiKey;
        private readonly string _uflUri;
        private readonly string _robiEnable;
        private readonly string _robiUserName;
        private readonly string _robiPass;
        private readonly string _robiFrom;
        private readonly string _robiMasking;
        private readonly string _robiUri;
        private readonly string _blEnable;
        private readonly string _blUserName;
        private readonly string _blPass;
        private readonly string _blFrom;
        private readonly string _blMasking;
        private readonly string _blUri;
        private readonly string _gpEnable;
        private readonly string _gpUserName;
        private readonly string _gpPass;
        private readonly string _gpFrom;
        private readonly string _gpMasking;
        private readonly string _gpUri;
        private readonly string _pushEnable;
        private readonly string _pushEvent;
        private readonly string _pushTitle;
        private readonly string _pushUserName;
        private readonly string _pushFromIp;
        private readonly string _pushUri;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpEmail;
        private readonly string _smtpSubject;
        private readonly string _smtpMasking;
        private readonly string _smtpPass;
        private readonly bool _smtpUseDefaultCredentials;
        private readonly bool _smtpEnableSsl;
        private readonly string _sslNewEnable;
        private readonly string _sslNewApiToken;
        private readonly string _sslNewSid;
        private readonly string _sslNewUri;

        #endregion

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _connection = new Connection();
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DbConnString");
            _clientFactory = clientFactory;
            _nasLic = _configuration.GetValue<string>("NasLic");
            _appMood = _configuration.GetValue<string>("AppExecution:Mood").ToUpper();
            _toNumber = _configuration.GetValue<string>("AppExecution:Mobile");
            _toEmail = _configuration.GetValue<string>("AppExecution:Email");
            _bankName = _configuration.GetValue<string>("BankName").ToUpper();
            _executePeriod = _configuration.GetValue<int>("ExecutePeriod");
            _alertTryPeriod = _configuration.GetValue<int>("AlertTryPeriod");
            _numberPrefix = _configuration.GetValue<string>("NumberPrefix");
            _isApplicationStart = _configuration.GetValue<string>("IsApplicationStart");
            _isSmsStart = _configuration.GetValue<string>("IsSmsStart");
            _isEmailStart = _configuration.GetValue<string>("IsEmailStart");
            _encryptEnable = _configuration.GetValue<string>("Encryption:Enable");
            _encryptUserId = _configuration.GetValue<string>("Encryption:UserId");
            _encryptPass = _configuration.GetValue<string>("Encryption:Password");
            _encryptConId = _configuration.GetValue<string>("Encryption:ConnectionId");
            _encryptUri = _configuration.GetValue<string>("Encryption:Uri");
            _sslEnable = _configuration.GetValue<string>("SslSms:Enable");
            _sslSid = _configuration.GetValue<string>("SslSms:Sid");
            _sslUser = _configuration.GetValue<string>("SslSms:User");
            _sslPass = _configuration.GetValue<string>("SslSms:Pass");
            _sslUri = _configuration.GetValue<string>("SslSms:Uri");
            _infobipEnable = _configuration.GetValue<string>("InfobipSms:Enable");
            _infobipFrom = _configuration.GetValue<string>("InfobipSms:From");
            _infobipUserName = _configuration.GetValue<string>("InfobipSms:UserName");
            _infobipPass = _configuration.GetValue<string>("InfobipSms:Password");
            _infobipUri = _configuration.GetValue<string>("InfobipSms:Uri");
            _metrotelEnable = _configuration.GetValue<string>("MetrotelSms:Enable");
            _metrotelApiKey = _configuration.GetValue<string>("MetrotelSms:ApiKey");
            _metrotelType = _configuration.GetValue<string>("MetrotelSms:Type");
            _metrotelSenderId = _configuration.GetValue<string>("MetrotelSms:SenderId");
            _metrotelUri = _configuration.GetValue<string>("MetrotelSms:Uri");
            _uflEnable = _configuration.GetValue<string>("UflSms:Enable");
            _uflApiKey = _configuration.GetValue<string>("UflSms:ApiKey");
            _uflUri = _configuration.GetValue<string>("UflSms:Uri");
            _robiEnable = _configuration.GetValue<string>("Robi:Enable");
            _robiUserName = _configuration.GetValue<string>("Robi:UserName");
            _robiPass = _configuration.GetValue<string>("Robi:Password");
            _robiFrom = _configuration.GetValue<string>("Robi:From");
            _robiMasking = _configuration.GetValue<string>("Robi:Masking");
            _robiUri = _configuration.GetValue<string>("Robi:Uri");
            _blEnable = _configuration.GetValue<string>("Banglalink:Enable");
            _blUserName = _configuration.GetValue<string>("Banglalink:UserName");
            _blPass = _configuration.GetValue<string>("Banglalink:Password");
            _blFrom = _configuration.GetValue<string>("Banglalink:From");
            _blMasking = _configuration.GetValue<string>("Banglalink:Masking");
            _blUri = _configuration.GetValue<string>("Banglalink:Uri");
            _gpEnable = _configuration.GetValue<string>("GrameenPhone:Enable");
            _gpUserName = _configuration.GetValue<string>("GrameenPhone:UserName");
            _gpPass = _configuration.GetValue<string>("GrameenPhone:Password");
            _gpFrom = _configuration.GetValue<string>("GrameenPhone:From");
            _gpMasking = _configuration.GetValue<string>("GrameenPhone:Masking");
            _gpUri = _configuration.GetValue<string>("GrameenPhone:Uri");
            _pushEnable = _configuration.GetValue<string>("Push:Enable");
            _pushEvent = _configuration.GetValue<string>("Push:Event");
            _pushTitle = _configuration.GetValue<string>("Push:Title");
            _pushUserName = _configuration.GetValue<string>("Push:UserName");
            _pushFromIp = _configuration.GetValue<string>("Push:FromIp");
            _pushUri = _configuration.GetValue<string>("Push:Uri");
            _smtpServer = _configuration.GetValue<string>("SmtpCredentials:Server");
            _smtpPort = _configuration.GetValue<int>("SmtpCredentials:Port");
            _smtpEmail = _configuration.GetValue<string>("SmtpCredentials:Email");
            _smtpSubject = _configuration.GetValue<string>("SmtpCredentials:Subject");
            _smtpMasking = _configuration.GetValue<string>("SmtpCredentials:Masking");
            _smtpPass = _configuration.GetValue<string>("SmtpCredentials:Password");
            _smtpUseDefaultCredentials = _configuration.GetValue<bool>("SmtpCredentials:UseDefaultCredentials");
            _smtpEnableSsl = _configuration.GetValue<bool>("SmtpCredentials:EnableSsl");
            _sslNewEnable = _configuration.GetValue<string>("SslSmsNew:Enable");
            _sslNewApiToken = _configuration.GetValue<string>("SslSmsNew:ApiToken");
            _sslNewSid = _configuration.GetValue<string>("SslSmsNew:Sid");
            _sslNewUri = _configuration.GetValue<string>("SslSmsNew:Uri");

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                applicationTimer = new Timer(new TimerCallback(TickTimer1), null, 0, _executePeriod);

                //apiTimer = new Timer(new TimerCallback(TickTimer2), null, 0, _alertTryPeriod);

                await Task.Delay(5);
            }
            catch (Exception e)
            {
                LogError(e.Message);
            }
        }

        private void TickTimer1(object id)
        {
            try
            {
                if (_isApplicationStart.Equals("1"))
                {
                    ExecuteApplication();
                }
            }
            catch (Exception e)
            {
                LogError(e.Message);
            }
        }

        void TickTimer2(object id)
        {
            try
            {
                if (_isSmsStart.Equals("1"))
                {
                    SendAlert();
                }
            }
            catch (Exception e)
            {
                LogError(e.Message);
            }
        }

        private void ExecuteApplication()
        {
            try
            {
                using IDbConnection con = _connection.GetConnection(_connectionString);

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                //string queryString = "select * from tbl_cmn_customers t where t.alert_flag = 0;";
                string queryString = "sp_get_customer_by_alert";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("alert_flag", 1);
                parameters.Add("alert_sent_flag", 0);


                IEnumerable<dynamic> result = con.QueryAsync(queryString, parameters, commandType: CommandType.StoredProcedure).Result;
                result = result?.Cast<IDictionary<string, object>>();

                if (result != null)
                {
                    //var options = new ParallelOptions() { MaxDegreeOfParallelism = 4 };

                    //Parallel.ForEach(result, options, rows =>
                    foreach (var rows in result)
                    {
                        if (rows is IDictionary<string, object> fields)
                        {
                            #region Sms
                            ExecuteSms(fields);

                            //SendSmsVendor(smsContent, toNumber, smsOperator, out smsRespFlag, out smsReasonForFail);

                            #endregion

                            #region Email

                            SendEmail();
                            ExecuteEmail(fields);
                            //SendEmail(emailBody, toEmail, emailSubject, out emailRespFlag, out emailReasonForFail);

                            #endregion

                            #region Update
                            //if (smsRespFlag.Equals(1) || emailRespFlag.Equals(1))
                            //{
                            //    OracleDynamicParameters parameters = new OracleDynamicParameters();
                            //    parameters.Add("pprovider_message_id", smsReasonForFail, OracleDbType.NVarchar2, ParameterDirection.Input);
                            //    parameters.Add("preason_for_fail", smsReasonForFail, OracleDbType.NVarchar2, ParameterDirection.Input);


                            //    //parameters.Add("psms_reason_for_fail", smsReasonForFail, OracleDbType.NVarchar2, ParameterDirection.Input);
                            //    //parameters.Add("pemail_reason_for_fail", emailReasonForFail, OracleDbType.NVarchar2, ParameterDirection.Input);
                            //    //parameters.Add("psms_resp_flag", smsRespFlag, OracleDbType.Int32, ParameterDirection.Input);
                            //    //parameters.Add("pemail_resp_flag", emailRespFlag, OracleDbType.Int32, ParameterDirection.Input);

                            //    parameters.Add("pnas_id", nasId, OracleDbType.Int32, ParameterDirection.Input);
                            //    parameters.Add("palert_flag", 1, OracleDbType.Int32, ParameterDirection.Input);

                            //    con.QueryAsync(recUpdateQuery, parameters, commandType: CommandType.StoredProcedure).Wait(5);
                            //}
                            #endregion
                        }
                    }
                }

                con.Close();
                con.Dispose();


                //var res = con.Query<Customer>("select * from tbl_cmn_customers t where t.alert_flag = 0;").ToList();







            }
            catch (Exception e)
            {
                //LogError("Connection Exeption");
                LogError(e.Message);
            }
        }

        private void ExecuteSms(IDictionary<string, object> fields)
        {
            try
            {
                if (_isSmsStart.Equals("1"))
                {
                    SendSms(fields);
                }
            }
            catch (Exception e)
            {
                LogError(e.Message);
            }
        }

        private void ExecuteEmail(IDictionary<string, object> fields)
        {
            try
            {
                if (_isEmailStart.Equals("1"))
                {
                    //SendEmail(fields);
                }
            }
            catch (Exception e)
            {
                LogError(e.Message);
            }
        }

        private void SendSms(IDictionary<string, object> fields)
        {
            int id = Convert.ToInt32(fields["id"]);
            int smsFlag = Convert.ToInt32(fields["sms_flag"]);
            //string smsOperator = Convert.ToString(fields["MOBILE_OPERATOR"]);
            string toNumber = Convert.ToString(fields["phone"]);
            string name = $"{Convert.ToString(fields["first_name"])} {Convert.ToString(fields["last_name"])}";

            if (_appMood.Equals("UAT"))
            {
                toNumber = _toNumber;
            }

            //string smsContent = Convert.ToString(fields["SMS_TEXT"]);
            //string toEmail = Convert.ToString(fields["EMAIL_ID"]);
            //string emailBody = Convert.ToString(fields["EMAIL_BODY"]);
            //int nasId = Convert.ToInt32(fields["NAS_ID"]);
            //string nasFncId = Convert.ToString(fields["NAS_FNC_ID"]).Trim();
            //string customerId = Convert.ToString(fields["CUSTOMER_ID"]).Trim();
            //int pushFlag = Convert.ToInt32(fields["PUSH_FLAG"]);

            //string emailSubject = Convert.ToString(fields["EMAIL_SUBJECT"]);
            string emailSubject = "";

            int emailRespFlag = 0;
            int smsRespFlag = 0;
            string smsReasonForFail = "";
            string emailReasonForFail = "";

            #region Phone Number Check

            //toNumber = GetToNumber(toNumber);

            if (string.IsNullOrWhiteSpace(toNumber))
            {
                smsFlag = 0;
                //const string query = "ALERT_SERVICE.alert_svc_rec_u";
                //OracleDynamicParameters parameters = new OracleDynamicParameters();
                //parameters.Add("pprovider_message_id", "", OracleDbType.NVarchar2, ParameterDirection.Input);
                //parameters.Add("preason_for_fail", "Invalid Phone Number", OracleDbType.NVarchar2, ParameterDirection.Input);
                //parameters.Add("pnas_id", nasId, OracleDbType.Int32, ParameterDirection.Input);
                //parameters.Add("palert_flag", 2, OracleDbType.Int32, ParameterDirection.Input);
                //con.QueryAsync(query, parameters, commandType: CommandType.StoredProcedure).Wait(5);

                smsRespFlag = 1;
                smsReasonForFail = "Invalid Phone Number";
            }
            #endregion

            string msg = $"Hi {name}, Today your account is created.";

            LogLicenseExpired(msg);

            //con.QueryAsync(recUpdateQuery, parameters, commandType: CommandType.StoredProcedure).Wait(5);




        }

        //private void SendEmail(IDictionary<string, object> fields)
        private void SendEmail()
        {
            //int emailFlag = Convert.ToInt32(fields["EMAIL_SENT_FLAG"]);

            // Gmail SMTP server details
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 465; // 465, 587
            string smtpUsername = "choiceautoinsurance23@gmail.com";
            string smtpPassword = "Choice@23";

            // Sender and recipient email addresses
            string fromEmail = "choiceautoinsurance23@gmail.com";
            string toEmail = "jessypdnathan@gmail.com";

            // Create a new MailMessage
            MailMessage message = new MailMessage(fromEmail, toEmail);
            message.Subject = "Hello from C#";
            message.Body = "This is the body of the email.";

            // Configure the SMTP client
            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

            try
            {
                // Send the email
                smtpClient.Send(message);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send email: " + ex.Message);
            }

            LogLicenseExpired("Email Execute");
        }

        #region Alert
        private void SendAlert()
        {
            try
            {
                //if (DateTime.Now.Ticks > new DateTime(2030, 12, 30).Ticks)
                //{
                //    LogLicenseExpired("License Expired from ExecuteNasEvent on 2030, 12, 30");
                //    return;
                //}

                //using IDbConnection con = _connection.GetConnection(_connectionString);
                //con.Open();

                //OracleDynamicParameters alertParameters = new OracleDynamicParameters();
                //alertParameters.Add("presult_cursor", null, OracleDbType.RefCursor, ParameterDirection.Output);

                //IEnumerable<dynamic> result = con.QueryAsync(nasRecGetQuery, alertParameters, commandType: CommandType.StoredProcedure).Result;
                //result = result?.Cast<IDictionary<string, object>>();

                //if (result != null)
                //{
                //    var options = new ParallelOptions() { MaxDegreeOfParallelism = 4 };

                //    Parallel.ForEach(result, options, rows =>
                //    {
                //        if (rows is IDictionary<string, object> fields)
                //        {
                //            int smsFlag = Convert.ToInt32(fields["SMS_SENT_FLAG"]);
                //            int emailFlag = Convert.ToInt32(fields["EMAIL_SENT_FLAG"]);
                //            string smsOperator = Convert.ToString(fields["MOBILE_OPERATOR"]);
                //            string toNumber = Convert.ToString(fields["MOBILE_NUMBER"]);
                //            string smsContent = Convert.ToString(fields["SMS_TEXT"]);
                //            string toEmail = Convert.ToString(fields["EMAIL_ID"]);
                //            string emailBody = Convert.ToString(fields["EMAIL_BODY"]);
                //            int nasId = Convert.ToInt32(fields["NAS_ID"]);
                //            string nasFncId = Convert.ToString(fields["NAS_FNC_ID"]).Trim();
                //            string customerId = Convert.ToString(fields["CUSTOMER_ID"]).Trim();
                //            int pushFlag = Convert.ToInt32(fields["PUSH_FLAG"]);

                //            //string emailSubject = Convert.ToString(fields["EMAIL_SUBJECT"]);
                //            string emailSubject = "";

                //            int emailRespFlag = 0;
                //            int smsRespFlag = 0;
                //            string smsReasonForFail = "";
                //            string emailReasonForFail = "";

                //            #region Phone Number Check

                //            toNumber = GetToNumber(toNumber);

                //            if (string.IsNullOrWhiteSpace(toNumber))
                //            {
                //                smsFlag = 0;
                //                //const string query = "ALERT_SERVICE.alert_svc_rec_u";
                //                //OracleDynamicParameters parameters = new OracleDynamicParameters();
                //                //parameters.Add("pprovider_message_id", "", OracleDbType.NVarchar2, ParameterDirection.Input);
                //                //parameters.Add("preason_for_fail", "Invalid Phone Number", OracleDbType.NVarchar2, ParameterDirection.Input);
                //                //parameters.Add("pnas_id", nasId, OracleDbType.Int32, ParameterDirection.Input);
                //                //parameters.Add("palert_flag", 2, OracleDbType.Int32, ParameterDirection.Input);
                //                //con.QueryAsync(query, parameters, commandType: CommandType.StoredProcedure).Wait(5);

                //                smsRespFlag = 1;
                //                smsReasonForFail = "Invalid Phone Number";
                //            }
                //            #endregion

                //            #region Sms
                //            if (smsFlag.Equals(1))
                //            {
                //                SendSmsVendor(smsContent, toNumber, smsOperator, out smsRespFlag, out smsReasonForFail);
                //            }
                //            #endregion

                //            #region Email
                //            if (emailFlag.Equals(1))
                //            {
                //                SendEmail(emailBody, toEmail, emailSubject, out emailRespFlag, out emailReasonForFail);
                //            }
                //            #endregion

                //            #region Update
                //            if (smsRespFlag.Equals(1) || emailRespFlag.Equals(1))
                //            {
                //                OracleDynamicParameters parameters = new OracleDynamicParameters();
                //                parameters.Add("pprovider_message_id", smsReasonForFail, OracleDbType.NVarchar2, ParameterDirection.Input);
                //                parameters.Add("preason_for_fail", smsReasonForFail, OracleDbType.NVarchar2, ParameterDirection.Input);


                //                //parameters.Add("psms_reason_for_fail", smsReasonForFail, OracleDbType.NVarchar2, ParameterDirection.Input);
                //                //parameters.Add("pemail_reason_for_fail", emailReasonForFail, OracleDbType.NVarchar2, ParameterDirection.Input);
                //                //parameters.Add("psms_resp_flag", smsRespFlag, OracleDbType.Int32, ParameterDirection.Input);
                //                //parameters.Add("pemail_resp_flag", emailRespFlag, OracleDbType.Int32, ParameterDirection.Input);

                //                parameters.Add("pnas_id", nasId, OracleDbType.Int32, ParameterDirection.Input);
                //                parameters.Add("palert_flag", 1, OracleDbType.Int32, ParameterDirection.Input);

                //                con.QueryAsync(recUpdateQuery, parameters, commandType: CommandType.StoredProcedure).Wait(5);
                //            }
                //            #endregion

                //            #region Push Notification
                //            if (_pushEnable.Equals("1"))
                //            {
                //                if (pushFlag.Equals(0))
                //                {
                //                    if (_pushEvent.Split(",").ToList().Any(x => x.Trim().Equals(nasFncId)))
                //                    {
                //                        PushApiAsync(smsContent, customerId).Wait(2);
                //                    }
                //                }
                //            }
                //            #endregion
                //        }
                //    });
                //}

                //con.Close();
                //con.Dispose();
            }
            catch (Exception e)
            {
                LogError("Exeption From SendAlert Section.");
                LogError(e.Message);
            }
        }

        //private async Task<string> GetUltimusCon()
        //{
        //    string response = string.Empty;

        //    try
        //    {
        //        if (_encryptEnable.Equals("1"))
        //        {
        //            User model = new User
        //            {
        //                UserId = _encryptUserId,
        //                Password = _encryptPass,
        //                ConnectionId = _encryptConId
        //            };
        //            string jsonModel = JsonConvert.SerializeObject(model);

        //            ConnectionRequest req = new ConnectionRequest()
        //            {
        //                BranchId = "",
        //                FunctionId = "",
        //                InstitueId = "",
        //                RequestAppBaseUrl = "",
        //                RequestAppIP = "",
        //                RequestCliedAgent = "",
        //                RequestCliedIP = "",
        //                RequestDateTime = "",
        //                RequestId = "",
        //                SessionId = "",
        //                UserId = "",
        //                BusinessData = jsonModel,
        //                SessionTimeout = 30
        //            };

        //            string jsonObject = JsonConvert.SerializeObject(req);
        //            StringContent content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
        //            HttpClient client = _clientFactory.CreateClient("encrypt");
        //            HttpResponseMessage result = await client.PostAsync(_encryptUri, content);

        //            if (result.IsSuccessStatusCode)
        //            {
        //                string readString = await result.Content.ReadAsStringAsync();
        //                ConnectionResponse res = JsonConvert.DeserializeObject<ConnectionResponse>(readString);
        //                string businessData = res.ResponseBusinessData;
        //                string encryptData = VASCrypto.Decrypt(businessData);
        //                DbConString dbConString = JsonConvert.DeserializeObject<DbConString>(encryptData);
        //                response = $"Data Source={dbConString.CONN_SCHEMA_NM};User id={dbConString.CONN_USER_ID};Password={dbConString.CONN_PASS_WORD}";
        //            }
        //            else
        //            {
        //                LogError("Connection string not found");
        //            }
        //        }
        //        else
        //        {
        //            response = _configuration.GetConnectionString("DbConnString");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogError("Connection string GetUltimusConString Api Calling Exception");
        //        LogError(e.Message);
        //    }

        //    return response;
        //}

        //private async Task<Tuple<int, string>> SslApiAsync(string smsContent, string toNumber)
        //{
        //    try
        //    {
        //        if (_appMood.Equals("UAT"))
        //        {
        //            toNumber = _toNumber;
        //        }

        //        if (!string.IsNullOrWhiteSpace(toNumber))
        //        {
        //            string queryString = $"?user={_sslUser}&pass={_sslPass}&msisdn={toNumber}&sms={smsContent}&csmsid={DateTime.Now.ToString("yyyyMMddHHmmssfff")}&sid={_sslSid}";
        //            HttpClient client = _clientFactory.CreateClient("ssl");
        //            HttpResponseMessage response = await client.PostAsync(_sslUri + queryString, null);
        //            string result = await response.Content.ReadAsStringAsync();
        //            SslSmsReply objReply;
        //            XmlSerializer serializer = new XmlSerializer(typeof(SslSmsReply));
        //            using TextReader reader = new StringReader(result);
        //            objReply = (SslSmsReply)serializer.Deserialize(reader);

        //            if (objReply.SMSINFO.Count.Equals(0))
        //            {
        //                LogError($"Parameter: {objReply.PARAMETER}, Login: {objReply.LOGIN}, PushApi: {objReply.PUSHAPI}, Stake Holder Id: {objReply.STAKEHOLDERID}, Permited: {objReply.PERMITTED}");
        //                return new Tuple<int, string>(0, $"Parameter: {objReply.PARAMETER}, Login: {objReply.LOGIN}, PushApi: {objReply.PUSHAPI}, Stake Holder Id: {objReply.STAKEHOLDERID}, Permited: {objReply.PERMITTED}");
        //            }

        //            return new Tuple<int, string>(1, "");
        //        }
        //        else
        //        {
        //            return new Tuple<int, string>(0, "Invalid Phone Number.");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogError("Exeption From SslApiHttpClientAsync Section.");
        //        LogError(e.Message);
        //        return new Tuple<int, string>(0, e.Message);
        //    }
        //}

        //private async Task<Tuple<int, string>> InfobipApiAsync(string smsContent, string toNumber)
        //{
        //    try
        //    {
        //        if (_appMood.Equals("UAT"))
        //        {
        //            toNumber = _toNumber;
        //        }

        //        if (!string.IsNullOrWhiteSpace(toNumber))
        //        {
        //            List<Destination> destinations = new List<Destination>()
        //            {
        //                new Destination { To = toNumber}
        //            };

        //            List<SmsMessage> smsMessages = new List<SmsMessage>()
        //            {
        //                    new SmsMessage {From = _infobipFrom, Text = smsContent, Destinations = destinations}
        //            };

        //            InfobipSmsRequest smsRequest = new InfobipSmsRequest()
        //            {
        //                Messages = smsMessages
        //            };

        //            string jsonString = JsonConvert.SerializeObject(smsRequest);
        //            HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
        //            var byteArray = Encoding.ASCII.GetBytes($"{_infobipUserName}:{_infobipPass}");
        //            HttpClient client = _clientFactory.CreateClient("infobip");
        //            client.DefaultRequestHeaders.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        //            HttpResponseMessage result = await client.PostAsync(_infobipUri, httpContent);
        //            string response = await result.Content.ReadAsStringAsync();

        //            if (result.IsSuccessStatusCode)
        //            {
        //                InfobipSuccessResponse successResponse = JsonConvert.DeserializeObject<InfobipSuccessResponse>(response);
        //                return new Tuple<int, string>(1, successResponse.Messages[0].MessageId);
        //            }
        //            else
        //            {
        //                InfobipErrorResponse infobipError = JsonConvert.DeserializeObject<InfobipErrorResponse>(response);
        //                string errorMsg = $"Failed, MessageId : {infobipError.RequestError.ServiceException.MessageId}, Text : {infobipError.RequestError.ServiceException.Text}";
        //                return new Tuple<int, string>(0, errorMsg);
        //            }
        //        }

        //        return new Tuple<int, string>(0, "Invalid Phone Number.");
        //    }
        //    catch (Exception e)
        //    {
        //        LogError("Exeption From InfobipApiHttpClientAsync Section.");
        //        LogError(e.Message);
        //        return new Tuple<int, string>(0, e.Message);
        //    }
        //}

        private async Task<Tuple<int, string>> MetrotelApiAsync(string smsContent, string toNumber)
        {
            try
            {
                if (_appMood.Equals("UAT"))
                {
                    toNumber = _toNumber;
                }

                if (!string.IsNullOrWhiteSpace(toNumber))
                {
                    string queryString = $"?api_key={_metrotelApiKey}&type={_metrotelType}&contacts={toNumber}&senderid={_metrotelSenderId}&msg={smsContent}";
                    HttpClient client = _clientFactory.CreateClient("metrotel");
                    HttpResponseMessage response = await client.PostAsync(_metrotelUri + queryString, null);
                    string result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return new Tuple<int, string>(1, "");
                    }
                    else
                    {
                        return new Tuple<int, string>(0, $"{result}");
                    }
                }

                return new Tuple<int, string>(0, "Invalid Phone Number.");
            }
            catch (Exception e)
            {
                LogError("Exeption From MetrotelApiHttpClientAsync Section.");
                LogError(e.Message);
                return new Tuple<int, string>(0, e.Message);
            }
        }

        private async Task<Tuple<int, string>> RobiApiAsync(string smsContent, string toNumber)
        {
            try
            {
                if (_appMood.Equals("UAT"))
                {
                    toNumber = _toNumber;
                }

                if (!string.IsNullOrWhiteSpace(toNumber))
                {
                    string queryString = "?Username=" + _robiUserName + "&Password=" + _robiPass + "&From=" + _robiMasking + "&To=" + toNumber + "&Message=" + smsContent;

                    HttpClient client = _clientFactory.CreateClient("robi");
                    HttpResponseMessage response = await client.GetAsync(_robiUri + queryString);
                    string result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return new Tuple<int, string>(1, "");
                    }
                    else
                    {
                        LogError("failed.");
                        return new Tuple<int, string>(0, $"{result}");
                    }
                }

                return new Tuple<int, string>(0, "Invalid Phone Number.");
            }
            catch (Exception e)
            {
                LogError("Exeption From RobiApi Section.");
                LogError(e.Message);
                return new Tuple<int, string>(0, e.Message);
            }
        }

        private async Task<Tuple<int, string>> BanglalinkApiAsync(string smsContent, string toNumber)
        {
            try
            {
                if (_appMood.Equals("UAT"))
                {
                    toNumber = _toNumber;
                }

                if (!string.IsNullOrWhiteSpace(toNumber))
                {
                    string queryString = "?msisdn=" + toNumber + "&message=" + smsContent + "&userID=" + _blUserName + "&passwd=" + _blPass + "&sender=" + _blMasking;
                    HttpClient client = _clientFactory.CreateClient("banglalink");
                    HttpResponseMessage response = await client.GetAsync(_blUri + queryString);
                    string result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return new Tuple<int, string>(1, "");
                    }
                    else
                    {
                        return new Tuple<int, string>(0, $"{result}");
                    }
                }

                return new Tuple<int, string>(0, "Invalid Phone Number.");
            }
            catch (Exception e)
            {
                LogError("Exeption From BanglalinkApi Section.");
                LogError(e.Message);
                return new Tuple<int, string>(0, e.Message);
            }
        }

        private async Task<Tuple<int, string>> GrameenPhoneApiAsync(string smsContent, string toNumber)
        {
            try
            {
                if (_appMood.Equals("UAT"))
                {
                    toNumber = _toNumber;
                }

                if (!string.IsNullOrWhiteSpace(toNumber))
                {
                    string queryString = "?username=" + _gpUserName + "&password=" + _gpPass + "&apicode=1&msisdn=" + toNumber + "&countrycode=880&cli= " + _gpMasking + "&messagetype=1&message=" + smsContent + "&messageid=0";
                    HttpClient client = _clientFactory.CreateClient("grameenphone");
                    HttpResponseMessage response = await client.GetAsync(_gpUri + queryString);
                    string result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return new Tuple<int, string>(1, "");
                    }
                    else
                    {
                        return new Tuple<int, string>(0, $"{result}");
                    }
                }

                return new Tuple<int, string>(0, "Invalid Phone Number.");
            }
            catch (Exception e)
            {
                LogError("Exeption From GrameenPhoneApi Section.");
                LogError(e.Message);
                return new Tuple<int, string>(0, e.Message);
            }
        }

        private async Task<Tuple<int, string>> UflApiAsync(string smsContent, string toNumber)
        {
            try
            {
                if (_appMood.Equals("UAT"))
                {
                    toNumber = _toNumber;
                }

                if (!string.IsNullOrWhiteSpace(toNumber))
                {
                    string queryString = "?MobileNo=" + toNumber + "&SmsBody=" + smsContent + "&RequestId=" + toNumber;
                    HttpClient client = _clientFactory.CreateClient("ufl");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("APIKey", _uflApiKey);
                    HttpResponseMessage response = await client.PostAsync(_uflUri + queryString, null);
                    string result = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return new Tuple<int, string>(1, "");
                    }
                    else
                    {
                        return new Tuple<int, string>(0, $"{result}");
                    }
                }

                return new Tuple<int, string>(0, "Network busy, please try again");
            }
            catch (Exception e)
            {
                LogError("Exeption From UflSmsSendApiAsync Section.");
                LogError(e.Message);
                return new Tuple<int, string>(0, e.Message);
            }
        }

        //private async Task<Tuple<int, string>> SslApiVersionThreeAsync(string smsContent, string toNumber)
        //{
        //    try
        //    {
        //        if (_appMood.Equals("UAT"))
        //        {
        //            toNumber = _toNumber;
        //        }

        //        if (!string.IsNullOrWhiteSpace(toNumber))
        //        {
        //            SslRequest req = new SslRequest()
        //            {
        //                ApiToken = _sslNewApiToken,
        //                Sid = _sslNewSid,
        //                Msisdn = toNumber,
        //                Sms = smsContent,
        //                CsmsId = DateTime.Now.ToString("yyyyMMddHHmmssfff")
        //            };

        //            string jsonString = JsonConvert.SerializeObject(req);
        //            HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
        //            HttpClient client = _clientFactory.CreateClient();
        //            HttpResponseMessage response = await client.PostAsync(_sslNewUri, httpContent);
        //            string result = await response.Content.ReadAsStringAsync();
        //            SslResponse successRes = JsonConvert.DeserializeObject<SslResponse>(result);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                if (successRes.Smsinfo.Count > 0)
        //                {
        //                    Smsinfo res = new Smsinfo();

        //                    foreach (var i in successRes.Smsinfo)
        //                    {
        //                        res.SmsStatus = i.SmsStatus;
        //                        res.StatusMessage = i.StatusMessage;
        //                        res.Msisdn = i.Msisdn;
        //                        res.SmsType = i.SmsType;
        //                        res.SmsBody = i.SmsBody;
        //                        res.CsmsId = i.CsmsId;
        //                        res.ReferenceId = i.ReferenceId;
        //                    }

        //                    return new Tuple<int, string>(1, res.ReferenceId);
        //                }
        //                else
        //                {
        //                    return new Tuple<int, string>(0, $"Status: {successRes.Status}, StatusCode: {successRes.StatusCode}, ErrorMessage: {successRes.ErrorMessage}");
        //                }
        //            }
        //            else
        //            {
        //                return new Tuple<int, string>(0, $"Status: {successRes.Status}, StatusCode: {successRes.StatusCode}, ErrorMessage: {successRes.ErrorMessage}");
        //            }
        //        }
        //        else
        //        {
        //            return new Tuple<int, string>(0, "Invalid Phone Number.");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogError("Exeption From SslApiVersionThreeAsync Section.");
        //        LogError(e.Message);
        //        return new Tuple<int, string>(0, e.Message);
        //    }
        //}

        //public void SendSmsVendor(string smsContent, string toNumber, string smsOperator, out int sendRes, out string reasonForFail)
        //{
        //    sendRes = 0;
        //    reasonForFail = "";

        //    try
        //    {
        //        if (_sslEnable.Equals("1"))
        //        {
        //            var res = SslApiAsync(smsContent, toNumber).Result;
        //            sendRes = res.Item1;
        //            reasonForFail = res.Item2;
        //        }
        //        else if (_infobipEnable.Equals("1"))
        //        {
        //            var res = InfobipApiAsync(smsContent, toNumber).Result;
        //            sendRes = res.Item1;
        //            reasonForFail = res.Item2;
        //        }
        //        else if (_metrotelEnable.Equals("1"))
        //        {
        //            var res = MetrotelApiAsync(smsContent, toNumber).Result;
        //            sendRes = res.Item1;
        //            reasonForFail = res.Item2;
        //        }
        //        else if (_uflEnable.Equals("1"))
        //        {
        //            var res = UflApiAsync(smsContent, toNumber).Result;
        //            sendRes = res.Item1;
        //            reasonForFail = res.Item2;
        //        }
        //        else if (_robiEnable.Equals("1"))
        //        {
        //            var res = RobiApiAsync(smsContent, toNumber).Result;
        //            sendRes = res.Item1;
        //            reasonForFail = res.Item2;
        //        }
        //        else if (_blEnable.Equals("1"))
        //        {
        //            var res = BanglalinkApiAsync(smsContent, toNumber).Result;
        //            sendRes = res.Item1;
        //            reasonForFail = res.Item2;
        //        }
        //        else if (_gpEnable.Equals("1"))
        //        {
        //            var res = GrameenPhoneApiAsync(smsContent, toNumber).Result;
        //            sendRes = res.Item1;
        //            reasonForFail = res.Item2;
        //        }
        //        else if (_sslNewEnable.Equals("1"))
        //        {
        //            var res = SslApiVersionThreeAsync(smsContent, toNumber).Result;
        //            sendRes = res.Item1;
        //            reasonForFail = res.Item2;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogError("Exeption From SendSms Section.");
        //        LogError(e.Message);
        //        sendRes = 0;
        //        reasonForFail = e.Message;
        //    }
        //}

        private void SendEmail(string clob, string toEmail, string emailSubject, out int sendRes, out string emaiReasonForFail)
        {
            sendRes = 1;
            emaiReasonForFail = "";

            try
            {
                if (_appMood.Equals("UAT"))
                {
                    toEmail = _toEmail;
                }

                string line1 = null;
                string clobString = clob.ToString();
                HtmlDocument body = new HtmlDocument();
                body.LoadHtml(clobString);
                HtmlNode bodyNode = body.DocumentNode.SelectSingleNode("//body");
                List<HtmlNode> nodesToRemove = new List<HtmlNode>();

                if (bodyNode != null)
                {
                    nodesToRemove.AddRange(bodyNode.ChildNodes.Where(des => des.Name != "table"));
                }

                foreach (HtmlNode node in nodesToRemove)
                {
                    node.Remove();
                }

                if (bodyNode != null)
                {
                    string s = bodyNode.InnerText;
                    s = s.Trim();
                    line1 = s.Split(new[] { '\r', '\n' }).FirstOrDefault();
                }

                try
                {
                    using MailMessage mailMessage = new MailMessage();
                    using SmtpClient client = new SmtpClient(_smtpServer, _smtpPort);

                    try
                    {
                        client.UseDefaultCredentials = _smtpUseDefaultCredentials;
                        client.EnableSsl = _smtpEnableSsl;
                        client.Credentials = new NetworkCredential(_smtpEmail, _smtpPass);
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        mailMessage.To.Add(toEmail);
                        mailMessage.Subject = emailSubject;
                        mailMessage.Body = clobString;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.From = new MailAddress(_smtpEmail, _smtpMasking);
                        client.Send(mailMessage);
                        sendRes = 1;
                    }
                    catch (Exception e)
                    {
                        sendRes = 0;
                        emaiReasonForFail = e.Message;
                        LogError(e.Message);
                    }
                }
                catch (Exception e)
                {
                    sendRes = 0;
                    emaiReasonForFail = e.Message;
                    LogError("Exeption From SendEmail Section.");
                    LogError(e.Message);
                }
            }
            catch (Exception e)
            {
                sendRes = 0;
                emaiReasonForFail = e.Message;
                LogError("Exeption From SendEmail-1 Section.");
                LogError(e.Message);
            }
        }

        private bool IsValidNumber(string numberPrefix)
        {
            try
            {
                List<string> value = _numberPrefix?.Split(',').ToList();

                foreach (var item in value)
                {
                    if (numberPrefix.Equals(item.Trim()))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                LogError("Exeption From IsValidNumber Section.");
                LogError(e.Message);
                return false;
            }

            return false;
        }

        private string GetToNumber(string toNumber)
        {
            try
            {
                string number = string.Empty;

                if (!string.IsNullOrWhiteSpace(toNumber))
                {
                    toNumber = toNumber.Trim();
                    int countToNumber = toNumber.Length;

                    if (countToNumber.Equals(11))
                    {
                        string numberPrefix = "88" + toNumber.Substring(0, 3);

                        if (IsValidNumber(numberPrefix))
                        {
                            number = "88" + toNumber;
                            return number;
                        }
                    }
                    else if (countToNumber.Equals(13))
                    {
                        string numberPrefix = toNumber.Substring(0, 5);

                        if (IsValidNumber(numberPrefix))
                        {
                            number = toNumber;
                            return number;
                        }
                    }
                    else if (countToNumber > 11)
                    {
                        toNumber = toNumber.Substring(toNumber.Length - 11, 11);
                        string numberPrefix = "88" + toNumber.Substring(0, 3);

                        if (IsValidNumber(numberPrefix))
                        {
                            number = "88" + toNumber;
                            return number;
                        }
                    }

                    return number;
                }

                return number;
            }
            catch (Exception e)
            {
                LogError("Exeption From GetToNumber Section.");
                LogError(e.Message);
                return "";
            }
        }

        private void LogLicenseExpired(string message)
        {
            try
            {
                string directory = $"{_nasLic}\\{DateTime.Now.Year}-{DateTime.Now:MM}-{DateTime.Now.Day}.tsv";

                if (File.Exists(directory))
                {
                    return;
                }
                else
                {
                    var dirname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    XmlConfigurator.Configure(new FileInfo(string.Format("{0}{1}", dirname, @"\log4net.config")));
                    string sLogFormat = $"{DateTime.Now}<======>{message} {Environment.NewLine}";
                    _logLicense.InfoFormat(sLogFormat);
                }
            }
            catch (Exception e)
            {
                LogError("Exeption From LogLicenseExpired Section.");
                LogError(e.Message);
            }
        }

        private void LogError(string errorMessage)
        {
            try
            {
                var dirname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                XmlConfigurator.Configure(new FileInfo(string.Format("{0}{1}", dirname, @"\log4net.config")));
                string sLogFormat = $"{DateTime.Now}<======>{errorMessage} {Environment.NewLine}";
                _errorLog.InfoFormat(sLogFormat);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        //private async Task PushApiAsync(string smsContent, string customerId)
        //{
        //    try
        //    {
        //        PushNotificationRequest ObjRequest = new PushNotificationRequest()
        //        {
        //            Title = _pushTitle,
        //            SmsContent = smsContent,
        //            CustomerId = customerId,
        //            Username = _pushUserName,
        //            FromIp = _pushFromIp
        //        };

        //        string jsonString = JsonConvert.SerializeObject(ObjRequest);
        //        HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
        //        HttpClient client = _clientFactory.CreateClient("push");
        //        HttpResponseMessage result = await client.PostAsync(_pushUri, httpContent);
        //    }
        //    catch (Exception e)
        //    {
        //        LogError("Exeption From CallPushNotificationApiAsync Section.");
        //        LogError(e.Message);
        //    }
        //}
        #endregion
    }
}
