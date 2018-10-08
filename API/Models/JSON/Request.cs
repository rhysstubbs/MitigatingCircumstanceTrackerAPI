using MCT.DataAccess.Interfaces.Models;

namespace RESTAPI.Models.JSON
{
    public class Request : IRequest
    {
        public int RequestId { get; set; }
        public string Description { get; set; }
    }
}