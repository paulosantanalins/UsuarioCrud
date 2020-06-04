using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Service
{
    public class FuncionalidadeService : IFuncionalidadeService
    {
        protected readonly IFuncionalidadeRepository _funcionalidadeRepository;
        public FuncionalidadeService(IFuncionalidadeRepository funcionalidadeRepository)
        {
            _funcionalidadeRepository = funcionalidadeRepository;
        }

        public List<Funcionalidade> BuscarTodasFuncionalidades()
        {
            var result = _funcionalidadeRepository.BuscarTodos();
            return result.ToList();
        }

       

    }
}
