using Forecast.Domain.SharedRoot;
using Forecast.Infra.Data.SqlServer.Context;

namespace Forecast.Infra.Data.SqlServer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ForecastContext _context;

        public UnitOfWork(ForecastContext context)
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

        public void DetachAllEntities()
        {
            _context.DetachAllEntities();
        }
    }
}
