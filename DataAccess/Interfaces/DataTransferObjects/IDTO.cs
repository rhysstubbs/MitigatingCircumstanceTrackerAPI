using System;
using System.Collections.Generic;
using System.Text;

namespace MCT.DataAccess.Interfaces.DataTransferObjects
{
    internal interface IDTO<TEntity> where TEntity : class
    {
        TEntity ToDbType();
    }
}
