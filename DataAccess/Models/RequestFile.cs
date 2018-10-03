using System;
using System.Collections.Generic;
using System.Text;

namespace MCT.DataAccess.Models
{
    public class RequestFile
    {
        public int RequestId { get; set; }
        public Request Request { get; set; }

        public int FileId { get; set; }
        public File File { get; set; }
    }
}
