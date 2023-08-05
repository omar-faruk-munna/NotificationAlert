using Microsoft.AspNetCore.Mvc;

namespace NotificationAlert.Api.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { result = "Api Run Successfully !!!" });
        }
    }
}
