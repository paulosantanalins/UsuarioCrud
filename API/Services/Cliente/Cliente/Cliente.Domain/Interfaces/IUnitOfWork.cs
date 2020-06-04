using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        bool Commit();
        void Dispose();
    }
}
