using Microsoft.AspNetCore.Mvc;

namespace MCT.RESTAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    internal class SubjectController : ControllerBase
    {
        public SubjectController()
        {
        }

        /// <summary>
        /// Retrieves a specific subject by ID.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult Get(int id)
        {
            return Ok();
        }

        /// <summary>
        /// Retrieves a specific subject by user ID.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("ByUserId/id")]
        public IActionResult GetByUserId(int id)
        {
            return Ok();
        }
    }
}