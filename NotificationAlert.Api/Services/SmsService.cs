using Microsoft.Extensions.Configuration;
using NotificationAlert.Api.Models;
using NotificationAlert.Api.Repositories;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NotificationAlert.Api.Services
{
    public class SmsService : ISmsRepository
    {
        #region Fields
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogRepository _errorLog;
        private readonly string _apiMood;
        private readonly string _toNumber;
        private readonly string _uflApiBaseUrl;
        private readonly string _uflApiKey;
        private readonly string _smsSuccessDir;
        private readonly string _smsFailDir;
        private readonly string _custSmsSuccessDir;
        private readonly string _custSmsFailDir;
        private readonly string _sslSmsUser;
        private readonly string _sslSmsPass;
        private readonly string _sslSmsSid;
        private readonly string _sslSmsUri;
        private const string _smsHeadFormat = "Sent Time\tProduct Id\tAccount Number\tMobile Number\tSms Text\tStatus";

        /**/


        #endregion

        public SmsService(IConfiguration configuration, ILogRepository errorLog, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _errorLog = errorLog;
            _apiMood = _configuration.GetSection("API_ExecutionMode").GetSection("MODE").Value.ToUpper();
            _toNumber = _configuration.GetSection("API_ExecutionMode").GetSection("MOBILE").Value;
            _uflApiBaseUrl = _configuration.GetSection("WebAPIBaseUrl").Value;
            _uflApiKey = _configuration.GetSection("ApiKey").Value;
            _smsSuccessDir = _configuration.GetSection("SmsSuccessDir").Value;
            _smsFailDir = _configuration.GetSection("SmsFailDir").Value;
            _custSmsSuccessDir = _configuration.GetSection("CustSmsSuccessDir").Value;
            _custSmsFailDir = _configuration.GetSection("CustSmsFailDir").Value;
            _sslSmsUser = _configuration.GetSection("SSL_SMS").GetSection("user").Value;
            _sslSmsPass = _configuration.GetSection("SSL_SMS").GetSection("pass").Value;
            _sslSmsSid = _configuration.GetSection("SSL_SMS").GetSection("sid").Value;
            _sslSmsUri = _configuration.GetSection("SSL_SMS").GetSection("URI").Value;

        }

        private string ConvertBanglatoUnicode(string banglaText)
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
                _errorLog.LogError(e.Message).Wait();
            }

            return unicode;
        }

        private async Task CustomSslApiBanglaHttpClientAsync(string smsContent, string toNumber, string accountNo)
        {
            try
            {
                if (string.Equals(_apiMood, "UAT"))
                {
                    toNumber = _toNumber;
                }

                if (!string.IsNullOrEmpty(toNumber) && toNumber.Length > 10)
                {
                    string unicode = ConvertBanglatoUnicode(smsContent);
                    string user = _configuration.GetSection("SSL_SMS").GetSection("BanglaUser").Value;
                    string password = _configuration.GetSection("SSL_SMS").GetSection("BanglaPass").Value;

                    FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user", user),
                        new KeyValuePair<string, string>("pass", password),
                        new KeyValuePair<string, string>("sms[0][0]", $"88{toNumber}"),
                        new KeyValuePair<string, string>("sms[0][1]", unicode),
                        new KeyValuePair<string, string>("sms[0][2]", DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                        new KeyValuePair<string, string>("sid", _configuration.GetSection("SSL_SMS").GetSection("BanglaSid").Value)
                    });

                    //Uri uri = new Uri(_sslUri);
                    HttpClient client = _clientFactory.CreateClient("CustomSslClient");
                    HttpResponseMessage response = await client.PostAsync(_configuration.GetSection("SSL_SMS").GetSection("BanglaUri").Value, stringContent);
                    string result = await response.Content.ReadAsStringAsync();
                    string newResult = Convert.ToString(result);
                    SslSmsReply objReply;
                    XmlSerializer serializer = new XmlSerializer(typeof(SslSmsReply));
                    using TextReader reader = new StringReader(newResult);
                    objReply = (SslSmsReply)serializer.Deserialize(reader);

                    if (objReply.SMSINFO.Count == 0)
                    {
                        _errorLog.LogCustomSmsFailTsv(_custSmsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                    }
                    else
                    {
                        _errorLog.LogCustomSmsSuccessTsv(_custSmsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                    }
                }
                else
                {
                    _errorLog.LogCustomSmsFailTsv(_custSmsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Invalid Mobile Number");
                }
            }
            catch (Exception e)
            {
                _errorLog.LogError(e.Message).Wait();
            }
        }

        private void SslApiWebClient(string smsContent, string toNumber, string accountNo)
        {
            try
            {
                if (_configuration.GetSection("API_ExecutionMode").GetSection("MODE").Value.ToUpper() == "UAT")
                {
                    toNumber = _configuration.GetSection("API_ExecutionMode").GetSection("MOBILE").Value;
                }

                if (!string.IsNullOrEmpty(toNumber) && toNumber.Length > 10)
                {
                    //string myParameters =
                    //    "user=" + user + 
                    //    "&pass=" + pass +
                    //    "&sms[0][0]=88" + mobileNo +                            //length 13
                    //    "&sms[0][1]=" + System.Web.HttpUtility.UrlEncode(message) +
                    //    "&sms[0][2]=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                    //    "&sms[1][0]=88***********" +
                    //    "&sms[1][1]=" + System.Web.HttpUtility.UrlEncode("TESTSMS2\nTESTSMS3") +
                    //    "&sms[1][2]=" + "1234567890" +
                    //    "&sid=" + sid;
                    string myParameters =
                                "user=" + _configuration.GetSection("SSL_SMS").GetSection("user").Value +
                                "&pass=" + _configuration.GetSection("SSL_SMS").GetSection("pass").Value +
                                "&sms[0][0]=" + "88" + toNumber +
                                "&sms[0][1]=" + System.Web.HttpUtility.UrlEncode(smsContent) +
                                "&sms[0][2]=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                "&sid=" + _configuration.GetSection("SSL_SMS").GetSection("sid").Value;

                    //string myParameters = $"user={ _user }&pass={ _pass }&msisdn=88{ toNumber }&sms={System.Web.HttpUtility.UrlEncode(smsContent)}&csmsid={DateTime.Now.ToString("yyyyMMddHHmmssfff")}&sid={_sid}";

                    SslSmsReply objReply;
                    string htmlResult;
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        htmlResult = wc.UploadString(_configuration.GetSection("SSL_SMS").GetSection("URI").Value, myParameters);
                        XmlSerializer serializer = new XmlSerializer(typeof(SslSmsReply));

                        using TextReader reader = new StringReader(htmlResult);
                        objReply = (SslSmsReply)serializer.Deserialize(reader);
                    }

                    if (objReply.SMSINFO.Count == 0)
                    {
                        string smsFailDir = _configuration.GetValue<string>("SmsFailDir");
                        _errorLog.LogSmsFailTsv(smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                    }
                    else
                    {
                        string smsSuccessDir = _configuration.GetValue<string>("SmsSuccessDir");
                        _errorLog.LogSmsSuccessTsv(smsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                    }
                }
                else
                {
                    string smsFailDir = _configuration.GetValue<string>("SmsFailDir");
                    _errorLog.LogSmsFailTsv(smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Invalid Mobile Number");
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex.Message).Wait();
            }
        }

        private async Task SslApiHttpClientAsync(string smsContent, string toNumber, string accountNo)
        {
            try
            {
                if (string.Equals(_apiMood, "UAT"))
                {
                    toNumber = _toNumber;
                }

                int numberLength = toNumber.Length;

                if (!string.IsNullOrEmpty(toNumber) && numberLength > 10)
                {
                    if (numberLength == 11)
                    {
                        FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("user", _sslSmsUser),
                            new KeyValuePair<string, string>("pass", _sslSmsPass),
                            new KeyValuePair<string, string>("sms[0][0]", $"88{toNumber}"),
                            new KeyValuePair<string, string>("sms[0][1]", smsContent),
                            new KeyValuePair<string, string>("sms[0][2]", DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                            new KeyValuePair<string, string>("sid", _sslSmsSid)
                        });

                        //Uri uri = new Uri(_sslUri);
                        HttpClient client = _clientFactory.CreateClient("SslClient");
                        HttpResponseMessage response = await client.PostAsync(_sslSmsUri, stringContent);
                        await Task.Delay(50);
                        string result = await response.Content.ReadAsStringAsync();
                        SslSmsReply objReply;
                        XmlSerializer serializer = new XmlSerializer(typeof(SslSmsReply));
                        using TextReader reader = new StringReader(result);
                        objReply = (SslSmsReply)serializer.Deserialize(reader);

                        if (objReply.SMSINFO.Count == 0)
                        {
                            _errorLog.LogSmsFailTsv(_smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                        }
                        else
                        {
                            _errorLog.LogSmsSuccessTsv(_smsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                        }
                    }
                    else
                    {
                        if (toNumber.Contains("+"))
                        {
                            string newNumber = toNumber.Substring(1);
                            numberLength = newNumber.Length;

                            if (numberLength == 13)
                            {
                                FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]
                                {
                                    new KeyValuePair<string, string>("user", _sslSmsUser),
                                    new KeyValuePair<string, string>("pass", _sslSmsPass),
                                    new KeyValuePair<string, string>("sms[0][0]", newNumber),
                                    new KeyValuePair<string, string>("sms[0][1]", smsContent),
                                    new KeyValuePair<string, string>("sms[0][2]", DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                    new KeyValuePair<string, string>("sid", _sslSmsSid)
                                });

                                //Uri uri = new Uri(_sslUri);
                                HttpClient client = _clientFactory.CreateClient("SslClient");
                                HttpResponseMessage response = await client.PostAsync(_sslSmsUri, stringContent);
                                await Task.Delay(50);
                                string result = await response.Content.ReadAsStringAsync();
                                SslSmsReply objReply;
                                XmlSerializer serializer = new XmlSerializer(typeof(SslSmsReply));
                                using TextReader reader = new StringReader(result);
                                objReply = (SslSmsReply)serializer.Deserialize(reader);

                                if (objReply.SMSINFO.Count == 0)
                                {
                                    _errorLog.LogSmsFailTsv(_smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                                }
                                else
                                {
                                    _errorLog.LogSmsSuccessTsv(_smsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                                }
                            }
                            else
                            {
                                _errorLog.LogSmsFailTsv(_smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Invalid Mobile Number");
                            }
                        }
                        else if (numberLength == 13)
                        {
                            FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]
                            {
                                new KeyValuePair<string, string>("user", _sslSmsUser),
                                new KeyValuePair<string, string>("pass", _sslSmsPass),
                                new KeyValuePair<string, string>("sms[0][0]", toNumber),
                                new KeyValuePair<string, string>("sms[0][1]", smsContent),
                                new KeyValuePair<string, string>("sms[0][2]", DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                new KeyValuePair<string, string>("sid", _sslSmsSid)
                            });

                            //Uri uri = new Uri(_sslUri);
                            HttpClient client = _clientFactory.CreateClient("SslClient");
                            HttpResponseMessage response = await client.PostAsync(_configuration.GetSection("SSL_SMS").GetSection("URI").Value, stringContent);
                            await Task.Delay(50);
                            string result = await response.Content.ReadAsStringAsync();
                            SslSmsReply objReply;
                            XmlSerializer serializer = new XmlSerializer(typeof(SslSmsReply));
                            using TextReader reader = new StringReader(result);
                            objReply = (SslSmsReply)serializer.Deserialize(reader);

                            if (objReply.SMSINFO.Count == 0)
                            {
                                _errorLog.LogSmsFailTsv(_smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                            }
                            else
                            {
                                _errorLog.LogSmsSuccessTsv(_smsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                            }
                        }
                    }
                }
                else
                {
                    _errorLog.LogSmsFailTsv(_smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Invalid Mobile Number");
                }
            }
            catch (Exception e)
            {
                _errorLog.LogError(e.Message).Wait();
            }
        }

        private void RobiApi(string smsContent, string toNumber, string accountNo, out string error, out string response)
        {
            response = string.Empty;
            error = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(toNumber) && toNumber.Length > 10)
                {
                    string robiProviderApi = _configuration.GetSection("ProviderAPI").GetSection("ROBI_API").Value;
                    string robiUserName = _configuration.GetSection("Robi").GetSection("USER_NAME_ROBI").Value;
                    string robiPass = _configuration.GetSection("Robi").GetSection("PASSWORD_ROBI").Value;
                    string robiMasking = _configuration.GetSection("Robi").GetSection("MASKING_ROBI").Value;
                    string url = robiProviderApi + "?Username=" + robiUserName + "&Password=" + robiPass + "&From=" + robiMasking + "&To=" + toNumber + "&Message=" + smsContent;
                    using WebClient wc = new WebClient();
                    string xml = wc.DownloadString(url);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    XmlNodeList xnList = doc.GetElementsByTagName("ServiceClass");

                    foreach (XmlNode xn in xnList)
                    {
                        error = xn["ErrorCode"].InnerText;
                    }
                    response = (error == "0") ? "1" : "0";

                    if (response == "1")
                    {
                        _errorLog.LogSmsSuccessTsv(_smsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                    }
                    else
                    {
                        string smsFailDir = _configuration.GetValue<string>("SmsFailDir");
                        _errorLog.LogSmsFailTsv(smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                    }
                }
                else
                {
                    string smsFailDir = _configuration.GetValue<string>("SmsFailDir");
                    _errorLog.LogSmsFailTsv(smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Invalid Mobile Number");
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                _errorLog.LogError(e.Message).Wait();
            }

        }

        private void BanglalinkApi(string smsContent, string toNumber, string accountNo, out string response)
        {
            response = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(toNumber) && toNumber.Length > 10)
                {
                    string blProviderApi = _configuration.GetSection("ProviderAPI").GetSection("BL_API").Value;
                    string blUserName = _configuration.GetSection("Banglalink").GetSection("USER_NAME_BL").Value;
                    string blPass = _configuration.GetSection("Banglalink").GetSection("PASSWORD_BL").Value;
                    string blMasking = _configuration.GetSection("Banglalink").GetSection("MASKING_BL").Value;
                    string url = blProviderApi + "?msisdn=" + toNumber + "&message=" + smsContent + "&userID=" + blUserName + "&passwd=" + blPass + "&sender=" + blMasking;
                    using WebClient wc = new WebClient();
                    string xml = wc.DownloadString(url);
                    string[] separator = { "and" };
                    string success = xml.Split(separator, StringSplitOptions.RemoveEmptyEntries)[0];
                    response = success.Contains('1') ? "1" : "0";

                    if (response == "1")
                    {
                        //this._errorLog.LogSmsSuccess("Status: Success, MobileNo:" + toNumber + ", Text:" + smsContent).Wait();
                        //Log.Information("SMS Successfully Sent to " + toNumber + ". Response " + response);
                        string smsSuccessDir = _configuration.GetValue<string>("SmsSuccessDir");
                        _errorLog.LogSmsSuccessTsv(smsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                    }
                    else
                    {
                        //this._errorLog.LogSmsFailed("Status: Failed, MobileNo:" + toNumber + ", Text:" + smsContent).Wait();
                        //Log.Information("Error sending SMS to " + toNumber + ". Response " + response);
                        string smsFailDir = _configuration.GetValue<string>("SmsFailDir");
                        _errorLog.LogSmsFailTsv(smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                    }
                }
                else
                {
                    string smsFailDir = _configuration.GetValue<string>("SmsFailDir");
                    _errorLog.LogSmsFailTsv(smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Invalid Mobile Number");
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                _errorLog.LogError(e.Message).Wait();
            }
        }

        private void DefaultApi(string smsContent, string toNumber, string accountNo, out string error, out string response)
        {
            response = string.Empty;
            error = string.Empty;

            try
            {
                string defaultProviderId = _configuration.GetSection("DefaultProvider").GetSection("DEFAULT_PROVIDER_ID").Value;

                //Default-Robi
                if (defaultProviderId.Contains("18"))
                {
                    RobiApi(smsContent, toNumber, accountNo, out error, out response);
                }
                //Default-BL
                else if (defaultProviderId.Contains("19"))
                {
                    BanglalinkApi(smsContent, toNumber, accountNo, out response);
                }
                //Default-GP
                else if (defaultProviderId.Contains("17"))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(toNumber) && toNumber.Length > 10)
                        {
                            string gpProviderApi = _configuration.GetSection("ProviderAPI").GetSection("GP_API").Value;
                            string gpUserName = _configuration.GetSection("GrameenPhone").GetSection("USER_NAME_GP").Value;
                            string gpPass = _configuration.GetSection("GrameenPhone").GetSection("PASSWORD_GP").Value;
                            string gpMasking = _configuration.GetSection("GrameenPhone").GetSection("MASKING_GP").Value;
                            string url = gpProviderApi + "?username=" + gpUserName + "&password=" + gpPass + "&apicode=1&msisdn=" + toNumber + "&countrycode=880&cli= " + gpMasking + "&messagetype=1&message=" + smsContent + "&messageid=0";
                            using WebClient wc = new WebClient();
                            string xml = wc.DownloadString(url);
                            response = xml;
                            string smsSuccessDir = _configuration.GetValue<string>("SmsSuccessDir");
                            _errorLog.LogSmsSuccessTsv(smsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                        }
                        else
                        {
                            string smsFailDir = _configuration.GetValue<string>("SmsFailDir");
                            _errorLog.LogSmsFailTsv(smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Invalid Mobile Number");
                        }
                    }
                    catch (Exception e)
                    {
                        _errorLog.LogError(e.Message).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex.Message).Wait();
            }
        }

        public async Task CustomSslApiHttpClientAsync(string smsContent, string toNumber, string accountNo)
        {
            try
            {
                if (string.Equals(_apiMood, "UAT"))
                {
                    toNumber = _toNumber;
                }

                int numberLength = toNumber.Length;

                if (!string.IsNullOrEmpty(toNumber) && toNumber.Length > 10)
                {
                    if (numberLength == 11)
                    {
                        FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("user", _sslSmsUser),
                            new KeyValuePair<string, string>("pass", _sslSmsPass),
                            new KeyValuePair<string, string>("sms[0][0]", $"88{toNumber}"),
                            new KeyValuePair<string, string>("sms[0][1]", smsContent),
                            new KeyValuePair<string, string>("sms[0][2]", DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                            new KeyValuePair<string, string>("sid", _sslSmsSid)
                        });

                        //Uri uri = new Uri(_sslUri);
                        HttpClient client = _clientFactory.CreateClient("CustomSslClient");
                        HttpResponseMessage response = await client.PostAsync(_sslSmsUri, stringContent);
                        await Task.Delay(50);
                        string result = await response.Content.ReadAsStringAsync();
                        SslSmsReply objReply;
                        XmlSerializer serializer = new XmlSerializer(typeof(SslSmsReply));
                        using TextReader reader = new StringReader(result);
                        objReply = (SslSmsReply)serializer.Deserialize(reader);

                        if (objReply.SMSINFO.Count == 0)
                        {
                            _errorLog.LogCustomSmsFailTsv(_custSmsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                        }
                        else
                        {
                            _errorLog.LogCustomSmsSuccessTsv(_custSmsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                        }
                    }
                    else
                    {
                        if (toNumber.Contains("+"))
                        {
                            string newNumber = toNumber.Substring(1);
                            numberLength = newNumber.Length;

                            if (numberLength == 13)
                            {
                                FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]
                                {
                                    new KeyValuePair<string, string>("user", _sslSmsUser),
                                    new KeyValuePair<string, string>("pass", _sslSmsPass),
                                    new KeyValuePair<string, string>("sms[0][0]", newNumber),
                                    new KeyValuePair<string, string>("sms[0][1]", smsContent),
                                    new KeyValuePair<string, string>("sms[0][2]", DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                    new KeyValuePair<string, string>("sid", _sslSmsSid)
                                });

                                //Uri uri = new Uri(_sslUri);
                                HttpClient client = _clientFactory.CreateClient("CustomSslClient");
                                HttpResponseMessage response = await client.PostAsync(_sslSmsUri, stringContent);
                                await Task.Delay(50);
                                string result = await response.Content.ReadAsStringAsync();
                                SslSmsReply objReply;
                                XmlSerializer serializer = new XmlSerializer(typeof(SslSmsReply));
                                using TextReader reader = new StringReader(result);
                                objReply = (SslSmsReply)serializer.Deserialize(reader);

                                if (objReply.SMSINFO.Count == 0)
                                {
                                    _errorLog.LogCustomSmsFailTsv(_custSmsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                                }
                                else
                                {
                                    _errorLog.LogCustomSmsSuccessTsv(_custSmsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                                }
                            }
                            else
                            {
                                _errorLog.LogCustomSmsFailTsv(_custSmsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Invalid Mobile Number");
                            }
                        }
                        else if (numberLength == 13)
                        {
                            FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]
                            {
                                new KeyValuePair<string, string>("user", _sslSmsUser),
                                new KeyValuePair<string, string>("pass", _sslSmsPass),
                                new KeyValuePair<string, string>("sms[0][0]", toNumber),
                                new KeyValuePair<string, string>("sms[0][1]", smsContent),
                                new KeyValuePair<string, string>("sms[0][2]", DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                new KeyValuePair<string, string>("sid", _sslSmsSid)
                            });

                            //Uri uri = new Uri(_sslUri);
                            HttpClient client = _clientFactory.CreateClient("CustomSslClient");
                            HttpResponseMessage response = await client.PostAsync(_sslSmsUri, stringContent);
                            await Task.Delay(50);
                            string result = await response.Content.ReadAsStringAsync();
                            SslSmsReply objReply;
                            XmlSerializer serializer = new XmlSerializer(typeof(SslSmsReply));
                            using TextReader reader = new StringReader(result);
                            objReply = (SslSmsReply)serializer.Deserialize(reader);

                            if (objReply.SMSINFO.Count == 0)
                            {
                                _errorLog.LogCustomSmsFailTsv(_custSmsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                            }
                            else
                            {
                                _errorLog.LogCustomSmsSuccessTsv(_custSmsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                            }
                        }
                    }
                }
                else
                {
                    _errorLog.LogCustomSmsFailTsv(_custSmsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Invalid Mobile Number");
                }
            }
            catch (Exception e)
            {
                _errorLog.LogError(e.Message).Wait();
            }
        }

        public async Task CustomSslApiBothHttpClientAsync(string smsContent, string toNumber, string accountNo)
        {
            try
            {
                if (_configuration.GetSection("API_ExecutionMode").GetSection("MODE").Value.ToUpper() == "UAT")
                {
                    toNumber = _configuration.GetSection("API_ExecutionMode").GetSection("MOBILE").Value;
                }

                if (!string.IsNullOrEmpty(toNumber) && toNumber.Length > 10)
                {
                    if (_configuration.GetSection("SSL_SMS").GetSection("IsBangla").Value.ToUpper() == "TRUE")
                    {
                        await CustomSslApiBanglaHttpClientAsync(smsContent, toNumber, accountNo);
                    }
                    else
                    {
                        FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("user", _configuration.GetSection("SSL_SMS").GetSection("user").Value),
                            new KeyValuePair<string, string>("pass", _configuration.GetSection("SSL_SMS").GetSection("pass").Value),
                            new KeyValuePair<string, string>("sms[0][0]", $"88{toNumber}"),
                            new KeyValuePair<string, string>("sms[0][1]", smsContent),
                            new KeyValuePair<string, string>("sms[0][2]", DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                            new KeyValuePair<string, string>("sid", _configuration.GetSection("SSL_SMS").GetSection("sid").Value)
                        });

                        //Uri uri = new Uri(_sslUri);
                        HttpClient client = _clientFactory.CreateClient("CustomSslClient");
                        HttpResponseMessage response = await client.PostAsync(_configuration.GetSection("SSL_SMS").GetSection("URI").Value, stringContent);
                        string result = await response.Content.ReadAsStringAsync();
                        string newResult = Convert.ToString(result);
                        SslSmsReply objReply;
                        XmlSerializer serializer = new XmlSerializer(typeof(SslSmsReply));
                        using TextReader reader = new StringReader(newResult);
                        objReply = (SslSmsReply)serializer.Deserialize(reader);

                        if (objReply.SMSINFO.Count == 0)
                        {
                            string custSmsFailDir = _configuration.GetValue<string>("CustSmsFailDir");
                            _errorLog.LogCustomSmsFailTsv(custSmsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                        }
                        else
                        {
                            string custSmsSuccessDir = _configuration.GetValue<string>("CustSmsSuccessDir");
                            _errorLog.LogCustomSmsSuccessTsv(custSmsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                        }
                    }
                }
                else
                {
                    string custSmsFailDir = _configuration.GetValue<string>("CustSmsFailDir");
                    _errorLog.LogCustomSmsFailTsv(custSmsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Invalid Mobile Number");
                }
            }
            catch (Exception e)
            {
                _errorLog.LogError(e.Message).Wait();
            }
        }

        public async Task UflSmsSendApiAsync(string smsContent, string toNumber, string accountNo)
        {
            try
            {
                if (string.Equals(_apiMood, "UAT"))
                {
                    toNumber = _toNumber;
                }

                if (!string.IsNullOrEmpty(toNumber) && toNumber.Length > 10)
                {
                    string endPoint = _uflApiBaseUrl + "?MobileNo=" + toNumber + "&SmsBody=" + smsContent + "&RequestId=" + accountNo;
                    //Uri uri = new Uri(endPoint);
                    HttpClient client = _clientFactory.CreateClient("BankClient");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("APIKey", _uflApiKey);
                    HttpResponseMessage result = await client.PostAsync(endPoint, null);
                    await Task.Delay(50);

                    if (result.IsSuccessStatusCode)
                    {
                        string response = await result.Content.ReadAsStringAsync();

                        if (string.Equals(response, "1"))
                        {
                            _errorLog.LogSmsSuccessTsv(_smsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                        }
                        else
                        {
                            _errorLog.LogSmsFailTsv(_smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                        }
                    }
                    else
                    {
                        _errorLog.LogSmsFailTsv(_smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                    }
                }
                else
                {
                    _errorLog.LogSmsFailTsv(_smsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Invalid Mobile Number");
                }
            }
            catch (Exception e)
            {
                _errorLog.LogError(e.Message).Wait();
            }
        }

        public async Task CustomUflSmsSendApiAsync(string smsContent, string toNumber, string accountNo)
        {
            try
            {
                if (string.Equals(_apiMood, "UAT"))
                {
                    toNumber = _toNumber;
                }

                if (!string.IsNullOrEmpty(toNumber) && toNumber.Length > 10)
                {
                    string endPoint = _uflApiBaseUrl + "?MobileNo=" + toNumber + "&SmsBody=" + smsContent + "&RequestId=" + accountNo;
                    //Uri uri = new Uri(endPoint);
                    HttpClient client = _clientFactory.CreateClient("CustomBankClient");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("APIKey", _uflApiKey);
                    HttpResponseMessage result = await client.PostAsync(endPoint, null);
                    await Task.Delay(50);

                    if (result.IsSuccessStatusCode)
                    {
                        string response = await result.Content.ReadAsStringAsync();

                        if (string.Equals(response, "1"))
                        {
                            _errorLog.LogCustomSmsSuccessTsv(_custSmsSuccessDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Success");
                        }
                        else
                        {
                            _errorLog.LogCustomSmsFailTsv(_custSmsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                        }
                    }
                    else
                    {
                        _errorLog.LogCustomSmsFailTsv(_custSmsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Failed");
                    }
                }
                else
                {
                    _errorLog.LogCustomSmsFailTsv(_custSmsFailDir, _smsHeadFormat, toNumber, smsContent, accountNo, "Invalid Mobile Number");
                }
            }
            catch (Exception e)
            {
                _errorLog.LogError(e.Message).Wait();
            }
        }

        public async Task SendSmsAsync(string smsContent, string toNumber, string smsOperator, string accountNo)
        {
            try
            {
                if (_configuration.GetSection("SSL_SMS").GetSection("enable").Value.ToUpper() == "TRUE")
                {
                    await SslApiHttpClientAsync(smsContent, toNumber, accountNo);
                }
                else
                {
                    string error = string.Empty;
                    string response = string.Empty;

                    switch (smsOperator)
                    {
                        case "Robi":
                            {
                                RobiApi(smsContent, toNumber, accountNo, out error, out response);
                                break;
                            }

                        case "Banglalink":
                            {
                                BanglalinkApi(smsContent, toNumber, accountNo, out response);
                                break;
                            }

                        default:
                            {
                                DefaultApi(smsContent, toNumber, accountNo, out error, out response);
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                _errorLog.LogError(e.Message).Wait();
            }
        }

    }
}
