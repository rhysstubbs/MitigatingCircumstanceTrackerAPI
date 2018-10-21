using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;

namespace RESTAPI.Models.JSON
{
    public class Request
    {
        [Key]
        public long Id { get; set; }

        public long Owner { get; set; }
        public string Description { get; set; }
        public DateTime DateSubmitted { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RequestStatus Status { get; set; }
    }
}