using Microsoft.EntityFrameworkCore.Storage;

namespace UsuarioApi.Domain.SharedRoot.UoW.Interfaces
{
    public interface IUnitOfWork
    {
        bool Commit();
        void Dispose();
        IDbContextTransaction BeginTran();
        void CommitTran();
    }
}
