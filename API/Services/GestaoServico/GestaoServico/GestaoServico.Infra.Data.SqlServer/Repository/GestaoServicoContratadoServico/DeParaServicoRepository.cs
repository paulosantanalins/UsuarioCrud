using Dapper;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Dto;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Repository;
using GestaoServico.Domain.OperacaoMigradaRoot.DTO;
using GestaoServico.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utils;
using Utils.Base;
using Utils.Connections;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoServicoContratadoServico
{
    public class DeParaServicoRepository : BaseRepository<DeParaServico>, IDeParaServicoRepository
    {
        protected IVariablesToken _variables;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        public DeParaServicoRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables, IOptions<ConnectionStrings> connectionStrings)
            : base(gestaoServicoContext, variables)
        {
            _connectionStrings = connectionStrings;
            _variables = variables;
        }

        public DeParaServico ObterDeParaPorId(int id)
        {
            var query = DbSet.Include(x => x.ServicoContratado)
                                .ThenInclude(x => x.EscopoServico)
                            .Include(x => x.ServicoContratado)
                                .ThenInclude(x => x.Contrato)
                            .Include(x => x.ServicoContratado)
                                .ThenInclude(x => x.VinculoMarkupServicosContratados)
                             .Where(x => x.Id == id);
            return query.FirstOrDefault();
        }

        public FiltroGenericoDtoBase<GridServicoMigradoDTO> Filtrar(FiltroGenericoDtoBase<GridServicoMigradoDTO> filtro)
        {
            //dar uma olhada
            var stringPendente = "PENDENTE";
            var stringMigrado = "MIGRADO";
            var stringMigradoAutomatico = "MIGRADO AUTOMATICO";

            var query = DbSet.Include(x => x.ServicoContratado)
                                .ThenInclude(x => x.Contrato)
                                    .ThenInclude(x => x.ClientesContratos)
                                        .ThenInclude(x => x.Contrato)
                             .Include(x => x.ServicoContratado)
                                .ThenInclude(x => x.VinculoMarkupServicosContratados)
                             .Include(x => x.ServicoContratado)
                                .ThenInclude(x => x.EscopoServico)
                                  .ThenInclude(x => x.PortfolioServico)
                              .Include(x => x.ServicoContratado)
                                .ThenInclude(x => x.DeParaServicos)
                             .AsNoTracking();


            if (filtro.ValorParaFiltrar != null)
            {
                query = query
                     .Where(x => x.ServicoContratado.EscopoServico.NmEscopoServico.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                     || x.ServicoContratado.IdCelula.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                     || x.ServicoContratado.EscopoServico.PortfolioServico.NmServico.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                     || x.DescEscopo.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                     || (
                               (x.DescStatus == "PE" && stringPendente.Contains(filtro.ValorParaFiltrar.ToUpper())) ||
                               (x.DescStatus == "MA" && stringMigradoAutomatico.Contains(filtro.ValorParaFiltrar.ToUpper())) ||
                               (x.DescStatus == "MI" && stringMigrado.Contains(filtro.ValorParaFiltrar.ToUpper()))
                        )
                     );
                //dar uma olhada
                //query = FiltrarDadosAlteracao(filtro, query);
            }

            var dados = query.Select(p => new GridServicoMigradoDTO
            {
                NmEscopo = p.ServicoContratado.EscopoServico != null ? p.ServicoContratado.EscopoServico.NmEscopoServico.ToUpper() : "",
                NmPortfolio = p.ServicoContratado.EscopoServico != null ? p.ServicoContratado.EscopoServico.PortfolioServico.NmServico.ToUpper() : "",
                Usuario = p.ServicoContratado.Usuario,
                DataAlteracao = p.ServicoContratado.DataAlteracao,
                Id = p.Id,
                IdServicoEacesso = p.IdServicoEacesso,
                DescStatus = p.DescStatus,
                IdCelula = p.ServicoContratado.IdCelula,
                NmServicoEacesso = p.ServicoContratado.DescricaoServicoContratado,
            });

            filtro.Total = dados.Count();
            dados = OrdenarPorCampoEscolhido(filtro, dados);
            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

            return filtro;
        }

        private static IQueryable<GridServicoMigradoDTO> OrdenarPorCampoEscolhido(FiltroGenericoDtoBase<GridServicoMigradoDTO> filtro, IQueryable<GridServicoMigradoDTO> dados)
        {
            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                dados = dados.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                dados = dados.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            } else
            {
                dados = dados.OrderBy(x => x.DescStatus == "MA").OrderBy(x => x.IdCelula).ThenBy(x => x.NmCliente).ThenBy(x => x.NmServicoEacesso);
            }

            return dados;
        }

        private static IQueryable<DeParaServico> FiltrarEscopoServico(FiltroGenericoDtoBase<GridServicoMigradoDTO> filtro, IQueryable<DeParaServico> query)
        {
            query = query.Where(x => x.ServicoContratado.EscopoServico.NmEscopoServico.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));
            return query;
        }

        private static IQueryable<DeParaServico> FiltrarEscopoServicoEacesso(FiltroGenericoDtoBase<GridServicoMigradoDTO> filtro, IQueryable<DeParaServico> query)
        {
            query = query.Where(x => x.DescEscopo.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));
            return query;
        }

        private static IQueryable<DeParaServico> FiltrarPortifolio(FiltroGenericoDtoBase<GridServicoMigradoDTO> filtro, IQueryable<DeParaServico> query)
        {
            query = query.Where(x => x.ServicoContratado.EscopoServico.PortfolioServico.NmServico.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));
            return query;
        }

        private static IQueryable<DeParaServico> FiltrarCliente(FiltroGenericoDtoBase<GridServicoMigradoDTO> filtro, IQueryable<DeParaServico> query)
        {
            query = query.Where(
                                x => true
                            //  x => x.ServicoContratado.Contrato.Cliente.NmFantasia != null
                            //? x.ServicoContratado.Contrato.Cliente.NmFantasia.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                            //: x.ServicoContratado.Contrato.Cliente.NmRazaoSocial.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                            );
            return query;
        }

        private static IQueryable<DeParaServico> FiltrarPorCelula(FiltroGenericoDtoBase<GridServicoMigradoDTO> filtro, IQueryable<DeParaServico> query)
        {
            query = query.Where(x => x.ServicoContratado.IdCelula.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));
            return query;
        }

        private static IQueryable<DeParaServico> FiltrarStatus(FiltroGenericoDtoBase<GridServicoMigradoDTO> filtro, IQueryable<DeParaServico> query)
        {
            //dar uma olhada
            var stringPendente = "PENDENTE";
            var stringMigrado = "MIGRADO";
            var stringMigradoAutomatico = "MIGRADO AUTOMATICO";

            return query.Where(x => x.DescStatus == "PE" && stringPendente.Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                              (x.DescStatus == "MA" && stringMigradoAutomatico.Contains(filtro.ValorParaFiltrar.ToUpper())) ||
                              (x.DescStatus == "MI" && stringMigrado.Contains(filtro.ValorParaFiltrar.ToUpper()))
                              );
        }

        private static IQueryable<DeParaServico> FiltrarDadosAlteracao(FiltroGenericoDtoBase<GridServicoMigradoDTO> filtro, IQueryable<DeParaServico> query)
        {
            return query.Where(x => x.DataAlteracao.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                               x.Usuario.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                               );
        }

        public int ObterIdServicoContratadoPorIdServicoEacesso(int idServicoEacesso)
        {
            return DbSet.Where(x => x.IdServicoEacesso == idServicoEacesso).Select(x => x.IdServicoContratado).FirstOrDefault();
        }

        public int ObterIdServicoContratadoPorIdServicoEacessoDapper(int idServicoEacesso)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.DefaultConnection))
            {
                string sQuery = "SELECT IDSERVICOCONTRATADO " +
                                "FROM tbldeparaservico WHERE IDSERVICOEACESSO = @idServicoEacesso";
                dbConnection.Open();
                return dbConnection.QueryFirstOrDefault<int>(sQuery, new { idServicoEacesso }, null, 300, CommandType.Text);
            }
        }

        public FiltroGenericoDtoBase<AgrupamentoDTO> FiltrarAgrupamentos(FiltroGenericoDtoBase<AgrupamentoDTO> filtro)
        {
            var query = DbSet.Include(x => x.ServicoContratado).Where(x => x.ServicoContratado.IdGrupoDelivery == filtro.Id).AsNoTracking();

            var dados = query.Select(p => new AgrupamentoDTO
            {
                Id = p.Id,
                ServicoStfcorp = p.ServicoContratado.Id + " | " + p.ServicoContratado.DescricaoServicoContratado,
                ServicoEacesso = p.IdServicoEacesso.ToString()
            });

            filtro.Total = dados.Count();
            dados = dados.OrderBy(x => x.ServicoStfcorp);
            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

            return filtro;
        }
    }
}
