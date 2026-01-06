using Microsoft.AspNetCore.Mvc;
namespace MasterWebAPI.Controllers
{
    [ApiController]
    [Route("Demo")]     
    public class DemoController : ControllerBase
    {
        [HttpGet("hello")]
        public IActionResult GetHello()
        {
            return Ok("Hello, World!");
        }        
    }
}