using Cadastro.Domain.CidadeRoot.Repository;
using Cadastro.Domain.EmpresaGrupoRoot.Repository;
using Cadastro.Domain.EmpresaGrupoRoot.Service.Interfaces;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.EnderecoRoot.Repository;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Cadastro.Domain.EmpresaGrupoRoot.Service
{

    public class EnderecoService : IEnderecoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IEstadoRepository _estadoRepository;
        private readonly ICidadeRepository _cidadeRepository;
        private readonly IPaisRepository _paisRepository;
        private readonly IAbreviaturaLogradouroRepository _abreviaturaLogradouroRepository;

        protected readonly Variables _variables;

        public EnderecoService(
            IUnitOfWork unitOfWork,
            IEnderecoRepository enderecoRepository,
            IEstadoRepository estadoRepository,
            ICidadeRepository cidadeRepository,
            IPaisRepository paisRepository,
            IAbreviaturaLogradouroRepository abreviaturaLogradouroRepository,
            Variables variables)
        {
            _variables = variables;
            _unitOfWork = unitOfWork;
            _enderecoRepository = enderecoRepository;
            _estadoRepository = estadoRepository;
            _cidadeRepository = cidadeRepository;
            _paisRepository = paisRepository;
            _abreviaturaLogradouroRepository = abreviaturaLogradouroRepository;
        }

        public IEnumerable<Estado> BuscarEstados()
        {
            var estados = _estadoRepository.BuscarEstadosBrasileiros();
            var estadosAgrupados = estados.GroupBy(x => x.NmEstado);
            var estadosFiltrados = estadosAgrupados.Select(x => x.FirstOrDefault());
            return estadosFiltrados;
        }


        public IEnumerable<Estado> BuscarEstadosDeUmPais(int id)
        {
            var estados = _estadoRepository.Buscar(x => x.IdPais == id).OrderBy(x => x.NmEstado);
            var estadosAgrupados = estados.GroupBy(x => x.NmEstado);
            var estadosFiltrados = estadosAgrupados.Select(x => x.FirstOrDefault());
            return estadosFiltrados;
        }

        public Pais BuscarPaisPorId(int idPais)
        {
            var pais = _paisRepository.BuscarPorId(idPais);
            return pais;
        }

        public IEnumerable<Cidade> BuscarCidadesPeloEstado(int idEstado)
        {
            var cidades = _cidadeRepository.Buscar(x => x.IdEstado == idEstado).OrderBy(x => x.NmCidade);
            var cidadesAgrupados = cidades.GroupBy(x => x.NmCidade);
            var cidadesFiltrados = cidadesAgrupados.Select(x => x.FirstOrDefault());
            return cidadesFiltrados;
        }

        public IEnumerable<AbreviaturaLogradouro> BuscarAbreviaturaLogradouro()
        {
            return _abreviaturaLogradouroRepository.BuscarTodos().OrderBy(x => x.Descricao);
        }
    }
}
