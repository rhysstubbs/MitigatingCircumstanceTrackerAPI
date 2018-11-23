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
    public class RequestController : BaseController
    {
        private readonly DatastoreDb datastore;
        private readonly KeyFactory keyFactory;

        public RequestController(IConfiguration configuration)
        {
            this.datastore = DatastoreDb.Create(configuration["GAE:ProjectId"]);
            this.keyFactory = datastore.CreateKeyFactory(Kind.Request.ToString());
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public IActionResult Get()
        {
            Query query = new Query(Kind.Request.ToString());
            DatastoreQueryResults results = this.datastore.RunQuery(query);

            if (!results.Entities.Any())
            {
                return NoContent();
            }

            return Ok(results.Entities.Select(x => x.ToRequest()));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult Get(long id)
        {
            Key key = new Key().WithElement(Kind.Request.ToString(), id);
            Entity request = this.datastore.Lookup(key);

            if (request == null)
            {
                return NotFound(id);
            }

            return Ok(request.ToRequest());
        }

        [HttpGet("ForUser/{username}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public IActionResult GetRequestsForUser(string username)
        {
            Query query = new Query(Kind.Request.ToString());
            Key userKey = new Key().WithElement(Kind.User.ToString(), username);

            if (userKey != null)
            {
                query.Filter = Filter.Equal("owner", userKey);
            }

            DatastoreQueryResults results = this.datastore.RunQuery(query);

            if (!results.Entities.Any())
            {
                return NoContent();
            }

            return Ok(results.Entities.Select(x => x.ToRequest()));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        public IActionResult Post([FromBody] JSONRequest request)
        {
            if (request == null)
            {
                return BadRequest("A request object must be present");
            }
            else if (request.Owner == null)
            {
                return BadRequest("A user key is required as part of the request object");
            }

            //var keys = request.Subjects.Select(key => new Key().WithElement(Kind.Subject.ToString(), key.ToLower()));
            //Key[] keyArray = keys.ToArray();

            Entity requestEntity = new Entity
            {
                Key = this.keyFactory.CreateIncompleteKey(),
                ["owner"] = new Key().WithElement(Kind.User.ToString(), request.Owner),
                ["status"] = RequestStatus.Submitted.ToString(),
                ["dateSubmitted"] = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                ["description"] = request.Description,
                //["subjects"] = new ArrayValue()
            };

            CommitResponse commitResponse;
            using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
            {
                try
                {
                    transaction.Insert(requestEntity);
                    commitResponse = transaction.Commit();
                }
                catch (Exception exception)
                {
                    return StatusCode(500, exception);
                }
            }

            if (commitResponse.MutationResults.Any())
            {
                var completeKey = commitResponse.MutationResults.First().Key;
                requestEntity.Key = completeKey;
            }

            return Ok(requestEntity.ToRequest());
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public IActionResult Patch(long id, [FromBody] Request request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            if (request.Id == 0)
            {
                request.Id = id;
            }

            Entity requestEntity = new Entity()
            {
                Key = new Key().WithElement(Kind.Request.ToString(), request.Id),
                ["description"] = request.Description ?? null,
                ["status"] = request.Status.ToString() ?? null
            };

            CommitResponse commitResponse;
            using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
            {
                try
                {
                    transaction.Update(requestEntity);
                    commitResponse = transaction.Commit();
                }
                catch (Exception exception)
                {
                    return StatusCode(500, exception);
                }
            }

            return Ok(requestEntity.ToRequest());
        }

        [HttpPatch("{id}/MarkAs/{status}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult PatchSetStatus(long id, RequestStatus status)
        {
            if (id == 0)
            {
                return BadRequest("Datastore Ids can not have an id of 0 (zero).");
            }

            Key key = new Key().WithElement(Kind.Request.ToString(), id);
            Entity currentRequest = this.datastore.Lookup(key);

            if (currentRequest == null)
            {
                return NotFound(key.ToString());
            }
            else
            {
                currentRequest["status"] = status.ToString();
            }

            CommitResponse commitResponse;
            using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
            {
                try
                {
                    transaction.Update(currentRequest);
                    commitResponse = transaction.Commit();
                }
                catch (Exception exception)
                {
                    return StatusCode(500, exception);
                }
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(202)]
        [ProducesResponseType(500)]
        public IActionResult Delete(long id)
        {
            Key key = new Key().WithElement(Kind.Request.ToString(), id);

            try
            {
                using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
                {
                    transaction.Delete(key);
                }
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception);
            }

            return Accepted();
        }
    }
}