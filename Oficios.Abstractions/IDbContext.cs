using System;
using System.Collections.Generic;
using System.Text;

namespace Oficios.Abstractions
{
    public interface IDbContext<T> : IDbOperation<T> where T : class
    {
    }
}
