using Cadastro.Domain.CidadeRoot.Dto;
using Cadastro.Domain.CidadeRoot.Repository;
using Cadastro.Domain.CidadeRoot.Service.Interfaces;
using Cadastro.Domain.EmpresaGrupoRoot.Repository;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.SharedRoot;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Utils.Base;
using Utils.Connections;

namespace Cadastro.Domain.CidadeRoot.Service
{
    public class CidadeService : ICidadeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICidadeRepository _cidadeRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IPaisRepository _paisRepository;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public CidadeService(IUnitOfWork unitOfWork,
            ICidadeRepository cidadeRepository,
            IEnderecoRepository enderecoRepository,
            IPaisRepository paisRepository,                 
            IOptions<ConnectionStrings> connectionStrings)
        {
            _unitOfWork = unitOfWork;
            _cidadeRepository = cidadeRepository;
            _enderecoRepository = enderecoRepository;
            _paisRepository = paisRepository;
            _connectionStrings = connectionStrings;
        }

        public IEnumerable<Pais> BuscarPaises()
        {
            var paises = _paisRepository.BuscarTodos().OrderBy(x => x.NmPais);
            var paisesAgrupados = paises.GroupBy(x => x.NmPais);
            var paisesFiltrados = paisesAgrupados.Select(x => x.FirstOrDefault());
            return paisesFiltrados;
        }

        public Cidade BuscarPorId(int id)
        {
            var cidade = _cidadeRepository.BuscarPorIdComIncludes(id);
            return cidade;
        }

        public FiltroGenericoDtoBase<CidadeDto> Filtrar(FiltroGenericoDtoBase<CidadeDto> filtro)
        {
            var cidades = _cidadeRepository.FiltrarCidades(filtro);
            return cidades;
        }

        public void PersistirCidade(Cidade cidade)
        {
            if (cidade.Id == 0)
            {
                var cidadeExists = _cidadeRepository.BuscarPorNomeComIncludes(cidade.NmCidade);

                if (cidadeExists != null
                    && cidade.NmCidade.Trim().ToUpper() == cidadeExists.NmCidade.Trim().ToUpper()
                    && cidade.Estado.Id == cidadeExists.Estado.Id
                    && cidade.Estado.IdPais == cidadeExists.Estado.IdPais )
                {
                    throw new ArgumentException();
                }

                cidade.NmCidade = cidade.NmCidade.ToUpper();
                _cidadeRepository.Adicionar(cidade);
                _unitOfWork.Commit();
            }
            else
            {
                _cidadeRepository.Update(cidade);
                _unitOfWork.Commit();
            }
        }
    }
}

