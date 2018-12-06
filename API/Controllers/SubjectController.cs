using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RESTAPI.Controllers;
using System;

namespace MCT.RESTAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class SubjectController : BaseController
    {
        private readonly DatastoreDb datastore;

        public SubjectController(IConfiguration configuration)
        {
            this.datastore = DatastoreDb.Create(configuration["GAE:ProjectId"]);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        public IActionResult Post([FromBody] string subject)
        {
            var keyFactory = this.datastore.CreateKeyFactory(EntityKind.Subject.ToString());

            Entity entity = new Entity()
            {
                Key = keyFactory.CreateIncompleteKey(),
                ["title"] = subject
            };

            CommitResponse commitResponse;

            using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
            {
                try
                {
                    transaction.Insert(entity);
                    commitResponse = transaction.Commit();
                }
                catch (Exception exception)
                {
                    return StatusCode(500, exception);
                }
            }

            return Ok();
        }
    }
}