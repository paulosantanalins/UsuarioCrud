using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using Newtonsoft.Json;
using RepasseEAcesso.Domain.DominioRoot.Repository;
using RepasseEAcesso.Domain.DominioRoot.Service.Interface;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Repository;
using RepasseEAcesso.Domain.RepasseRoot.Dto;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Repository;
using RepasseEAcesso.Domain.RepasseRoot.Service.Interfaces;
using RepasseEAcesso.Domain.SharedRoot.UoW.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Utils;
using Utils.Base;
using Utils.Extensions;
using static RepasseEAcesso.Domain.SharedRoot.SharedEnuns;

namespace RepasseEAcesso.Domain.RepasseRoot.Service
{
    public class RepasseNivelUmService : IRepasseNivelUmService
    {
        private readonly IRepasseNivelUmRepository _repasseNivelUmRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepasseRepository _repasseRepository;
        private readonly IPeriodoRepasseRepository _periodoRepasseRepository;
        private readonly IDominioService _dominioService;
        private readonly IDominioRepository _dominioRepository;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly ILogRepasseRepository _logRepasseRepository;
        private readonly IVariablesToken _variables;

        public RepasseNivelUmService(
            IRepasseNivelUmRepository repasseNivelUmRepository,
            IRepasseRepository repasseRepository,
            IPeriodoRepasseRepository periodoRepasseRepository,
            IDominioService dominioService,
            IDominioRepository dominioRepository,
            MicroServicosUrls microServicosUrls,
            ILogRepasseRepository logRepasseRepository,
            IUnitOfWork unitOfWork, IVariablesToken variables)
        {
            _repasseNivelUmRepository = repasseNivelUmRepository;
            _unitOfWork = unitOfWork;
            _variables = variables;
            _repasseRepository = repasseRepository;
            _periodoRepasseRepository = periodoRepasseRepository;
            _dominioService = dominioService;
            _dominioRepository = dominioRepository;
            _microServicosUrls = microServicosUrls;
            _logRepasseRepository = logRepasseRepository;
        }


        public void Persistir(RepasseNivelUm repasseStfCorp, Repasse repasseEAcesso)
        {
            using (var stfCorpTran = _unitOfWork.BeginTranStfCorp())
            using (var eacessoTran = _unitOfWork.BeginTranEAcesso())
            {
                try
                {
                    if (repasseStfCorp.Id == 0)
                    {
                        AdicionarRepasseEacesso(repasseEAcesso);
                        int idDominioRepasseCadastradoLog = _dominioRepository.Buscar((int)StatusRepasseEacesso.REPASSE_CADASTRADO, StatusRepasseEacesso.VL_TIPO_DOMINIO.GetDescription()).Id;
                        AdicionarRepasseStfCorp(repasseStfCorp, repasseEAcesso, idDominioRepasseCadastradoLog);

                        if (repasseStfCorp.QtdVezesRepetir > 1) AdicionarParcelasDeRepasseNoStfcorpENoEacesso(repasseStfCorp, repasseEAcesso, idDominioRepasseCadastradoLog);
                    }
                    else
                    {
                        ValidarSeRepassePodeSerEditado(repasseStfCorp);
                        AtualizarRepasseStfCorp(repasseStfCorp);
                        _unitOfWork.Commit();

                        repasseEAcesso.Id = (int)repasseStfCorp.IdRepasseEacesso;
                        AtualizarRepasseEacesso(repasseEAcesso);
                        _unitOfWork.CommitLegado();
                    }
                    stfCorpTran.Commit();
                    eacessoTran.Commit();
                }
                catch (Exception ex)
                {
                    stfCorpTran.Rollback();
                    eacessoTran.Rollback();
                    throw ex;
                }
            }

            AprovarRepasseSeTemFuncionalidade(repasseStfCorp);
        }

        public RepasseNivelUm BuscarComIncludeId(int id)
        {
            var repasse = _repasseNivelUmRepository.BuscarComIncludeId(id);
            repasse.LogsRepasse = repasse.LogsRepasse.Select(x => { x.DescricaoStatus = _dominioService.ObterDescricaoStatus(x.IdStatusRepasse); return x; }).ToList();
            repasse.QtdVezesRepetir = ObterNumeroParcelasRepasse(repasse);
            if (repasse.IdProfissional != null) repasse.ProfissionalAtivo = ProfissionalEstaAtivo(repasse.IdProfissional);
            return repasse;
        }

