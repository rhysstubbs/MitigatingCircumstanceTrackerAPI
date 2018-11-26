using MCT.RESTAPI.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace RESTAPI.Models.JSON
{
    public class JSONRequest
    {
        public long Id { get; set; }

        [JsonRequired]
        public string Owner { get; set; }

        [JsonRequired]
        public string Description { get; set; }

        public DateTime DateSubmitted { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RequestStatus Status { get; set; }

        public string[] Subjects { get; set; }
    }
}