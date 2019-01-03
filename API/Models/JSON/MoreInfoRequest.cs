using Newtonsoft.Json;

namespace MCT.RESTAPI.Models.JSON
{
    public class MoreInfoRequest
    {
        [JsonRequired]
        public string description;
    }
}
