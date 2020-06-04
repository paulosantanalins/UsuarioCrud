using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.Base;
using Utils.Relatorios.Models;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoPacoteServico
{
    public class ServicoContratadoRepository : BaseRepository<ServicoContratado>, IServicoContratadoRepository
    {
        private readonly GestaoServicoContext _gestaoServicoContext;
        private readonly IVariablesToken _variables;

        public ServicoContratadoRepository(
            GestaoServicoContext gestaoServicoContext,
            IVariablesToken variables
        ) : base(gestaoServicoContext, variables)
        {
            _gestaoServicoContext = gestaoServicoContext;
            _variables = variables;
        }

        public FiltroGenericoDto<ServicoContratadoDto> Filtrar(FiltroGenericoDto<ServicoContratadoDto> filtro)
        {
            var query = DbSet.AsNoTracking()
                .Include(x => x.VinculoServicoCelulaComercial)
               .Include(x => x.EscopoServico)
               .Include(x => x.Contrato)
               .AsQueryable();

            var dados = query.Select(p => new ServicoContratadoDto
            {
                Id = p.Id,
                DescPortfolio = p.EscopoServico.NmEscopoServico,
                NrAssetSalesForce = p.Contrato.NrAssetSalesForce,
                IdContrato = p.IdContrato,
                DtInicial = p.DtInicial,
                IdCelula = p.IdCelula,
                DtFinal = p.DtFinal,
                Usuario = p.Usuario,
                DataAlteracao = p.DataAlteracao,
                CelulasComerciaisResponsaveis = ObterCelulaComercialPorServicoContratado(p),
                IdMoedaContrato = p.Contrato.IdMoeda,
                DataFinalContrato = p.Contrato.DtFinalizacao,
                DataInicialContrato = p.Contrato.DtInicial
            });

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                dados = dados.Where(x =>
                  x.Id.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                 (x.IdCelulaComercial.HasValue ? (x.IdCelulaComercial.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())) : false) ||
                 (x.IdCelula.HasValue ? (x.IdCelula.ToString().Contains(filtro.ValorParaFiltrar)) : false) ||
                 (x.DescPortfolio != null ? x.DescPortfolio.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : false) ||
                 (x.DescCliente != null ? x.DescCliente.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : false));
            }

            filtro.Total = dados.Count();

            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                dados = dados.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                dados = dados.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else
            {
                dados = dados.OrderBy(x => x.Id);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        private string ObterCelulaComercialPorServicoContratado(ServicoContratado servicoContratado)
        {
            var celulas = string.Join(", ", servicoContratado.VinculoServicoCelulaComercial.Select(x => x.IdCelulaComercial));
            return celulas;
        }

        public int Validar(ServicoContratado servicoContratado)
        {
            var result = DbSet.FirstOrDefault(x => (x.Id != servicoContratado.Id) &&
                                          (x.IdContrato == servicoContratado.IdContrato) &&
                                          (x.IdEscopoServico == servicoContratado.IdEscopoServico) &&
                                          (x.IdEmpresa == servicoContratado.IdEmpresa) &&
                                          (x.IdFilial == servicoContratado.IdFilial));
            if (result != null)
            {
                return result.Id;
            }
            return 0;
        }

        public ServicoContratado BuscarComInclude(int id)
        {
            var result = DbSet
                .Include(x => x.VinculoMarkupServicosContratados)
                .Include(x => x.EscopoServico)
                            .Include(x => x.Contrato)
                                    .ThenInclude(x => x.ServicoContratados)
                                .FirstOrDefault(x => x.Id == id);
            //Faltando relacionamento com Filial
            return result;
        }

        public List<MultiselectDto> PreencherComboServicoContratadoPorCelulaCliente(int idCelula, int idCliente)
        {
            var query = DbSet
                .Include(x => x.DeParaServicos)
                .Include(x => x.EscopoServico)
                .Include(x => x.Contrato)
                .Where(x => x.IdCelula == idCelula && x.Contrato.ClientesContratos.Any(y => y.IdCliente == idCliente));

            var result = query.Select(x => new MultiselectDto { Id = x.Id, Nome = x.DescricaoServicoContratado, IdSecundario = x.Contrato.IdMoeda }).ToList();
            return result;
        }

        public List<MultiselectDto> ObterServicoContratadoPorCliente(int idCliente)
        {
            var query = DbSet
                .Include(x => x.EscopoServico)
                .Include(x => x.Contrato)
                .Where(x => x.Contrato.ClientesContratos.Any(y => y.IdCliente == idCliente));

            var result = query.Select(x => new MultiselectDto { Id = x.Id, Nome = x.EscopoServico.NmEscopoServico }).ToList();
            return result;
        }

        public int? VerificarServicoContratadoComercialUnicoPorContrato(int idContrato)
        {
            var result = DbSet.Where(x => x.IdContrato == idContrato &&
                            x.DescTipoCelula == "COM");

            if (result != null && result.Count() == 1)
            {
                return result.FirstOrDefault().Id;
            }
            return null;
        }

        public int? ObterCelulaComercialVigenteContrato(int idContrato)
        {
            var result = DbSet.Where(x => x.IdContrato == idContrato &&
                            x.DescTipoCelula == "COM").OrderByDescending(x => x.DtInicial);

            if (result != null && result.Count() == 1)
            {
                return result.FirstOrDefault().IdCelula;
            }
            return null;
        }

        public List<MultiselectDto> ObterServicoContratadoPorCelula(int idCelula)
        {
            var query = DbSet
                .Include(x => x.EscopoServico)
                .Include(x => x.Contrato)
                .AsNoTracking();

            query = query.Where(x => x.IdCelula == idCelula);

            var result = query.Select(x => new MultiselectDto { Id = x.Id, Nome = x.EscopoServico.NmEscopoServico, IdSecundario = x.Contrato.ClientesContratos.FirstOrDefault().IdCliente }).ToList();
            return result;
        }

        public List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicoContratadoPorCelulaRelatorioRentabilidade(int idCelula)
        {
            var query = DbSet
                .Include(x => x.EscopoServico)
                .Include(x => x.Contrato)
                .Include(x => x.VinculoMarkupServicosContratados)
                .Include(x => x.DeParaServicos)
                .AsNoTracking();

            query = query.Where(x => x.IdCelula == idCelula);

            var result = query.Select(x => new ServicoContratadoRelatorioRentabilidadeModel
            {
                Id = x.Id,
                DescEscopo = x.IdEscopoServico != null ? x.EscopoServico.NmEscopoServico : x.DeParaServicos.FirstOrDefault().NmServicoEacesso,
                IdCliente = x.Contrato.ClientesContratos.FirstOrDefault().IdCliente,
                VlMarkup = x.VinculoMarkupServicosContratados.Any() ? x.VinculoMarkupServicosContratados.OrderByDescending(y => y.Id).FirstOrDefault().VlMarkup : 0,
                IdCelula = idCelula,
                DescTipoCelula = x.DescTipoCelula,
                SiglaTipoServico = x.DeParaServicos.Any() ? x.DeParaServicos.FirstOrDefault().DescTipoServico : "",
                ValoresMarkup = x.VinculoMarkupServicosContratados.Select(vm => new VinculoMarkupModel
                {
                    DtFimVigencia = vm.DtFimVigencia,
                    DtInicioVigencia = vm.DtInicioVigencia,
                    IdServicoContratado = vm.IdServicoContratado,
                    VlMarkup = vm.VlMarkup
                }).ToList()
            }).ToList();
            return result;
        }

        public List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicoContratadoPorCelulaClienteRelatorioRentabilidade(int idCelula, int idCliente)
        {
            var query = DbSet
                .Include(x => x.EscopoServico)
                .Include(x => x.Contrato)
                .Include(x => x.VinculoMarkupServicosContratados)
                .Include(x => x.DeParaServicos)
                .AsNoTracking();

            query = query.Where(x => x.IdCelula == idCelula && x.Contrato.ClientesContratos.Any(y => y.IdCliente == idCliente));

            var result = query.Select(x => new ServicoContratadoRelatorioRentabilidadeModel
            {
                Id = x.Id,
                DescEscopo = x.IdEscopoServico != null ? x.EscopoServico.NmEscopoServico : x.DeParaServicos.FirstOrDefault().NmServicoEacesso,
                IdCliente = x.Contrato.ClientesContratos.FirstOrDefault().IdCliente,
                VlMarkup = x.VinculoMarkupServicosContratados.OrderByDescending(y => y.Id).FirstOrDefault().VlMarkup,
                IdCelula = idCelula,
                DescTipoCelula = x.DescTipoCelula,
                SiglaTipoServico = x.DeParaServicos.Any() ? x.DeParaServicos.FirstOrDefault().DescTipoServico : "",
                ValoresMarkup = x.VinculoMarkupServicosContratados.Select(vm => new VinculoMarkupModel
                {
                    DtFimVigencia = vm.DtFimVigencia,
                    DtInicioVigencia = vm.DtInicioVigencia,
                    IdServicoContratado = vm.IdServicoContratado,
                    VlMarkup = vm.VlMarkup
                }).ToList()
            }).ToList();

            return result;
        }

        public List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicoContratadoPorCelulasRelatorioDiretoria(List<int> idsCelula)
        {
            var query = DbSet
                .Include(x => x.EscopoServico)
                .Include(x => x.Contrato)
                .Include(x => x.VinculoMarkupServicosContratados)
                .Include(x => x.DeParaServicos)
                .AsNoTracking();

            query = query.Where(x => idsCelula.Contains(x.IdCelula)).OrderBy(x => x.IdCelula);

            return MontarRetornoQueryBuscarServicosRelatorioRentabilidade(query);
        }


        public FiltroGenericoDto<ServicoContratado> FiltrarAccordions(FiltroGenericoDto<ServicoContratado> filtro)
        {
            var query = DbSet.AsNoTracking()
               .Include(x => x.VinculoMarkupServicosContratados)
               .Include(x => x.DeParaServicos)
               .Include(x => x.EscopoServico)
               .Include(x => x.Contrato)
               .AsQueryable();


            query = query.Where(x => x.DescTipoCelula != "COM" && x.IdContrato == filtro.Id);

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                query = query.Where(x =>
                                     x.Id.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                                     (x.IdCelula.ToString().Contains(filtro.ValorParaFiltrar.ToUpper())) ||
                                     (x.EscopoServico.NmEscopoServico != null ? x.EscopoServico.NmEscopoServico.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : false) ||
                                     (x.DeParaServicos.Any() ? x.DeParaServicos.FirstOrDefault().DescEscopo.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : true)
                                     )
                                     ;
            }

            filtro.Total = query.Count();
            query = query.OrderBy(x => x.Id);

            filtro.Valores = query.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        private static List<ServicoContratadoRelatorioRentabilidadeModel> MontarRetornoQueryBuscarServicosRelatorioRentabilidade(IQueryable<ServicoContratado> query)
        {
            return query.Select(x => new ServicoContratadoRelatorioRentabilidadeModel
            {
                Id = x.Id,
                DescEscopo = x.IdEscopoServico != null ? x.EscopoServico.NmEscopoServico : x.DeParaServicos.FirstOrDefault().NmServicoEacesso,
                IdCliente = x.Contrato.ClientesContratos.FirstOrDefault().IdCliente,
                VlMarkup = x.VinculoMarkupServicosContratados.Any() ? x.VinculoMarkupServicosContratados.OrderByDescending(y => y.Id).FirstOrDefault().VlMarkup : 0,
                IdCelula = x.IdCelula,
                DescTipoCelula = x.DescTipoCelula,
                SiglaTipoServico = x.DeParaServicos.Any() ? x.DeParaServicos.FirstOrDefault().DescTipoServico : "",
                ValoresMarkup = x.VinculoMarkupServicosContratados.Select(vm => new VinculoMarkupModel
                {
                    DtFimVigencia = vm.DtFimVigencia,
                    DtInicioVigencia = vm.DtInicioVigencia,
                    IdServicoContratado = vm.IdServicoContratado,
                    VlMarkup = vm.VlMarkup
                }).ToList()
            }).ToList();
        }


        public ServicoContratado ObterServicoComercialVigenteContrato(int idContrato)
        {
            var result = DbSet.Where(x => x.IdContrato == idContrato &&
                            x.DescTipoCelula == "COM" &&
                            (
                                (x.DtFinal != null && x.DtFinal >= DateTime.Now && x.DtInicial <= DateTime.Now)
                                ||
                                (x.DtFinal == null)
                            )
                            ).OrderBy(x => x.DtInicial).FirstOrDefault();

            return result;
        }
    }
}
