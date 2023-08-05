using Dapper;
using HtmlAgilityPack;
using LeadSoft.VASConn;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Xml;
using System.Xml.Serialization;
using WorkerServiceCustom.Helpers;

namespace WorkerServiceCustom
{
    public class Worker : BackgroundService
    {
        #region Fields
        const string nasRecGetQuery = "ALERT_SERVICE.alert_svc_rec_custom_ga";
        const string recUpdateQuery = "ALERT_SERVICE.alert_svc_rec_custom_u";
        private static Timer callingTimer = null;
        private readonly int _alertTryPeriod;
        private readonly Connection _connection;
        //private readonly ILogger<Worker> _logger;
        //private static readonly ILog _errorLog = LogManager.GetLogger("ErrorLog");
        //private static readonly ILog _logLicense = LogManager.GetLogger("LogLicense");
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _nasLic;
        private readonly string _appMood;
        private readonly string _toEmail;
        private readonly string _toNumber;
        private readonly string _bankName;
        private readonly string _numberPrefix;
        private readonly string _isSmsStart;
        private readonly string _encryptEnable;
        private readonly string _encryptUserId;
        private readonly string _encryptPass;
        private readonly string _encryptConId;
        private readonly string _encryptUri;
        private readonly string _connectionString;
        private readonly string _pushEnable;
        private readonly string _pushEvent;
        private readonly string _pushTitle;
        private readonly string _pushUserName;
        private readonly string _pushFromIp;
        private readonly string _pushUri;
        private static readonly ILog _errorLog = LogManager.GetLogger("ErrorLog");
        private static readonly ILog _licenseLog = LogManager.GetLogger("LogLicense");
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
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpEmail;
        private readonly string _smtpMasking;
        private readonly string _smtpPass;
        private readonly bool _smtpUseDefaultCredentials;
        private readonly bool _smtpEnableSsl;

        #endregion

