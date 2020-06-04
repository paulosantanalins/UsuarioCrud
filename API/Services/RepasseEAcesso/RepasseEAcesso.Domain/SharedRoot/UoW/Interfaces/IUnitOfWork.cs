using Microsoft.EntityFrameworkCore.Storage;

namespace RepasseEAcesso.Domain.SharedRoot.UoW.Interfaces
{
    public interface IUnitOfWork
    {
        bool Commit();
        void Dispose();

        bool CommitLegado();
        void DisposeLegado();
        IDbContextTransaction BeginTranStfCorp();
        IDbContextTransaction BeginTranEAcesso();
    }
}
