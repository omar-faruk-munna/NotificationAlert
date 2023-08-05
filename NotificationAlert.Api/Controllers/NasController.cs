using Microsoft.AspNetCore.Mvc;
using NotificationAlert.Api.Repositories;
using System;
using System.Threading.Tasks;

namespace NotificationAlert.Api.Controllers
{
    [Route("api/nas")]
    [ApiController]
    public class NasController : ControllerBase
    {
        private readonly INasRepo _nasRepo;
        private readonly ILogRepository _errorLog;
        private readonly IUltimusConString _ultimusCon;

        public NasController(INasRepo nasRepo, ILogRepository errorLog, IUltimusConString ultimusCon)
        {
            this._nasRepo = nasRepo;
            this._errorLog = errorLog;
            this._ultimusCon = ultimusCon;
        }

        [HttpGet]
        public async Task<IActionResult> NTask()
        {
            try
            {
                string _dbCon = await _ultimusCon.GetUltimusConString();

                while (true)
                {
                    await _nasRepo.NasAsync(_dbCon);
                    await Task.Delay(10000);
                }
            }
            catch (Exception e)
            {
                _errorLog.LogError(e.Message).Wait();
            }

            return Ok();
        }

    }
}
