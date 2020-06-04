using System;
using ControleAcesso.Domain.BroadcastRoot.Dto;
using ControleAcesso.Domain.BroadcastRoot.Entity;
using ControleAcesso.Domain.BroadcastRoot.Repository;
using ControleAcesso.Domain.BroadcastRoot.Service.Interfaces;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Domain.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ControleAcesso.Domain.BroadcastRoot.Service
{
    public class BroadcastService : IBroadcastService
    {
        private readonly IBroadcastRepository _broadcastRepository;
        private readonly IBroadcastItemRepository _broadcastItemRepository;
        private readonly IFuncionalidadeRepository _funcionalidadeRepository;
        private readonly IUsuarioPerfilRepository _usuarioPerfilRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BroadcastService(IBroadcastRepository broadcastRepository,
            IBroadcastItemRepository broadcastItemRepository,
            IFuncionalidadeRepository funcionalidadeRepository,
            IUsuarioPerfilRepository usuarioPerfilRepository,
            IUnitOfWork unitOfWork)
        {
            _broadcastRepository = broadcastRepository;
            _broadcastItemRepository = broadcastItemRepository;
            _funcionalidadeRepository = funcionalidadeRepository;
            _usuarioPerfilRepository = usuarioPerfilRepository;
            _unitOfWork = unitOfWork;
        }



        public IEnumerable<Broadcast> ObterBroadcastsDoUsuario(string usuario, string valorParaFiltrar)
        {
            var broadcasts = _broadcastRepository.ObterBroadcastsDoUsuario(usuario, valorParaFiltrar);

            return broadcasts;
        }

        public IEnumerable<Broadcast> ObterTodosBroadcastsNaoExcluidos(string usuario) => _broadcastRepository.ObterTodosBroadcastsNaoExcluidos(usuario);

        public Broadcast MarcarBroadcastComoLido(Broadcast broadcast)
        {
            var broadcastDB = _broadcastRepository.BuscarPorId(broadcast.Id);
            broadcastDB.Lido = true;
            _broadcastRepository.Update(broadcastDB);
            _unitOfWork.Commit();
            return broadcastDB;
        }

        public Broadcast MarcarBroadcastComoExcluido(Broadcast broadcast)
        {
            var broadcastDB = _broadcastRepository.BuscarPorId(broadcast.Id);
            broadcastDB.Excluido = true;
            _broadcastRepository.Update(broadcastDB);
            _unitOfWork.Commit();
            return broadcastDB;
        }

        public void CriarBroadcastsParaAberturaPeriodoRepasse(PeriodoRepasseDto periodoRepasseDto)
        {
            var funcionalidadesRepasse = _funcionalidadeRepository.Buscar(x => x.NmFuncionalidade.ToUpper().Contains("REPASSE"));
            var usuarios = _usuarioPerfilRepository.BuscarUsuariosPorFuncionalidades(funcionalidadesRepasse.Select(x => x.Id).ToArray()).Distinct();

            var lastBroadcastId = _broadcastItemRepository.BuscarUltimoId();
            var broadcastItem = new BroadcastItem
            {
                //TO_DO validar auto-increment  
                Id = ++lastBroadcastId,
                Descricao = MontarDescricaoBroadcastDeAberturaPeriodoRepasse(periodoRepasseDto, periodoRepasseDto.ehAlteracaoCronograma),
                Titulo = periodoRepasseDto.ehAlteracaoCronograma ? "Alteração de Cronograma - Periodo de Repasse" : "Abertura de Período de Repasse",
            };
            _broadcastItemRepository.Adicionar(broadcastItem);

            _broadcastRepository.AddRange(usuarios.Select(user => new Broadcast
                {LgUsuarioVinculado = user, IdBroadcastItem = broadcastItem.Id, DataCriacao = DateTime.Now}));
            _unitOfWork.Commit();
        }

        public void CriarBroadcastsParaAprovacaoHoras(List<BroadcastAprovacaoHorasDto> aprovacoes)
        {
            var lastBroadcastId = _broadcastItemRepository.BuscarUltimoId();
            var broadcastList = new List<Broadcast>();
            //TO_DO validar esse id

            foreach (var aprovacao in aprovacoes)
            {
                var broadcastItem = new BroadcastItem
                {
                    Id = ++lastBroadcastId,
                    Descricao = MontarDescricaoBroadcastDeAprovacaoHoras(aprovacao),
                    Titulo = "Abertura de Aprovação de Pagamento",
                };
                _broadcastItemRepository.Adicionar(broadcastItem);
                broadcastList.Add(new Broadcast 
                    { LgUsuarioVinculado = aprovacao.LoginAprovador, IdBroadcastItem = broadcastItem.Id, DataCriacao = DateTime.Now});
            }
            _broadcastRepository.AddRange(broadcastList);
            _unitOfWork.Commit();
        }

        public void CriarBroadcastsParaReajusteDeContrato(string funcionalidadeEnvio)
        {
            var funcionalidades =
                _funcionalidadeRepository.Buscar(x => x.NmFuncionalidade == funcionalidadeEnvio);
            var usuarios = _usuarioPerfilRepository.BuscarUsuariosPorFuncionalidades(funcionalidades.Select(x => x.Id).ToArray()).Distinct();
            var lastBroadcastId = _broadcastItemRepository.BuscarUltimoId();

            var broadcastItem = new BroadcastItem
            {
                Id = ++lastBroadcastId,
                Descricao = MontarDescricaoReajusteDeContrato(),
                Titulo = "Solicitação de Reajuste de Contrato"
            };
            _broadcastItemRepository.Adicionar(broadcastItem);

            _broadcastRepository.AddRange(usuarios.Select(user => new Broadcast
                {LgUsuarioVinculado = user, IdBroadcastItem = broadcastItem.Id, DataCriacao = DateTime.Now}));
            _unitOfWork.Commit();
        }

        private string MontarDescricaoBroadcastDeAberturaPeriodoRepasse(PeriodoRepasseDto periodo, bool ehEdicaoDeCronograma)
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            return $@"Cronograma para lançamentos de Repasses no mês de
                <b>{periodo.DtLancamento.ToString("MMMM/yyyy", culture)}</b>, referente as despesas  de <b>{periodo.DtLancamento.AddMonths(-1).ToString("MMMM/yyyy", culture)}</b>.
                <br>
                <br> <b>De {periodo.DtLancamentoInicio.ToString("dd/MM/yyyy")} a {periodo.DtLancamentoFim.ToString("dd/MM/yyyy")}</b> - Disponível para lançamentos <b>(ATÉ ÀS 23:59 hs)</b>;
                <br> <b>De {periodo.DtAnaliseInicio.ToString("dd/MM/yyyy")} a {periodo.DtAnaliseFim.ToString("dd/MM/yyyy")}</b> - Análise, negociações, alterações, aprovações.
                <br> <b>De {periodo.DtAprovacaoInicio.ToString("dd/MM/yyyy")} a { periodo.DtAprovacaoFim.ToString("dd/MM/yyyy")}</b> - Aprovação segundo nível.
                <br>
                <br> <b>Fechamento final dia {periodo.DtAprovacaoFim.ToString("dd/MM/yyyy")} às 23:59 hrs</b>";
        }

        private string MontarDescricaoBroadcastDeAprovacaoHoras(BroadcastAprovacaoHorasDto aprovacao)
        {
            return $@"Prezado(a) {aprovacao.NomeAprovador},<br><br>As horas referentes a competência do mês de {aprovacao.PeriodoCompetencia}
                      para os prestadores sob sua gestão que recebem dia {aprovacao.DiaPagamento} já estão lançadas.<br><br>
                      Favor prosseguir com a aprovação das horas até o dia {aprovacao.DiaLimite}.<br><br>
                      <a href={aprovacao.Link}>Aprovar Horas</a><br><br>
                      Atenciosamente,<br>Gestão de Contratos.";
        }

        private string MontarDescricaoReajusteDeContrato()
        {
            return @"Existe aprovação de Reajuste de Contrato pendente. Por gentileza, verifique.
                    <br>
                    <br> Atenciosamente,
                    <br> Gestão de Reajustes de Contratos";
        }
    }
}