        public List<RepasseNivelUm> BuscarTodos() => _repasseNivelUmRepository.BuscarComInclude();
        public FiltroGenericoDtoBase<AprovarRepasseDto> Filtrar(FiltroGenericoDtoBase<AprovarRepasseDto> filtro) => _repasseNivelUmRepository.Filtrar(filtro);
        public FiltroRepasseNivelUmDto<AprovarRepasseDto> FiltrarRepassesNivelUm(FiltroRepasseNivelUmDto<AprovarRepasseDto> filtro) => _repasseNivelUmRepository.Filtrar(filtro);
        public FiltroRepasseNivelDoisDto<AprovarRepasseNivelDoisDto> FiltrarRepassesNivelDois(FiltroRepasseNivelDoisDto<AprovarRepasseNivelDoisDto> filtro) => _repasseNivelUmRepository.Filtrar(filtro);

        private bool? ProfissionalEstaAtivo(int? idProfissional)
        {
            using (var client = new HttpClient())
            {
                var httpResult = client.GetAsync($"{_microServicosUrls.UrlApiServico}api/Profissional/ObterProfissionaisPorId/{idProfissional}").Result;
                var json = httpResult.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<StfCorpHttpSingleResult<ProfissionalEAcessoDto>>(json);
                return !result?.Dados?.Inativo;
            }
        }

        private int ObterNumeroParcelasRepasse(RepasseNivelUm repasse)
        {
            int vezesRepertir;
            if (repasse.IdRepasseEacesso != null)
                vezesRepertir = _repasseRepository.BuscarTodosFilhosLegado((int)repasse.IdRepasseEacesso).Count();
            else
                vezesRepertir = _repasseNivelUmRepository.BuscarTodosFilhos(repasse.Id).Count();
            return vezesRepertir + 1;
        }

        public RepasseNivelUm BuscarPorId(int id)
        {
            var repasse = _repasseNivelUmRepository.BuscarPorId(id);
            var vezesRepertir = _repasseNivelUmRepository.BuscarTodosFilhos(id);
            repasse.QtdVezesRepetir = vezesRepertir.Count();
            return repasse;
        }

        public void AprovarRepasseSemValidacaoCelulaExterior(AprovarRepasseDto aprovarRepasseDto)
        {
            var repasseStfCorp = _repasseNivelUmRepository.BuscarPorId(aprovarRepasseDto.Id);

            /*
            *  ATUALIZAÇÃO REGISTRO NA BASE [STFCORP_FENIX].[stfcorp].[EFATURAMENTO_Repasse]
            *  SE USUÁRIO TIVER PERMISÃO APROVAÇÃO NIVEL DOIS            
            */

            if (aprovarRepasseDto.Status == StatusRepasseEacesso.APROVADO_NIVEL_DOIS.GetDescription())
            {
                repasseStfCorp.Status = aprovarRepasseDto.Status;
                AtualizarRepasseEAcesso(repasseStfCorp);
            }

            AtualizarRepasseQuandoAprovandoOuNegando(aprovarRepasseDto, repasseStfCorp);

        }

        public void AprovarRepasse(AprovarRepasseDto aprovarRepasseDto)
        {
            var repasseStfCorp = _repasseNivelUmRepository.BuscarPorId(aprovarRepasseDto.Id);

            /*
            *  ATUALIZAÇÃO REGISTRO NA BASE [STFCORP_FENIX].[stfcorp].[EFATURAMENTO_Repasse]
            *  SE USUÁRIO TIVER PERMISÃO APROVAÇÃO NIVEL DOIS            
            */

            if (CelulaDestinoPagadoraExterior(repasseStfCorp.IdCelulaDestino))
            {
                aprovarRepasseDto.Status = StatusRepasseEacesso.APROVADO_NIVEL_DOIS.GetDescription();
            }

            if (aprovarRepasseDto.Status == StatusRepasseEacesso.APROVADO_NIVEL_DOIS.GetDescription())
            {
                repasseStfCorp.Status = aprovarRepasseDto.Status;
                AtualizarRepasseEAcesso(repasseStfCorp);
            }

            AtualizarRepasseQuandoAprovandoOuNegando(aprovarRepasseDto, repasseStfCorp);

        }

