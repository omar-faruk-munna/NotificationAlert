using Microsoft.AspNetCore.Mvc;
using NotificationAlert.Api.Models;
using NotificationAlert.Api.Repositories;
using System;
using System.Threading.Tasks;

namespace NotificationAlert.Api.Controllers
{
    [Route("api/custom-sms")]
    [ApiController]
    public class CustomSmsController : ControllerBase
    {
        private readonly ICustomSmsRepository _repository;
        private readonly ILogRepository _errorLog;
        private readonly ISmsRepository sms;

        public CustomSmsController(ICustomSmsRepository repository, ILogRepository errorLog, ISmsRepository sms)
        {
            this._repository = repository;
            this._errorLog = errorLog;
            this.sms = sms;
        }

        [HttpPost]
        public async Task<IActionResult> Post(SmsModelWrap model)
        {
            try
            {
                await _repository.CustomSmsSendAsync(model);
            }
            catch (Exception e)
            {
                _errorLog.LogError(e.Message).Wait();
            }

            return Ok("ok");
        }

    }

}
