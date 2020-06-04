using Microsoft.EntityFrameworkCore.Storage;
using UsuarioApi.Infra.Data.SqlServer.Context;
using UsuarioApi.Domain.SharedRoot.UoW.Interfaces;

namespace UsuarioApi.Domain.SharedRoot.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public void Dispose() => _context.Dispose();
        public bool Commit() => _context.SaveChanges() > 0;
        public void CommitTran() => _context.Database.CommitTransaction();
        public IDbContextTransaction BeginTran() => _context.Database.BeginTransaction();
    }
}
