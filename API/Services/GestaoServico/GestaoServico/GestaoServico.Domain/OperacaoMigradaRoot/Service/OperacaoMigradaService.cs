using Dapper;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Repository;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using GestaoServico.Domain.OperacaoMigradaRoot.DTO;
using GestaoServico.Domain.OperacaoMigradaRoot.Repository;
using GestaoServico.Domain.OperacaoMigradaRoot.Service.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utils.Base;
using Utils.Connections;

namespace GestaoServico.Domain.OperacaoMigradaRoot.Service
{
    public class OperacaoMigradaService : IOperacaoMigradaService
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IDeParaServicoRepository _deParaServicoRepository;
        private readonly IOperacaoMigradaRepository _operacaoMigradaRepository;

        public OperacaoMigradaService(
            IOptions<ConnectionStrings> connectionStrings,
            IDeParaServicoRepository deParaServicoRepository,
            IOperacaoMigradaRepository operacaoMigradaRepository)
        {
            _operacaoMigradaRepository = operacaoMigradaRepository;
            _deParaServicoRepository = deParaServicoRepository;
            _connectionStrings = connectionStrings;
        }

        public void AtualizarStatus(List<int> idsServicos)
        {
            var servicos = _operacaoMigradaRepository.Buscar(x => idsServicos.Any(y => y == x.IdServico));
            foreach (var servico in servicos)
            {
                servico.Status = "T";
                _operacaoMigradaRepository.Update(servico);
            }
        }

        public List<OperacaoMigradaDTO> BuscarServicosPorGrupoDelivery(int idGrupoDelivery)
        {
            var result = _operacaoMigradaRepository.BuscarServicosPorGrupoDelivery(idGrupoDelivery);
            return result;
        }

        public FiltroGenericoDtoBase<OperacaoMigradaDTO> Filtrar(FiltroGenericoDtoBase<OperacaoMigradaDTO> filtro)
        {
            var result = _operacaoMigradaRepository.Filtrar(filtro);
            return result;
        }

        public FiltroGenericoDtoBase<AgrupamentoDTO> FiltrarAgrupamentos(FiltroGenericoDtoBase<AgrupamentoDTO> filtro)
        {
            var result = _deParaServicoRepository.FiltrarAgrupamentos(filtro);
            return result;
        }
    }
}
