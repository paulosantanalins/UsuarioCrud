using LancamentoFinanceiro.Domain;
using LancamentoFinanceiro.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ServiceContext _context;

        public UnitOfWork(ServiceContext context)
        {
            _context = context;
        }

        public bool Commit()
        {
            var changes = _context.SaveChanges() > 0;
            //_context.DetachAllEntities();

            return changes;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void DetachAllEntities()
        {
            _context.DetachAllEntities();
        }
    }
}
