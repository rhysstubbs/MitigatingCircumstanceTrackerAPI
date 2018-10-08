using MCT.DataAccess.DataTransferObjects;
using MCT.DataAccess.Models;

namespace MCT.DataAccess.Repositories
{
    public interface ISubjectRepository
    {
        GenericRepository<Subject> SubjectRepo { get; }
    
        SubjectDTO Get(int requestId);
        SubjectDTO Insert(SubjectDTO requestDTO);
    }
}