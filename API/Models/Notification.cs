using Google.Cloud.Datastore.V1;
using System;
using System.ComponentModel.DataAnnotations;

namespace MCT.RESTAPI.Models
{
    internal class Notification
    {
        [Key]
        public long Id { get; set; }

        public string Description { get; set; }

        public bool Read { get; set; }

        public DateTime Created { get; set; }

    }
}
