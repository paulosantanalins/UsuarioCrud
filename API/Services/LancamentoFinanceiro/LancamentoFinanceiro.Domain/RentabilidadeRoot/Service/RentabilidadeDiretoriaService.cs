﻿using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Domain.RentabilidadeRoot.Dto;
using LancamentoFinanceiro.Domain.RentabilidadeRoot.Service.Interfaces;
using Microsoft.Extensions.Options;
using MoreLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Utils.Base;
using Utils.Connections;
using Utils.EacessoLegado.Models;
using Utils.EacessoLegado.Service;
using Utils.Relatorios.Models;

namespace LancamentoFinanceiro.Domain.RentabilidadeRoot.Service
{
    public class RentabilidadeDiretoriaService : IRentabilidadeDiretoriaService
    {
        protected readonly ILancamentoFinanceiroRepository _lancamentoFinanceiroRepository;
        protected readonly IItemLancamentoFinanceiroRepository _itemLancamentoFinanceiroRepository;
        protected readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly MicroServicosUrls _microServicosUrls;

        public RentabilidadeDiretoriaService(ILancamentoFinanceiroRepository lancamentoFinanceiroRepository,
                                             IItemLancamentoFinanceiroRepository itemLancamentoFinanceiroRepository,
                                             IOptions<ConnectionStrings> connectionStrings,
                                             MicroServicosUrls microServicosUrls)
        {
            _lancamentoFinanceiroRepository = lancamentoFinanceiroRepository;
            _itemLancamentoFinanceiroRepository = itemLancamentoFinanceiroRepository;
            _connectionStrings = connectionStrings;
            _microServicosUrls = microServicosUrls;
        }

