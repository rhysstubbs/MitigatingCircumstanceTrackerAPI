using MCT.DataAccess.Interfaces.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCT.DataAccess.Models
{
    public class Request : IRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }
        [Required]
        public string Description { get; set; }
        public ICollection<RequestSubject> RequestSubjects { get; set; } = new List<RequestSubject>();
        public ICollection<RequestFile> RequestFiles { get; set; } = new List<RequestFile>();
    }
}