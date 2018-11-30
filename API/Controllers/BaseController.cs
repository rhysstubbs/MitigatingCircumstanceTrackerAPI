using Microsoft.AspNetCore.Mvc;

namespace RESTAPI.Controllers
{
    [ApiController]
    [Route("/test")]
    public class BaseController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}