        public List<ValoresRelatorioRentabilidadeDto> ObterInformacoesPorDiretoria(FiltroRelatorioRentabilidadeDiretoriaDto filtro)
        {
            var idCelulaSelecionada = filtro.IdsCelula.LastOrDefault();
            var proceduresService = new ProceduresService(_connectionStrings.Value.EacessoConnection);
            var celulaValidadas = proceduresService.ObterHierarquiaCelulas(filtro.IdsCelula.LastOrDefault());
            var idsCelulaValidadas = celulaValidadas.Select(x => x.IdCelula).Concat(celulaValidadas.Select(x => x.IdCelulaSup)).Distinct().ToList();
            filtro.IdsCelula = idsCelulaValidadas;
            #region variaveis
            var servicos = new List<ServicoContratadoRelatorioRentabilidadeModel>();
            var repassesPagos = new List<RepasseRelatorioRentabilidadeModel>();
            var repassesRecebidos = new List<RepasseRelatorioRentabilidadeModel>();
            List<ValoresRelatorioRentabilidadeDto> valoresRelatorio = new List<ValoresRelatorioRentabilidadeDto>();
            List<ValoresRelatorioRentabilidadeDto> valoresRelatorioCliente = new List<ValoresRelatorioRentabilidadeDto>();
            List<ValoresRelatorioRentabilidadeDto> valoresRelatorioCelula = new List<ValoresRelatorioRentabilidadeDto>();
            var itensLancamentosServico = new List<ItemLancamentoFinanceiro>();
            var itensLancamentosRepassesPagos = new List<ItemLancamentoFinanceiro>();
            var itensLancamentosRepassesRecebidos = new List<ItemLancamentoFinanceiro>();
            var celulas = new List<CelulaEacesso>();
            var celulasService = new CelulaEacessoService(_connectionStrings.Value.EacessoConnection);
            celulas = celulasService.ObterCelulasPorIds(filtro.IdsCelula);
            #endregion


            itensLancamentosServico = ObterItensLancamentoPorServico(filtro, out servicos);
            itensLancamentosRepassesPagos = ObterItensLancamentoRepassesPagos(filtro, out repassesPagos);
            itensLancamentosRepassesRecebidos = ObterItensLancamentoRepassesRecebidos(filtro, out repassesRecebidos);

            var itensAgrupados = itensLancamentosServico.GroupBy(x => x.IdServicoContratado);
            var itensAgrupadosRepassesPagos = itensLancamentosRepassesPagos.GroupBy(x => x.IdRepasse);
            var itensAgrupadosRepassesRecebidos = itensLancamentosRepassesRecebidos.GroupBy(x => x.IdRepasse);
            var repassesPagosAgrupados = repassesPagos.GroupBy(x => x.IdServicoContratado);
            var repassesRecebidosAgrupados = repassesRecebidos.GroupBy(x => x.IdServicoContratado);

            var idsServicos = itensAgrupados.Select(x => new ManipularValoresRelatorio { Id = x.Key.Value, Tipo = "S" }).Distinct().ToList();
            var idsRepassesPagos = itensAgrupadosRepassesPagos.Select(x => new ManipularValoresRelatorio { Id = x.Key.Value, Tipo = "R" }).Distinct();
            var idsRepassesRecebidos = itensAgrupadosRepassesRecebidos.Select(x => new ManipularValoresRelatorio { Id = x.Key.Value, Tipo = "R" }).Distinct();

            foreach (var item in repassesPagosAgrupados)
            {
                if (!idsServicos.Any(x => x.Id == item.Key))
                {
                    idsServicos.Add(new ManipularValoresRelatorio { Id = item.Key, Tipo = "S" });
                }
            }

            foreach (var item in repassesRecebidosAgrupados)
            {
                if (!idsServicos.Any(x => x.Id == item.Key))
                {
                    idsServicos.Add(new ManipularValoresRelatorio { Id = item.Key, Tipo = "S" });
                }
            }

            foreach (var idItem in idsServicos)
            {

                var servicoAtual = new ServicoContratadoRelatorioRentabilidadeModel();
                if (idItem.Tipo == "S")
                {
                    servicoAtual = servicos.FirstOrDefault(x => x.Id == idItem.Id);
                }
                else
                {
                    var idServico = repassesPagosAgrupados.FirstOrDefault(x => x.Any(y => y.Id == idItem.Id));
                    if (idServico != null)
                    {
                        servicoAtual = servicos.FirstOrDefault(x => x.Id == idServico.Key);
                    }
                    else
                    {
                        idServico = repassesRecebidosAgrupados.FirstOrDefault(x => x.Any(y => y.Id == idItem.Id));
                        if (idServico != null)
                        {
                            servicoAtual = servicos.FirstOrDefault(x => x.Id == idServico.Key);
                        }
                    }
                }
                IGrouping<int?, ItemLancamentoFinanceiro> itemServico = null;
                itemServico = itensAgrupados.FirstOrDefault(x => x.Key == servicoAtual.Id);
                if (itemServico == null)
                {
                    var idsPagos = repassesPagos.Where(x => x.IdServicoContratado == servicoAtual.Id).Select(x => x.Id).ToList();
                    var itens = itensLancamentosRepassesPagos.Where(x => idsPagos.Contains(x.IdRepasse.Value));
                    foreach (var item in itens)
                    {
                        item.IdServicoContratado = repassesPagos.FirstOrDefault(x => x.Id == item.IdRepasse).IdServicoContratado;
                    }
                    itemServico = itensLancamentosRepassesPagos.Where(x => idsPagos.Contains(x.IdRepasse.Value)).GroupBy(x => x.IdServicoContratado).FirstOrDefault();
                }
                if (itemServico == null)
                {
                    var idsPagos = repassesRecebidos.Where(x => x.IdServicoContratado == servicoAtual.Id).Select(x => x.Id).ToList();
                    var itens = itensLancamentosRepassesRecebidos.Where(x => idsPagos.Contains(x.IdRepasse.Value));
                    foreach (var item in itens)
                    {
                        item.IdServicoContratado = repassesRecebidos.FirstOrDefault(x => x.Id == item.IdRepasse).IdServicoContratado;
                    }
                    itemServico = itensLancamentosRepassesRecebidos.Where(x => idsPagos.Contains(x.IdRepasse.Value)).GroupBy(x => x.IdServicoContratado).FirstOrDefault();
                }
                if (itemServico == null)
                {
                    continue;
                }
                var valorRelatorio = new ValoresRelatorioRentabilidadeDto();
                valorRelatorio.Descricao = "SERVIÇO:" + servicos.FirstOrDefault(x => x.Id == servicoAtual.Id).DescEscopo;
                valorRelatorio.Tipo = "";
                valorRelatorio.Nivel = 4;
                valorRelatorio.IdServico = servicoAtual.Id;
                valorRelatorio.IdCelula = servicoAtual.IdCelula;
                valorRelatorio.IdCliente = servicos.FirstOrDefault(x => x.Id == servicoAtual.Id).IdCliente;

                CalcularValorFaturamentoServico(valorRelatorio, itemServico, celulas);
                CalcularValorAjusteFaturamento(valorRelatorio, itemServico, servicos.FirstOrDefault(x => x.Id == servicoAtual.Id), itensLancamentosRepassesPagos.ToList(), repassesPagos, celulas);
                CalcularValorFaturamentoAjusteServico(valorRelatorio, celulas);
                CalcularValorMarkup(valorRelatorio, itemServico, servicos.FirstOrDefault(x => x.Id == servicoAtual.Id), itensLancamentosRepassesPagos.ToList(), repassesPagos, celulas);
                CalcularValorDespesasGerais(valorRelatorio, itemServico, servicos.FirstOrDefault(x => x.Id == servicoAtual.Id), itensLancamentosRepassesPagos.ToList(), repassesPagos, celulas);
                CalcularValorDespesasSalario(valorRelatorio, itemServico, servicos.FirstOrDefault(x => x.Id == servicoAtual.Id), itensLancamentosRepassesPagos.ToList(), repassesPagos, celulas);
                CalcularValorDespesasCelula(valorRelatorio, itemServico, servicos.FirstOrDefault(x => x.Id == servicoAtual.Id), itensLancamentosRepassesPagos.ToList(), repassesPagos, celulas);
                CalcularValorTotalDespesa(valorRelatorio, itemServico);
                CalcularValorImpostoRepasse(valorRelatorio, itensLancamentosRepassesPagos.ToList(), repassesPagos, servicoAtual, celulas);
                CalcularValorRepassePago(valorRelatorio, itensLancamentosRepassesPagos.ToList(), repassesPagos, servicoAtual, celulas);
                CalcularValorRepasseRecebido(valorRelatorio, itensLancamentosRepassesRecebidos.ToList(), repassesRecebidos, servicoAtual, celulas);
                CalcularValorLucroServico(valorRelatorio);
                CalcularValorPorcentagemLucroServico(valorRelatorio);

                valoresRelatorio.Add(valorRelatorio);
            }

            return FiltrarTipoRelatorio(filtro, idCelulaSelecionada, celulaValidadas, valoresRelatorio);
        }

