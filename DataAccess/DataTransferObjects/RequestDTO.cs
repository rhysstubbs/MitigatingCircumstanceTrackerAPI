using MCT.DataAccess.Interfaces.DataTransferObjects;
using MCT.DataAccess.Interfaces.Models;
using MCT.DataAccess.Models;

namespace DataAccess.DataTransferObjects
{
    public class RequestDTO : IDTO<Request>
    {
        public int RequestId { get; set; }
        public string Description { get; set; }

        public RequestDTO()
        {
        }

        public RequestDTO(IRequest request)
        {
            this.RequestId = request.RequestId;
            this.Description = request.Description;
        }

        public Request ToDbType()
        {
            return new Request()
            {
                RequestId = this.RequestId,
                Description = this.Description
            };
        }
    }
}