        public bool CelulaDestinoPagadoraExterior(int IdCelulaDestino)
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(120),
                BaseAddress = new Uri(_microServicosUrls.UrlApiControle)
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync($"api/Celula/buscar-por-id?={IdCelulaDestino}").Result;
            var jsonString = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<CelulaDto>(jsonString);

            if(result.IdPais != 4)
            {
                return true;
            }

            return false;
        }

        public void AprovarRepasseNivelDois(AprovarRepasseNivelDoisDto aprovarRepasseDto)
        {
            var repasseStfCorp = _repasseNivelUmRepository.BuscarPorId(aprovarRepasseDto.Id);
            repasseStfCorp.Status = aprovarRepasseDto.Status;
            _repasseNivelUmRepository.Update(repasseStfCorp);

            int idDominioRepasseAprovadoNivelDois = _dominioRepository.Buscar((int)StatusRepasseEacesso.APROVADO_NIVEL_DOIS, StatusRepasseEacesso.VL_TIPO_DOMINIO.GetDescription()).Id;
            AdicionarLogRepasse(repasseStfCorp.Id, idDominioRepasseAprovadoNivelDois, string.Empty);

            _unitOfWork.Commit();

            AtualizarRepasseEAcesso(repasseStfCorp);
        }

        public void NegarRepasse(AprovarRepasseDto aprovarRepasseDto)
        {
            var repasseStfCorp = _repasseNivelUmRepository.BuscarPorId(aprovarRepasseDto.Id);
            repasseStfCorp.Status = aprovarRepasseDto.Status;
            //ATUALIZAÇÃO REGISTRO NA BASE [STFCORP_FENIX].[stfcorp].[EFATURAMENTO_Repasse]
            AtualizarRepasseEAcesso(repasseStfCorp);
            AtualizarRepasseQuandoAprovandoOuNegando(aprovarRepasseDto, repasseStfCorp);
        }

        public void NegarRepasseNivelDois(AprovarRepasseNivelDoisDto aprovarRepasseDto)
        {
            var repasseStfCorp = _repasseNivelUmRepository.BuscarPorId(aprovarRepasseDto.Id);
            AtualizarRepasseNivelUmAposNegadoNivelDois(aprovarRepasseDto, repasseStfCorp);
            AtualizarRepasseEAcesso(repasseStfCorp);
        }

        private void AtualizarRepasseQuandoAprovandoOuNegando(AprovarRepasseDto aprovarRepasseDto, RepasseNivelUm repasseNivelUm)
        {
            repasseNivelUm.Usuario = aprovarRepasseDto.Usuario;
            repasseNivelUm.Status = aprovarRepasseDto.Status;

            LogRepasse log = new LogRepasse
            {
                IdStatusRepasse = repasseNivelUm.Status == StatusRepasseEacesso.NEGADO.GetDescription()
                    ? _dominioRepository.Buscar(x => x.DescricaoValor.Equals(StatusRepasseEacesso.NEGADO.GetDescription())).FirstOrDefault().Id
                    : _dominioRepository.Buscar(x => x.DescricaoValor.Equals(StatusRepasseEacesso.APROVADO_NIVEL_UM.GetDescription())).FirstOrDefault().Id,
                Descricao = repasseNivelUm.Status == StatusRepasseEacesso.NEGADO.GetDescription()
                    ? aprovarRepasseDto.Motivo
                    : string.Empty,
                DataAlteracao = DateTime.Now,
                Usuario = repasseNivelUm.Usuario
            };

            repasseNivelUm.MotivoNegacao = repasseNivelUm.Status == StatusRepasseEacesso.NEGADO.GetDescription() ? aprovarRepasseDto.Motivo : null;
            repasseNivelUm.LogsRepasse.Add(log);

            if (aprovarRepasseDto.Status == StatusRepasseEacesso.APROVADO_NIVEL_DOIS.GetDescription() 
                && !CelulaDestinoPagadoraExterior(repasseNivelUm.IdCelulaDestino))
            {
                int idDominioRepasseAprovadoNivelDois = _dominioRepository.Buscar((int)StatusRepasseEacesso.APROVADO_NIVEL_DOIS, StatusRepasseEacesso.VL_TIPO_DOMINIO.GetDescription()).Id;
                log = new LogRepasse
                {
                    Id = 0,
                    IdRepasse = repasseNivelUm.Id,
                    IdStatusRepasse = idDominioRepasseAprovadoNivelDois,
                    Descricao = string.Empty,
                    DataAlteracao = DateTime.Now,
                    Usuario = repasseNivelUm.Usuario
                };
                
                repasseNivelUm.LogsRepasse.Add(log);
                         
            }

            _repasseNivelUmRepository.Update(repasseNivelUm);
            _unitOfWork.Commit();
        }

