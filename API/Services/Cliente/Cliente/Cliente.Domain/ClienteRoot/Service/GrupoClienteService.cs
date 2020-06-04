using Cliente.Domain.ClienteRoot.Entity;
using Cliente.Domain.ClienteRoot.Repository;
using Cliente.Domain.ClienteRoot.Service.Interfaces;
using Cliente.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Cliente.Domain.SharedRoot;
using Utils;

namespace Cliente.Domain.ClienteRoot.Service
{
    public class GrupoClienteService : IGrupoClienteService
    {
        protected readonly IGrupoClienteRepository _grupoClienteRepository;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IVariablesToken _variables;
        public GrupoClienteService(IGrupoClienteRepository grupoClienteRepository,
                                   IUnitOfWork unitOfWork,
                                   IVariablesToken variables)
        {
            _grupoClienteRepository = grupoClienteRepository;
            _unitOfWork = unitOfWork;
            _variables = variables;
        }

        public int PersistirGrupoClienteSalesForce(string idSalesForceClienteMae, string descricaoGrupoCliente)
        {
            _variables.UserName = "salesForce";
            GrupoCliente grupoCliente = new GrupoCliente
            {
                FlStatus = true,
                DescGrupoCliente = descricaoGrupoCliente,
                IdClienteMae = idSalesForceClienteMae
            };
            _grupoClienteRepository.Adicionar(grupoCliente);
            _unitOfWork.Commit();

            return grupoCliente.Id;
        }

        public int? ObterIdGrupoClientePorIdClienteMae(string idSalesForceClienteMae)
        {
            var result = _grupoClienteRepository.ObterIdGrupoClientePorIdClienteMae(idSalesForceClienteMae);
            return result;
        }
    }
}
