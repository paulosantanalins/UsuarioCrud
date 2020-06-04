using GestaoServico.Domain.PessoaRoot.Entity;

namespace GestaoServico.Domain.PessoaRoot.Service.Interfaces
{
    public interface IPessoaService
    {
        Pessoa Buscar(int? idEacesso);
        void Migrar();
        void AtualizarMigracao();
        Pessoa Buscar(int id);
    }
}