        private void AtualizarRepasseNivelUmAposNegadoNivelDois(AprovarRepasseNivelDoisDto aprovarRepasseDto, RepasseNivelUm repasseNivelUm)
        {
            repasseNivelUm.Usuario = aprovarRepasseDto.Usuario;
            repasseNivelUm.Status = aprovarRepasseDto.Status;
            repasseNivelUm.MotivoNegacao = aprovarRepasseDto.Motivo;
            int idDominioRepasseNegado = _dominioRepository.Buscar((int)StatusRepasseEacesso.NEGADO, StatusRepasseEacesso.VL_TIPO_DOMINIO.GetDescription()).Id;

            AdicionarLogRepasse(repasseNivelUm.Id, idDominioRepasseNegado, aprovarRepasseDto.Motivo);
            _repasseNivelUmRepository.Update(repasseNivelUm);
            _unitOfWork.Commit();
        }


        private void AdicionarRepasseStfCorp(RepasseNivelUm repasseStfCorp, Repasse repasseEAcesso, int idDominioRepasseCadastrado)
        {
            var periodoVigente = _periodoRepasseRepository.BuscarPeriodoVigente();
            repasseStfCorp.Status = DefinirStatusRepasseStfCorp(repasseStfCorp);
            repasseStfCorp.IdRepasseEacesso = repasseEAcesso.Id;
            repasseStfCorp.DataLancamento = periodoVigente.DtLancamento;
            repasseStfCorp.ParcelaRepasse = $"1/{repasseEAcesso.QtdVezesRepetir.ToString()}";
            _repasseNivelUmRepository.Adicionar(repasseStfCorp);

            AdicionarLogRepasse(repasseStfCorp.Id, idDominioRepasseCadastrado, string.Empty);

           
            _unitOfWork.Commit();
        }

        private void AdicionarRepasseEacesso(Repasse repasse)
        {
            repasse.Status = DefinirStatusRepasseEAcesso(repasse);
            var descricaoOriginal = repasse.DescricaoProjeto;
            repasse.DescricaoProjeto = repasse.QtdVezesRepetir > 1
                ? repasse.DescricaoProjeto + $" - 1/{repasse.QtdVezesRepetir.ToString()}"
                : repasse.DescricaoProjeto;

            _repasseRepository.Adicionar(repasse);
            _unitOfWork.CommitLegado();

            repasse.DescricaoProjeto = descricaoOriginal;
        }

        private void AtualizarRepasseStfCorp(RepasseNivelUm repasse)
        {
            repasse.Status = DefinirStatusRepasseStfCorp(repasse);
            _repasseNivelUmRepository.Update(repasse);

            var idStatusRePasse = _dominioRepository.Buscar(x => x.DescricaoValor.Equals(StatusRepasseEacesso.REPASSE_EDITADO.GetDescription())).FirstOrDefault().Id;

            AdicionarLogRepasse(repasse.Id, idStatusRePasse, "");
        }

        private void AtualizarRepasseEacesso(Repasse repasse)
        {

            repasse.Status = DefinirStatusRepasseEAcesso(repasse);
            _repasseRepository.Update(repasse);
        }

