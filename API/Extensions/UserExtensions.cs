using Google.Cloud.Datastore.V1;
using MCT.RESTAPI.Enums;
using MCT.RESTAPI.Models;
using System;
using System.Globalization;

namespace MCT.RESTAPI.Extensions
{
    internal static class UserExtensionMethods
    {
        public static MinimalUser ToMinimalUser(this Entity entity)
        {
            return new MinimalUser()
            {
                Username = (string)entity["username"],
                Password = (string)entity["password"],
                ISAdmin = (bool)entity["isAdmin"]
            };
        }
    }
}