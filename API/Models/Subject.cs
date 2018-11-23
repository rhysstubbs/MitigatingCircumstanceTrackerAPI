using Google.Cloud.Datastore.V1;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MCT.RESTAPI.Models
{
    public class Subject
    {
        [Key]
        [JsonProperty]
        public string Title { get; set; }
    }
}