        private void AtualizarRepasseEAcesso(RepasseNivelUm repasseNivelUm)
        {
            if (repasseNivelUm.IdRepasseEacesso != null && repasseNivelUm.IdRepasseEacesso > 0)
            {
                var repasseEacesso = _repasseRepository.BuscarPorId((int)repasseNivelUm.IdRepasseEacesso);
                repasseEacesso.Status = repasseNivelUm.Status;
                if (repasseNivelUm.Status == StatusRepasseEacesso.NEGADO.GetDescription())
                {
                    repasseEacesso.MotivoNegacao = repasseNivelUm.MotivoNegacao;
                }
                _repasseRepository.Update(repasseEacesso);
                _unitOfWork.CommitLegado();
            }
        }

        private string DefinirStatusRepasseStfCorp(RepasseNivelUm repasse)
        {
            if (repasse.Id == 0)
            {
                return repasse.IdCelulaOrigem == repasse.IdCelulaDestino && _repasseNivelUmRepository.ValidarUsuarioResponsavelPelaCelula(repasse.IdCelulaDestino)
                    ? StatusRepasseEacesso.APROVADO_NIVEL_UM.GetDescription()
                    : StatusRepasseEacesso.NAO_ANALISADO.GetDescription();
            }
            else
            {
                return repasse.Status == StatusRepasseEacesso.NEGADO.GetDescription()
                    ? StatusRepasseEacesso.NAO_ANALISADO.GetDescription()
                    : repasse.Status;
            }
        }

        private string DefinirStatusRepasseEAcesso(Repasse repasse)
        {
            if (repasse.Id == 0)
            {
                return StatusRepasseEacesso.NAO_ANALISADO.GetDescription();
            }
            else
            {
                return repasse.Status == StatusRepasseEacesso.NEGADO.GetDescription()
                    ? StatusRepasseEacesso.NAO_ANALISADO.GetDescription()
                    :
                repasse.Status;
            }
        }

        private LogRepasse AdicionarLogRepasse(int idRepasse, int idStatus, string motivo)
        {
            LogRepasse log = new LogRepasse
            {
                Id = 0,
                IdRepasse = idRepasse,
                IdStatusRepasse = idStatus,
                Descricao = motivo,
            };

            _logRepasseRepository.Adicionar(log);
            return log;
        }

        private void ValidarSeRepassePodeSerEditado(RepasseNivelUm repasse)
        {
            if (repasse.Status != StatusRepasseEacesso.NAO_ANALISADO.GetDescription() &&
                repasse.Status != StatusRepasseEacesso.NEGADO.GetDescription())
            {
                throw new Exception("Um repasse só pode ser editado se seu status estiver como Negado ou Não Analisado.");
            }
        }

        private void AdicionarParcelasDoRepasseStfCorp(RepasseNivelUm repasse)
        {
            RepasseNivelUm cloneRepasse = repasse.Clone();
            cloneRepasse.IdRepasseMae = repasse.Id;
            for (int i = 1; i < repasse.QtdVezesRepetir; i++)
            {
                cloneRepasse.Id = 0;
                cloneRepasse.DataRepasse = cloneRepasse.DataRepasse.AddMonths(1);
                _repasseNivelUmRepository.Adicionar(cloneRepasse);
                cloneRepasse = cloneRepasse.Clone();
            }
        }

        private void AdicionarParcelasDoRepasseEAcesso(Repasse repasse)
        {
            Repasse cloneRepasse = repasse.Clone();
            for (int i = 1; i < repasse.QtdVezesRepetir; i++)
            {
                cloneRepasse.Id = 0;
                cloneRepasse.DataRepasse = cloneRepasse.DataRepasse.AddMonths(1);
                _repasseRepository.Adicionar(cloneRepasse);
                cloneRepasse = cloneRepasse.Clone();
            }
        }


