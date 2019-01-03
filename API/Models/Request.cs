using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;

namespace RESTAPI.Models
{
    public class Request
    {
        [Key]
        public long Id { get; set; }

        [JsonRequired]
        public Key Owner { get; set; }

        public bool Extension { get; set; }

        [JsonRequired]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        public string Reason { get; set; }

        [JsonRequired]
        public bool AgreementSigned { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateSubmitted { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateStarted { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateEnded { get; set; }

        public bool OnGoing { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RequestStatus Status { get; set; }
    }
}