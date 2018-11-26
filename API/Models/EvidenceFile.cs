using Google.Cloud.Datastore.V1;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MCT.RESTAPI.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EvidenceFile
    {
        [Key]
        [JsonRequired]
        [JsonProperty]
        public long Id { get; set; }

        [JsonProperty]
        [JsonRequired]
        public Key Request { get; set; }

        [JsonProperty]
        public string Name { get; set; }
    }
}