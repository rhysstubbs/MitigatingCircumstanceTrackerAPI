using MCT.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MCT.RESTAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class SubjectController : ControllerBase
    {
        private ISubjectRepository subjectRepository;

        public SubjectController(ISubjectRepository subjectRepository)
        {
            this.subjectRepository = subjectRepository;
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
            var result = this.subjectRepository.Get(id);
            if (result == null)
            {
                return NotFound(id);
            }

            return Ok(result);
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