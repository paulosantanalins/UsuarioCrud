using GestaoServico.Domain.Interfaces;
using GestaoServico.Infra.Data.SqlServer.Context;

namespace GestaoServico.Infra.Data.SqlServer.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GestaoServicoContext _context;

        public UnitOfWork(GestaoServicoContext context)
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
