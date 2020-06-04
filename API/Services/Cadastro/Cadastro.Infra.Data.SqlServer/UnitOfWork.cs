using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cadastro.Infra.Data.SqlServer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CadastroContexto _context;

        public UnitOfWork(CadastroContexto context) { _context = context; }

        public bool Commit() => _context.SaveChanges() > 0;
        public void Dispose() => _context.Dispose();
        public void DetachAllEntities() => _context.DetachAllEntities();
        public IDbContextTransaction BeginTran() => _context.Database.BeginTransaction();
        public void CommitTransaction() => _context.Database.CommitTransaction();
    }
}
