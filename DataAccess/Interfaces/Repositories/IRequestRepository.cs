using DataAccess.DataTransferObjects;
using MCT.DataAccess.Models;
using MCT.DataAccess.Repositories;

namespace DataAccess.Interfaces.Repositories
{
    public interface IRequestRepository
    {
        GenericRepository<Request> RequestRepo { get; }

        RequestDTO Update(RequestDTO request);
        RequestDTO Get(int requestId);
        RequestDTO Insert(RequestDTO requestDTO);
        bool Delete(int id);
    }
}