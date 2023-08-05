using LogLog4Net;
using Microsoft.Extensions.Configuration;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Sms
{
    public class SmsRepository : ISmsRepository
    {
        private readonly ILoggerRepository _error;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
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
        private readonly string _sslName;
        private readonly string _infobipName;
        private readonly string _metrotelName;
        private readonly string _robiName;
        private readonly string _blName;
        private readonly string _gpName;
        private readonly string _uflName;


        public SmsRepository(ILoggerRepository error, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            this._error = error;
            this._configuration = configuration;
            this._clientFactory = clientFactory;
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
            _sslName = _configuration.GetValue<string>("Client:SslName");
            _infobipName = _configuration.GetValue<string>("Client:InfobipName");
            _metrotelName = _configuration.GetValue<string>("Client:MetrotelName");
            _robiName = _configuration.GetValue<string>("Client:RobiName");
            _blName = _configuration.GetValue<string>("Client:BlName");
            _gpName = _configuration.GetValue<string>("Client:GpName");
            _uflName = _configuration.GetValue<string>("Client:UflName");
        }

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
                this._error.LogError("Sms", "SmsSend", e.Message);
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
                HttpClient client = _clientFactory.CreateClient(_sslName);
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
                this._error.LogError("Sms", "SslApiAsync", e.Message);
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
                HttpClient client = _clientFactory.CreateClient(_infobipName);
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
                this._error.LogError("Sms", "InfobipApiAsync", e.Message);
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
                HttpClient client = _clientFactory.CreateClient(_metrotelName);
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
                this._error.LogError("Sms", "MetrotelApiAsync", e.Message);
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

                HttpClient client = _clientFactory.CreateClient(_robiName);
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
                this._error.LogError("Sms", "RobiApiAsync", e.Message);
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
                HttpClient client = _clientFactory.CreateClient(_blName);
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
                this._error.LogError("Sms", "BanglalinkApiAsync", e.Message);
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
                HttpClient client = _clientFactory.CreateClient(_gpName);
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
                this._error.LogError("Sms", "GrameenPhoneApiAsync", e.Message);
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
                HttpClient client = _clientFactory.CreateClient(_uflName);
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
                this._error.LogError("Sms", "UflApiAsync", e.Message);
                resp.IsSuccess = "0";
                resp.SmsProiderName = "UFL";
                resp.ReasonForFail = e.Message;
                resp.SmsResponseTime = DateTime.Now.ToString();
                resp.SmsProiderId = "";

                return resp;
            }
        }
    }
}