        private List<ValoresRelatorioRentabilidadeDto> FiltrarTipoRelatorio(FiltroRelatorioRentabilidadeDiretoriaDto filtro, int idCelulaSelecionada, List<ProcedureHierarquiaModel> celulaValidadas, List<ValoresRelatorioRentabilidadeDto> valoresRelatorio)
        {
            var result = PrepararResultadoRelatorio(valoresRelatorio, celulaValidadas, idCelulaSelecionada);
            if (filtro.TipoRelatorio == "CO")
            {
                result = result.Where(x => x.Tipo != "" && x.Tipo != "CLIENTE: ").ToList();
            }

            return result;
        }

        private List<ValoresRelatorioRentabilidadeDto> PrepararResultadoRelatorio(List<ValoresRelatorioRentabilidadeDto> valoresRelatorio, List<ProcedureHierarquiaModel> celulaValidadas, int idCelulaSelecionada)
        {
            var result = new List<ValoresRelatorioRentabilidadeDto>();
            var celulasDiretorias = celulaValidadas.ToList();

            foreach (var diretoria in celulasDiretorias.Select(x => x.IdCelulaSup).Distinct().ToList())
            {
                var celulasSubordinadas = celulasDiretorias.Where(x => x.IdCelulaSup == diretoria).Select(x => x.IdCelula).ToList();
                var valorRelatorioDiretoria = new ValoresRelatorioRentabilidadeDto();
                valorRelatorioDiretoria.Tipo = "GESTOR: ";
                valorRelatorioDiretoria.Descricao = valorRelatorioDiretoria.Tipo + diretoria;
                valorRelatorioDiretoria.Nivel = 1;
                valorRelatorioDiretoria.IdCliente = 0;
                valorRelatorioDiretoria.IdCelula = diretoria;
                PreencherValoresAgrupados(valorRelatorioDiretoria, valoresRelatorio.Where(x => celulasSubordinadas.Contains(x.IdCelula)).ToList());
                result.Add(valorRelatorioDiretoria);
                foreach (var celula in celulasSubordinadas)
                {
                    var valorRelatorioCelula = new ValoresRelatorioRentabilidadeDto();
                    var valoresPorCelula = valoresRelatorio.Where(x => x.IdCelula == celula).ToList();
                    valorRelatorioCelula.Tipo = "CELULA: ";
                    valorRelatorioCelula.Descricao = valorRelatorioCelula.Tipo + celula;
                    valorRelatorioCelula.Nivel = 2;
                    valorRelatorioCelula.IdCliente = 0;
                    valorRelatorioCelula.IdCelula = celula;
                    PreencherValoresAgrupados(valorRelatorioCelula, valoresPorCelula);
                    result.Add(valorRelatorioCelula);
                    var clientesPorCelula = valoresPorCelula.GroupBy(x => x.IdCliente).ToList();
                    var nomesClientes = ObterClientesPorIds(clientesPorCelula.Select(x => x.Key).ToList());
                    nomesClientes = nomesClientes.OrderBy(x => x.Nome).ToList();
                    foreach (var cliente in clientesPorCelula)
                    {
                        var valorRelatorioCliente = new ValoresRelatorioRentabilidadeDto();
                        valorRelatorioCliente.Tipo = "CLIENTE: ";
                        valorRelatorioCliente.Descricao = valorRelatorioCliente.Tipo + cliente.Key.ToString();
                        valorRelatorioCliente.Nivel = 3;
                        valorRelatorioCliente.IdCliente = cliente.Key;
                        valorRelatorioCliente.IdCelula = celula;
                        var servicosPorCliente = cliente.Select(x => x).ToList();
                        PreencherValorDespesaCelulaPorServico(valorRelatorioCelula, servicosPorCliente);
                        PreencherValoresAgrupados(valorRelatorioCliente, cliente);
                        result.Add(valorRelatorioCliente);
                        result.AddRange(servicosPorCliente);
                    }
                }
            }


            var valorRelatorioGeral = new ValoresRelatorioRentabilidadeDto();
            valorRelatorioGeral.Tipo = "TOTAL GERAL: ";
            valorRelatorioGeral.Descricao = valorRelatorioGeral.Tipo;
            valorRelatorioGeral.Nivel = 1;
            valorRelatorioGeral.IdCliente = 0;
            valorRelatorioGeral.IdCelula = 0;
            PreencherValoresAgrupados(valorRelatorioGeral, valoresRelatorio);
            result.Add(valorRelatorioGeral);


            return result;
        }

        private static void PreencherValorDespesaCelulaPorServico(ValoresRelatorioRentabilidadeDto valorRelatorioCelula, List<ValoresRelatorioRentabilidadeDto> servicosPorCliente)
        {
            if (valorRelatorioCelula.VlFatAjustTotal > 0)
            {
                foreach (var servicoCliente in servicosPorCliente)
                {
                    servicoCliente.VlDespCel = Math.Round(Math.Round((servicoCliente.VlFatAjustTotal * valorRelatorioCelula.VlDespCel), 2) / valorRelatorioCelula.VlFatAjustTotal, 2);
                }
            }
        }

