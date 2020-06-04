using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Repository;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using GestaoServico.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Service
{
    public class CelulaService : ICelulaService
    {
        protected readonly IUnitOfWork _unitOfWork;
        private readonly Variables _variables;
        private readonly ICelulaRepository _celulaRepository;

        public CelulaService(
            IUnitOfWork unitOfWork,
            Variables variables,
            ICelulaRepository celulaRepository)
        {
            _unitOfWork = unitOfWork;
            _variables = variables;
            _celulaRepository = celulaRepository;
        }

        public IEnumerable<Celula> ObterTodos()
        {
            return _celulaRepository.BuscarTodos();
        }
    }
}
