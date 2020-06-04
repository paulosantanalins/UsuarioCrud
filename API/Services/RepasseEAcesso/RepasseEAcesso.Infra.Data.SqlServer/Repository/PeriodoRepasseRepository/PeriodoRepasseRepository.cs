
using Microsoft.EntityFrameworkCore;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Dto;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Entity;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Repository;
using RepasseEAcesso.Infra.Data.SqlServer.Context;
using System.Linq;
using Utils;
using Utils.Base;

namespace RepasseEAcesso.Infra.Data.SqlServer.Repository.PeriodoRepasseRepository
{
    public class PeriodoRepasseRepository : BaseRepository<PeriodoRepasse>, IPeriodoRepasseRepository
    {
        private readonly RepasseEAcessoContext repasseEAcessoContext;

        public PeriodoRepasseRepository(RepasseEAcessoContext repasseEAcessoContext, IVariablesToken variables)
            : base(repasseEAcessoContext, variables)
        {
            this.repasseEAcessoContext = repasseEAcessoContext;
        }

        private bool ExisteRepasseCadastradoNoPeriodo(PeriodoRepasse periodo) =>
            _context.RepasseNivelUm.Where(x => x.DataRepasse >= periodo.DtLancamentoInicio && x.DataRepasse <= periodo.DtLancamentoFim) != null;

        public FiltroGenericoDtoBase<PeriodoRepasseDto> FiltrarPeriodo(FiltroGenericoDtoBase<PeriodoRepasseDto> filtro)
        {
            var query = DbSet.AsQueryable();


            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                query = query.Where(x => (x.Id.ToString().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.DtAnaliseFim.ToString("dd/MM/yyyy").Equals(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.DtAnaliseInicio.ToString("dd/MM/yyyy").Equals(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.DtAprovacaoFim.ToString("dd/MM/yyyy").Equals(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.DtAprovacaoInicio.ToString("dd/MM/yyyy").Equals(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.DtLancamentoFim.ToString("dd/MM/yyyy").Equals(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.DtLancamento.ToString("MM/yyyy").Equals(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.DtLancamento.AddMonths(-1).ToString("MM/yyyy").Equals(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.DtLancamentoInicio.ToString("dd/MM/yyyy").Equals(filtro.ValorParaFiltrar.ToUpper())))))))))));
            }

            var dados = query.Select(x => new PeriodoRepasseDto
            {
                Id = x.Id,
                Usuario = x.Usuario,
                DataAlteracao = x.DataAlteracao,
                DtLancamentoInicio = x.DtLancamentoInicio,
                DtLancamentoFim = x.DtLancamentoFim,
                DtAprovacaoInicio = x.DtAprovacaoInicio,
                DtAprovacaoFim = x.DtAprovacaoFim,
                DtAnaliseInicio = x.DtAnaliseInicio,
                DtAnaliseFim = x.DtAnaliseFim,
                DtLancamento = x.DtLancamento,
                ExisteRepasseNessePeriodo = ExisteRepasseCadastradoNoPeriodo(x),
                DtReferencia = x.DtLancamento.AddMonths(-1),
                PeriodoVigente = false
            });
            filtro.Total = dados.Count();

            switch (filtro?.CampoOrdenacao)
            {
                case "id":
                    dados = filtro.OrdemOrdenacao.Equals("asc") ? dados.OrderBy(x => x.DtLancamento) : dados.OrderByDescending(x => x.DtLancamento);
                    break;
                default:
                    dados = dados.OrderByDescending(x => x.DtLancamentoInicio);
                    break;
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

            var vigente = DbSet.AsNoTracking().OrderByDescending(x => x.Id).FirstOrDefault();
            if (vigente != null && filtro.Valores.Any())
            {
                var vigenteFiltrado = filtro.Valores.FirstOrDefault(x => x.Id == vigente.Id);
                if (vigenteFiltrado != null)
                {
                    filtro.Valores.FirstOrDefault(x => x.Id == vigente.Id).PeriodoVigente = true;
                }
            }


            return filtro;
        }

        public PeriodoRepasse BuscarPeriodoVigente() => DbSet.LastOrDefault();
    }
}
