using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MasterWebAPI.Controllers
{
    [ApiController]
    [Route("api/demo")]     
    public class DemoController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("hello")]
        public IActionResult GetHello()
        {
            return Ok("Hello, World!");
        }        
    }
}