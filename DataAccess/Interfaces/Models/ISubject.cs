using MCT.DataAccess.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.DataAccess.Interfaces.Models
{
    public interface ISubject
    {
        int SubjectId { get; set; }
        string Name { get; set; }
        ICollection<RequestSubject> RequestSubjects { get; set; }
    }
}