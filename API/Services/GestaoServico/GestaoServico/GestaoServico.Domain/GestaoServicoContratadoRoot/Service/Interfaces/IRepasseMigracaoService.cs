using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System.Collections.Generic;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces
{
    public interface IRepasseMigracaoService
    {
        List<EAcessoRepasse> BuscarRepassesEAcesso(string dtInicio, string dtFim);
        void MigrarRepassesEacesso(List<Repasse> repasses);
    }
}
