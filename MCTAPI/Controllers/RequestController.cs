using Microsoft.AspNetCore.Mvc;

namespace RESTAPI.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class RequestController : BaseController
    {
        [HttpGet("ByUserId/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetByUserId(int id)
        {
            return Ok();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Get(int id)
        {
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public ActionResult Post()
        {
            return Ok();
        }

        [HttpPut]
        public ActionResult Put()
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult Delete(int id)
        {
            return Ok();
        }
    }
}