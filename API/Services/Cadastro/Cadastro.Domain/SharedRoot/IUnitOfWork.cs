using Microsoft.EntityFrameworkCore.Storage;

namespace Cadastro.Domain.SharedRoot
{
    public interface IUnitOfWork
    {
        bool Commit();
        void Dispose();
        void DetachAllEntities();
        IDbContextTransaction BeginTran();
        void CommitTransaction();
    }
}
