using Google.Cloud.Datastore.V1;
using Newtonsoft.Json;
using RESTAPI.Models.JSON;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.RESTAPI.Models
{
    internal class User
    {
        [Key]
        public Key Id { get; set; }

        [JsonProperty]
        public string Username { get; set; }

        [JsonProperty]
        public bool IsAdmin { get; set; }

        public List<Request> Requests { get; set; }
    }
}