        private static void PreencherValoresAgrupados(ValoresRelatorioRentabilidadeDto valorRelatorioCliente, List<ValoresRelatorioRentabilidadeDto> valoresAgrupados)
        {
            valorRelatorioCliente.VlAjustFat = valoresAgrupados.Sum(y => y.VlAjustFat);
            valorRelatorioCliente.VlDespCel = valoresAgrupados.Sum(y => y.VlDespCel);
            valorRelatorioCliente.VlDespGeral = valoresAgrupados.Sum(y => y.VlDespGeral);
            valorRelatorioCliente.VlDespSal = valoresAgrupados.Sum(y => y.VlDespSal);
            valorRelatorioCliente.VlFat = valoresAgrupados.Sum(y => y.VlFat);
            valorRelatorioCliente.VlFatAjustTotal = valoresAgrupados.Sum(y => y.VlFatAjustTotal);
            valorRelatorioCliente.VlLucro = valoresAgrupados.Sum(y => y.VlLucro);
            valorRelatorioCliente.VlLucroPercent = valoresAgrupados.Sum(y => y.VlLucroPercent);
            valorRelatorioCliente.VlMarkUp = valoresAgrupados.Sum(y => y.VlMarkUp);
            valorRelatorioCliente.VlRepPag = valoresAgrupados.Sum(y => y.VlRepPag);
            valorRelatorioCliente.VlRepRec = valoresAgrupados.Sum(y => y.VlRepRec);
            valorRelatorioCliente.VlRepPagImp = valoresAgrupados.Sum(y => y.VlRepPagImp);
            valorRelatorioCliente.VlTotalDesp = valoresAgrupados.Sum(y => y.VlTotalDesp);
            valorRelatorioCliente.VlFatIr = valoresAgrupados.Sum(y => y.VlFatIr);
            valorRelatorioCliente.VlMarkUpIr = valoresAgrupados.Sum(y => y.VlMarkUpIr);
        }
        private static void PreencherValoresAgrupados(ValoresRelatorioRentabilidadeDto valorRelatorioCliente, IGrouping<int, ValoresRelatorioRentabilidadeDto> valoresAgrupados)
        {
            valorRelatorioCliente.VlAjustFat = valoresAgrupados.Sum(y => y.VlAjustFat);
            valorRelatorioCliente.VlDespCel = valoresAgrupados.Sum(y => y.VlDespCel);
            valorRelatorioCliente.VlDespGeral = valoresAgrupados.Sum(y => y.VlDespGeral);
            valorRelatorioCliente.VlDespSal = valoresAgrupados.Sum(y => y.VlDespSal);
            valorRelatorioCliente.VlFat = valoresAgrupados.Sum(y => y.VlFat);
            valorRelatorioCliente.VlFatAjustTotal = valoresAgrupados.Sum(y => y.VlFatAjustTotal);
            valorRelatorioCliente.VlLucro = valoresAgrupados.Sum(y => y.VlLucro);
            valorRelatorioCliente.VlLucroPercent = valoresAgrupados.Sum(y => y.VlLucroPercent);
            valorRelatorioCliente.VlMarkUp = valoresAgrupados.Sum(y => y.VlMarkUp);
            valorRelatorioCliente.VlRepPag = valoresAgrupados.Sum(y => y.VlRepPag);
            valorRelatorioCliente.VlRepRec = valoresAgrupados.Sum(y => y.VlRepRec);
            valorRelatorioCliente.VlRepPagImp = valoresAgrupados.Sum(y => y.VlRepPagImp);
            valorRelatorioCliente.VlTotalDesp = valoresAgrupados.Sum(y => y.VlTotalDesp);
            valorRelatorioCliente.VlFatIr = valoresAgrupados.Sum(y => y.VlFatIr);
            valorRelatorioCliente.VlMarkUpIr = valoresAgrupados.Sum(y => y.VlMarkUpIr);
        }

        #region ObterItensLancamento
        private List<ItemLancamentoFinanceiro> ObterItensLancamentoPorServico(FiltroRelatorioRentabilidadeDiretoriaDto filtro, out List<ServicoContratadoRelatorioRentabilidadeModel> servicos)
        {
            List<ItemLancamentoFinanceiro> itensLancamentos = new List<ItemLancamentoFinanceiro>();
            servicos = ObterServicosPorCelulas(filtro.IdsCelula);

            var servicosLista = SplitListServico(servicos);

            foreach (var servicosAgrupado in servicosLista)
            {
                itensLancamentos = _itemLancamentoFinanceiroRepository.ObterItensLancamentoPorIdServicoContratadoPorPeriodo(servicosAgrupado.Select(x => x.Id).ToList(), filtro.DtInicio, filtro.DtFim);
            }

            return itensLancamentos;
        }

        private List<ItemLancamentoFinanceiro> ObterItensLancamentoRepassesPagos(FiltroRelatorioRentabilidadeDiretoriaDto filtro, out List<RepasseRelatorioRentabilidadeModel> repassesPagos)
        {
            List<ItemLancamentoFinanceiro> itensLancamentos = new List<ItemLancamentoFinanceiro>();

            repassesPagos = ObterRepassesPagosPorCelulas(filtro.IdsCelula);
            var repassesLista = SplitListRepasse(repassesPagos);

            foreach (var repasseAgrupado in repassesLista)
            {
                itensLancamentos.AddRange(_itemLancamentoFinanceiroRepository.ObterItensLancamentoPorIdRepassePagoPorPeriodo(repasseAgrupado.Select(x => x.Id).ToList(), filtro.DtInicio, filtro.DtFim));
            }

            return itensLancamentos;
        }

