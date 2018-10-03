using MCT.DataAccess.Models;
using System.Collections.Generic;

namespace MCT.DataAccess.Interfaces.Models
{
    public interface IFile
    {
        int FileId { get; set; }
        string Filename { get; set; }
        string Path { get; set; }
        ICollection<RequestFile> Requests { get; set; }
    }
}