using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using MCT.RESTAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NotificationProvider.Interfaces;
using NotificationProvider.Models.Notifications;
using System;
using System.Linq;
using System.Net.Mail;

namespace RESTAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        #region Properties

        private readonly DatastoreDb datastore;
        private readonly IConfiguration configuration;
        private readonly INotificationService notificationService;

        #endregion Properties

        #region Constructor

        public UserController(IConfiguration configuration, INotificationService notificationService)
        {
            this.configuration = configuration;
            this.datastore = DatastoreDb.Create(configuration["GAE:projectId"]);
            this.notificationService = notificationService;
        }

        #endregion Constructor

        #region Methods

        [HttpGet("{username}/exists")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUserExists(string username)
        {
            Query query = new Query(EntityKind.User.ToString())
            {
                Filter = Filter.Equal("username", username),
                Projection = { "__key__" }
            };

            DatastoreQueryResults result = this.datastore.RunQuery(query);

            if (!result.Entities.Any())
            {
                return NotFound(username);
            }

            return Ok(result.Entities.First());
        }

        [HttpGet("confirm")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetValidateConfirmedUser([FromQuery] string token)
        {
            var decodedToken = Uri.UnescapeDataString(token);

            Query query = new Query(EntityKind.Confirmation.ToString())
            {
                Filter = Filter.Equal("token", decodedToken)
            };

            DatastoreQueryResults result = this.datastore.RunQuery(query);

            if (!result.Entities.Any())
            {
                return NotFound(token);
            }

            Entity confEntity = result.Entities.First();
            var createdAt = DateTime.ParseExact(confEntity["created_at"].StringValue, "yyyyMMddHHmmssfff", System.Globalization.CultureInfo.InstalledUICulture);

            if (createdAt < DateTime.Now.AddHours(-24))
            {
                return BadRequest("Expired");
            }

            var keyFactory = this.datastore.CreateKeyFactory(EntityKind.User.ToString());

            var username = confEntity["user"].StringValue;
            var isAdmin = username.ToCharArray().First() != 'i';

            // Create the user account
            Entity newUser = new Entity()
            {
                Key = keyFactory.CreateIncompleteKey(),
                ["username"] = username,
                ["isAdmin"] = isAdmin
            };

            CommitResponse response;
            using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
            {
                transaction.Insert(newUser);
                transaction.Delete(confEntity);

                response = transaction.Commit();
            }

            if (response.MutationResults.Count != 2)
            {
                return StatusCode(500);
            }

            return Ok();
        }

        [HttpPost("{username}/confirm")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult PostConfirmUser(string username)
        {
            var emailAddress = new MailAddress($"{username}@bournemouth.ac.uk");

            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var kf = this.datastore.CreateKeyFactory(EntityKind.Confirmation.ToString());

            Entity confirmationEntity = new Entity()
            {
                Key = kf.CreateIncompleteKey(),
                ["token"] = token,
                ["created_at"] = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                ["user"] = username
            };

            CommitResponse commitResponse;
            using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
            {
                try
                {
                    transaction.Insert(confirmationEntity);
                    commitResponse = transaction.Commit();
                }
                catch (Exception exception)
                {
                    return StatusCode(500, exception);
                }
            }

            var uriBuilder = new UriBuilder()
            {
                Scheme = this.Request.IsHttps ? "https" : "http",
                Host = this.Request.Host.ToString(),
                Path = $"login/confirm/{ Uri.EscapeDataString(token)}"
            };

            var mail = new EmailNotification(emailAddress.ToString(), "Account Confirmation", $"Click the link to confirm your account - {uriBuilder.ToString()}");
            this.notificationService.PushAsync(mail);

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult PostCreateUser([FromBody] User user)
        {
            string entityName = user.Username;
            var keyFactory = this.datastore.CreateKeyFactory(EntityKind.User.ToString());
            Key key = keyFactory.CreateKey(entityName);

            Entity userEntity = new Entity
            {
                Key = key,
                ["username"] = user.Username,
                ["password"] = user.Password,
                ["firstname"] = user.Firstname,
                ["lastname"] = user.Lastname,
                ["isAdmin"] = user.IsAdmin
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
                    return StatusCode(500, exception);
                }
            }

            return Ok();
        }

        #endregion Methods
    }
}