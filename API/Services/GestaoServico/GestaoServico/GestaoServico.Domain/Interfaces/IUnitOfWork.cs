using System.Threading.Tasks;

namespace GestaoServico.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        bool Commit();
        void Dispose();
    }
}
