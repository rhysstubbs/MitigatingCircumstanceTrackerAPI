using MCT.DataAccess.Interfaces.DataTransferObjects;
using MCT.DataAccess.Models;

namespace MCT.DataAccess.DataTransferObjects
{
    public class FileDTO : IDTO<File>
    {
        public int FileId { get; set; }
        public string Filename { get; set; }
        public string Path { get; set; }

        public FileDTO()
        {
        }

        public File ToDbType()
        {
            return new File()
            {
                FileId = this.FileId,
                Filename = this.Filename,
                Path = this.Path
            };
        }

    }
}