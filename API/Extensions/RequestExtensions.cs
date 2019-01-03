using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using MCT.RESTAPI.Models;
using RESTAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MCT.RESTAPI.Extensions
{
    internal static class EntityExtensionMethods
    {
        public static Key ToKey(this long id) =>
            new Key().WithElement(EntityKind.Request.ToString(), id);

        public static long ToId(this Key key) => key.Path.First().Id;

        public static Entity ToEntity(this Request request, EntityKind kind) => new Entity()
        {
            Key = new Key().WithElement(kind.ToString(), request.Id),
            ["description"] = request.Description,
            ["owner"] = request.Owner,
            ["status"] = request.Status.ToString(),
            ["dateSubmitted"] = request.DateSubmitted
        };

        public static Notification ToNotification(this Entity entity) => new Notification()
        {
            Id = entity.Key.ToId() != 0 ? entity.Key.ToId() : 0,
            Description = entity["description"].StringValue,
            Read = entity["read"].BooleanValue,
            Created = DateTime.ParseExact(entity["createdAt"]?.StringValue ?? DateTime.Now.ToString("yyyyMMddHHmmssfff"), "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture),
        };

        public static Request ToRequest(this Entity entity) => new Request()
        {
                Id = entity.Key.ToId() != 0 ? entity.Key.ToId() : 0,
                Owner = entity["owner"].KeyValue ?? null,
                Extension = entity["extension"].BooleanValue,
                Description = (string)entity?["description"] ?? null,
                Reason = entity["reason"].StringValue ?? null,
                AgreementSigned = entity["agreementSigned"].BooleanValue,
                DateSubmitted = DateTime.ParseExact(entity["dateSubmitted"]?.StringValue ?? DateTime.Now.ToString("yyyyMMddHHmmssfff"), "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture),
                DateStarted = DateTime.ParseExact(entity["dateStarted"].StringValue, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture),
                DateEnded = entity["dateEnded"]?.StringValue != null ? DateTime.ParseExact(entity["dateEnded"].StringValue, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture) : (DateTime?) null,
                OnGoing = entity["onGoing"].BooleanValue,
                Status = (RequestStatus)Enum.Parse(typeof(RequestStatus), entity["status"]?.StringValue, true)
        };

    }
}