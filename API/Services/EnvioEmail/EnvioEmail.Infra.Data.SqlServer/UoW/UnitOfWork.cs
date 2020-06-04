using EnvioEmail.Domain.SharedRoot;
using EnvioEmail.Infra.Data.SqlServer.Context;

namespace EnvioEmail.Infra.Data.SqlServer.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ServiceBContext _context;

        public UnitOfWork(ServiceBContext context)
        {
            _context = context;
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

        public void DetachAllEntities()
        {
            _context.DetachAllEntities();
        }
    }
}
