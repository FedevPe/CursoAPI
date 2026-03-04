using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/environment")]
    public class EnvironmentController(
        IConfiguration conf,
        IWebHostEnvironment env) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("ambiente")]
        public IActionResult GetEnvironment()
        {
            var mensaje = conf.GetValue<string>("Ambiente");
            var ambiente = env.EnvironmentName;
            return Ok(new { Ambiente = ambiente, Mensaje = mensaje });
        }
    }
}