using Microsoft.EntityFrameworkCore.Storage;
using RepasseEAcesso.Domain.SharedRoot.UoW.Interfaces;
using RepasseEAcesso.Infra.Data.SqlServer.Context;

namespace RepasseEAcesso.Domain.SharedRoot.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RepasseEAcessoContext _context;
        private readonly RepasseLegadoContext _contextLegado;

        public UnitOfWork(RepasseEAcessoContext context,
                          RepasseLegadoContext contextLegado)
        {
            _context = context;
            _contextLegado = contextLegado;
        }

        public bool Commit()
        {
            var rowsAffected = _context.SaveChanges();
            return rowsAffected > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public bool CommitLegado()
        {
            var rowsAffected = _contextLegado.SaveChanges();
            return rowsAffected > 0;
        }

        public void DisposeLegado()
        {
            _contextLegado.Dispose();
        }

        public IDbContextTransaction BeginTranStfCorp() => _context.Database.BeginTransaction();
        public IDbContextTransaction BeginTranEAcesso() => _contextLegado.Database.BeginTransaction();

    }
}
