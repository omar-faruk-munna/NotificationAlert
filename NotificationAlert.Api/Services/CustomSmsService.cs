using Microsoft.Extensions.Configuration;
using NotificationAlert.Api.Models;
using NotificationAlert.Api.Repositories;
using Serilog;
using System;
using System.Threading.Tasks;

namespace NotificationAlert.Api.Services
{
    public class CustomSmsService : ICustomSmsRepository
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly ILogRepository _errorLog;
        private readonly ISmsRepository _smsRepo;
        private readonly string _bankName;
        #endregion

        public CustomSmsService(IConfiguration configuration, ILogRepository errorLog, ISmsRepository smsRepo)
        {
            this._configuration = configuration;
            this._errorLog = errorLog;
            this._smsRepo = smsRepo;
            this._bankName = _configuration.GetValue<string>("BankName");

        }

        public async Task CustomSmsSendAsync(SmsModelWrap model)
        {
            try
            {
                //this is for license
                //ufl:31-12-2030, mmbl:5-4-2021, lbfl:5-4-2021
                if (DateTime.Now.Ticks > new DateTime(2030, 12, 31).Ticks)
                {
                    _errorLog.LogLicenseExpired("Custom License Expired on 31-12-2030").Wait();
                    return;
                }

                switch (_bankName.ToUpper())
                {
                    case "LBFL"://lanka bangla - robi api
                        foreach (SmsModel customer in model.ListSmsModel)
                        {
                            if (!string.IsNullOrEmpty(customer.AccountNo) && !string.IsNullOrEmpty(customer.Number) && !string.IsNullOrEmpty(model.Message))
                            {
                                await _smsRepo.SendSmsAsync(model.Message, customer.Number, "", customer.AccountNo);
                            }
                        }
                        break;
                    case "MMBL"://modhu moti -ssl
                        foreach (SmsModel customer in model.ListSmsModel)
                        {
                            if (!string.IsNullOrEmpty(customer.AccountNo) && !string.IsNullOrEmpty(customer.Number) && !string.IsNullOrEmpty(model.Message))
                            {
                                //await _smsRepo.CustomSslApiHttpClientAsync(model.Message, customer.Number, customer.AccountNo);
                                await _smsRepo.CustomSslApiBothHttpClientAsync(model.Message, customer.Number, customer.AccountNo);
                            }
                        }
                        break;
                    case "UFL"://united finance -self api
                        foreach (SmsModel customer in model.ListSmsModel)
                        {
                            if (!string.IsNullOrEmpty(customer.AccountNo) && !string.IsNullOrEmpty(customer.Number) && !string.IsNullOrEmpty(model.Message))
                            {
                                await _smsRepo.CustomUflSmsSendApiAsync(model.Message, customer.Number, customer.AccountNo);
                            }
                        }
                        break;
                    case "UBL"://Uttara bank -ssl
                        foreach (SmsModel customer in model.ListSmsModel)
                        {
                            if (!string.IsNullOrEmpty(customer.AccountNo) && !string.IsNullOrEmpty(customer.Number) && !string.IsNullOrEmpty(model.Message))
                            {
                                await _smsRepo.CustomSslApiHttpClientAsync(model.Message, customer.Number, customer.AccountNo);
                            }
                        }
                        break;
                    case "MBL"://Meghna Bank -ssl
                        foreach (SmsModel customer in model.ListSmsModel)
                        {
                            if (!string.IsNullOrEmpty(customer.AccountNo) && !string.IsNullOrEmpty(customer.Number) && !string.IsNullOrEmpty(model.Message))
                            {
                                await _smsRepo.CustomSslApiHttpClientAsync(model.Message, customer.Number, customer.AccountNo);
                            }
                        }
                        break;
                    case "FSIBL"://First Security -ssl
                        foreach (SmsModel customer in model.ListSmsModel)
                        {
                            if (!string.IsNullOrEmpty(customer.AccountNo) && !string.IsNullOrEmpty(customer.Number) && !string.IsNullOrEmpty(model.Message))
                            {
                                await _smsRepo.CustomSslApiHttpClientAsync(model.Message, customer.Number, customer.AccountNo);
                            }
                        }
                        break;
                    default:
                        break;
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
