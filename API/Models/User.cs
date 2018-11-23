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
        [JsonConverter(typeof(StringEnumConverter))]
        public Key Id { get; set; }

        [JsonProperty]
        public string Username { get; set; }

        [JsonProperty]
        public string Password { get; set; }

        [JsonProperty]
        public string Firstname { get; set; }

        [JsonProperty]
        public string Lastname { get; set; }

        [JsonProperty]
        public bool IsAdmin { get; set; }

        [JsonProperty]
        public HashSet<Subject> Subjects { get; set; }

        public string Fullname()
        {
            return $"{this.Firstname} {this.Lastname}";
        }
    }
}