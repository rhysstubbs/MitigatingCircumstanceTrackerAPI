using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using MCT.RESTAPI.Extensions;
using MCT.RESTAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace RESTAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatastoreDb datastore;
        private readonly KeyFactory keyFactory;
        private readonly IConfiguration configuration;

        public UserController(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.datastore = DatastoreDb.Create(configuration["GAE:projectId"]);
            this.keyFactory = this.datastore.CreateKeyFactory(Kind.User.ToString());
        }

        [HttpGet("exists/{username}")]
        public IActionResult GetUserExists(string username)
        {
            Query query = new Query(Kind.User.ToString())
            {
                Filter = Filter.And(Filter.Equal("username", username))
            };

            DatastoreQueryResults result = this.datastore.RunQuery(query);

            if (!result.Entities.Any())
            {
                return NotFound(username);
            }

            return Ok(result.Entities.Select(u => u.ToMinimalUser()).FirstOrDefault());
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public ActionResult PostCreateUser([FromBody] User user)
        {
            string entityName = user.Username;
            Key key = this.keyFactory.CreateKey(entityName);

            Entity userEntity = new Entity
            {
                Key = key,
                ["username"] = user.Username,
                ["password"] = user.Password,
                ["firstname"] = user.Firstname,
                ["lastname"] = user.Lastname
            };

            using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
            {
                try
                {
                    transaction.Insert(userEntity);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    return StatusCode(500);
                }
            }

            return Ok();
        }
    }
}