        private List<ItemLancamentoFinanceiro> ObterItensLancamentoRepassesRecebidos(FiltroRelatorioRentabilidadeDiretoriaDto filtro, out List<RepasseRelatorioRentabilidadeModel> repassesRecebidos)
        {
            List<ItemLancamentoFinanceiro> itensLancamentos = new List<ItemLancamentoFinanceiro>();

            repassesRecebidos = ObterRepassesRecebidosPorCelulas(filtro.IdsCelula);
            var repassesLista = SplitListRepasse(repassesRecebidos);

            foreach (var repasseAgrupado in repassesLista)
            {
                itensLancamentos.AddRange(_itemLancamentoFinanceiroRepository.ObterItensLancamentoPorIdRepasseRecebidoPorPeriodo(repasseAgrupado.Select(x => x.Id).ToList(), filtro.DtInicio, filtro.DtFim));
            }

            return itensLancamentos;
        }
        #endregion

        #region Calculo valores relatorio

        private static void CalcularValorTotalDespesa(ValoresRelatorioRentabilidadeDto valoresRelatorio, IGrouping<int?, ItemLancamentoFinanceiro> item)
        {
            valoresRelatorio.VlTotalDesp = valoresRelatorio.VlDespCel + valoresRelatorio.VlDespGeral + valoresRelatorio.VlDespSal;
        }

        private static void CalcularValorFaturamentoServico(ValoresRelatorioRentabilidadeDto valoresRelatorio, IGrouping<int?, ItemLancamentoFinanceiro> item, List<CelulaEacesso> celulas)
        {
            var valorFaturamento = (decimal)0.0;
            var valorFaturamentoIr = (decimal)0.0;
            var valoresValidos = item.Where(x => x.LancamentoFinanceiro.DescricaoOrigemLancamento == "FT").ToList();
            foreach (var itemFaturamento in valoresValidos)
            {
                if (itemFaturamento.LancamentoFinanceiro.VlBaixado != itemFaturamento.LancamentoFinanceiro.VlOriginal)
                {
                    var percent = itemFaturamento.VlLancamento / itemFaturamento.LancamentoFinanceiro.VlOriginal;
                    var valor = (decimal)(itemFaturamento.LancamentoFinanceiro.VlBaixado * percent);
                    var valorIr = (decimal)(valor + (itemFaturamento.LancamentoFinanceiro.VlIr * percent));
                    valorFaturamento += valor;
                    valorFaturamentoIr += valorIr;
                }
                else
                {
                    var percent = itemFaturamento.VlLancamento / itemFaturamento.LancamentoFinanceiro.VlBaixado;
                    valorFaturamento = itemFaturamento.VlLancamento;
                    valorFaturamentoIr = (decimal)(itemFaturamento.VlLancamento + (itemFaturamento.LancamentoFinanceiro.VlIr * percent));
                }
            }
            if (valoresValidos != null && valoresValidos.Any())
            {
                valoresRelatorio.VlFat += valorFaturamento;
                valoresRelatorio.VlFatIr += valorFaturamentoIr;
            }
            else
            {
                valoresRelatorio.VlFat = 0;
            }
        }

        private void CalcularValorDespesasGerais(ValoresRelatorioRentabilidadeDto valorRelatorio, IGrouping<int?, ItemLancamentoFinanceiro> itemServico, ServicoContratadoRelatorioRentabilidadeModel servico, List<ItemLancamentoFinanceiro> itensLancamentosRepassesPagos, List<RepasseRelatorioRentabilidadeModel> repassesPagos, List<CelulaEacesso> celulas)
        {
            var tiposValidos = new string[] { "DG", "DF" };
            var somenteCompras = itemServico.Where(x => x.LancamentoFinanceiro.DescricaoOrigemLancamento == "CP").ToList();

            somenteCompras = somenteCompras.Where(x =>
                                                  x.LancamentoFinanceiro.IdTipoDespesa != null &&
                                                  tiposValidos.Contains(x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa) &&
                                                  (x.LancamentoFinanceiro.DescOrigemCompraEacesso != "GPD" || x.LancamentoFinanceiro.DescOrigemCompraEacesso == null)
                                                  ).ToList();

            valorRelatorio.VlDespGeral += somenteCompras.Sum(x => x.VlLancamento);
            var repassesPorServico = repassesPagos.Where(x => x.IdServicoContratado == itemServico.Key).Select(x => x.Id).ToList();

            CalcularValorDespesasGeraisRepasse(valorRelatorio, servico, itensLancamentosRepassesPagos.Where(x => repassesPorServico.Contains(x.IdRepasse.Value)).ToList(), celulas);
        }

