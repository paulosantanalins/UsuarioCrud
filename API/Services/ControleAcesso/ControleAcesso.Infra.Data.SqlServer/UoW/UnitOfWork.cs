using ControleAcesso.Domain.Interfaces;
using ControleAcesso.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Infra.Data.SqlServer.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ControleAcessoContext _context;

        public UnitOfWork(ControleAcessoContext context)
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

        public IDbContextTransaction BeginTran() => _context.Database.BeginTransaction();
    }
}
