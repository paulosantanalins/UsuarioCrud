using Cliente.Domain.Interfaces;
using Cliente.Infra.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Infra.Data.SqlServer.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClienteContext _context;

        public UnitOfWork(ClienteContext context)
        {
            _context = context;
        }

        public bool Commit()
        {
            return _context.SaveChanges() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
