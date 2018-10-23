using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;

namespace RESTAPI.Models.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Request
    {
        [Key]
        public Key Id { get; set; }

        [JsonProperty]
        public string Owner { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public DateTime DateSubmitted { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public RequestStatus Status { get; set; }
    }
}