using DataAccess.DataTransferObjects;
using DataAccess.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using RESTAPI.Models.JSON;

namespace RESTAPI.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class RequestController : BaseController
    {
        private readonly IRequestRepository requestRepository;

        public RequestController(IRequestRepository requestRepository)
        {
            this.requestRepository = requestRepository;
        }

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
            var result = this.requestRepository.Get(id);
            if (result == null)
            {
                return NotFound(id);
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public ActionResult Post([FromBody] Request request)
        {
            if (request == null)
            {
                return BadRequest(request);
            }

            var requestDTO = new RequestDTO(request);
            var result = this.requestRepository.Insert(requestDTO);
            if (result == null)
            {
                return StatusCode(500);
            }

            return CreatedAtAction(nameof(Get), new { id = result.RequestId }, result);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Request request)
        {

            if (request == null)
            {
                return BadRequest();
            }

            request.RequestId = id;
            var requestDTO = new RequestDTO(request);
            var result = this.requestRepository.Update(requestDTO);
            if (result == null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult Delete(int id)
        {
            var result = this.requestRepository.Delete(id);
            if (!result)
            {
                StatusCode(500);
            }

            return Ok();
        }
    }
}