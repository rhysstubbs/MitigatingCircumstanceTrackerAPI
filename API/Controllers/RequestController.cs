using Google.Cloud.Datastore.V1;
using Google.Cloud.Storage.V1;
using MCT.RESTAPI.Enums;
using MCT.RESTAPI.Extensions;
using MCT.RESTAPI.Models.GoogleCloud;
using MCT.RESTAPI.Models.JSON;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NotificationProvider.Interfaces;
using NotificationProvider.Models.Notifications;
using NotificationProvider.Services;
using RESTAPI.Models;
using RESTAPI.Models.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RESTAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class RequestController : BaseController
    {
        #region Properties

        private readonly DatastoreDb datastore;
        private readonly CloudStorageOptions storageOptions;
        private readonly StorageClient storage;
        private readonly INotificationService notificationService;
        private readonly Slack slackClient;

        #endregion Properties

        #region Constructor

        public RequestController(
            IConfiguration configuration,
            IOptions<CloudStorageOptions> options,
            INotificationService notificationService,
            Slack slackClient)
        {
            this.datastore = DatastoreDb.Create(configuration["GAE:ProjectId"]);
            this.storageOptions = options.Value;
            this.storage = StorageClient.Create();
            this.notificationService = notificationService;
            this.slackClient = slackClient;
        }

        #endregion Constructor

        #region Methods

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult Get()
        {
            Query query = new Query(EntityKind.Request.ToString());
            DatastoreQueryResults results = this.datastore.RunQuery(query);

            if (!results.Entities.Any())
            {
                return NoContent();
            }

            return Ok(results.Entities.Select(x => x.ToRequest()));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Get(long id)
        {
            Key key = new Key().WithElement(EntityKind.Request.ToString(), id);
            Entity request = this.datastore.Lookup(key);

            if (request == null)
            {
                return NotFound(id);
            }

            return Ok(request.ToRequest());
        }

        [HttpPost("{id}/files/upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UploadFilesAsync(long id)
        {
            List<IFormFile> uploads = this.Request.Form.Files.ToList();
            Dictionary<string, string> results = new Dictionary<string, string>();

            if (uploads.Any())
            {
                foreach (var f in uploads)
                {
                    if (f.Length <= 0 || f.Length > 1000000)
                    {
                        return BadRequest($"{f.FileName} - must be <= 25Mb in size and not empty.");
                    }
                    else
                    {
                        Google.Apis.Storage.v1.Data.Object uploadObject;
                        int timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

                        using (var ms = new MemoryStream())
                        {
                            f.CopyTo(ms);
                            uploadObject = await this.storage.UploadObjectAsync(storageOptions.BucketName, $"{f.FileName}-{timestamp}", f.ContentType, ms);
                        }

                        try
                        {
                            if (uploadObject.Id.Length > 0)
                            {
                                var keyFactory = this.datastore.CreateKeyFactory(EntityKind.File.ToString());

                                Entity fileEntity = new Entity
                                {
                                    Key = keyFactory.CreateIncompleteKey(),
                                    ["request"] = new Key().WithElement(EntityKind.Request.ToString(), id),
                                    ["name"] = f.FileName
                                };

                                CommitResponse commitResponse;
                                using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
                                {
                                    try
                                    {
                                        transaction.Insert(fileEntity);
                                        commitResponse = transaction.Commit();
                                    }
                                    catch (Exception exception)
                                    {
                                        return StatusCode(500, exception.Message);
                                    }
                                }

                                if (commitResponse.MutationResults.Any())
                                {
                                    results.Add(f.FileName, uploadObject.MediaLink.ToString());
                                    var completeKey = commitResponse.MutationResults.First().Key;
                                    fileEntity.Key = completeKey;
                                }
                                else
                                {
                                    results.Add(f.FileName, null);
                                }
                            }
                        }
                        catch (UploadValidationException exception)
                        {
                            return BadRequest(exception.HelpLink);
                        }
                        catch (Exception exception)
                        {
                            return BadRequest(exception.Message);
                        }
                    }
                }
            }
            else
            {
                return BadRequest("Files must be sent multipart/form-data");
            }

            return Ok(results);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult Post([FromBody] JSONRequest request)
        {
            KeyFactory keyFactory = datastore.CreateKeyFactory(EntityKind.Request.ToString());

            if (request == null)
            {
                return BadRequest("A request object must be present");
            }
            else if (request.Owner == null)
            {
                return BadRequest("A user key is required as part of the request object");
            }

            request.DateSubmitted = DateTime.Now;
            request.Status = RequestStatus.Submitted;

            Entity requestEntity = request.ToEntity();
            requestEntity.Key = keyFactory.CreateIncompleteKey();
            CommitResponse commitResponse;

            using (DatastoreTransaction transaction = datastore.BeginTransaction())
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

            try
            {
                slackClient.PostToChannel("advanced-development", $"A new request has been submited by {request.Owner}.");
                notificationService.PushAsync(new EmailNotification($"{request.Owner}@bournemouth.ac.uk", "We have your submission", "Thank you for your submision, you will recieve an update within 3-5 working days."));
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception);
            }

            return Ok(requestEntity.ToRequest());
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult Patch(long id, [FromBody] JSONRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            if (request.Id == 0)
            {
                request.Id = id;
            }

            request.Status = RequestStatus.Updated;

            Entity requestEntity = request.ToEntity();
            CommitResponse commitResponse;

            using (DatastoreTransaction transaction = datastore.BeginTransaction())
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

            try
            {
                slackClient.PostToChannel("advanced-development", $"Request {request.Id} has been updated by {request.Owner}.");
                notificationService.PushAsync(new EmailNotification($"{request.Owner}@bournemouth.ac.uk", "Submission Updated", "Thank you for your updating your submission, you will recieve an update within 3-5 working days."));
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception);
            }

            return Ok(requestEntity.ToRequest());
        }

        [HttpPatch("{id}/markAs/{status}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult PatchSetStatus(long id, RequestStatus status)
        {
            if (id == 0)
            {
                return BadRequest("Datastore Ids can not have an id of 0 (zero).");
            }

            Key key = new Key().WithElement(EntityKind.Request.ToString(), id);
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

        [HttpPost("{id}/moreInfo")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult PostRequestMoreInfo(long id, [FromBody] MoreInfoRequest moreInfo)
        {
            Key key = new Key().WithElement(EntityKind.Request.ToString(), id);
            Entity request = this.datastore.Lookup(key);

            if (request == null)
            {
                return NotFound(key.ToString());
            }
            else
            {
                request["status"] = RequestStatus.MoreInfoRequired.ToString();
            }

            var keyFactory = this.datastore.CreateKeyFactory(EntityKind.Notification.ToString());

            Entity notification = new Entity()
            {
                Key = keyFactory.CreateIncompleteKey(),
                ["description"] = moreInfo.description,
                ["request"] = key,
                ["user"] = request["owner"].KeyValue,
                ["createAt"] = DateTime.Now.ToString(),
                ["read"] = false
            };

            CommitResponse commitResponse;
            using (DatastoreTransaction transaction = this.datastore.BeginTransaction())
            {
                try
                {
                    transaction.Insert(notification);
                    transaction.Update(request);

                    commitResponse = transaction.Commit();
                }
                catch (Exception exception)
                {
                    return StatusCode(500, exception);
                }
            }

            return Ok(request.ToRequest());
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult Delete(long id)
        {
            Key key = new Key().WithElement(EntityKind.Request.ToString(), id);

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

        #endregion Methods
    }
}