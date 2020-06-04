using System.Collections.Generic;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class FinalizacaoContratoService : IFinalizacaoContratoService
    {
        private readonly IFinalizacaoContratoRepository _finalizacaoContratoRepository;
        private readonly IPrestadorRepository _prestadorRepository;
        private readonly IInativacaoPrestadorRepository _inativacaoPrestadorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVariablesToken _variables;

        public FinalizacaoContratoService(IFinalizacaoContratoRepository finalizacaoContratoRepository,
            IUnitOfWork unitOfWork, 
            IPrestadorRepository prestadorRepository, 
            IInativacaoPrestadorRepository inativacaoPrestadorRepository, IVariablesToken variables)
        {
            _finalizacaoContratoRepository = finalizacaoContratoRepository;
            _unitOfWork = unitOfWork;
            _prestadorRepository = prestadorRepository;
            _inativacaoPrestadorRepository = inativacaoPrestadorRepository;
            _variables = variables;
        }

        public IEnumerable<PeriodoTransferenciaPrestadorDto> BuscarPeriodosComFinalizacaoCadastradas() =>
            _finalizacaoContratoRepository.BuscarPeriodosComFinalizacaoCadastrada();

        public FiltroComPeriodo<FinalizarContratoGridDto> Filtrar(FiltroComPeriodo<FinalizarContratoGridDto> filtro)
        {
            var result = _finalizacaoContratoRepository.Filtrar(filtro);
            return result;
        }

        public IEnumerable<KeyValueDto> ObterPrestadoresPorCelula(int idCelula, bool filtrar)
        {
            var result = _finalizacaoContratoRepository.ObterPrestadoresPorCelula(idCelula, filtrar);
            return result;
        }

        public void FinalizarContrato(FinalizacaoContrato finalizacaoContrato,
            FinalizacaoContratoDto finalizacaoContratoDto)
        {
            if (finalizacaoContratoDto.FinalizarImediatamente)
            {
                finalizacaoContrato.Situacao = SharedEnuns.SituacoesFinalizarContrato.Finalizado.GetHashCode();
                
                // Executar fim de contrato
                FinalizarContratoEInativar(finalizacaoContrato);
            }
            else
            {
                finalizacaoContrato.Situacao = SharedEnuns.SituacoesFinalizarContrato.Pendente.GetHashCode();
            }

            _finalizacaoContratoRepository.AdicionarComLog(finalizacaoContrato);

            _unitOfWork.Commit();
        }

        public FinalizacaoContratoDto ConsultarFinalizacao(int id)
        {
            var result = _finalizacaoContratoRepository.ConsultarFinalizacao(id);
            return result;
        }

        public void InativarFinalizacao(InativacaoFinalizacaoContratoDto inativacaoFinalizacaoContratoDto)
        {
            var finalizacao = new FinalizacaoContrato
            {
                Id = inativacaoFinalizacaoContratoDto.Id,
                Situacao = SharedEnuns.SituacoesFinalizarContrato.Cancelado.GetHashCode()
            };

            _finalizacaoContratoRepository.InativarFinalizacao(finalizacao, inativacaoFinalizacaoContratoDto.Motivo);

            _unitOfWork.Commit();
        }

        public void EditarFinalizacao(FinalizacaoContrato finalizacaoContrato, bool finalizarImediatamente)
        {
            if (finalizarImediatamente)
            {
                finalizacaoContrato.Situacao = SharedEnuns.SituacoesFinalizarContrato.Finalizado.GetHashCode();
                
                // Executar fim de contrato
                FinalizarContratoEInativar(finalizacaoContrato);
            }
            else
            {
                finalizacaoContrato.Situacao = SharedEnuns.SituacoesFinalizarContrato.Pendente.GetHashCode();
            }

            _finalizacaoContratoRepository.UpdateComLog(finalizacaoContrato);

            _unitOfWork.Commit();
        }

        public IEnumerable<LogFinalizacaoContrato> ObterLogsPorId(int id)
        {
            var logs = _finalizacaoContratoRepository.ObterLogsPorId(id);

            return logs;
        }

        public void EfetuarFinalizacoes()
        {
            var finalizacoes = _finalizacaoContratoRepository.ObterFinalizacoesParaJob();

            foreach (var finalizacao in finalizacoes)
            {
                finalizacao.Situacao = SharedEnuns.SituacoesFinalizarContrato.Finalizado.GetHashCode();

                FinalizarContratoEInativar(finalizacao);

                _finalizacaoContratoRepository.UpdateComLog(finalizacao);
            }

            _unitOfWork.Commit();
        }

        private void FinalizarContratoEInativar(FinalizacaoContrato finalizacaoContrato)
        {
            var prestador = _prestadorRepository.ObterPorIdComInativacoes(finalizacaoContrato.IdPrestador);

            prestador.DataDesligamento = finalizacaoContrato.DataFimContrato;

            var inativacao = new InativacaoPrestador
            {
                CodEacessoLegado = prestador.CodEacessoLegado,
                DataDesligamento = finalizacaoContrato.DataFimContrato,
                Motivo = finalizacaoContrato.Motivo,
                FlagIniciativaDesligamento = 0,
                IdPrestador = prestador.Id,
                FlagRetorno = finalizacaoContrato.RetornoPermitido ? 1 : 0,
                Responsavel = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName
            };

            _prestadorRepository.Update(prestador);
            _inativacaoPrestadorRepository.Adicionar(inativacao);
        }
    }
}
