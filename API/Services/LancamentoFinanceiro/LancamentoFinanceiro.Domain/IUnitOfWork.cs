namespace LancamentoFinanceiro.Domain
{
    public interface IUnitOfWork
    {
        bool Commit();
        void Dispose();
        void DetachAllEntities();
    }
}