        private void CalcularValorMarkup(ValoresRelatorioRentabilidadeDto valorRelatorio, IGrouping<int?, ItemLancamentoFinanceiro> itemServico, ServicoContratadoRelatorioRentabilidadeModel servicoContratadoRelatorioRentabilidadeModel, List<ItemLancamentoFinanceiro> itensLancamentosRepassesPagos, List<RepasseRelatorioRentabilidadeModel> repassesPagos, List<CelulaEacesso> celulas)
        {
            var repassesPorServico = repassesPagos.Where(x => x.IdServicoContratado == itemServico.Key).Select(x => x.Id).ToList();
            var itensValidos = itensLancamentosRepassesPagos.Where(x => repassesPorServico.Contains(x.IdRepasse.Value)).ToList();
            var lancamentosValidos = itensValidos.Where(x => x.LancamentoFinanceiro.IdTipoDespesa != null && x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "AJ").ToList();

            // var valorMarkupServico = (itemServico.Where(x => x.LancamentoFinanceiro.DescricaoOrigemLancamento == "FT").ToList().Sum(x => x.VlLancamento) * (Math.Round(servicoContratadoRelatorioRentabilidadeModel.VlMarkup / 100, 2)));
            var valorMarkupServico = valorRelatorio.VlFat * (Math.Round(servicoContratadoRelatorioRentabilidadeModel.VlMarkup / 100, 2));
            var valorMarkupServicoIr = valorRelatorio.VlFatIr * (Math.Round(servicoContratadoRelatorioRentabilidadeModel.VlMarkup / 100, 2));
            var valorMarkupRepasse = Math.Round((lancamentosValidos.Sum(x => (x.VlInc.Value - x.VlDesc.Value)) * (servicoContratadoRelatorioRentabilidadeModel.VlMarkup / 100)), 2);
            valorRelatorio.VlMarkUp = valorMarkupServico + valorMarkupRepasse;
            valorRelatorio.VlMarkUpIr = valorMarkupServicoIr + valorMarkupRepasse;
        }

        private void CalcularValorImpostoRepasse(ValoresRelatorioRentabilidadeDto valorRelatorio, List<ItemLancamentoFinanceiro> itensLancamentosRepassesPagos, List<RepasseRelatorioRentabilidadeModel> repassesPagos, ServicoContratadoRelatorioRentabilidadeModel servico, List<CelulaEacesso> celulas)
        {
            var tipoCelula = celulas.FirstOrDefault(x => x.IdCelula == servico.IdCelula);
            var tiposPossiveis = new List<string> { "PR", "IN", "PI", "CO", "CS", "IR", "FG", "BN" };
            var repassesPorServico = repassesPagos.Where(x => x.IdServicoContratado == servico.Id).Select(x => x.Id).ToList();

            var itensLancamentoRepassePorServico =
                itensLancamentosRepassesPagos.Where(x =>
                                                    repassesPorServico.Contains(x.IdRepasse.Value) &&
                                                    (tipoCelula.IdTipoCelula != 1 || servico.SiglaTipoServico != "ACC")
                                                    &&
                                                    tiposPossiveis.Contains(x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa)
                                                   );

            valorRelatorio.VlRepPagImp = itensLancamentoRepassePorServico.Sum(x => x.VlLancamento);
        }

        private void CalcularValorDespesasSalario(ValoresRelatorioRentabilidadeDto valorRelatorio, IGrouping<int?, ItemLancamentoFinanceiro> itemServico, ServicoContratadoRelatorioRentabilidadeModel servico, List<ItemLancamentoFinanceiro> itensLancamentosRepassesPagos, List<RepasseRelatorioRentabilidadeModel> repassesPagos, List<CelulaEacesso> celulas)
        {
            var tipoCelula = celulas.FirstOrDefault(x => x.IdCelula == servico.IdCelula);
            var somenteCompras = itemServico.Where(x => x.LancamentoFinanceiro.DescricaoOrigemLancamento == "CP").ToList();
            somenteCompras = somenteCompras.Where(x => (x.LancamentoFinanceiro.IdTipoDespesa != null) &&
            (
                (x.LancamentoFinanceiro.DescOrigemCompraEacesso != "GPD" || x.LancamentoFinanceiro.DescOrigemCompraEacesso == null)
                &&
                (
                    (x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "SP") ||
                    (x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "SG" && tipoCelula.IdTipoCelula != 1)
                )
                &&
                (servico.SiglaTipoServico != "ACC" || tipoCelula.IdTipoCelula != 1)
            )
            ).ToList();

            valorRelatorio.VlDespSal += somenteCompras.Sum(x => x.VlLancamento);

            //var repassesPorServico = repassesPagos.Where(x => x.IdServicoContratado == itemServico.Key).Select(x => x.Id).ToList();

            //CalcularValorDespesasSalarioRepasse(valorRelatorio, servico, itensLancamentosRepassesPagos.Where(x => repassesPorServico.Contains(x.IdRepasse.Value)).ToList(), celulas);
        }

        private void CalcularValorAjusteFaturamento(ValoresRelatorioRentabilidadeDto valorRelatorio, IGrouping<int?, ItemLancamentoFinanceiro> itemServico, ServicoContratadoRelatorioRentabilidadeModel servico, List<ItemLancamentoFinanceiro> itensLancamentosRepassesPagos, List<RepasseRelatorioRentabilidadeModel> repassesPagos, List<CelulaEacesso> celulas)
        {
            var repassesPorServico = repassesPagos.Where(x => x.IdServicoContratado == itemServico.Key).Select(x => x.Id).ToList();
            var itensValidos = itensLancamentosRepassesPagos.Where(x => repassesPorServico.Contains(x.IdRepasse.Value));

            var lancamentosValidos = itensValidos.Where(x => x.LancamentoFinanceiro.IdTipoDespesa != null
                                                        && x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "AJ"
                                                       ).ToList();

            valorRelatorio.VlAjustFat += lancamentosValidos.Sum(x => (x.VlInc.Value - x.VlDesc.Value));
        }

