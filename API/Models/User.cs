using Google.Cloud.Datastore.V1;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.RESTAPI.Models
{
    public class User
    {
        [Key]
        public Key Id { get; set; }

        [JsonProperty]
        public string Username { get; set; }

        [JsonProperty]
        public bool IsAdmin { get; set; }

    }
}