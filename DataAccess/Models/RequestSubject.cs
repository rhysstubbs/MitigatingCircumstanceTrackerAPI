using System;
using System.Collections.Generic;
using System.Text;

namespace MCT.DataAccess.Models
{
    public class RequestSubject
    {
        public int RequestId { get; set; }
        public Request Request { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