        private void CalcularValorDespesasCelula(ValoresRelatorioRentabilidadeDto valorRelatorio, IGrouping<int?, ItemLancamentoFinanceiro> itemServico, ServicoContratadoRelatorioRentabilidadeModel servico, List<ItemLancamentoFinanceiro> itensLancamentosRepassesPagos, List<RepasseRelatorioRentabilidadeModel> repassesPagos, List<CelulaEacesso> celulas)
        {
            var tipoCelula = celulas.FirstOrDefault(x => x.IdCelula == servico.IdCelula);
            var somenteCompras = itemServico.Where(x => x.LancamentoFinanceiro.DescricaoOrigemLancamento == "CP").ToList();
            somenteCompras = somenteCompras.Where(x => x.LancamentoFinanceiro.IdTipoDespesa != null &&
                                                       (
                                                           (x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "SG" && tipoCelula.IdTipoCelula == 1) ||
                                                           (x.LancamentoFinanceiro.DescOrigemCompraEacesso == "GPD") ||
                                                           (x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "SP" && tipoCelula.IdTipoCelula == 1 && servico.SiglaTipoServico == "ACC")
                                                       )
                                                       ).ToList();

            valorRelatorio.VlDespCel += somenteCompras.Sum(x => x.VlLancamento);
            var repassesPorServico = repassesPagos.Where(x => x.IdServicoContratado == itemServico.Key).Select(x => x.Id).ToList();


            CalcularValorDespesasCelulaRepasse(valorRelatorio, servico, itensLancamentosRepassesPagos.Where(x => repassesPorServico.Contains(x.IdRepasse.Value)).ToList(), celulas);
        }

        private void CalcularValorDespesasCelulaRepasse(ValoresRelatorioRentabilidadeDto valorRelatorio, ServicoContratadoRelatorioRentabilidadeModel servico, List<ItemLancamentoFinanceiro> itensLancamentoFinanceiro, List<CelulaEacesso> celulas)
        {
            var tipoCelula = celulas.FirstOrDefault(x => x.IdCelula == servico.IdCelula);
            var tiposPossiveis = new string[] { "PR", "IN", "PI", "CO", "CS", "IR", "FG", "BN" };
            var lancamentosValidos =
                itensLancamentoFinanceiro.Where(x => x.LancamentoFinanceiro.IdTipoDespesa != null &&
                                                (
                                                    (x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "SP" && tipoCelula.IdTipoCelula == 1 && servico.SiglaTipoServico == "ACC") ||
                                                    (x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "SG" && tipoCelula.IdTipoCelula == 1) ||
                                                    (
                                                        tiposPossiveis.Contains(x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa) && (tipoCelula.IdTipoCelula == 1 && servico.SiglaTipoServico == "ACC")
                                                    )
                                                )
                                                );
            valorRelatorio.VlDespCel += lancamentosValidos.Sum(x => x.VlLancamento);

        }

        private void CalcularValorDespesasGeraisRepasse(ValoresRelatorioRentabilidadeDto valorRelatorio, ServicoContratadoRelatorioRentabilidadeModel servico, List<ItemLancamentoFinanceiro> itensLancamentoFinanceiro, List<CelulaEacesso> celulas)
        {

            var lancamentosValidos = itensLancamentoFinanceiro.Where(x => x.LancamentoFinanceiro.IdTipoDespesa != null &&
                                                                    (x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "DG"
                                                                    || x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "DF")
                                                                    );
            valorRelatorio.VlDespGeral += lancamentosValidos.Sum(x => x.VlLancamento);
        }

        private void CalcularValorDespesasSalarioRepasse(ValoresRelatorioRentabilidadeDto valorRelatorio, ServicoContratadoRelatorioRentabilidadeModel servico, List<ItemLancamentoFinanceiro> itensLancamentoFinanceiro, List<CelulaEacesso> celulas)
        {
            var tipoCelula = celulas.FirstOrDefault(x => x.IdCelula == servico.IdCelula);
            var lancamentosValidos = itensLancamentoFinanceiro.Where(x => x.LancamentoFinanceiro.IdTipoDespesa != null &&
                                                                    (
                                                                        (
                                                                            (x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "SP" && (servico.SiglaTipoServico != "ACC" || tipoCelula.IdTipoCelula != 1)) ||
                                                                            (x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "SG" && tipoCelula.IdTipoCelula != 1)
                                                                        )
                                                                    )
                                                                    );

            valorRelatorio.VlDespSal += lancamentosValidos.Sum(x => x.VlLancamento);
        }

        private static void CalcularValorFaturamentoAjusteServico(ValoresRelatorioRentabilidadeDto valorRelatorio, List<CelulaEacesso> celulas)
        {
            valorRelatorio.VlFatAjustTotal += valorRelatorio.VlFat + valorRelatorio.VlAjustFat;
        }

        private static void CalcularValorRepassePago(ValoresRelatorioRentabilidadeDto valoresRelatorio, List<ItemLancamentoFinanceiro> itemLancamentoFinanceiros, List<RepasseRelatorioRentabilidadeModel> repassesPagos, ServicoContratadoRelatorioRentabilidadeModel servico, List<CelulaEacesso> celulas)
        {
            var repassesPorServico = repassesPagos.Where(x => x.IdServicoContratado == servico.Id).Select(x => x.Id).ToList();
            var itensLancamentoRepassePorServico = itemLancamentoFinanceiros.Where(x => repassesPorServico.Contains(x.IdRepasse.Value));

            valoresRelatorio.VlRepPag += itensLancamentoRepassePorServico.Where(x => x.LancamentoFinanceiro.DescricaoTipoLancamento == "D" && x.LancamentoFinanceiro.IdTipoDespesa != null && x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa == "RP").Sum(x => x.VlLancamento);
        }

