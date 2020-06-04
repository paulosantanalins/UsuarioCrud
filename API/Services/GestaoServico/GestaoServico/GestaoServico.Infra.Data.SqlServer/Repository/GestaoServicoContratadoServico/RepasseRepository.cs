using GestaoServico.Domain.GestaoPacoteServicoRoot.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Utils;
using Utils.Base;
using Utils.Relatorios.Models;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoPacoteServico
{
    public class RepasseRepository : BaseRepository<Repasse>, IRepasseRepository
    {
        protected IVariablesToken _variables;
        public RepasseRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables)
            : base(gestaoServicoContext, variables)
        {
            _variables = variables;
        }

        public Repasse ObterRepasePorIdPorData(int id, DateTime data)
        {
            var query = DbSet//.Include(x => x.RepasseFilhos)
                            .Include(x => x.ServicoContratadoDestino)
                                .ThenInclude(x => x.Contrato)
                             .Include(x => x.ServicoContratadoOrigem)
                             .AsNoTracking();

            query = query.Where(x => x.Id == id && x.DtRepasse == data);
            return query.FirstOrDefault();
        }

        public Repasse ObterRepasseComDescricaoParcela(int id, DateTime data)
        {
            var query = DbSet.Include(x => x.RepasseMae)
                                //.ThenInclude(x => x.RepasseFilhos)
                             //.Include(x => x.RepasseFilhos)
                             .Include(x => x.ServicoContratadoDestino)
                                .ThenInclude(x => x.Contrato)
                                .Include(x => x.ServicoContratadoOrigem)
                             .AsNoTracking();

            query = query.Where(x => x.Id == id && x.DtRepasse == data);
            var result = query.FirstOrDefault();
            if (result.NrParcela != null)
            {
                //result.DescProjeto = result.DescProjeto ☺= result.DescProjeto + " - PARCELA " + result.NrParcela + " / " + result.RepasseMae.RepasseFilhos.Count;
                //result.DescProjeto = result.DescProjeto + " - PARCELA " + result.NrParcela + " / " + result.RepasseMae.RepasseFilhos.Count;
            }

            return result;
        }

        public FiltroGenericoDtoBase<GridRepasseDto> Filtrar(FiltroGenericoDtoBase<GridRepasseDto> filtro)
        {
            string aprovado = "APROVADO";
            string cancelado = "CANCELADO";
            string naoAnalisado = "NÃO ANALISADO";
            string negado = "NEGADO";

            var query = DbSet.Include(x => x.ServicoContratadoDestino)
                                .Include(x => x.ServicoContratadoDestino)
                                    .ThenInclude(x => x.Contrato)
                                        .ThenInclude(x => x.ClientesContratos)
                                 .Include(x => x.ServicoContratadoDestino)
                                    .ThenInclude(x => x.EscopoServico)
                             .Include(x => x.ServicoContratadoOrigem)
                             .AsNoTracking();

            var dados = query.Select(p => new GridRepasseDto
            {
                Id = p.Id,
                CelDestino = p.ServicoContratadoDestino.IdCelula.ToString(),
                CelOrigem = p.ServicoContratadoOrigem.IdCelula.ToString(),
                Usuario = p.Usuario,
                DataAlteracao = p.DataAlteracao,
                FlStatus = p.FlStatus,
                CliDestino = string.Join(",",p.ServicoContratadoDestino.Contrato.ClientesContratos.Select(x => x.IdCliente)),
                PacDestino = p.ServicoContratadoDestino.DescricaoServicoContratado,
                ValRepasse = p.VlTotal ?? 0,
                DtRepasse = p.DtRepasse
            });

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                dados = dados.Where(x => 
                                x.Id.ToString().Equals(filtro.ValorParaFiltrar)
                            || (x.CelDestino == filtro.ValorParaFiltrar)
                            || (x.CelOrigem == filtro.ValorParaFiltrar)
                            || x.CliDestino.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                            || x.PacDestino.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                            || x.ValRepasse.ToString().Replace(".", ",").Contains(filtro.ValorParaFiltrar.ToUpper())
                            || (x.FlStatus == "AP" && aprovado.Contains(filtro.ValorParaFiltrar.ToUpper()))
                            || (x.FlStatus == "CC" && cancelado.Contains(filtro.ValorParaFiltrar.ToUpper()))
                            || (x.FlStatus == "NG" && negado.Contains(filtro.ValorParaFiltrar.ToUpper()))
                            || (x.FlStatus == "NA" && naoAnalisado.Contains(filtro.ValorParaFiltrar.ToUpper()))
                            );
            }

            filtro.Total = dados.Count();

            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                dados =
                    filtro.CampoOrdenacao.Equals("Id") ? dados.OrderBy(x => x.Id) :
                    (filtro.CampoOrdenacao.Equals("CelDestino") ? dados.OrderBy(x => x.CelDestino) :
                    (filtro.CampoOrdenacao.Equals("CelOrigem") ? dados.OrderBy(x => x.CelOrigem) :
                    (filtro.CampoOrdenacao.Equals("PacDestino") ? dados.OrderBy(x => x.CelOrigem) :
                    (filtro.CampoOrdenacao.Equals("ValRepasse") ? dados.OrderBy(x => x.CelOrigem) :
                    (filtro.CampoOrdenacao.Equals("FlStatus") ? dados.OrderBy(x => x.CelOrigem) :
                    (filtro.CampoOrdenacao.Equals("CliDestino") ? dados.OrderBy(x => x.CelOrigem) :
                    dados))))));
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                dados =
                    filtro.CampoOrdenacao.Equals("Id") ? dados.OrderByDescending(x => x.Id) :
                    (filtro.CampoOrdenacao.Equals("CelDestino") ? dados.OrderByDescending(x => x.CelDestino) :
                    (filtro.CampoOrdenacao.Equals("CelOrigem") ? dados.OrderByDescending(x => x.CelOrigem) :
                    (filtro.CampoOrdenacao.Equals("PacDestino") ? dados.OrderByDescending(x => x.CelOrigem) :
                    (filtro.CampoOrdenacao.Equals("ValRepasse") ? dados.OrderByDescending(x => x.CelOrigem) :
                    (filtro.CampoOrdenacao.Equals("FlStatus") ? dados.OrderByDescending(x => x.CelOrigem) :
                    (filtro.CampoOrdenacao.Equals("CliDestino") ? dados.OrderByDescending(x => x.CelOrigem) :
                    dados))))));
            }
            else
            {
                dados = dados.OrderByDescending(x => x.Id);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        public List<Repasse> ObterRepassesPorIdRelacionados(int id)
        {
            var query = DbSet.Where(x => x.Id == id).FirstOrDefault();

            var result = DbSet.Where(x => x.Id == id || (x.IdRepasseMae == query.IdRepasseMae && x.DtRepasse > query.DtRepasse));

            return result.ToList();
        }

        public void UpdateComposte(Repasse repasse)
        {
            var entityDB = DbSet.Find(new object[] { repasse.Id, repasse.DtRepasse });
            entityDB.Usuario = _variables.UserName;
            entityDB.DataAlteracao = DateTime.Now;
            _context.Entry(entityDB).CurrentValues.SetValues(entityDB);
            DbSet.Update(entityDB);
        }

        public bool VerificarExistenciaRepasseFuturo(int id)
        {
            var query = DbSet//.Include(x => x.RepasseFilhos)
                             .Include(x => x.RepasseMae);

            var query2 = DbSet//.Include(x => x.RepasseFilhos)
                             .Include(x => x.RepasseMae);
            var result = query.Any(x => x.Id == id && (/*&& x.RepasseFilhos.Any(y => y.DtRepasse > x.DtRepasse) || */ (x.IdRepasseMae != null && query2.Any(z => z.DtRepasse > x.DtRepasse && z.IdRepasseMae == x.IdRepasseMae))));

            return result;
        }

        public FiltroAprovarRepasseDto FiltrarAprovar(FiltroAprovarRepasseDto filtro)
        {
            var query = DbSet.Include(x => x.ServicoContratadoDestino)
                                .ThenInclude(x => x.EscopoServico)
                                .Include(x => x.ServicoContratadoDestino)
                             .Include(x => x.ServicoContratadoOrigem)
                                .ThenInclude(x => x.EscopoServico)
                             .AsNoTracking();

            if (!filtro.AvaliarTodos)
                query = query.Where(x => x.ServicoContratadoDestino.IdCelula == filtro.Celula);

            if (filtro.AvaliarTodos)
                query = FiltrarPelaCelula(filtro, query);

            query = FiltrarPeloPeriodo(filtro, query);

            query = FiltrarPeloTipoDeConsulta(filtro, query);

            filtro.Total = query.Count();
            var dados = query.Select(p => new GridRepasseAprovarDto
            {
                Usuario = p.Usuario,
                DataAlteracao = p.DataAlteracao,
                Id = p.Id,
                Data = p.DtRepasse,
                Destino = p.ServicoContratadoDestino.IdCelula,
                Origem = p.ServicoContratadoOrigem.IdCelula,
                Projeto = filtro.TpRepasse == 1 ? p.ServicoContratadoDestino.EscopoServico.NmEscopoServico : p.ServicoContratadoOrigem.EscopoServico.NmEscopoServico,
                Cliente = "",
                VlUnitario = p.VlUnitario,
                VlRepasse = p.VlTotal,
                Horas = p.QtdRepasse,
                Aprovado = p.FlStatus
            });

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
                dados = dados.OrderBy(x => x.Aprovado.ElementAt(1)).ThenBy(x => x.Id);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList(); ;

            return filtro;
        }

        private static IQueryable<Repasse> FiltrarPeloTipoDeConsulta(FiltroAprovarRepasseDto filtro, IQueryable<Repasse> query)
        {
            switch (filtro.TpConsulta)
            {
                case 1:
                    query = query.Where(x => x.FlStatus == "NA");
                    break;
                case 2:
                    query = query.Where(x => x.FlStatus == "AP");
                    break;
                case 3:
                    query = query.Where(x => x.FlStatus == "NG");
                    break;
            }

            return query;
        }

        private static IQueryable<Repasse> FiltrarPeloPeriodo(FiltroAprovarRepasseDto filtro, IQueryable<Repasse> query)
        {
            if (filtro.DtInicial.HasValue && filtro.DtFinal.HasValue)
            {
                query = query.Where(x => x.DtRepasse >= filtro.DtInicial && x.DtRepasse <= filtro.DtFinal);
            }

            return query;
        }

        private static IQueryable<Repasse> FiltrarPelaCelula(FiltroAprovarRepasseDto filtro, IQueryable<Repasse> query)
        {
            if (!filtro.AvaliarTodos)
            {
                query = query.Where(x => x.ServicoContratadoDestino.IdCelula == filtro.Celula);
            }

            if (filtro.CelulasInteresse.Any())
            {
                query = filtro.TpRepasse == 1 ? query.Where(x => filtro.CelulasInteresse.Contains(x.ServicoContratadoDestino.IdCelula)) : query.Where(x => filtro.CelulasInteresse.Contains(x.ServicoContratadoOrigem.IdCelula));
            }

            return query;
        }

        #region tratamento query repasses relatorio rentabilidade
        public List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorServicoContratadoDestino(int idServicoContratado)
        {
            IQueryable<Repasse> query = QueryBaseFiltrarRepasseDestino();

            query = query.Where(x => x.IdServicoContratadoDestino == idServicoContratado);
            query = query.Where(x => x.FlStatus.Equals("AP"));
            return TratarRetornoQueryFiltrarRepasseDestino(query);
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorCelulaDestino(int idCelula)
        {
            IQueryable<Repasse> query = QueryBaseFiltrarRepasseDestino();

            query = query.Where(x => (x.ServicoContratadoDestino.IdCelula == idCelula));
            query = query.Where(x => x.FlStatus.Equals("AP"));
            return TratarRetornoQueryFiltrarRepasseDestino(query);
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorCelulaClienteDestino(int idCelula, int idCliente)
        {
            IQueryable<Repasse> query = QueryBaseFiltrarRepasseDestino();

            query = query.Where(x => x.ServicoContratadoDestino.IdCelula == idCelula);
            query = query.Where(x => x.ServicoContratadoDestino.Contrato.ClientesContratos.Any(y => y.IdCliente == idCliente));
            query = query.Where(x => x.FlStatus.Equals("AP"));


            return TratarRetornoQueryFiltrarRepasseDestino(query);
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorServicoContratadoOrigem(int idServicoContratado)
        {
            IQueryable<Repasse> query = QueryBaseFiltrarRepasseOrigem();

            query = query.Where(x => x.IdServicoContratadoOrigem == idServicoContratado);
            query = query.Where(x => x.FlStatus.Equals("AP"));
            return TratarRetornoQueryFiltrarRepasseOrigem(query);
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorCelulaOrigem(int idCelula)
        {
            IQueryable<Repasse> query = QueryBaseFiltrarRepasseOrigem();
            query = query.Where(x => x.ServicoContratadoOrigem.IdCelula == idCelula);
            query = query.Where(x => x.FlStatus.Equals("AP"));
            return TratarRetornoQueryFiltrarRepasseOrigem(query);
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorCelulaClienteOrigem(int idCelula, int idCliente)
        {
            var query = DbSet.Include(x => x.ServicoContratadoOrigem)
                                .ThenInclude(x => x.Contrato)
                             .AsNoTracking();

            query = query.Where(x => x.ServicoContratadoOrigem.IdCelula == idCelula);
            query = query.Where(x => x.ServicoContratadoOrigem.Contrato.ClientesContratos.Any(y => y.IdCliente == idCliente));
            query = query.Where(x => x.FlStatus.Equals("AP"));

            return TratarRetornoQueryFiltrarRepasseOrigem(query);
        }

        #region Relatorio diretoria
        public List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorCelulasRelatorioDiretoriaDestino(List<int> idsCelula)
        {
            IQueryable<Repasse> query = QueryBaseFiltrarRepasseDestino();

            query = query.Where(x => idsCelula.Contains(x.ServicoContratadoDestino.IdCelula)).OrderBy(x => x.ServicoContratadoDestino.IdCelula);
            query = query.Where(x => x.FlStatus.Equals("AP"));
            return TratarRetornoQueryFiltrarRepasseDestino(query);
        }

        public List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorCelulasRelatorioDiretoriaOrigem(List<int> idsCelula)
        {
            IQueryable<Repasse> query = QueryBaseFiltrarRepasseDestino();

            query = query.Where(x => idsCelula.Contains(x.ServicoContratadoOrigem.IdCelula)).OrderBy(x => x.ServicoContratadoOrigem.IdCelula);
            query = query.Where(x => x.FlStatus.Equals("AP"));
            return TratarRetornoQueryFiltrarRepasseOrigem(query);
        }

        #endregion

        private static List<RepasseRelatorioRentabilidadeModel> TratarRetornoQueryFiltrarRepasseOrigem(IQueryable<Repasse> query)
        {
            return query.Select(x => new RepasseRelatorioRentabilidadeModel
            {
                Id = x.Id,
                IdServicoContratado = x.ServicoContratadoOrigem.Id,
                VlMarkup = x.ServicoContratadoOrigem.VinculoMarkupServicosContratados.Any() ? x.ServicoContratadoOrigem.VinculoMarkupServicosContratados.OrderByDescending(y => y.Id).FirstOrDefault().VlMarkup : 0,
                DescEscopo = x.ServicoContratadoOrigem.IdEscopoServico != null ? x.ServicoContratadoOrigem.EscopoServico.NmEscopoServico : x.ServicoContratadoOrigem.DeParaServicos.FirstOrDefault().NmServicoEacesso,
                IdCelula = x.ServicoContratadoOrigem.IdCelula,
                IdCliente = x.ServicoContratadoOrigem.Contrato.ClientesContratos.FirstOrDefault().IdCliente,
                SiglaTipoServico = x.ServicoContratadoOrigem.DeParaServicos.Any() ? x.ServicoContratadoOrigem.DeParaServicos.FirstOrDefault().DescTipoServico : "",
                ValoresMarkup = x.ServicoContratadoOrigem.VinculoMarkupServicosContratados.Select(vm => new VinculoMarkupModel
                {
                    DtFimVigencia = vm.DtFimVigencia,
                    DtInicioVigencia = vm.DtInicioVigencia,
                    IdServicoContratado = vm.IdServicoContratado,
                    VlMarkup = vm.VlMarkup
                }).ToList()
            }).ToList();
        }

        private static List<RepasseRelatorioRentabilidadeModel> TratarRetornoQueryFiltrarRepasseDestino(IQueryable<Repasse> query)
        {
            return query.Select(x => new RepasseRelatorioRentabilidadeModel
            {
                Id = x.Id,
                IdServicoContratado = x.ServicoContratadoDestino.Id,
                VlMarkup = x.ServicoContratadoOrigem.VinculoMarkupServicosContratados.Any() ? x.ServicoContratadoDestino.VinculoMarkupServicosContratados.OrderByDescending(y => y.Id).FirstOrDefault().VlMarkup : 0,
                DescEscopo = x.ServicoContratadoDestino.IdEscopoServico != null ? x.ServicoContratadoDestino.EscopoServico.NmEscopoServico : x.ServicoContratadoDestino.DeParaServicos.FirstOrDefault().NmServicoEacesso,
                IdCelula = x.ServicoContratadoDestino.IdCelula,
                IdCliente = x.ServicoContratadoDestino.Contrato.ClientesContratos.FirstOrDefault().IdCliente,
                SiglaTipoServico = x.ServicoContratadoDestino.DeParaServicos.Any() ? x.ServicoContratadoDestino.DeParaServicos.FirstOrDefault().DescTipoServico : "",
                ValoresMarkup = x.ServicoContratadoDestino.VinculoMarkupServicosContratados.Select(vm => new VinculoMarkupModel
                {
                    DtFimVigencia = vm.DtFimVigencia,
                    DtInicioVigencia = vm.DtInicioVigencia,
                    IdServicoContratado = vm.IdServicoContratado,
                    VlMarkup = vm.VlMarkup
                }).ToList()
            }).ToList();
        }

        private IQueryable<Repasse> QueryBaseFiltrarRepasseOrigem()
        {
            return DbSet.Include(x => x.ServicoContratadoOrigem)
                                .ThenInclude(x => x.VinculoMarkupServicosContratados)
                             .Include(x => x.ServicoContratadoOrigem)
                                .ThenInclude(x => x.EscopoServico)
                             .Include(x => x.ServicoContratadoOrigem)
                                .ThenInclude(x => x.Contrato)
                            .Include(x => x.ServicoContratadoOrigem)
                                .ThenInclude(x => x.DeParaServicos)
                                .AsNoTracking();
        }

        private IQueryable<Repasse> QueryBaseFiltrarRepasseDestino()
        {
            return DbSet.Include(x => x.ServicoContratadoDestino)
                                .ThenInclude(x => x.VinculoMarkupServicosContratados)
                             .Include(x => x.ServicoContratadoDestino)
                                .ThenInclude(x => x.EscopoServico)
                             .Include(x => x.ServicoContratadoDestino)
                                .ThenInclude(x => x.Contrato)
                             .Include(x => x.ServicoContratadoDestino)
                                .ThenInclude(x => x.DeParaServicos)
                                .AsNoTracking();
        }
        #endregion
    }
}
