using MCT.DataAccess.Interfaces.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.DataAccess.Models
{
    public class File : IFile
    {
        public int FileId { get; set; }
        [Required]
        public string Filename { get; set; }
        [Required]
        public string Path { get; set; }
        public virtual ICollection<RequestFile> Requests { get; set; }
    }
}