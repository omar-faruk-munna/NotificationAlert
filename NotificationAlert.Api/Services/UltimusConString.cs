using LeadSoft.VASConn;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NotificationAlert.Api.Models;
using NotificationAlert.Api.Repositories;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NotificationAlert.Api.Services
{
    public class UltimusConString : IUltimusConString
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly string _endPoint;
        private readonly string _userId;
        private readonly string _password;
        private readonly string _connectionId;
        private readonly ILogRepository _errorLog;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _isEncryption;
        private readonly string _connectionString;
        #endregion

        public UltimusConString(IConfiguration configuration, ILogRepository errorLog, IHttpClientFactory clientFactory)
        {
            this._configuration = configuration;
            this._errorLog = errorLog;
            this._clientFactory = clientFactory;
            this._userId = _configuration.GetSection("User").GetSection("UserId").Value;
            this._password = _configuration.GetSection("User").GetSection("Password").Value;
            this._connectionId = _configuration.GetSection("User").GetSection("ConnectionId").Value;
            this._endPoint = _configuration.GetSection("User").GetSection("Uri").Value;
            this._isEncryption = _configuration.GetValue<string>("IsEncryption");
            this._connectionString = _configuration.GetSection("ConnectionStrings").GetSection("DbConnString").Value;
        }

        public async Task<string> GetUltimusConString()
        {
            string response = string.Empty;

            try
            {
                if (_isEncryption.ToUpper() == "TRUE")
                {
                    response = await GetUltimusCon();
                }
                else
                {
                    response = _connectionString;
                }
            }
            catch (Exception e)
            {
                _errorLog.LogError("Connection string Api Calling Exception").Wait();
                _errorLog.LogError(e.Message).Wait();
            }

            return response;
        }
        private async Task<string> GetUltimusCon()
        {
            string response = string.Empty;

            try
            {
                User model = new User
                {
                    UserId = _userId,
                    Password = _password,
                    ConnectionId = _connectionId
                };
                string jsonModel = JsonConvert.SerializeObject(model);

                ConnectionRequest req = new ConnectionRequest()
                {
                    BranchId = "",
                    FunctionId = "",
                    InstitueId = "",
                    RequestAppBaseUrl = "",
                    RequestAppIP = "",
                    RequestCliedAgent = "",
                    RequestCliedIP = "",
                    RequestDateTime = "",
                    RequestId = "",
                    SessionId = "",
                    UserId = "",
                    BusinessData = jsonModel,
                    SessionTimeout = 30
                };

                string jsonObject = JsonConvert.SerializeObject(req);
                StringContent data = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                HttpClient client = _clientFactory.CreateClient("BankClient");
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage result = await client.PostAsync(_endPoint, data);

                if (result.IsSuccessStatusCode)
                {
                    response = await result.Content.ReadAsStringAsync();
                    ConnectionResponse res = JsonConvert.DeserializeObject<ConnectionResponse>(response);
                    response = res.ResponseBusinessData;
                    response = VASCrypto.Decrypt(response);
                    DbConString dbConString = JsonConvert.DeserializeObject<DbConString>(response);
                    response = $"Data Source={dbConString.CONN_SCHEMA_NM};User id={dbConString.CONN_USER_ID};Password={dbConString.CONN_PASS_WORD};";
                }
                else
                {
                    _errorLog.LogError("Connection string not found").Wait();
                }
            }
            catch (Exception e)
            {
                _errorLog.LogError("Exception in connection string api calling").Wait();
                _errorLog.LogError(e.Message).Wait();
            }

            return response;
        }
    }
}
