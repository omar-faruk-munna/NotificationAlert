using Database;
using LogLog4Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;
using NotificationAlertCustom.Models;
using NotificationAlertCustom.Repositories;
using Sms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationAlertCustom.Controllers
{
    [Route("api/sms")]
    [ApiController]
    public class CustomSmsController : ControllerBase
    {
        private readonly ILoggerRepository _error;
        private readonly ISmsRepository _sms;
        private readonly IConfiguration _config;
        private readonly IDbRepository _db;

        public CustomSmsController(ILoggerRepository error, ISmsRepository sms, IConfiguration config, IDbRepository db)
        {
            this._error = error;
            this._sms = sms;
            this._config = config;
            this._db = db;
        }

        [HttpPost]
        [Route("single")]
        public async Task<IActionResult> SingleSms(SmsRequest m)
        {
            var res = new RootResponse<SmsResponse>();
            List<SmsResponse> results = new List<SmsResponse>();

            try
            {
                if (m == null)
                    return BadRequest();

                var result = await _sms.SmsSend(m.ContentType, m.Operator, m.ToNumber, m.SmsContent);

                if (string.Equals(result.IsSuccess, "1"))
                {
                    await _db.CreateRecord(Convert.ToInt32(m.MessageFunctionId), "", "", "", "", 1, m.ToNumber, m.SmsContent, Convert.ToInt32(result.IsSuccess), result.ReasonForFail, 0, "", "", "", 0, "", 1, 1);
                }

                results.Add(result);
                res.Status = "Success";
                res.Message = "SMS Send Successfully";
            }
            catch (Exception e)
            {
                _error.LogError("NotificationAlertCustom:CustomSmsController", "SingleSms", e.Message);
                res.Status = "Failed";
                res.Message = "Network Busy, Please try again.";
            }

            res.Results = results;
            return Ok(res);
        }

        [HttpPost]
        [Route("multiple")]
        public async Task<IActionResult> MultipleSms(MultipleSmsRequest m)
        {
            var res = new RootResponse<SmsResponse>();
            List<SmsResponse> results = new List<SmsResponse>();

            try
            {
                if (m == null)
                    return BadRequest();

                foreach (var i in m.SmsList)
                {
                    var result = await _sms.SmsSend(m.ContentType, i.Operator, i.ToNumber, m.SmsContent);

                    if (string.Equals(result.IsSuccess, "1"))
                    {
                        await _db.CreateRecord(Convert.ToInt32(m.MessageFunctionId), "", "", "", "", 1, i.ToNumber, m.SmsContent, Convert.ToInt32(result.IsSuccess), result.ReasonForFail, 0, "", "", "", 0, "", 1, 1);
                    }

                    results.Add(result);
                }

                res.Status = "Success";
                res.Message = "SMS Send Successfully";
            }
            catch (Exception e)
            {
                //return StatusCode(StatusCodes.Status500InternalServerError, "Network Busy, Please try again.");
                _error.LogError("NotificationAlertCustom:CustomSmsController", "MultipleSms", e.Message);
                res.Status = "Failed";
                res.Message = "Network Busy, Please try again.";
            }

            res.Results = results;
            return Ok(res);
        }

        [HttpPost]
        [Route("alert")]
        public async Task<IActionResult> SendAlert(SmsAlertRequest m)
        {
            var res = new RootResponse<SmsResponse>();
            List<SmsResponse> results = new List<SmsResponse>();

            try
            {
                if (m == null)
                    return BadRequest();

                IEnumerable<dynamic> dataResult = _db.GetCustomerContact(Convert.ToInt32(m.IsMultiple), m.BranchId, m.AccountNo);

                if (dataResult.Count() > 0)
                {
                    dataResult = dataResult?.Cast<IDictionary<string, object>>();
                    var options = new ParallelOptions() { MaxDegreeOfParallelism = 4 };

                    Parallel.ForEach(dataResult, options, async rows =>
                    {
                        if (rows is IDictionary<string, object> fields)
                        {
                            string customerId = Convert.ToString(fields["CUST_GROUP_ID"]).Trim();
                            string smsOperator = Convert.ToString(fields["MOBILE_OPERATOR"]);
                            string toNumber = Convert.ToString(fields["MOBILE_NUMBER"]);
                            string toEmail = Convert.ToString(fields["EMAIL_ID"]);

                            var result = await _sms.SmsSend(m.ContentType, smsOperator, toNumber, m.SmsContent);

                            if (string.Equals(result.IsSuccess, "1"))
                            {
                                await _db.CreateRecord(Convert.ToInt32(m.MessageFunctionId), "", "", "", customerId, 1, toNumber, m.SmsContent, Convert.ToInt32(result.IsSuccess), result.ReasonForFail, 0, "", "", "", 0, "", 1, 1);
                            }

                            results.Add(result);
                        }
                    });

                    res.Status = "Success";
                    res.Message = "SMS Send Successfully";
                    await Task.Delay(1);
                }
                else
                {
                    res.Status = "Success";
                    res.Message = "No Data Found.";
                }
            }
            catch (Exception e)
            {
                _error.LogError("NotificationAlertCustom:CustomSmsController", "SendAlert", e.Message);
                res.Status = "Failed";
                res.Message = "Network Busy, Please try again.";
            }

            res.Results = results;
            return Ok(res);
        }

        [HttpGet]
        public IActionResult Test()
        {
            //for (int i = 0; i < 1000; i++)
            //{
            //    _errorLog.SmsSendCountMonthly("","");
            //}


            return Ok(new { Result = "API Started !!!" });
        }
    }
}
