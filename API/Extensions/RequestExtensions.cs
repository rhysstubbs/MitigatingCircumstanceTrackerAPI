using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using RESTAPI.Models.JSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MCT.RESTAPI.Extensions
{
    internal static class RequestExtensionMethods
    {
        public static Key ToKey(this long id) =>
            new Key().WithElement(EntityKind.Request.ToString(), id);

        public static ArrayValue ToKeys(this HashSet<string> subjects) =>
            subjects.Select(k => new Key().WithElement(EntityKind.Subject.ToString(), k.ToLower())).ToArray();

        public static long ToId(this Key key) => key.Path.First().Id;

        public static Entity ToEntity(this Request request, EntityKind kind) => new Entity()
        {
            Key = new Key().WithElement(kind.ToString(), request.Id),
            ["description"] = request.Description,
            ["owner"] = request.Owner,
            ["status"] = request.Status.ToString(),
            ["dateSubmitted"] = request.DateSubmitted
        };

        public static Request ToRequest(this Entity entity)
        {
            return new Request
            {
                Id = entity.Key.ToId() != 0 ? entity.Key.ToId() : 0,
                Owner = entity["owner"].KeyValue ?? null,
                Description = (string)entity?["description"] ?? null,
                DateSubmitted = DateTime.ParseExact(entity["dateSubmitted"]?.StringValue ?? DateTime.Now.ToString("yyyyMMddHHmmssfff"), "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture),
                Status = (RequestStatus)Enum.Parse(typeof(RequestStatus), entity["status"]?.StringValue, true)
            };
        }
    }
}