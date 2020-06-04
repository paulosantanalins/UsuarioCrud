using Microsoft.EntityFrameworkCore.Storage;

namespace ControleAcesso.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        bool Commit();
        void Dispose();
        IDbContextTransaction BeginTran();
    }
}