        private void AdicionarParcelasDeRepasseNoStfcorpENoEacesso(
            RepasseNivelUm repasseStfCorp, Repasse repasseEAcesso, int repasseCadastradoLogIdDominio
            )
        {
            int idDominioAprovadoNivelUm = _dominioRepository.Buscar((int)StatusRepasseEacesso.APROVADO_NIVEL_UM, StatusRepasseEacesso.VL_TIPO_DOMINIO.GetDescription()).Id;

            Repasse cloneRepasseEacesso = repasseEAcesso.Clone();
            cloneRepasseEacesso.IdRepasseMae = repasseEAcesso.Id;

            RepasseNivelUm cloneRepasseStfCorp = repasseStfCorp.Clone();
            cloneRepasseStfCorp.IdRepasseMae = repasseStfCorp.Id;
            cloneRepasseStfCorp.IdRepasseMaeEAcesso = repasseEAcesso.Id;

            for (int i = 1; i < repasseEAcesso.QtdVezesRepetir; i++)
            {
                cloneRepasseEacesso.Id = 0;
                cloneRepasseEacesso.DescricaoProjeto = $"{repasseEAcesso.DescricaoProjeto} - {i + 1}/{repasseEAcesso.QtdVezesRepetir.ToString()}";
                cloneRepasseEacesso.DataRepasse = cloneRepasseEacesso.DataRepasse.AddMonths(1);
                _repasseRepository.Adicionar(cloneRepasseEacesso);
                _unitOfWork.CommitLegado();
                cloneRepasseEacesso = cloneRepasseEacesso.Clone();


                cloneRepasseStfCorp.Id = 0;
                cloneRepasseStfCorp.DataRepasse = cloneRepasseStfCorp.DataRepasse.AddMonths(1);
                cloneRepasseStfCorp.DataLancamento = cloneRepasseStfCorp.DataLancamento.AddMonths(1);
                cloneRepasseStfCorp.IdRepasseEacesso = cloneRepasseEacesso.Id;
                cloneRepasseStfCorp.LogsRepasse = null;
                cloneRepasseStfCorp.ParcelaRepasse = $"{i + 1}/{repasseEAcesso.QtdVezesRepetir.ToString()}";

                //cloneRepasseStfCorp.DescricaoProjeto = $"{repasseEAcesso.DescricaoProjeto} - {i + 1}/{repasseEAcesso.QtdVezesRepetir.ToString()}";
                _repasseNivelUmRepository.Adicionar(cloneRepasseStfCorp);
                _unitOfWork.Commit();
                _repasseNivelUmRepository.DetachEntity(cloneRepasseStfCorp);
                AdicionarLogRepasse(cloneRepasseStfCorp.Id, repasseCadastradoLogIdDominio, String.Empty);

                if (cloneRepasseStfCorp.IdCelulaOrigem == cloneRepasseStfCorp.IdCelulaDestino)
                    AdicionarLogRepasse(cloneRepasseStfCorp.Id, idDominioAprovadoNivelUm, String.Empty);

                _unitOfWork.Commit();
                cloneRepasseStfCorp = cloneRepasseStfCorp.Clone();
            }
        }

        private void AprovarRepasseSeTemFuncionalidade(RepasseNivelUm repasseStfCorp)
        {
            var logins = _repasseNivelUmRepository.ObterLoginsComFuncionalidadeAprovarNivelDois();

            if (logins.Contains(_variables.UserName))
            {
                var aprovarRepasseDtoNivelUm = new AprovarRepasseDto
                {
                    Id = repasseStfCorp.Id,
                    Status = StatusRepasseEacesso.APROVADO_NIVEL_UM.GetDescription(),
                    Usuario = _variables.UserName
                };

                if (!CelulaDestinoPagadoraExterior(repasseStfCorp.IdCelulaDestino))
                    AprovarRepasseSemValidacaoCelulaExterior(aprovarRepasseDtoNivelUm);
                else
                    AprovarRepasse(aprovarRepasseDtoNivelUm);

                if (!CelulaDestinoPagadoraExterior(repasseStfCorp.IdCelulaDestino))
                {
                    var aprovarRepasseDtoNivelDois = new AprovarRepasseNivelDoisDto
                    {
                        Id = repasseStfCorp.Id,
                        Status = StatusRepasseEacesso.APROVADO_NIVEL_DOIS.GetDescription()
                    };
                    AprovarRepasseNivelDois(aprovarRepasseDtoNivelDois);
                }
            }
        }
    }
}
