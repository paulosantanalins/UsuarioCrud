using System;
using System.Collections.Generic;
using LancamentoFinanceiro.Domain.Core.Notifications;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Service.Interfaces;
using Utils;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Service
{
    public class LancamentoFinanceiroService : ILancamentoFinanceiroService
    {
        private readonly ILancamentoFinanceiroRepository _lancamentoFinanceiroRepository;
        private readonly IItemLancamentoFinanceiroRepository _itemLancamentoFinanceiroRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVariablesToken _variables;
        private readonly NotificationHandler _notificationHandler;

        public LancamentoFinanceiroService(
            ILancamentoFinanceiroRepository lancamentoFinanceiroRepository,
            IItemLancamentoFinanceiroRepository itemLancamentoFinanceiroRepository,
            IUnitOfWork unitOfWork,
            IVariablesToken variables,
            NotificationHandler notificationHandler)
        {
            _lancamentoFinanceiroRepository = lancamentoFinanceiroRepository;
            _itemLancamentoFinanceiroRepository = itemLancamentoFinanceiroRepository;
            _unitOfWork = unitOfWork;
            _variables = variables;
            _notificationHandler = notificationHandler;
        }

        public IEnumerable<RootLancamentoFinanceiro> ObterTodos()
        {
            var lancamentosBD = _lancamentoFinanceiroRepository.BuscarTodos();
            return lancamentosBD;
        }

        public void AdicionarRange(List<RootLancamentoFinanceiro> rootLancamentos)
        {
            foreach (var lancamento in rootLancamentos)
            {
                foreach (var item in lancamento.ItensLancamentoFinanceiro)
                {
                    TratarItem(item);
                }
            }
            
            _lancamentoFinanceiroRepository.AdicionarRange(rootLancamentos);
            _unitOfWork.Commit();
            
            Console.WriteLine("Persistencia lancamento financeiro: "+DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        }

        public void Adicionar(RootLancamentoFinanceiro rootLancamento)
        {
            _variables.UserName = "EPM";
            foreach (var item in rootLancamento.ItensLancamentoFinanceiro)
            {
                TratarItem(item);
            }
            _lancamentoFinanceiroRepository.Adicionar(rootLancamento);
            _unitOfWork.Commit();
        }

        private void TratarItem(ItemLancamentoFinanceiro item)
        {
            _variables.UserName = "EPM";
            item.LgUsuario = _variables.UserName;
            item.DtAlteracao = DateTime.Now.Date;
            item.VlDesc = item.VlDesc != null ? Math.Round(item.VlDesc.Value, 2) : 0;
            item.VlInc = item.VlInc != null ? Math.Round(item.VlInc.Value, 2) : 0;
            item.VlLancamento = Math.Round(item.VlLancamento, 2);
        }

        public void RemoverLancamentosFinanceirosPorRepasse(int idRepasse)
        {
            try
            {
                var lancamentosDb = _lancamentoFinanceiroRepository.ObterLancamentosPorRepasse(idRepasse);
                foreach (var lancamento in lancamentosDb)
                {
                    var countItensRemovidosPorRepasse = 0;
                    foreach (var item in lancamento.ItensLancamentoFinanceiro)
                    {
                        if (item.IdRepasse == idRepasse)
                        {
                            _itemLancamentoFinanceiroRepository.Remove(item);
                            countItensRemovidosPorRepasse++;
                        }
                    }
                    if (countItensRemovidosPorRepasse == lancamento.ItensLancamentoFinanceiro.Count)
                    {
                        _lancamentoFinanceiroRepository.Remove(lancamento);
                    }
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _notificationHandler.AddMensagem("Error", e.Message);
            }
        }


    }
}
