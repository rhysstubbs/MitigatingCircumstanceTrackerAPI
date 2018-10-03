using MCT.DataAccess.Interfaces.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.DataAccess.Models
{
    public class Request : IRequest
    {
        public int RequestId { get; set; }
        [Required]
        public string Description { get; set; }
        public ICollection<RequestSubject> RequestSubjects { get; set; } = new List<RequestSubject>();
        public ICollection<RequestFile> RequestFiles { get; set; } = new List<RequestFile>();
    }
}