using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using RESTAPI.Models.JSON;
using System;
using System.Globalization;
using System.Linq;

namespace MCT.RESTAPI.Extensions
{
    internal static class BooExtensionMethods
    {
        /// <summary>
        /// Create a datastore entity with the same values as book.
        /// </summary>
        /// <param name="book">The book to store in datastore.</param>
        /// <returns>A datastore entity.</returns>
        /// [START toentity]
        public static Entity ToEntity(this Request request) => new Entity()
        {
            //Key = request.Id.ToKey()
        };

        // [END toentity]

        /// <summary>
        /// Unpack a Request from a datastore entity.
        /// </summary>
        /// <param name="entity">An entity retrieved from datastore.</param>
        /// <returns>A Request.</returns>
        public static Request ToRequest(this Entity entity)
        {
            return new Request()
            {
                Id = entity.Key,
                Owner = (long)entity["owner"],
                Description = (string)entity["description"],
                DateSubmitted = DateTime.ParseExact(entity["dateSubmitted"].StringValue, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture),
                Status = (RequestStatus)Enum.Parse(typeof(RequestStatus), entity["status"].StringValue, true)
            };
        }
    }
}