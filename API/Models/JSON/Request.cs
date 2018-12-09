using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RESTAPI.Models.JSON
{
    public class JSONRequest
    {
        [Key]
        public long Id { get; set; }

        [JsonRequired]
        public string Owner { get; set; }

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
        public DateTime DateEnded { get; set; }

        public bool OnGoing { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RequestStatus Status { get; set; }

        public List<string> Subjects { get; set; }

        public Entity ToEntity() => new Entity()
        {
            ["owner"] = new Key().WithElement(EntityKind.User.ToString(), this.Owner),
            ["extension"] = this.Extension,
            ["description"] = this.Description,
            ["reason"] = this.Reason,
            ["agreementSigned"] = this.AgreementSigned,
            ["dateSubmitted"] = this.DateSubmitted.ToString("yyyyMMddHHmmssfff"),
            ["dateStarted"] = this.DateStarted.ToString("yyyyMMddHHmmssfff"),
            ["dateEnded"] = this.DateEnded.ToString("yyyyMMddHHmmssfff"),
            ["onGoing"] = this.OnGoing,
            ["status"] = this.Status.ToString()
        };
    }
}