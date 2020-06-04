using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Repository;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Dto;
using GestaoServico.Domain.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utils;
using Utils.Base;
using Utils.Relatorios.Models;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Service
{
    public class RepasseService : IRepasseService
    {
        protected readonly IUnitOfWork _unitOfWork;
        private readonly IVariablesToken _variables;
        private readonly NotificationHandler _notificationHandler;
        private readonly IRepasseRepository _repasseRepository;
        private readonly MicroServicosUrls _microServicosUrls;

        public RepasseService(
            IUnitOfWork unitOfWork,
            IVariablesToken variables,
            IRepasseRepository repasseRepository,
            NotificationHandler notificationHandler,
            MicroServicosUrls microServicosUrls
        )
        {
            _unitOfWork = unitOfWork;
            _variables = variables;
            _repasseRepository = repasseRepository;
            _notificationHandler = notificationHandler;
            _microServicosUrls = microServicosUrls;
        }

        public FiltroGenericoDtoBase<GridRepasseDto> Filtrar(FiltroGenericoDtoBase<GridRepasseDto> filtro)
        {
            var result = _repasseRepository.Filtrar(filtro);
            return result;
        }

        public Repasse ObterRepassePorId(int id)
        {
            var result = _repasseRepository.BuscarPorId(id);
            return result;
        }

        public Repasse ObterRepassePorIdPorDataComDescricaoParcelada(int id, DateTime dataRepasse)
        {
            var result = _repasseRepository.ObterRepasseComDescricaoParcela(id, dataRepasse);
            return result;
        }

        public Repasse ObterRepassePorIdPorData(int id, DateTime dataRepasse)
        {
            var result = _repasseRepository.ObterRepasePorIdPorData(id, dataRepasse);
            return result;
        }

        public bool VerificarExistenciaParcelasFuturas(int id)
        {
            var result = _repasseRepository.VerificarExistenciaRepasseFuturo(id);
            return result;
        }

        public void CancelarRepasse(int id)
        {
            var result = _repasseRepository.ObterRepassesPorIdRelacionados(id);

            foreach (var repasse in result)
            {
                repasse.FlStatus = "CC";
                _repasseRepository.UpdateComposte(repasse);
            }
            _unitOfWork.Commit();
        }

        public void CancelarRepasseUnico(int id)
        {
            var result = _repasseRepository.Buscar(x => x.Id == id).FirstOrDefault();
            result.FlStatus = "CC";
            _repasseRepository.UpdateCompose(result, new object[] { result.Id, result.DtRepasse.Date });
            _unitOfWork.Commit();
        }

        public void PersistirRepasse(Repasse repasse, int vezesRepetidas)
        {
            var clonado = (Repasse)repasse.Clone();
            repasse.DtRepasse = repasse.DtRepasse.Date;
            if (vezesRepetidas > 1)
            {
                repasse.NrParcela = 1;
                repasse.DescProjeto = repasse.DescProjeto + " - PARCELA 1" + " / " + vezesRepetidas;
                _repasseRepository.Adicionar(repasse);
                for (int i = 1; i < vezesRepetidas; i++)
                {
                    var parcela = (Repasse)clonado.Clone();
                    parcela.NrParcela = i + 1;
                    parcela.IdRepasseMae = repasse.Id;
                    parcela.DtRepasse = repasse.DtRepasse.AddMonths(i).Date;
                    parcela.DtRepasseMae = repasse.DtRepasse.Date;
                    parcela.DescProjeto = parcela.DescProjeto + " - PARCELA " + (i + 1) + " / " + vezesRepetidas;
                    if (_repasseRepository.BuscarPorId(repasse.Id) != null) { 
                        _repasseRepository.Update(parcela);
                    }
                    else
                    {
                        _repasseRepository.Adicionar(parcela);

                    }
                }
            }
            else
            {
                repasse.NrParcela = null;
                if (_repasseRepository.BuscarPorId(repasse.Id) != null)
                {
                    _repasseRepository.Update(repasse);
                }
                else
                {
                    _repasseRepository.Adicionar(repasse);

                }
            }
            _unitOfWork.Commit();
        }

        public void AtualizarRepasse(Repasse repasse)
        {
            repasse.DtRepasse = repasse.DtRepasse.Date;
            _repasseRepository.UpdateCompose(repasse, new object[] { repasse.Id, repasse.DtRepasse });
            _unitOfWork.Commit();
        }

        public FiltroAprovarRepasseDto FiltrarAprovar(FiltroAprovarRepasseDto filtro)
        {
            var result = _repasseRepository.FiltrarAprovar(filtro);
            return result;
        }

        public void AtualizarRepassesFuturos(Repasse repasse)
        {
            var result = _repasseRepository.ObterRepassesPorIdRelacionados(repasse.Id);
            foreach (var repasseDB in result)
            {
                AtualizarValores(repasse, repasseDB);
                repasseDB.DtRepasse = repasseDB.DtRepasse.Date;
                _repasseRepository.UpdateCompose(repasseDB, new object[] { repasseDB.Id, repasseDB.DtRepasse });
            }
            _unitOfWork.Commit();
        }

        public void NegarRepasse(int id, string motivo)
        {
            var repasse = _repasseRepository.Buscar(x => x.Id == id).FirstOrDefault();
            repasse.DescMotivoNegacao = motivo;
            var statusAnterior = repasse.FlStatus;
            repasse.FlStatus = "NG";
            _repasseRepository.UpdateCompose(repasse, new object[] { repasse.Id, repasse.DtRepasse });
            _unitOfWork.Commit();

            RealizarConscistenciaRepasseRemocao(repasse, statusAnterior);
        }

        public void AprovarRepasse(int id)
        {
            var repasse = _repasseRepository.Buscar(x => x.Id == id).FirstOrDefault();
            var statusAnterior = repasse.FlStatus;
            repasse.DescMotivoNegacao = "";
            repasse.FlStatus = "AP";
            _repasseRepository.UpdateCompose(repasse, new object[] { repasse.Id, repasse.DtRepasse });
            _unitOfWork.Commit();
            RealizarConscistenciaRepasseAprovacao(repasse, statusAnterior);
        }

        public void ResetarRepasse(int id)
        {
            var repasse = _repasseRepository.Buscar(x => x.Id == id).FirstOrDefault();
            var statusAnterior = repasse.FlStatus;
            repasse.DescMotivoNegacao = "";
            repasse.FlStatus = "NA";
            _repasseRepository.UpdateCompose(repasse, new object[] { repasse.Id, repasse.DtRepasse });
            _unitOfWork.Commit();

            RealizarConscistenciaRepasseRemocao(repasse, statusAnterior);
        }

        private void RealizarConscistenciaRepasseAprovacao(Repasse repasse, string statusAnterior)
        {
            if (!RealizarPersistenciaLancamentosFinanceiros(repasse))
            {
                repasse.FlStatus = statusAnterior;
                AtualizarRepasse(repasse);
                _notificationHandler.AddMensagem("Error!", "Ocorreu um erro durante a remoção dos lançamentos financeiros");

            }
        }

        private void RealizarConscistenciaRepasseRemocao(Repasse repasse, string statusAnterior)
        {
            if (!RealizarRemocaoLancamentosFinanceiros(repasse.Id))
            {
                repasse.FlStatus = statusAnterior;
                AtualizarRepasse(repasse);
                _notificationHandler.AddMensagem("Error!", "Ocorreu um erro durante a remoção dos lançamentos financeiros");

            }
        }

        public List<GridRepasseAprovarDto> AprovarBlocoRepasse(List<GridRepasseAprovarDto> repasses)
        {
            List<GridRepasseAprovarDto> errados = new List<GridRepasseAprovarDto>();

            foreach (var repasse in repasses)
            {
                try
                {
                    var repasseAtual = _repasseRepository.Buscar(x => x.Id == repasse.Id).FirstOrDefault();
                    var statusAnterior = repasseAtual.FlStatus;
                    repasseAtual.DescMotivoNegacao = "";
                    repasseAtual.FlStatus = "AP";
                    _repasseRepository.UpdateCompose(repasseAtual, new object[] { repasseAtual.Id, repasseAtual.DtRepasse });
                    _unitOfWork.Commit();
                    if (!RealizarPersistenciaLancamentosFinanceiros(repasseAtual))
                    {
                        repasseAtual.FlStatus = statusAnterior;
                        AtualizarRepasse(repasseAtual);
                        errados.Add(repasse);
                    }
                }
                catch (Exception)
                {
                    errados.Add(repasse);
                    continue;
                }
            }

            return errados;

        }

        public void RemoverRepasse(Repasse repasse)
        {
            _repasseRepository.Remover(repasse);
            _unitOfWork.Commit();
        }

        private static List<LancamentoFinanceiroRepasseDto> GerarLancamentosFinanceiros(Repasse repasse)
        {
            var lancamentoBase = new LancamentoFinanceiroRepasseDto
            {
                DtBaixa = DateTime.Now.Date,
                DtLancamento = DateTime.Now.Date,
                CodigoColigada = null,
                DescricaoOrigemLancamento = "RP",
                IdLan = null,
                IdTipoDespesa = 13
            };

            var lancamentoCredito = (LancamentoFinanceiroRepasseDto)lancamentoBase.Clone();
            var lancamentoDebito = (LancamentoFinanceiroRepasseDto)lancamentoBase.Clone();

            lancamentoCredito.ItensLancamentoFinanceiro = new List<ItemLancamentoFinanceiroRepasseDto> {
                new ItemLancamentoFinanceiroRepasseDto
                {
                    DtRepasse = repasse.DtRepasse,
                    IdRepasse = repasse.Id,
                    //verificar
                    VlLancamento = repasse.VlTotal.HasValue ? repasse.VlTotal.Value : 0,
                    IdServicoContratado = repasse.IdServicoContratadoDestino
                }
            };
            lancamentoCredito.DescricaoTipoLancamento = "C";

            lancamentoDebito.ItensLancamentoFinanceiro = new List<ItemLancamentoFinanceiroRepasseDto> {
                new ItemLancamentoFinanceiroRepasseDto
                {
                    DtRepasse = repasse.DtRepasse,
                    IdRepasse = repasse.Id,
                    //verificar
                    VlLancamento = repasse.VlTotal.HasValue ? repasse.VlTotal.Value : 0,
                    IdServicoContratado = repasse.IdServicoContratadoOrigem
                }
            };
            lancamentoDebito.DescricaoTipoLancamento = "D";
            return new List<LancamentoFinanceiroRepasseDto> { lancamentoDebito, lancamentoCredito };

        }

        private bool RealizarPersistenciaLancamentosFinanceiros(Repasse repasse)
        {
            var lancamentos = GerarLancamentosFinanceiros(repasse);
            var client = new HttpClient();
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiLancamentoFinanceiro);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("usuario", _variables.UserName);
            var content = new StringContent(JsonConvert.SerializeObject(lancamentos), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/LancamentoFinanceiro/gerar-credito-debito", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool RealizarRemocaoLancamentosFinanceiros(int idRepasse)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiLancamentoFinanceiro);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("usuario", _variables.UserName);
            var response = client.DeleteAsync("api/LancamentoFinanceiro/" + idRepasse + "/remover-lancamentos-relacionados-repasse").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void AtualizarValores(Repasse repasse, Repasse repasseDB)
        {
            repasseDB.VlUnitario = repasse.VlUnitario;
            repasseDB.VlTotal = repasse.VlTotal;
            repasseDB.VlCustoProfissional = repasse.VlCustoProfissional;
            repasseDB.QtdRepasse = repasse.QtdRepasse;
            repasseDB.IdServicoContratadoOrigem = repasse.IdServicoContratadoOrigem;
            repasseDB.IdServicoContratadoDestino = repasse.IdServicoContratadoDestino;
            repasseDB.IdProfissional = repasse.IdProfissional;
            repasseDB.IdMoeda = repasse.IdMoeda;
            if (repasse.Id == repasseDB.Id)
            {
                repasseDB.DtRepasse = repasse.DtRepasse;
                repasseDB.DescProjeto = repasse.DescProjeto;
            }
            repasseDB.IdEpm = repasse.IdEpm;
            repasseDB.DescJustificativa = repasse.DescJustificativa;
            repasseDB.DescMotivoNegacao = repasse.DescMotivoNegacao;
            if (repasse.IdRepasseMae == null)
            {
                repasseDB.DtRepasseMae = repasse.DtRepasse;
            }
            repasseDB.IdTipoDespesa = repasse.IdTipoDespesa;
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorServicoContratadoOrigem(int idServicoContratado)
        {
            var result = _repasseRepository.ObterIdsRepassePorServicoContratadoOrigem(idServicoContratado);
            return result;
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorCelulaOrigem(int idCelula)
        {
            var result = _repasseRepository.ObterIdsRepassePorCelulaOrigem(idCelula);
            return result;
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorCelulaClienteOrigem(int idCelula, int idCliente)
        {
            var result = _repasseRepository.ObterIdsRepassePorCelulaClienteOrigem(idCelula, idCliente);
            return result;
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorServicoContratadoDestino(int idServicoContratado)
        {
            var result = _repasseRepository.ObterIdsRepassePorServicoContratadoDestino(idServicoContratado);
            return result;
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorCelulaDestino(int idCelula)
        {
            var result = _repasseRepository.ObterIdsRepassePorCelulaDestino(idCelula);
            return result;
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorCelulaClienteDestino(int idCelula, int idCliente)
        {
            var result = _repasseRepository.ObterIdsRepassePorCelulaClienteDestino(idCelula, idCliente);
            return result;
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorCelulasDestinoRelatorioDiretoria(List<int> idsCelula)
        {
            var result = _repasseRepository.ObterIdsRepassePorCelulasRelatorioDiretoriaDestino(idsCelula);
            return result;
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorCelulasOrigemRelatorioDiretoria(List<int> idsCelula)
        {
            var result = _repasseRepository.ObterIdsRepassePorCelulasRelatorioDiretoriaOrigem(idsCelula);
            return result;
        }
    }
}