        private static void CalcularValorRepasseRecebido(ValoresRelatorioRentabilidadeDto valoresRelatorio, List<ItemLancamentoFinanceiro> itemLancamentoFinanceiros, List<RepasseRelatorioRentabilidadeModel> repassesRecebidos, ServicoContratadoRelatorioRentabilidadeModel servico, List<CelulaEacesso> celulas)
        {
            var repassesPorServico = repassesRecebidos.Where(x => x.IdServicoContratado == servico.Id).Select(x => x.Id).ToList();
            var itensLancamentoRepassePorServico = itemLancamentoFinanceiros.Where(x => repassesPorServico.Contains(x.IdRepasse.Value)).ToList();
            var tiposPossiveis = new string[] { "RP", "PR", "IN", "PI", "CO", "CS", "IR", "FG", "BN" };
            valoresRelatorio.VlRepRec += itensLancamentoRepassePorServico.Where(x => x.LancamentoFinanceiro.DescricaoTipoLancamento == "C" && x.LancamentoFinanceiro.IdTipoDespesa != null && tiposPossiveis.Contains(x.LancamentoFinanceiro.TipoDespesa.SgTipoDespesa)).ToList().Sum(x => x.VlLancamento);

        }

        private static void CalcularValorLucroServico(ValoresRelatorioRentabilidadeDto valoresRelatorio)
        {
            valoresRelatorio.VlLucro = (
                valoresRelatorio.VlFat +
                valoresRelatorio.VlAjustFat +
                valoresRelatorio.VlRepRec -
                valoresRelatorio.VlMarkUp -
                valoresRelatorio.VlDespGeral -
                valoresRelatorio.VlDespSal -
                valoresRelatorio.VlDespCel -
                valoresRelatorio.VlRepPag -

                //esse valor ainda nao existe
                valoresRelatorio.VlRepPagImp
                );
        }

        private static void CalcularValorPorcentagemLucroServico(ValoresRelatorioRentabilidadeDto valoresRelatorio)
        {
            if (valoresRelatorio.VlFat + valoresRelatorio.VlAjustFat <= 0)
            {
                valoresRelatorio.VlLucroPercent = 0;
            }
            else
            {
                valoresRelatorio.VlLucroPercent = Math.Round((valoresRelatorio.VlLucro / (valoresRelatorio.VlFat + valoresRelatorio.VlAjustFat)) * 100, 2);
            }
        }
        #endregion

        #region Obter Servicos e Repasses

        private string ConstruirQueryString(List<int> idsCelula)
        {
            var query = "?";
            foreach (var idCelula in idsCelula)
            {
                query += "idsCelula=" + idCelula.ToString() + "&";
            }
            return query.Substring(0, query.Length - 1);
        }

        private List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicosPorCelulas(List<int> idsCelula)
        {
            var query = ConstruirQueryString(idsCelula);

            var client = new HttpClient();
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiServico);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/ServicoContratados/obter-pacotes-por-celulas-relatorio-diretoria" + query).Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //notification handler
                return null;
            }
            else
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<ServicoContratadoRelatorioRentabilidadeModel>>(jsonString);
            }
        }

        private List<RepasseRelatorioRentabilidadeModel> ObterRepassesPagosPorCelulas(List<int> idsCelula)
        {
            var query = ConstruirQueryString(idsCelula);
            var client = new HttpClient();
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiServico);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/Repasse/obter-repasses-destino-por-celulas-relatorio-diretoria" + query).Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //notification handler
                return null;
            }
            else
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<RepasseRelatorioRentabilidadeModel>>(jsonString);
            }
        }

        private List<RepasseRelatorioRentabilidadeModel> ObterRepassesRecebidosPorCelulas(List<int> idsCelula)
        {
            var query = ConstruirQueryString(idsCelula);
            var client = new HttpClient();
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiServico);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/Repasse/obter-repasses-origem-por-celulas-relatorio-diretoria" + query).Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //notification handler
                return null;
            }
            else
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<RepasseRelatorioRentabilidadeModel>>(jsonString);
            }
        }
        private List<MultiselectDto> ObterClientesPorIds(List<int> idsClientes)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(120);
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiCliente);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(idsClientes), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/cliente/ObterClientesPorIds", content).Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //notification handler
                return null;
            }
            else
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<MultiselectDto>>(jsonString);
            }

        }

        public static List<List<RepasseRelatorioRentabilidadeModel>> SplitListRepasse(List<RepasseRelatorioRentabilidadeModel> valores, int nSize = 4000)
        {
            var list = new List<List<RepasseRelatorioRentabilidadeModel>>();

            for (int i = 0; i < valores.Count; i += nSize)
            {
                list.Add(valores.GetRange(i, Math.Min(nSize, valores.Count - i)));
            }

            return list;
        }

        public static List<List<ServicoContratadoRelatorioRentabilidadeModel>> SplitListServico(List<ServicoContratadoRelatorioRentabilidadeModel> valores, int nSize = 4000)
        {
            var list = new List<List<ServicoContratadoRelatorioRentabilidadeModel>>();

            for (int i = 0; i < valores.Count; i += nSize)
            {
                list.Add(valores.GetRange(i, Math.Min(nSize, valores.Count - i)));
            }

            return list;
        }

        #endregion

    }
}