        public Worker(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            this._connection = new Connection();
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString("DbConnString");
            this._clientFactory = clientFactory;
            _nasLic = _configuration.GetValue<string>("NasLic");
            _appMood = _configuration.GetValue<string>("AppExecution:Mood").ToUpper();
            _toNumber = _configuration.GetValue<string>("AppExecution:Mobile");
            _toEmail = _configuration.GetValue<string>("AppExecution:Email");
            _bankName = _configuration.GetValue<string>("BankName").ToUpper();
            _alertTryPeriod = _configuration.GetValue<int>("AlertTryPeriod");
            _numberPrefix = _configuration.GetValue<string>("NumberPrefix");
            _isSmsStart = _configuration.GetValue<string>("IsSmsStart");
            _encryptEnable = _configuration.GetValue<string>("Encryption:Enable");
            _encryptUserId = _configuration.GetValue<string>("Encryption:UserId");
            _encryptPass = _configuration.GetValue<string>("Encryption:Password");
            _encryptConId = _configuration.GetValue<string>("Encryption:ConnectionId");
            _encryptUri = _configuration.GetValue<string>("Encryption:Uri");
            _pushEnable = _configuration.GetValue<string>("Push:Enable");
            _pushEvent = _configuration.GetValue<string>("Push:Event");
            _pushTitle = _configuration.GetValue<string>("Push:Title");
            _pushUserName = _configuration.GetValue<string>("Push:UserName");
            _pushFromIp = _configuration.GetValue<string>("Push:FromIp");
            _pushUri = _configuration.GetValue<string>("Push:Uri");
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
            this._smtpServer = _configuration.GetValue<string>("SmtpCredentials:Server");
            this._smtpPort = _configuration.GetValue<int>("SmtpCredentials:Port");
            this._smtpEmail = _configuration.GetValue<string>("SmtpCredentials:Email");
            this._smtpMasking = _configuration.GetValue<string>("SmtpCredentials:Masking");
            this._smtpPass = _configuration.GetValue<string>("SmtpCredentials:Password");
            this._smtpUseDefaultCredentials = _configuration.GetValue<bool>("SmtpCredentials:UseDefaultCredentials");
            this._smtpEnableSsl = _configuration.GetValue<bool>("SmtpCredentials:EnableSsl");


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                callingTimer = new Timer(new TimerCallback(TickTimer1), null, 0, _alertTryPeriod);
                await Task.Delay(5);
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "ExecuteAsync", e.Message);
            }
        }

        void TickTimer1(object id)
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
                LogError("WorkerServiceCustom", "TickTimer1", e.Message);
            }
        }

        #region Alert
        private async void SendAlert()
        {
            try
            {
                //ufl: 2021, 9, 30
                //ubl: 2021, 09, 30
                //mmbl: 2021, 12, 30
                //mbl: 2021, 07, 07
                //lbfl: 2030, 12, 30 (suggest by marketing rakib vai)
                //dbl: 
                //sebl:
                //fsibl: 2021, 12, 30
                //Cityzen: 2021, 09, 30
                //probashi:

                if (DateTime.Now.Ticks > new DateTime(2030, 12, 30).Ticks)
                {
                    LogLicenseExpired("WorkerServiceCustom", _nasLic, "License Expired on 2030, 12, 30");
                    return;
                }

                using IDbConnection con = _connection.GetConnection(_connectionString);
                con.Open();

                OracleDynamicParameters alertParameters = new OracleDynamicParameters();
                alertParameters.Add("presult_cursor", null, OracleDbType.RefCursor, ParameterDirection.Output);

                IEnumerable<dynamic> result = con.QueryAsync(nasRecGetQuery, alertParameters, commandType: CommandType.StoredProcedure).Result;

                if (result.Count() > 0)
                {
                    result = result?.Cast<IDictionary<string, object>>();
                    var options = new ParallelOptions() { MaxDegreeOfParallelism = 4 };

                    Parallel.ForEach(result, options, async rows =>
                    {
                        if (rows is IDictionary<string, object> fields)
                        {
                            int nasId = Convert.ToInt32(fields["NAS_ID"]);
                            int emailFlag = Convert.ToInt32(fields["EMAIL_SENT_FLAG"]);
                            int smsFlag = Convert.ToInt32(fields["SMS_SENT_FLAG"]);
                            string nasFncId = Convert.ToString(fields["NAS_FNC_ID"]);
                            string customerId = Convert.ToString(fields["CUSTOMER_ID"]);
                            int pushFlag = Convert.ToInt32(fields["PUSH_FLAG"]);
                            string smsContent = Convert.ToString(fields["SMS_TEXT"]);
                            string smsOperator = Convert.ToString(fields["MOBILE_OPERATOR"]);
                            string toNumber = Convert.ToString(fields["MOBILE_NUMBER"]);
                            string emailBody = Convert.ToString(fields["EMAIL_BODY"]);
                            string toEmail = Convert.ToString(fields["EMAIL_ID"]);
                            string emailSubject = Convert.ToString(fields["EMAIL_SUBJECT"]);
                            int banglaFlag = Convert.ToInt32(fields["BANGLA_FLAG"]);
                            bool isBangla = banglaFlag == 1 ? true : false;

                            #region Phone Number Check
                            toNumber = GetValidNumber(toNumber, _numberPrefix);
                            SmsResponse smsResp = new SmsResponse();

                            if (string.IsNullOrWhiteSpace(toNumber))
                            {
                                smsFlag = 0;
                                smsResp.IsSuccess = "1";
                                smsResp.ReasonForFail = "Invalid Phone Number";
                                smsResp.SmsResponseTime = DateTime.Now.ToString();
                            }
                            #endregion

                            #region Sms
                            if (smsFlag.Equals(1))
                            {
                                if (_appMood.Equals("UAT"))
                                {
                                    toNumber = _toNumber;
                                }

                                smsResp = await SmsSend("0", smsOperator, toNumber, smsContent);
                            }
                            #endregion

                            #region Email
                            EmailResponse emailResp = new EmailResponse();

                            if (emailFlag.Equals(1))
                            {
                                if (_appMood.Equals("UAT"))
                                {
                                    toEmail = _toEmail;
                                }

                                emailResp = SendEmail(toEmail, emailSubject, emailBody);
                            }
                            #endregion

                            #region Update
                            if (smsResp.IsSuccess.Equals("1") || emailResp.IsSuccess.Equals("1"))
                            {
                                OracleDynamicParameters parameters = new OracleDynamicParameters();
                                parameters.Add("psms_provider_message_id", smsResp.SmsProiderId, OracleDbType.NVarchar2, ParameterDirection.Input);
                                parameters.Add("pemail_provider_message_id", emailResp.EmailProiderId, OracleDbType.NVarchar2, ParameterDirection.Input);
                                parameters.Add("psms_reason_for_fail", smsResp.ReasonForFail, OracleDbType.NVarchar2, ParameterDirection.Input);
                                parameters.Add("pemail_reason_for_fail", emailResp.ReasonForFail, OracleDbType.NVarchar2, ParameterDirection.Input);
                                parameters.Add("psms_resp_flag", smsResp.IsSuccess, OracleDbType.Int32, ParameterDirection.Input);
                                parameters.Add("pemail_resp_flag", emailResp.IsSuccess, OracleDbType.Int32, ParameterDirection.Input);
                                parameters.Add("pnas_id", nasId, OracleDbType.Int32, ParameterDirection.Input);
                                parameters.Add("palert_flag", 1, OracleDbType.Int32, ParameterDirection.Input);

                                con.QueryAsync(recUpdateQuery, parameters, commandType: CommandType.StoredProcedure).Wait(5);
                            }
                            #endregion

                            #region Push Notification
                            if (_pushEnable.Equals("1"))
                            {
                                if (pushFlag.Equals(0))
                                {
                                    if (_pushEvent.Split(",").ToList().Any(x => x.Trim().Equals(nasFncId)))
                                    {
                                        await PushApiAsync(smsContent, customerId);
                                    }
                                }
                            }
                            #endregion
                        }
                    });
                }

                con.Close();
                con.Dispose();
                await Task.Delay(1);
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "SendAlert", e.Message);
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
        //            HttpClient client = _clientFactory.CreateClient(_encName);
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
        //                LogError("WorkerServiceCustom", "GetUltimusCon", "Connection string not found");
        //            }
        //        }
        //        else
        //        {
        //            response = _configuration.GetConnectionString("DbConnString");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogError("WorkerServiceCustom", "GetUltimusCon", e.Message);
        //    }

        //    return response;
        //}

        private async Task PushApiAsync(string smsContent, string customerId)
        {
            try
            {
                PushNotificationRequest ObjRequest = new PushNotificationRequest()
                {
                    Title = _pushTitle,
                    SmsContent = smsContent,
                    CustomerId = customerId,
                    Username = _pushUserName,
                    FromIp = _pushFromIp
                };

                string jsonString = JsonConvert.SerializeObject(ObjRequest);
                HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpClient client = _clientFactory.CreateClient("push2");
                HttpResponseMessage result = await client.PostAsync(_pushUri, httpContent);
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "PushApiAsync", e.Message);
            }
        }
        #endregion

        #region SMS
        public async Task<SmsResponse> SmsSend(string ContentType, string smsOperator, string toNumber, string smsContent)
        {
            SmsResponse resp = new SmsResponse() { IsSuccess = "0" };

            try
            {

                if (_sslEnable.Equals("1"))
                {
                    resp = await SslApiAsync(smsOperator, toNumber, smsContent);
                }
                else if (_infobipEnable.Equals("1"))
                {
                    resp = await InfobipApiAsync(smsOperator, toNumber, smsContent);
                }
                else if (_metrotelEnable.Equals("1"))
                {
                    resp = await MetrotelApiAsync(smsOperator, toNumber, smsContent);
                }
                else if (_uflEnable.Equals("1"))
                {
                    resp = await UflApiAsync(smsOperator, toNumber, smsContent);
                }
                else if (_robiEnable.Equals("1"))
                {
                    resp = await RobiApiAsync(smsOperator, toNumber, smsContent);
                }
                else if (_blEnable.Equals("1"))
                {
                    resp = await BanglalinkApiAsync(smsOperator, toNumber, smsContent);
                }
                else if (_gpEnable.Equals("1"))
                {
                    resp = await GrameenPhoneApiAsync(smsOperator, toNumber, smsContent);
                }

                return resp;

            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "SmsSend", e.Message);
                resp.IsSuccess = "0";
                resp.SmsResponseTime = DateTime.Now.ToString();
                resp.ReasonForFail = e.Message;

                return resp;
            }
        }

        private async Task<SmsResponse> SslApiAsync(string smsOperator, string toNumber, string smsContent)
        {
            SmsResponse resp = new SmsResponse();

            try
            {
                string queryString = $"?user={_sslUser}&pass={_sslPass}&msisdn={toNumber}&sms={smsContent}&csmsid={DateTime.Now.ToString("yyyyMMddHHmmssfff")}&sid={_sslSid}";
                HttpClient client = _clientFactory.CreateClient("ssl2");
                HttpResponseMessage response = await client.PostAsync(_sslUri + queryString, null);
                string result = await response.Content.ReadAsStringAsync();
                SslSmsReply objReply;
                XmlSerializer serializer = new XmlSerializer(typeof(SslSmsReply));
                using TextReader reader = new StringReader(result);
                objReply = (SslSmsReply)serializer.Deserialize(reader);

                if (objReply.SMSINFO.Count.Equals(0))
                {
                    resp.IsSuccess = "0";
                    resp.SmsProiderName = "SSL";
                    resp.ReasonForFail = $"Parameter: {objReply.PARAMETER}, Login: {objReply.LOGIN}";
                    resp.SmsResponseTime = DateTime.Now.ToString();
                    resp.SmsProiderId = "";

                    return resp;
                }

                resp.IsSuccess = "1";
                resp.SmsProiderName = "SSL";
                resp.ReasonForFail = "OK";
                resp.SmsResponseTime = DateTime.Now.ToString();
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(result);
                //Read the values into XML Node list
                XmlNodeList xnList = xml.SelectNodes("/REPLY/SMSINFO/REFERENCEID");
                foreach (XmlNode xn in xnList)
                {
                    resp.SmsProiderId = xn.ChildNodes[0].InnerText;
                }

                return resp;
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "SslApiAsync", e.Message);
                resp.IsSuccess = "0";
                resp.SmsProiderName = "SSL";
                resp.ReasonForFail = e.Message;
                resp.SmsResponseTime = DateTime.Now.ToString();
                resp.SmsProiderId = "";

                return resp;
            }
        }

        private async Task<SmsResponse> InfobipApiAsync(string smsOperator, string toNumber, string smsContent)
        {
            SmsResponse resp = new SmsResponse();

            try
            {
                List<Destination> destinations = new List<Destination>()
                    {
                        new Destination { To = toNumber}
                    };

                List<SmsMessage> smsMessages = new List<SmsMessage>()
                    {
                            new SmsMessage {From = _infobipFrom, Text = smsContent, Destinations = destinations}
                    };

                InfobipSmsRequest smsRequest = new InfobipSmsRequest()
                {
                    Messages = smsMessages
                };

                string jsonString = JsonConvert.SerializeObject(smsRequest);
                HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var byteArray = Encoding.ASCII.GetBytes($"{_infobipUserName}:{_infobipPass}");
                HttpClient client = _clientFactory.CreateClient("infobip2");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                HttpResponseMessage result = await client.PostAsync(_infobipUri, httpContent);
                string response = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    InfobipSuccessResponse successResponse = JsonConvert.DeserializeObject<InfobipSuccessResponse>(response);
                    resp.IsSuccess = "1";
                    resp.SmsProiderId = successResponse.Messages[0].MessageId;
                    resp.SmsProiderName = "INFOBIP";
                    resp.ReasonForFail = "OK";
                    resp.SmsResponseTime = DateTime.Now.ToString();

                    return resp;
                }

                InfobipErrorResponse infobipError = JsonConvert.DeserializeObject<InfobipErrorResponse>(response);
                resp.IsSuccess = "0";
                resp.SmsProiderId = infobipError.RequestError.ServiceException.MessageId;
                resp.SmsProiderName = "INFOBIP";
                resp.ReasonForFail = infobipError.RequestError.ServiceException.Text;
                resp.SmsResponseTime = DateTime.Now.ToString();

                return resp;
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "InfobipApiAsync", e.Message);
                resp.IsSuccess = "0";
                resp.SmsProiderName = "INFOBIP";
                resp.ReasonForFail = e.Message;
                resp.SmsResponseTime = DateTime.Now.ToString();
                resp.SmsProiderId = "";

                return resp;
            }
        }

        private async Task<SmsResponse> MetrotelApiAsync(string smsOperator, string toNumber, string smsContent)
        {
            SmsResponse resp = new SmsResponse();

            try
            {
                string queryString = $"?api_key={_metrotelApiKey}&type={_metrotelType}&contacts={toNumber}&senderid={_metrotelSenderId}&msg={smsContent}";
                HttpClient client = _clientFactory.CreateClient("metrotel2");
                HttpResponseMessage response = await client.PostAsync(_metrotelUri + queryString, null);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    resp.IsSuccess = "1";
                    resp.SmsProiderId = "";
                    resp.SmsProiderName = "METROTEL";
                    resp.ReasonForFail = "OK";
                    resp.SmsResponseTime = DateTime.Now.ToString();

                    return resp;
                }

                resp.IsSuccess = "0";
                resp.SmsProiderId = "";
                resp.SmsProiderName = "METROTEL";
                resp.ReasonForFail = "Network Error, please try again.";
                resp.SmsResponseTime = DateTime.Now.ToString();

                return resp;
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "MetrotelApiAsync", e.Message);
                resp.IsSuccess = "0";
                resp.SmsProiderName = "METROTEL";
                resp.ReasonForFail = e.Message;
                resp.SmsResponseTime = DateTime.Now.ToString();
                resp.SmsProiderId = "";

                return resp;
            }
        }

        private async Task<SmsResponse> RobiApiAsync(string smsOperator, string toNumber, string smsContent)
        {
            SmsResponse resp = new SmsResponse();

            try
            {
                string queryString = "?Username=" + _robiUserName + "&Password=" + _robiPass + "&From=" + _robiMasking + "&To=" + toNumber + "&Message=" + smsContent;

                HttpClient client = _clientFactory.CreateClient("robi2");
                HttpResponseMessage response = await client.GetAsync(_robiUri + queryString);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    resp.IsSuccess = "1";
                    resp.SmsProiderId = "";
                    resp.SmsProiderName = "ROBI";
                    resp.ReasonForFail = "OK";
                    resp.SmsResponseTime = DateTime.Now.ToString();

                    return resp;
                }

                resp.IsSuccess = "0";
                resp.SmsProiderId = "";
                resp.SmsProiderName = "ROBI";
                resp.ReasonForFail = "Network Error, please try again.";
                resp.SmsResponseTime = DateTime.Now.ToString();

                return resp;
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "RobiApiAsync", e.Message);
                resp.IsSuccess = "0";
                resp.SmsProiderName = "ROBI";
                resp.ReasonForFail = e.Message;
                resp.SmsResponseTime = DateTime.Now.ToString();
                resp.SmsProiderId = "";

                return resp;
            }
        }

        private async Task<SmsResponse> BanglalinkApiAsync(string smsOperator, string toNumber, string smsContent)
        {
            SmsResponse resp = new SmsResponse();

            try
            {
                string queryString = "?msisdn=" + toNumber + "&message=" + smsContent + "&userID=" + _blUserName + "&passwd=" + _blPass + "&sender=" + _blMasking;
                HttpClient client = _clientFactory.CreateClient("banglalink2");
                HttpResponseMessage response = await client.GetAsync(_blUri + queryString);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    resp.IsSuccess = "1";
                    resp.SmsProiderId = "";
                    resp.SmsProiderName = "BL";
                    resp.ReasonForFail = "OK";
                    resp.SmsResponseTime = DateTime.Now.ToString();

                    return resp;
                }

                resp.IsSuccess = "0";
                resp.SmsProiderId = "";
                resp.SmsProiderName = "BL";
                resp.ReasonForFail = "Network Error, please try again.";
                resp.SmsResponseTime = DateTime.Now.ToString();

                return resp;
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "BanglalinkApiAsync", e.Message);
                resp.IsSuccess = "0";
                resp.SmsProiderName = "BL";
                resp.ReasonForFail = e.Message;
                resp.SmsResponseTime = DateTime.Now.ToString();
                resp.SmsProiderId = "";

                return resp;
            }
        }

        private async Task<SmsResponse> GrameenPhoneApiAsync(string smsOperator, string toNumber, string smsContent)
        {
            SmsResponse resp = new SmsResponse();

            try
            {
                string queryString = "?username=" + _gpUserName + "&password=" + _gpPass + "&apicode=1&msisdn=" + toNumber + "&countrycode=880&cli= " + _gpMasking + "&messagetype=1&message=" + smsContent + "&messageid=0";
                HttpClient client = _clientFactory.CreateClient("grameenphone2");
                HttpResponseMessage response = await client.GetAsync(_gpUri + queryString);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    resp.IsSuccess = "1";
                    resp.SmsProiderId = "";
                    resp.SmsProiderName = "GP";
                    resp.ReasonForFail = "OK";
                    resp.SmsResponseTime = DateTime.Now.ToString();

                    return resp;
                }

                resp.IsSuccess = "0";
                resp.SmsProiderId = "";
                resp.SmsProiderName = "GP";
                resp.ReasonForFail = "Network Error, please try again.";
                resp.SmsResponseTime = DateTime.Now.ToString();

                return resp;
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "GrameenPhoneApiAsync", e.Message);
                resp.IsSuccess = "0";
                resp.SmsProiderName = "GP";
                resp.ReasonForFail = e.Message;
                resp.SmsResponseTime = DateTime.Now.ToString();
                resp.SmsProiderId = "";

                return resp;
            }
        }

        private async Task<SmsResponse> UflApiAsync(string smsOperator, string toNumber, string smsContent)
        {
            SmsResponse resp = new SmsResponse();

            try
            {
                string queryString = "?MobileNo=" + toNumber + "&SmsBody=" + smsContent + "&RequestId=" + toNumber;
                HttpClient client = _clientFactory.CreateClient("ufl2");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("APIKey", _uflApiKey);
                HttpResponseMessage response = await client.PostAsync(_uflUri + queryString, null);
                string result = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    resp.IsSuccess = "1";
                    resp.SmsProiderId = "";
                    resp.SmsProiderName = "UFL";
                    resp.ReasonForFail = "OK";
                    resp.SmsResponseTime = DateTime.Now.ToString();

                    return resp;
                }

                resp.IsSuccess = "0";
                resp.SmsProiderId = "";
                resp.SmsProiderName = "UFL";
                resp.ReasonForFail = "Network Error, please try again.";
                resp.SmsResponseTime = DateTime.Now.ToString();

                return resp;
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "UflApiAsync", e.Message);
                resp.IsSuccess = "0";
                resp.SmsProiderName = "UFL";
                resp.ReasonForFail = e.Message;
                resp.SmsResponseTime = DateTime.Now.ToString();
                resp.SmsProiderId = "";

                return resp;
            }
        }
        #endregion

        #region Email
        public EmailResponse SendEmail(string toEmail, string emailSubject, string emailContent)
        {
            EmailResponse resp = new EmailResponse();

            try
            {
                string line1 = null;
                string clobString = emailContent.ToString();
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

                using MailMessage mailMessage = new MailMessage();
                using SmtpClient client = new SmtpClient(_smtpServer, _smtpPort);
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
                resp.IsSuccess = "1";
                resp.EmailResponseTime = DateTime.Now.ToString();
                resp.EmailProiderName = _smtpEmail;

                return resp;
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "SendEmail", e.Message);
                resp.IsSuccess = "0";
                resp.EmailResponseTime = DateTime.Now.ToString();
                resp.EmailProiderName = _smtpEmail;
                resp.ReasonForFail = e.Message;

                return resp;
            }
        }
        #endregion

        #region Helpers
        public string ConvertBanglatoUnicode(string banglaText)
        {
            string unicode = string.Empty;

            try
            {
                StringBuilder sb = new StringBuilder();

                foreach (char c in banglaText)
                {
                    sb.AppendFormat("{1:x4}", c, (int)c);
                }

                unicode = sb.ToString().ToUpper();
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "ConvertBanglatoUnicode", e.Message);
                unicode = string.Empty;
            }

            return unicode;
        }

        public string GetValidNumber(string toNumber, string numberPrefixList)
        {
            string resp = "";

            try
            {
                if (!string.IsNullOrWhiteSpace(toNumber))
                {
                    toNumber = toNumber.Trim();
                    int countToNumber = toNumber.Length;

                    if (countToNumber.Equals(11))
                    {
                        string numberPrefix = "88" + toNumber.Substring(0, 3);

                        if (IsValidNumberPrefix(numberPrefix, numberPrefixList))
                        {
                            resp = "88" + toNumber;
                            return resp;
                        }
                    }
                    else if (countToNumber.Equals(13))
                    {
                        string numberPrefix = toNumber.Substring(0, 5);

                        if (IsValidNumberPrefix(numberPrefix, numberPrefixList))
                        {
                            resp = toNumber;
                            return resp;
                        }
                    }
                    else if (countToNumber > 11)
                    {
                        toNumber = toNumber.Substring(toNumber.Length - 11, 11);
                        string numberPrefix = "88" + toNumber.Substring(0, 3);

                        if (IsValidNumberPrefix(numberPrefix, numberPrefixList))
                        {
                            resp = "88" + toNumber;
                            return resp;
                        }
                    }

                    return resp;
                }

                return resp;
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "GetValidNumber", e.Message);
                return "";
            }
        }

        private bool IsValidNumberPrefix(string numberPrefix, string numberPrefixList)
        {
            try
            {
                List<string> prefixList = numberPrefixList?.Split(',').ToList();

                foreach (var item in prefixList)
                {
                    if (numberPrefix.Equals(item.Trim()))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                LogError("WorkerServiceCustom", "IsValidNumber", e.Message);
                return false;
            }

            return false;
        }
        #endregion

        #region Logger
        public void LogError(string projectName, string functionName, string errorMessage)
        {
            var dirname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            XmlConfigurator.Configure(new FileInfo(string.Format("{0}{1}", dirname, @"\log4net.config")));
            string sLogFormat = $"{DateTime.Now} === Project Name: {projectName} === Function Name: {functionName} === Message: {errorMessage} {Environment.NewLine}";
            _errorLog.InfoFormat(sLogFormat);
        }

        public void LogLicenseExpired(string projectName, string location, string expiredMessage)
        {
            string directory = $"{location}\\{DateTime.Now.Year}-{DateTime.Now:MM}-{DateTime.Now.Day}.tsv";

            if (File.Exists(directory))
            {
                return;
            }
            else
            {
                var dirname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                XmlConfigurator.Configure(new FileInfo(string.Format("{0}{1}", dirname, @"\log4net.config")));
                string sLogFormat = $"{DateTime.Now} === Project Name: {projectName} === Message: {expiredMessage} {Environment.NewLine}";
                _licenseLog.InfoFormat(sLogFormat);
            }
        }
        #endregion
    }
}
