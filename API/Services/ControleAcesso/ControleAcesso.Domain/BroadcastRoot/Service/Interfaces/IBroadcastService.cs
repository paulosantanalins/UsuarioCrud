using ControleAcesso.Domain.BroadcastRoot.Dto;
using ControleAcesso.Domain.BroadcastRoot.Entity;
using System.Collections.Generic;

namespace ControleAcesso.Domain.BroadcastRoot.Service.Interfaces
{
    public interface IBroadcastService
    {
        IEnumerable<Broadcast> ObterBroadcastsDoUsuario(string usuario, string valorParaFiltrar);
        IEnumerable<Broadcast> ObterTodosBroadcastsNaoExcluidos(string usuario);
        Broadcast MarcarBroadcastComoLido(Broadcast broadcast);
        Broadcast MarcarBroadcastComoExcluido(Broadcast broadcast);
        void CriarBroadcastsParaAberturaPeriodoRepasse(PeriodoRepasseDto periodoRepasseDto);
        void CriarBroadcastsParaAprovacaoHoras(List<BroadcastAprovacaoHorasDto> aprovacao);
        void CriarBroadcastsParaReajusteDeContrato(string funcionalidadeEnvio);
    }
}
