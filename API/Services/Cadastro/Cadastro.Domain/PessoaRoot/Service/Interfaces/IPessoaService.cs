using Cadastro.Domain.PessoaRoot.Dto;
using Cadastro.Domain.PessoaRoot.Entity;
using System;

namespace Cadastro.Domain.PessoaRoot.Service.Interfaces
{
    public interface IPessoaService
    {
        void AtualizarMigracao();
        int? ObterIdPessoa(int? idEacesso);
        UsuarioAdDto ObterUsuarioAd(string login);
        Pessoa ObterPessoa(int id);
        void RealizarMigracaoProfissionaisNatcorp(DateTime? aPartirDe);

        Pessoa BuscarPorEmailInterno(string email);
        Pessoa BuscarPorIdEAcesso(int idEAcesso);
    }
}
