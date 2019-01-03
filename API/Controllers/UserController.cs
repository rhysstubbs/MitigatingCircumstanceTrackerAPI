using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using MCT.RESTAPI.Extensions;
using MCT.RESTAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NotificationProvider.Interfaces;
using NotificationProvider.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace RESTAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class UserController : BaseController
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

        [HttpGet("{username}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUser(string username, [FromQuery] bool withRequests = false, [FromQuery] bool withNotifications = false)
        {
            Key userKey = new Key().WithElement(EntityKind.User.ToString(), username);
            Entity result = this.datastore.Lookup(userKey);

            if (result == null)
            {
                return NotFound(username);
            }

            var props = result.Properties;
            var user = new User()
            {
                Id = userKey,
                Username = result.Key.Path.First().Name,
                IsAdmin = props["isAdmin"].BooleanValue
            };

            if (withRequests)
            {
                Query requestQuery = new Query(EntityKind.Request.ToString())
                {
                    Filter = Filter.Equal("owner", userKey),
                    Order = { { "dateSubmitted", PropertyOrder.Types.Direction.Descending } },
                };

                DatastoreQueryResults requests = this.datastore.RunQuery(requestQuery);
                if (requests.Entities.Any())
                {
                    user.Requests = requests.Entities.Select(x => x.ToRequest()).ToList();
                }
            }

            if (withNotifications)
            {
                Query query = new Query(EntityKind.Notification.ToString())
                {
                    Filter = Filter.And(Filter.Equal("user", userKey), Filter.Equal("read", false))
                };

                DatastoreQueryResults results = this.datastore.RunQuery(query);
                if (results.Entities.Any())
                {
                    user.Notifications = results.Entities.Select(x => x.ToNotification()).ToList();
                }
            }

            return Ok(user);
        }

        [HttpGet("{username}/exists")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult GetUserExists(string username, [FromQuery] string email)
        {
            if (email == null)
            {
                return BadRequest("A requester email must be provided for authorisaton");
            }

            var userKey = new Key().WithElement(EntityKind.User.ToString(), username);
            Query userQuery = new Query(EntityKind.User.ToString())
            {
                Filter = Filter.Equal("__key__", userKey)
            };

            DatastoreQueryResults result = this.datastore.RunQuery(userQuery);

            List<string> emails = result.Entities.First().Properties["authorisedEmails"]?.ArrayValue.Values.Select(x => x.StringValue).ToList();

            if (result == null || result.Entities.Count == 0)
            {
                return NotFound(username);
            }
            else if (emails.Count == 0 || emails.Any(authorisedEmail => authorisedEmail != email))
            {
                return StatusCode(403);
            }

            return Ok();
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

            var username = confEntity.Properties["user"].KeyValue.Path.First().Name;
            var isAdmin = username.ToCharArray().First() != 'i';

            List<string> authorisedEmails = new List<string>()
            {
                confEntity["origin"].StringValue
            };

            Entity newUser = new Entity()
            {
                Key = keyFactory.CreateKey(username),
                ["isAdmin"] = isAdmin,
                ["createdAt"] = DateTime.Now.ToString(),
                ["authorisedEmails"] = authorisedEmails.ToArray()
            };

            CommitResponse response;
            using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
            {
                transaction.Insert(newUser);
                transaction.Delete(confEntity.Key);

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
        public IActionResult PostConfirmUser(string username, [FromQuery] string authEmail)
        {
            var emailAddress = new MailAddress($"{username}@bournemouth.ac.uk");

            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var kf = this.datastore.CreateKeyFactory(EntityKind.Confirmation.ToString());

            Entity confirmationEntity = new Entity()
            {
                Key = kf.CreateIncompleteKey(),
                ["token"] = token,
                ["created_at"] = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                ["user"] = new Key().WithElement(EntityKind.User.ToString(), username),
                ["origin"] = authEmail
            };

            Query existingConfirmationsQuery = new Query(EntityKind.Confirmation.ToString())
            {
                Filter = Filter.Equal("user", new Key().WithElement(EntityKind.User.ToString(), username))
            };

            var existingConfirmations = this.datastore.RunQuery(existingConfirmationsQuery);

            CommitResponse commitResponse;
            using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
            {
                if (existingConfirmations.Entities.Any())
                {
                    transaction.Delete(existingConfirmations.Entities);
                }

                transaction.Insert(confirmationEntity);
                commitResponse = transaction.Commit();
            }

            var uriBuilder = new UriBuilder()
            {
                Scheme = "https",
                Host = configuration["CORS:Origin"].ToString(),
                Path = $"login/confirm/{ Uri.EscapeDataString(token)}"
            };

            var mail = new EmailNotification(emailAddress.ToString(), "Account Confirmation", $"Click the link to confirm your account - {uriBuilder.ToString()}");
            this.notificationService.PushAsync(mail);

            return Ok();
        }

        [HttpGet("{username}/requests")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult GetRequestsForUser(string username)
        {
            Query query = new Query(EntityKind.Request.ToString());
            Key userKey = new Key().WithElement(EntityKind.User.ToString(), username);

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

        [HttpGet("{username}/notifications")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult GetNotificationsForUser(string username)
        {
            Query query = new Query(EntityKind.Notification.ToString());
            Key userKey = new Key().WithElement(EntityKind.User.ToString(), username);

            if (userKey != null)
            {
                query.Filter = Filter.And(Filter.Equal("user", userKey), Filter.Equal("read", false));
            }

            DatastoreQueryResults results = this.datastore.RunQuery(query);

            if (!results.Entities.Any())
            {
                return NoContent();
            }

            return Ok(results.Entities.Select(x => x.ToNotification()));
        }

        [HttpDelete("{username}/notifications/{notificationId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult DeleteNotificationForUser(string username, long notificationId)
        {

            Key notificationKey = new Key().WithElement(EntityKind.Notification.ToString(), notificationId);

            CommitResponse commitResponse;
            using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
            {
                transaction.Delete(notificationKey);
                commitResponse = transaction.Commit();
            }

            if (commitResponse.MutationResults.Count != 1)
            {
                return StatusCode(500, "Failed to remove notification");
            }

            return Ok();
        }

        #endregion Methods
    }
}