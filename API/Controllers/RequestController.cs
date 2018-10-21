using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using MCT.RESTAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RESTAPI.Models.JSON;
using System;
using System.Linq;

namespace RESTAPI.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    internal class RequestController : BaseController
    {
        private readonly DatastoreDb datastore;
        private readonly KeyFactory keyFactory;
        private readonly IConfiguration configuration;

        public RequestController(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.datastore = DatastoreDb.Create(configuration["GAE:projectId"]);
            this.keyFactory = this.datastore.CreateKeyFactory(Kind.Request.ToString());
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public IActionResult Get()
        {
            Query query = new Query(Kind.Request.ToString());

            DatastoreQueryResults results = this.datastore.RunQuery(query);

            if (results.Entities.Count == 0)
            {
                return NoContent();
            }

            return Ok(results.Entities.Select(entity => entity.ToRequest()));
        }

        [HttpGet("{key}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult Get(Key key)
        {
            Entity request = this.datastore.Lookup(key);

            if (request == null)
            {
                return NotFound(key.ToString());
            }

            return Ok(request.ToRequest());
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public ActionResult Post([FromBody] Request request)
        {
            string now = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            string entityName = $"{request.Owner}_{now}";
            Key key = this.keyFactory.CreateKey(entityName);

            Entity requestEntity = new Entity
            {
                Key = key,
                ["owner"] = request.Owner,
                ["status"] = RequestStatus.Submitted.ToString(),
                ["dateSubmitted"] = now,
                ["description"] = request.Description
            };

            using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
            {
                try
                {
                    transaction.Upsert(requestEntity);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    return StatusCode(500);
                }
            }

            return Ok(requestEntity.ToRequest());
        }

        [HttpPatch("{key}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public ActionResult Patch(Key key, [FromBody] Request request)
        {
            Entity requestEntity = this.datastore.Lookup(key);

            if (request == null)
            {
                return NotFound(key.ToString());
            }
            else
            {
                requestEntity["status"] = request.Status.ToString();
                requestEntity["description"] = request.Description.ToString();

                try
                {
                    this.datastore.Update(requestEntity);
                }
                catch (Exception exception)
                {
                    return StatusCode(500);
                }
            }

            return Ok(requestEntity.ToRequest());
        }

        [HttpDelete("{key}")]
        [ProducesResponseType(202)]
        [ProducesResponseType(500)]
        public ActionResult Delete(Key key)
        {
            try
            {
                this.datastore.Delete(key);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return Accepted();
        }
    }
}