using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Domain.RentabilidadeRoot.Dto;
using LancamentoFinanceiro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Repository
{
    public class ItemLancamentoFinanceiroRepository : BaseRepository<ItemLancamentoFinanceiro>, IItemLancamentoFinanceiroRepository
    {
        protected readonly IVariablesToken _variables;
        protected readonly IAuditoriaRepository __auditoriaRepository;
        public ItemLancamentoFinanceiroRepository(ServiceContext servicoContext, IVariablesToken variables, IAuditoriaRepository auditoriaRepository) : base(servicoContext, variables, auditoriaRepository)
        {
            _variables = variables;
            __auditoriaRepository = auditoriaRepository;
        }

        public List<ItemLancamentoFinanceiro> ObterItensLancamentoPorIdServicoContratadoPorPeriodo(List<int> idsServicos, DateTime? dtInicio, DateTime? dtFim)
        {
            var json = JsonConvert.SerializeObject(idsServicos);
            var result = DbSet
                .Include(x => x.LancamentoFinanceiro)
                                .ThenInclude(x => x.TipoDespesa)
                            .Where(x => (x.IdServicoContratado.HasValue && idsServicos.Contains(x.IdServicoContratado.Value)) 
                             && (x.LancamentoFinanceiro.DtBaixa >= dtInicio && x.LancamentoFinanceiro.DtBaixa <= dtFim))
                             .AsNoTracking();
            return result.ToList();
        }

        public List<ItemLancamentoFinanceiro> ObterItensLancamentoPorIdRepasseRecebidoPorPeriodo(List<int> idsRepasses, DateTime? dtInicio, DateTime? dtFim)
        {
            var result = DbSet
                .Include(x => x.LancamentoFinanceiro)
                                .ThenInclude(x => x.TipoDespesa)
                            .Where(x => (x.IdRepasse.HasValue && idsRepasses.Contains(x.IdRepasse.Value))
                             && (x.LancamentoFinanceiro.DtBaixa >= dtInicio && x.LancamentoFinanceiro.DtBaixa <= dtFim)
                             && x.LancamentoFinanceiro.DescricaoTipoLancamento == "C")
                             .AsNoTracking();
            return result.ToList();
        }

        public List<ItemLancamentoFinanceiro> ObterItensLancamentoPorIdRepassePagoPorPeriodo(List<int> idsRepasses, DateTime? dtInicio, DateTime? dtFim)
        {
            var result = DbSet
                .Include(x => x.LancamentoFinanceiro)
                                .ThenInclude(x => x.TipoDespesa)
                            .Where(x => (x.IdRepasse.HasValue && idsRepasses.Contains(x.IdRepasse.Value))
                             && (x.LancamentoFinanceiro.DtBaixa >= dtInicio && x.LancamentoFinanceiro.DtBaixa <= dtFim)
                             && x.LancamentoFinanceiro.DescricaoTipoLancamento == "D")
                             .AsNoTracking();
            return result.ToList();
        }
    }
}
