using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using Utils.Connections;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class ClienteServicoPrestadorService : IClienteServicoPrestadorService
    {
        private readonly IClienteServicoPrestadorRepository _clienteServicoPrestadorRepository;
        private readonly IPrestadorService _prestadorService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public ClienteServicoPrestadorService(
            IClienteServicoPrestadorRepository clienteServicoPrestadorRepository,
            IPrestadorService prestadorService,
            IOptions<ConnectionStrings> connectionStrings,
            IUnitOfWork unitOfWork)
        {
            _clienteServicoPrestadorRepository = clienteServicoPrestadorRepository;
            _prestadorService = prestadorService;
            _connectionStrings = connectionStrings;
            _unitOfWork = unitOfWork;
        }

        public ClienteServicoPrestador Adicionar(ClienteServicoPrestador clienteServicoPrestador)
        {
            _clienteServicoPrestadorRepository.Adicionar(clienteServicoPrestador);
            _unitOfWork.Commit();
            return clienteServicoPrestador;
        }

        public ClienteServicoPrestador BuscarPorId(int id)
        {
            return _clienteServicoPrestadorRepository.BuscarPorId(id);
        }


        public ClienteServicoPrestador Atualizar(ClienteServicoPrestador clienteServicoPrestador)
        {
            _clienteServicoPrestadorRepository.Update(clienteServicoPrestador);
            _unitOfWork.Commit();
            return clienteServicoPrestador;
        }

        public void Inativar(int idClienteServicoPrestador)
        {
            var result = _clienteServicoPrestadorRepository.BuscarPorId(idClienteServicoPrestador);
            result.Ativo = false;
            _clienteServicoPrestadorRepository.Update(result);
            _unitOfWork.Commit();
            var prestador = _prestadorService.BuscarPorId(result.IdPrestador);
            result.Prestador = prestador;
            var connectionStringEAcesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEAcesso))
            {
                dbConnection.Open();
                var tran = dbConnection.BeginTransaction();
                var idClienteServicoEacesso = AtualizarClienteServicoPrestadorEAcesso(result, dbConnection, tran);
                tran.Commit();
            }
        }

        public void InserirClienteServicoPrestadorEAcesso(ClienteServicoPrestador clienteServicoPrestador, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var idClienteServicoEacesso = InserirClienteServicoEAcesso(clienteServicoPrestador, dbConnection, dbTransaction);
        }

        public void AtualizarEAcesso(ClienteServicoPrestador clienteServicoPrestador, IDbConnection connection, IDbTransaction dbTransaction)
        {
            var idClienteServicoEacesso = AtualizarClienteServicoPrestadorEAcesso(clienteServicoPrestador, connection, dbTransaction);
        }

        private int? InserirClienteServicoEAcesso(ClienteServicoPrestador clienteServicoPrestador, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var prestador = _prestadorService.BuscarPorId(clienteServicoPrestador.IdPrestador);
            try
            {
                StringBuilder stringBuilder = new StringBuilder("");
                stringBuilder.Append(@"INSERT INTO stfcorp.tblProfissionaisClientes (
                          IdProfissional
                          ,Dtinicio
                          ,DtFim
                          ,IdCliente
                          ,IdServico
                          ,Descricao
                          ,VlrCusto
                          ,VlrRepasse
                          ,VlrVenda
                          ,IdLocTrab
                          ,FatAutom
                          ,Inativo
                         )");
                stringBuilder.Append("VALUES (");
                stringBuilder.Append($"{prestador.CodEacessoLegado}");
                stringBuilder.Append(",@DtInicio");
                if (clienteServicoPrestador.DataPrevisaoTermino != null)
                {
                    stringBuilder.Append(",@DtFim");
                }
                else
                {
                    stringBuilder.Append(",null");
                }
                stringBuilder.Append($",{clienteServicoPrestador.IdCliente}");
                stringBuilder.Append("," + clienteServicoPrestador.IdServico + "");
                stringBuilder.Append(",'" + clienteServicoPrestador.DescricaoTrabalho + "'");
                stringBuilder.Append(",@ValorCusto");
                stringBuilder.Append(",@ValorRepasse");
                stringBuilder.Append(",@ValorVenda");
                stringBuilder.Append("," + clienteServicoPrestador.IdLocalTrabalho + "");
                stringBuilder.Append("," + 0 + "");
                if (clienteServicoPrestador.Ativo == true)
                {
                    stringBuilder.Append(",0");
                }
                else
                {
                    stringBuilder.Append(",1");
                }
                stringBuilder.Append(")");

                dbConnection.Query(stringBuilder.ToString(),
                    new
                    {
                        ValorCusto = clienteServicoPrestador.ValorCusto,
                        ValorRepasse = clienteServicoPrestador.ValorRepasse,
                        ValorVenda = clienteServicoPrestador.ValorVenda,
                        DtInicio = clienteServicoPrestador.DataInicio ?? (object)DBNull.Value,
                        DtFim = clienteServicoPrestador.DataPrevisaoTermino ?? (object)DBNull.Value,
                        FatAutom = false,
                        Inativo = (clienteServicoPrestador.Ativo == true ? false : true)
                    }, dbTransaction);
                return clienteServicoPrestador.Prestador.CodEacessoLegado;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private int? AtualizarClienteServicoPrestadorEAcesso(ClienteServicoPrestador clienteServicoPrestador, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            var usCulture = "en-US";
            stringBuilder.Append(@"UPDATE stfcorp.tblProfissionaisClientes SET ");
            stringBuilder.Append(" Dtinicio = @DataInicio");
            if (clienteServicoPrestador.DataPrevisaoTermino != null) { stringBuilder.Append(",DtFim = @DataPrevisaoTermino"); }
            stringBuilder.Append(",IdCliente = " + clienteServicoPrestador.IdCliente);
            stringBuilder.Append(",IdServico = " + clienteServicoPrestador.IdServico);
            stringBuilder.Append(",Descricao = '" + clienteServicoPrestador.DescricaoTrabalho + "'");
            stringBuilder.Append(",VlrCusto = '" + clienteServicoPrestador.ValorCusto.ToString().Replace(",", ".") + "'");
            stringBuilder.Append(",VlrRepasse = '" + clienteServicoPrestador.ValorRepasse.ToString().Replace(",", ".") + "'");
            stringBuilder.Append(",VlrVenda = '" + clienteServicoPrestador.ValorVenda.ToString().Replace(",", ".") + "'");
            stringBuilder.Append(",IdLocTrab = " + clienteServicoPrestador.IdLocalTrabalho);

            if (clienteServicoPrestador.Ativo == true)
            {
                stringBuilder.Append(",Inativo = " + 0);
            }
            else
            {
                stringBuilder.Append(",Inativo = " + 1);

            }
            stringBuilder.Append(" WHERE IdProfissional = " + clienteServicoPrestador.Prestador.CodEacessoLegado + " and idcliente = " + clienteServicoPrestador.IdCliente
                + " and idservico = " + clienteServicoPrestador.IdServico);

            dbConnection.Query(stringBuilder.ToString(), new
            {
                DataPrevisaoTermino = clienteServicoPrestador.DataPrevisaoTermino,
                DataInicio = clienteServicoPrestador?.DataInicio ?? (object)DBNull.Value
            }, dbTransaction);
            return clienteServicoPrestador.Prestador.CodEacessoLegado;
        }

        private int ObterIdEmpresaGrupoEacesso(IDbConnection dbConnection, int? idLocalTrabalho)
        {
            var query = "SELECT * from stfcorp.tblEmpresasGrupo where erpExterno = " + idLocalTrabalho;
            var id = dbConnection.Query<int>(query).FirstOrDefault();
            return id;
        }
    }
}
