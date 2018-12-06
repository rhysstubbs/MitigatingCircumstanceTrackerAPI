using Microsoft.AspNetCore.Mvc;

namespace RESTAPI.Controllers
{
    [ApiController]
    [Route("/")]
    public class BaseController : ControllerBase
    {
        [HttpGet("isLive")]
        [ProducesResponseType(200)]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}