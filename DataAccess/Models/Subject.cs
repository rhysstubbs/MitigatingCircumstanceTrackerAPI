using MCT.DataAccess.Interfaces.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.DataAccess.Models
{
    public class Subject : ISubject
    {
        public int SubjectId { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<RequestSubject> RequestSubjects { get; set; }
    }
}
