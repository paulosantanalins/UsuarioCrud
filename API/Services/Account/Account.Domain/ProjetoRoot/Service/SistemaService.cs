using Account.Domain.ProjetoRoot.Entity;
using Account.Domain.ProjetoRoot.Repository;
using Account.Domain.ProjetoRoot.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Account.Domain.ProjetoRoot.Service
{
    public class SistemaService : ISistemaService
    {
        protected readonly ISistemaRepository _sistemaRepository;
        protected readonly IVariablesToken _variables;
        public SistemaService(ISistemaRepository sistemaRepository,
            IVariablesToken variables)
        {
            _sistemaRepository = sistemaRepository;
            _variables = variables;
        }


        public async Task PersistirSistema(Sistema sistema)
        {
            sistema.DtAlteracao = DateTime.Now;
            sistema.LgUsuario = _variables.UserName;
            if (sistema.Id != 0)
            {
                await _sistemaRepository.UpdateSistema(sistema);
            }
            else
            {
                await _sistemaRepository.AddSistema(sistema);
            }
        }


    }
}
