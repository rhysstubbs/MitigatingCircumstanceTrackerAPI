﻿using MCT.DataAccess.Models;
using System.Collections.Generic;

namespace MCT.DataAccess.Interfaces.Models
{
    public interface IRequest
    {
        int RequestId { get; set; }
        string Description { get; set; }
    }
}