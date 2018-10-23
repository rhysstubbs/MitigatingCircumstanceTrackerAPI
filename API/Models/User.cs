﻿using Google.Cloud.Datastore.V1;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace MCT.RESTAPI.Models
{
    public class User
    {
        [Key]
        [JsonConverter(typeof(StringEnumConverter))]
        public Key Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public bool IsAdmin { get; set; }

        public string Fullname()
        {
            return $"{this.Firstname} {this.Lastname}";
        }

    }
}