using MCT.DataAccess.Interfaces.DataTransferObjects;
using MCT.DataAccess.Interfaces.Models;
using MCT.DataAccess.Models;

namespace MCT.DataAccess.DataTransferObjects
{
    public class SubjectDTO : IDTO<Subject>
    {
        public int SubjectID;
        public string Name;

        public SubjectDTO()
        {
        }

        public SubjectDTO(ISubject subect)
        {
            this.SubjectID = subect.SubjectId;
            this.Name = subect.Name;
        }

        public Subject ToDbType()
        {
            return new Subject()
            {
                SubjectId = this.SubjectID,
                Name = this.Name
            };
        }
    }
}