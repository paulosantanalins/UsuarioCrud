namespace Forecast.Domain.SharedRoot
{
    public interface IUnitOfWork
    {
        bool Commit();
        void Dispose();
        void DetachAllEntities();
    }
}
