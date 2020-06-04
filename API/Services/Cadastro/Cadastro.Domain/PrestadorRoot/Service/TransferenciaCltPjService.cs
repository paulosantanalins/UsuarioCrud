using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utils;
using Utils.Base;
using Utils.Connections;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class TransferenciaCltPjService : ITransferenciaCltPjService
    {
        private readonly IPrestadorService _prestadorService;
        private readonly IPrestadorRepository _prestadorRepository;
        private readonly ITransferenciaCltPjRepository _transferenciaCltPjRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IVariablesToken _variables;

        public TransferenciaCltPjService(
            ITransferenciaCltPjRepository transferenciaCltPjRepository,
            IPrestadorRepository prestadorRepository,
            IPrestadorService prestadorService,
            IOptions<ConnectionStrings> connectionStrings,
            IUnitOfWork unitOfWork, IVariablesToken variables)
        {
            _transferenciaCltPjRepository = transferenciaCltPjRepository;
            _prestadorService = prestadorService;
            _prestadorRepository = prestadorRepository;
            _unitOfWork = unitOfWork;
            _variables = variables;
            _connectionStrings = connectionStrings;
        }

        public string Adicionar(DadosCltEacessoDto dadosCltEacesso)
        {
            bool profissionalExisteNoEacesso = VerificarExistenciaProfissional(dadosCltEacesso.IdEacessoLegado);
            if (profissionalExisteNoEacesso)
            {
                Prestador prestador = BuscarPrestadorPorIdEacesso(dadosCltEacesso);
                if (prestador != null)
                {
                    throw new ArgumentException("Profissional já migrado como Prestador para o STFCORP");
                }

                AtualizarTipoContratoEacesso(dadosCltEacesso.IdEacessoLegado);
                _prestadorService.RealizarMigracaoCltPj(dadosCltEacesso.IdEacessoLegado);
                prestador = BuscarPrestadorPorIdEacesso(dadosCltEacesso);

                TransferenciaCltPj transferenciaCltPj = MontarEntidadeTransferencia(dadosCltEacesso, prestador, _variables.UserName);
                _transferenciaCltPjRepository.Adicionar(transferenciaCltPj);
                _unitOfWork.Commit();
                return "";
            }
            else
            {
                throw new ArgumentException("Não existe profissional no E-Acesso com o Id informado.");
            }
        }

        private static TransferenciaCltPj MontarEntidadeTransferencia(DadosCltEacessoDto dadosCltEacesso, Prestador prestador, string usuario)
        {
            return new TransferenciaCltPj
            {
                IdEacessoLegado = dadosCltEacesso.IdEacessoLegado,
                IdPrestadorTransferido = prestador.Id,
                NomePrestador = prestador.Pessoa.Nome,
                DataAlteracao = DateTime.Now,
                Usuario = usuario
            };
        }

        private Prestador BuscarPrestadorPorIdEacesso(DadosCltEacessoDto dadosCltEacesso)
        {
            return _prestadorRepository.Buscar(x => x.CodEacessoLegado.HasValue && x.CodEacessoLegado.Value == dadosCltEacesso.IdEacessoLegado).FirstOrDefault();
        }

        public FiltroGenericoDtoBase<TransferenciaCltPjDto> Filtrar(FiltroGenericoDtoBase<TransferenciaCltPjDto> filtro)
        {
            var result = _transferenciaCltPjRepository.Filtrar(filtro);
            return result;
        }

        private bool VerificarExistenciaProfissional(int idEacessoLegado)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                string query = "select count(1) from stfcorp.tblprofissionais where id = " + idEacessoLegado;
                var quantidadeEncontrada = dbConnection.Query<int>(query).FirstOrDefault();

                dbConnection.Close();
                return quantidadeEncontrada > 0;
            }
        }

        private void AtualizarTipoContratoEacesso(int idEacessoLegado)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                string query = "select idTipoContrato from stfcorp.tblprofissionais where id = " + idEacessoLegado;
                var idTipoContratoAnterior = dbConnection.Query<int?>(query).FirstOrDefault();

                query = "update stfcorp.tblprofissionais set idTipoContrato = 2, idTipoContratoAnterior = " + idTipoContratoAnterior + " where id = " + idEacessoLegado;
                var quantidadeEncontrada = dbConnection.Query(query);

                dbConnection.Close();
            }
        }
    }
}
