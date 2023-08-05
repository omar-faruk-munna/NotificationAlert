using Microsoft.Extensions.Configuration;
using Model;
using Sms;
using Helper;
using NotificationAlertCustom.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationAlertCustom.Services
{
    public class CustomSmsService : ICustomSmsRepository
    {
        //#region Fields
        //private readonly IConfiguration _configuration;
        //private readonly ILogRepository _errorLog;
        //private readonly ISmsRepository _smsRepo;
        //private readonly IEmailRepository _email;
        //private readonly IDbRepository dbRepository;
        //private readonly ISmsRepository _sms;
        //private readonly IHelperRepository _helper;
        //private readonly string _bankName;
        //#endregion

        //public CustomSmsService(IConfiguration configuration, ILogRepository errorLog, ISmsRepository smsRepo, IEmailRepository email, IDbRepository dbRepository, ISmsRepository sms, IHelperRepository helper)
        //{
        //    _configuration = configuration;
        //    _errorLog = errorLog;
        //    _smsRepo = smsRepo;
        //    _email = email;
        //    this.dbRepository = dbRepository;
        //    this._sms = sms;
        //    this._helper = helper;
        //    _bankName = _configuration.GetSection("BankName").Value;
        //}

        //public async Task<RootResponse<SmsResponse>> SmsSendSingleAsync(CustomSingleSmsRequest model)
        //{
        //    try
        //    {
        //        //ufl: 2030, 12, 30
        //        //ubl: 
        //        //mmbl: 2021, 4, 30
        //        //mbl: 2021, 07, 07
        //        //lbfl: 2021, 07, 07
        //        //dbl: 
        //        //sebl: 
        //        //fsibl: 2021, 05, 31
        //        //Cityzen: 
        //        //probashi: 

        //        //if (DateTime.Now.Ticks > new DateTime(2021, 05, 31).Ticks)
        //        //{
        //        //    _errorLog.LogLicenseExpired("Custom License Expired on 2021, 05, 31");
        //        //    return;
        //        //}

        //        //RootResponse<SmsResponse> resp = new RootResponse<SmsResponse>();

        //        string toNumber = "";

        //        if (_appMood.Equals("UAT"))
        //        {
        //            toNumber = _toNumber;
        //        }


        //        #region Phone Number Check
        //        toNumber = this._helper.GetValidNumber(model.ToNumber, _numberPrefix);
        //        List<SmsResponse> smsResps = new List<SmsResponse>();

        //        if (string.IsNullOrWhiteSpace(toNumber))
        //        {
        //            SmsResponse smsResp = new SmsResponse();
        //            smsResp.IsSuccess = 1;
        //            smsResp.ReasonForFail = "Invalid Phone Number";
        //            smsResp.SmsResponseTime = DateTime.Now.ToString();
        //            smsResps.Add(smsResp);
        //            var resp = new RootResponse<SmsResponse>()
        //            {
        //                Status = "OK",
        //                Message = smsResp.ReasonForFail,
        //                Results = smsResps
        //            };

        //            return resp;
        //        }
        //        #endregion

        //        var smsResp = await _sms.SmsSend(model.IsBanglaContent, model.SmsOperator, model.ToNumber, model.SmsContent);



        //        //SmsSend






        //    }
        //    catch (Exception e)
        //    {
        //        _errorLog.LogError(e.Message);
        //    }
        //}

        //public async Task<RootResponse<Result>> SendOtpAlert(OtpRequest model)
        //{
        //    SmsResponse sms = new SmsResponse();
        //    EmailResponse email = new EmailResponse();

        //    try
        //    {
        //        int emailRespFlag = 0;
        //        string emailReasonForFail = "";

        //        switch (_bankName.ToUpper())
        //        {
        //            case "LBFL"://lanka bangla - robi api
        //                if (model.IsSms == 1)
        //                {
        //                    var smsRes = await _smsRepo.SendOtpVendor(model.SmsContent, model.ToNumber, "", "");
        //                    sms.IsSmsSuccess = smsRes.Item1;
        //                    sms.SmsReasonForFail = smsRes.Item2;
        //                    sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                if (model.IsEmail == 1)
        //                {
        //                    _email.SendOtpEmail(model.IsHtmlBody, model.ToEmail, model.EmailSubject, model.EmailBody, out emailRespFlag, out emailReasonForFail);
        //                    email.IsEmailSuccess = emailRespFlag;
        //                    email.EmailReasonForFail = emailReasonForFail;
        //                    email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                break;
        //            case "MMBL"://modhu moti -ssl
        //                if (model.IsSms == 1)
        //                {
        //                    var smsRes = await _smsRepo.SendOtpVendor(model.SmsContent, model.ToNumber, "", "");
        //                    sms.IsSmsSuccess = smsRes.Item1;
        //                    sms.SmsReasonForFail = smsRes.Item2;
        //                    sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                if (model.IsEmail == 1)
        //                {
        //                    _email.SendOtpEmail(model.IsHtmlBody, model.ToEmail, model.EmailSubject, model.EmailBody, out emailRespFlag, out emailReasonForFail);
        //                    email.IsEmailSuccess = emailRespFlag;
        //                    email.EmailReasonForFail = emailReasonForFail;
        //                    email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                break;
        //            case "UFL"://united finance -self api
        //                if (model.IsSms == 1)
        //                {
        //                    var smsRes = await _smsRepo.SendOtpVendor(model.SmsContent, model.ToNumber, "", "");
        //                    sms.IsSmsSuccess = smsRes.Item1;
        //                    sms.SmsReasonForFail = smsRes.Item2;
        //                    sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                if (model.IsEmail == 1)
        //                {
        //                    _email.SendOtpEmail(model.IsHtmlBody, model.ToEmail, model.EmailSubject, model.EmailBody, out emailRespFlag, out emailReasonForFail);
        //                    email.IsEmailSuccess = emailRespFlag;
        //                    email.EmailReasonForFail = emailReasonForFail;
        //                    email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                break;
        //            case "UBL"://Uttara bank -ssl
        //                if (model.IsSms == 1)
        //                {
        //                    var smsRes = await _smsRepo.SendOtpVendor(model.SmsContent, model.ToNumber, "", "");
        //                    sms.IsSmsSuccess = smsRes.Item1;
        //                    sms.SmsReasonForFail = smsRes.Item2;
        //                    sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                if (model.IsEmail == 1)
        //                {
        //                    _email.SendOtpEmail(model.IsHtmlBody, model.ToEmail, model.EmailSubject, model.EmailBody, out emailRespFlag, out emailReasonForFail);
        //                    email.IsEmailSuccess = emailRespFlag;
        //                    email.EmailReasonForFail = emailReasonForFail;
        //                    email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                break;
        //            case "MBL"://Meghna Bank -ssl
        //                if (model.IsSms == 1)
        //                {
        //                    var smsRes = await _smsRepo.SendOtpVendor(model.SmsContent, model.ToNumber, "", "");
        //                    sms.IsSmsSuccess = smsRes.Item1;
        //                    sms.SmsReasonForFail = smsRes.Item2;
        //                    sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                if (model.IsEmail == 1)
        //                {
        //                    _email.SendOtpEmail(model.IsHtmlBody, model.ToEmail, model.EmailSubject, model.EmailBody, out emailRespFlag, out emailReasonForFail);
        //                    email.IsEmailSuccess = emailRespFlag;
        //                    email.EmailReasonForFail = emailReasonForFail;
        //                    email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                break;
        //            case "FSIBL"://First Security -infobip
        //                if (model.IsSms == 1)
        //                {
        //                    var smsRes = await _smsRepo.SendOtpVendor(model.SmsContent, model.ToNumber, "", "");
        //                    sms.IsSmsSuccess = smsRes.Item1;
        //                    sms.SmsReasonForFail = smsRes.Item2;
        //                    sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                if (model.IsEmail == 1)
        //                {
        //                    _email.SendOtpEmail(model.IsHtmlBody, model.ToEmail, model.EmailSubject, model.EmailBody, out emailRespFlag, out emailReasonForFail);
        //                    email.IsEmailSuccess = emailRespFlag;
        //                    email.EmailReasonForFail = emailReasonForFail;
        //                    email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                break;
        //            case "CZBL"://City Zen -metrotel
        //                if (model.IsSms == 1)
        //                {
        //                    var smsRes = await _smsRepo.SendOtpVendor(model.SmsContent, model.ToNumber, "", "");
        //                    sms.IsSmsSuccess = smsRes.Item1;
        //                    sms.SmsReasonForFail = smsRes.Item2;
        //                    sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                if (model.IsEmail == 1)
        //                {
        //                    _email.SendOtpEmail(model.IsHtmlBody, model.ToEmail, model.EmailSubject, model.EmailBody, out emailRespFlag, out emailReasonForFail);
        //                    email.IsEmailSuccess = emailRespFlag;
        //                    email.EmailReasonForFail = emailReasonForFail;
        //                    email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                }

        //                break;
        //            default:
        //                break;
        //        }

        //        if (sms.IsSmsSuccess.Equals(1) || email.IsEmailSuccess.Equals(1))
        //        {
        //            //------------insert db
        //            await dbRepository.CreateRecord(901, "", "", "", "", model.IsSms, model.ToNumber, model.SmsContent, sms.IsSmsSuccess, sms.SmsReasonForFail, model.IsEmail, model.ToEmail, model.EmailSubject, model.EmailBody, email.IsEmailSuccess, email.EmailReasonForFail, 1, 1);

        //            Result result = new Result()
        //            {
        //                SmsResponse = sms,
        //                EmailResponse = email
        //            };

        //            List<Result> results = new List<Result>();
        //            results.Add(result);

        //            RootResponse<Result> rootResponse = new RootResponse<Result>()
        //            {
        //                Results = results,
        //                Status = "1",
        //                Message = ""
        //            };

        //            return rootResponse;
        //        }
        //        else
        //        {
        //            Result result = new Result()
        //            {
        //                SmsResponse = sms,
        //                EmailResponse = email
        //            };

        //            List<Result> results = new List<Result>();
        //            results.Add(result);

        //            RootResponse<Result> rootResponse = new RootResponse<Result>()
        //            {
        //                Results = results,
        //                Status = "0",
        //                Message = "Network Busy, Please try again."
        //            };

        //            return rootResponse;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _errorLog.LogError(e.Message);

        //        sms.IsSmsSuccess = 0;
        //        sms.SmsReasonForFail = "Network Busy, Please try again.";
        //        sms.SmsSentTime = "";
        //        email.IsEmailSuccess = 0;
        //        email.EmailReasonForFail = "Network Busy, Please try again.";
        //        email.EmailSentTime = "";

        //        Result result = new Result()
        //        {
        //            SmsResponse = sms,
        //            EmailResponse = email
        //        };

        //        List<Result> results = new List<Result>();
        //        results.Add(result);

        //        RootResponse<Result> rootResponse = new RootResponse<Result>()
        //        {
        //            Results = results,
        //            Status = "0",
        //            Message = "Network Busy, Please try again."
        //        };

        //        return rootResponse;
        //    }
        //}

        //public async Task<RootResponse<Result>> SendCustomAlert(AlertRequest request)
        //{
        //    string status = "0";
        //    SmsResponse sms = new SmsResponse();
        //    EmailResponse email = new EmailResponse();
        //    List<Result> results = new List<Result>();
        //    RootResponse<Result> rootResponse = new RootResponse<Result>();

        //    try
        //    {
        //        IEnumerable<dynamic> result = dbRepository.GetCustomerContact(request.IsMultiple, request.BranchId, request.AccountNo);

        //        result = result?.Cast<IDictionary<string, object>>();

        //        if (result != null)
        //        {
        //            var options = new ParallelOptions() { MaxDegreeOfParallelism = 4 };

        //            Parallel.ForEach(result, options, async rows =>
        //            {
        //                if (rows is IDictionary<string, object> fields)
        //                {
        //                    //OtpRes res = new OtpRes();
        //                    //res.SentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                    int emailRespFlag = 0;
        //                    string emailReasonForFail = "";

        //                    string customerId = Convert.ToString(fields["CUST_GROUP_ID"]).Trim();
        //                    string smsOperator = Convert.ToString(fields["MOBILE_OPERATOR"]);
        //                    string toNumber = Convert.ToString(fields["MOBILE_NUMBER"]);
        //                    string toEmail = Convert.ToString(fields["EMAIL_ID"]);

        //                    switch (_bankName)
        //                    {
        //                        case "LBFL"://lanka bangla
        //                            if (request.IsSms.Equals(1))
        //                            {
        //                                var smsRes = await _smsRepo.SendOtpVendor(request.SmsContent, toNumber, "", "");
        //                                sms.IsSmsSuccess = smsRes.Item1;
        //                                sms.SmsReasonForFail = smsRes.Item2;
        //                                sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            if (request.IsEmail.Equals(1))
        //                            {
        //                                _email.SendOtpEmail(request.IsHtmlBody, toEmail, request.EmailSubject, request.EmailContent, out emailRespFlag, out emailReasonForFail);
        //                                email.IsEmailSuccess = emailRespFlag;
        //                                email.EmailReasonForFail = emailReasonForFail;
        //                                email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            break;
        //                        case "MMBL"://modhu moti
        //                            if (request.IsSms.Equals(1))
        //                            {
        //                                var smsRes = await _smsRepo.SendOtpVendor(request.SmsContent, toNumber, "", "");
        //                                sms.IsSmsSuccess = smsRes.Item1;
        //                                sms.SmsReasonForFail = smsRes.Item2;
        //                                sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            if (request.IsEmail.Equals(1))
        //                            {
        //                                _email.SendOtpEmail(request.IsHtmlBody, toEmail, request.EmailSubject, request.EmailContent, out emailRespFlag, out emailReasonForFail);
        //                                email.IsEmailSuccess = emailRespFlag;
        //                                email.EmailReasonForFail = emailReasonForFail;
        //                                email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            break;
        //                        case "UFL"://united finance
        //                            if (request.IsSms.Equals(1))
        //                            {
        //                                var smsRes = await _smsRepo.SendOtpVendor(request.SmsContent, toNumber, "", "");
        //                                sms.IsSmsSuccess = smsRes.Item1;
        //                                sms.SmsReasonForFail = smsRes.Item2;
        //                                sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            if (request.IsEmail.Equals(1))
        //                            {
        //                                _email.SendOtpEmail(request.IsHtmlBody, toEmail, request.EmailSubject, request.EmailContent, out emailRespFlag, out emailReasonForFail);
        //                                email.IsEmailSuccess = emailRespFlag;
        //                                email.EmailReasonForFail = emailReasonForFail;
        //                                email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            break;
        //                        case "UBL"://uttara bank
        //                            if (request.IsSms.Equals(1))
        //                            {
        //                                var smsRes = await _smsRepo.SendOtpVendor(request.SmsContent, toNumber, "", "");
        //                                sms.IsSmsSuccess = smsRes.Item1;
        //                                sms.SmsReasonForFail = smsRes.Item2;
        //                                sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            if (request.IsEmail.Equals(1))
        //                            {
        //                                _email.SendOtpEmail(request.IsHtmlBody, toEmail, request.EmailSubject, request.EmailContent, out emailRespFlag, out emailReasonForFail);
        //                                email.IsEmailSuccess = emailRespFlag;
        //                                email.EmailReasonForFail = emailReasonForFail;
        //                                email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            break;
        //                        case "MBL"://Meghna Bank
        //                            if (request.IsSms.Equals(1))
        //                            {
        //                                var smsRes = await _smsRepo.SendOtpVendor(request.SmsContent, toNumber, "", "");
        //                                sms.IsSmsSuccess = smsRes.Item1;
        //                                sms.SmsReasonForFail = smsRes.Item2;
        //                                sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            if (request.IsEmail.Equals(1))
        //                            {
        //                                _email.SendOtpEmail(request.IsHtmlBody, toEmail, request.EmailSubject, request.EmailContent, out emailRespFlag, out emailReasonForFail);
        //                                email.IsEmailSuccess = emailRespFlag;
        //                                email.EmailReasonForFail = emailReasonForFail;
        //                                email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            break;
        //                        case "FSIBL"://First Security
        //                            if (request.IsSms.Equals(1))
        //                            {
        //                                var smsRes = await _smsRepo.SendOtpVendor(request.SmsContent, toNumber, "", "");
        //                                sms.IsSmsSuccess = smsRes.Item1;
        //                                sms.SmsReasonForFail = smsRes.Item2;
        //                                sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            if (request.IsEmail.Equals(1))
        //                            {
        //                                _email.SendOtpEmail(request.IsHtmlBody, toEmail, request.EmailSubject, request.EmailContent, out emailRespFlag, out emailReasonForFail);
        //                                email.IsEmailSuccess = emailRespFlag;
        //                                email.EmailReasonForFail = emailReasonForFail;
        //                                email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            break;
        //                        case "SEBL"://South East
        //                            if (request.IsSms.Equals(1))
        //                            {
        //                                var smsRes = await _smsRepo.SendOtpVendor(request.SmsContent, toNumber, "", "");
        //                                sms.IsSmsSuccess = smsRes.Item1;
        //                                sms.SmsReasonForFail = smsRes.Item2;
        //                                sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            if (request.IsEmail.Equals(1))
        //                            {
        //                                _email.SendOtpEmail(request.IsHtmlBody, toEmail, request.EmailSubject, request.EmailContent, out emailRespFlag, out emailReasonForFail);
        //                                email.IsEmailSuccess = emailRespFlag;
        //                                email.EmailReasonForFail = emailReasonForFail;
        //                                email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            break;
        //                        case "DBL"://Dhaka Bank
        //                            if (request.IsSms.Equals(1))
        //                            {
        //                                var smsRes = await _smsRepo.SendOtpVendor(request.SmsContent, toNumber, "", "");
        //                                sms.IsSmsSuccess = smsRes.Item1;
        //                                sms.SmsReasonForFail = smsRes.Item2;
        //                                sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            if (request.IsEmail.Equals(1))
        //                            {
        //                                _email.SendOtpEmail(request.IsHtmlBody, toEmail, request.EmailSubject, request.EmailContent, out emailRespFlag, out emailReasonForFail);
        //                                email.IsEmailSuccess = emailRespFlag;
        //                                email.EmailReasonForFail = emailReasonForFail;
        //                                email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            break;
        //                        case "CZBL"://Cityzenes Bank
        //                            if (request.IsSms.Equals(1))
        //                            {
        //                                var smsRes = await _smsRepo.SendOtpVendor(request.SmsContent, toNumber, "", "");
        //                                sms.IsSmsSuccess = smsRes.Item1;
        //                                sms.SmsReasonForFail = smsRes.Item2;
        //                                sms.SmsSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            if (request.IsEmail.Equals(1))
        //                            {
        //                                _email.SendOtpEmail(request.IsHtmlBody, toEmail, request.EmailSubject, request.EmailContent, out emailRespFlag, out emailReasonForFail);
        //                                email.IsEmailSuccess = emailRespFlag;
        //                                email.EmailReasonForFail = emailReasonForFail;
        //                                email.EmailSentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                            }

        //                            break;
        //                        default:
        //                            break;
        //                    }

        //                    #region Insert DB
        //                    if (sms.IsSmsSuccess.Equals(1) || email.IsEmailSuccess.Equals(1))
        //                    {
        //                        //------------insert db
        //                        await dbRepository.CreateRecord(902, "", request.BranchId, request.AccountNo, customerId, request.IsSms, toNumber, request.SmsContent, sms.IsSmsSuccess, sms.SmsReasonForFail, request.IsEmail, toEmail, request.EmailSubject, request.EmailContent, email.IsEmailSuccess, email.EmailReasonForFail, 1, 1);

        //                        Result result = new Result()
        //                        {
        //                            SmsResponse = sms,
        //                            EmailResponse = email
        //                        };

        //                        results.Add(result);
        //                        status = "1";
        //                    }
        //                    else
        //                    {
        //                        Result result = new Result()
        //                        {
        //                            SmsResponse = sms,
        //                            EmailResponse = email
        //                        };

        //                        results.Add(result);
        //                    }

        //                    #endregion

        //                }
        //            });
        //        }

        //        await Task.Delay(5);

        //        rootResponse.Results = results;
        //        rootResponse.Status = status;
        //        rootResponse.Message = "";

        //        return rootResponse;
        //    }
        //    catch (Exception e)
        //    {
        //        _errorLog.LogError(e.Message);

        //        sms.IsSmsSuccess = 0;
        //        sms.SmsReasonForFail = "Network Busy, Please try again.";
        //        sms.SmsSentTime = "";
        //        email.IsEmailSuccess = 0;
        //        email.EmailReasonForFail = "Network Busy, Please try again.";
        //        email.EmailSentTime = "";

        //        Result result = new Result()
        //        {
        //            SmsResponse = sms,
        //            EmailResponse = email
        //        };

        //        results.Add(result);

        //        rootResponse.Results = results;
        //        rootResponse.Status = "0";
        //        rootResponse.Message = "Network Busy, Please try again.";

        //        return rootResponse;
        //    }
        //}

        //public void test()
        //{
        //    OtpListResponse response = new OtpListResponse();
        //    List<OtpRes> otpList = new List<OtpRes>();
        //    string re = "";

        //    try
        //    {
        //        IEnumerable<dynamic> result = dbRepository.GetCustomerTest();

        //        result = result?.Cast<IDictionary<string, object>>();

        //        if (result != null)
        //        {
        //            var options = new ParallelOptions() { MaxDegreeOfParallelism = 4 };

        //            Parallel.ForEach(result, options, async rows =>
        //            {
        //                if (rows is IDictionary<string, object> fields)
        //                {
        //                    OtpRes res = new OtpRes();
        //                    res.SentTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        //                    int emailRespFlag = 0;
        //                    string emailReasonForFail = "";

        //                    //string customerId = Convert.ToString(fields["CUST_GROUP_ID"]).Trim();
        //                    //string smsOperator = Convert.ToString(fields["MOBILE_OPERATOR"]);
        //                    //string toNumber = Convert.ToString(fields["MOBILE_NUMBER"]);
        //                    //string toEmail = Convert.ToString(fields["EMAIL_ID"]);
        //                    string emailBody = Convert.ToString(fields["EMAIL_BODY"]);

        //                    re = emailBody;

        //                    _errorLog.LogError(re);

        //                }
        //            });
        //        }


        //    }
        //    catch (Exception e)
        //    {
        //        _errorLog.LogError(e.Message);

        //    }
        //}

    }
}
