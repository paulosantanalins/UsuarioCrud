using Cadastro.Domain.CidadeRoot.Service.Interfaces;
using Cadastro.Domain.EmpresaGrupoRoot.Repository;
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
using Utils.Connections;

namespace Cadastro.Domain.PrestadorRoot.Service
{


    public class EmpresaService : IEmpresaService
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly ICidadeService _cidadeService;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IPrestadorService _prestadorService;
        const string FORMATO_DATA_DB_EACESSO = "yyyy-MM-dd HH:mm:ss";

        public EmpresaService(
            IEmpresaRepository empresaRepository,
            ICidadeService cidadeService,
            IEnderecoRepository enderecoRepository,
            IOptions<ConnectionStrings> connectionStrings,
            IPrestadorService prestadorService,
            IUnitOfWork unitOfWork)
        {
            _empresaRepository = empresaRepository;
            _cidadeService = cidadeService;
            _enderecoRepository = enderecoRepository;
            _unitOfWork = unitOfWork;
            _connectionStrings = connectionStrings;
            _prestadorService = prestadorService;
        }

        public Empresa BuscarPorId(int id)
        {
            var result = _empresaRepository.BuscarPorId(id);
            return result;
        }

        public void AtualizarEmpresaPrestador(Empresa empresa)
        {
            _empresaRepository.Update(empresa);
            _enderecoRepository.Update(empresa.Endereco);
            _unitOfWork.Commit();
        }

        public void AtualizarEmpresaDoPrestadorEAcesso(Empresa empresa, int idPrestadorEacesso)
        {
            var connectionStringEAcesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEAcesso))
            {
                dbConnection.Open();
                var tran = dbConnection.BeginTransaction();
                try
                {
                    
                    var nomeCidade = _cidadeService.BuscarPorId(empresa.Endereco.IdCidade.Value);
                    var idCidade = _prestadorService.ObterCidadeEmpresaEAcesso(nomeCidade.NmCidade, dbConnection, tran);
                    var prestador = _prestadorService.BuscarPorId(idPrestadorEacesso);
                    var erpExternoStr = " ";
                    if (empresa.IdEmpresaRm != null)
                    {
                        erpExternoStr = ",ErpExterno = " + empresa.IdEmpresaRm;
                    }

                    var query = @"UPDATE stfcorp.tblprofissionaisempresas SET" +
                            " RazaoSocial = '" + empresa.RazaoSocial + "'" +
                            ",AbrevLogradouro = '" + empresa.Endereco.SgAbrevLogradouro + "'" +
                            ",Endereco = '" + empresa.Endereco.NmEndereco + "'" +
                            ",Num = " + empresa.Endereco.NrEndereco +
                            ",Complemento = '" + empresa.Endereco.NmCompEndereco + "'" +
                            ",Bairro = '" + empresa.Endereco.NmBairro + "'" +
                            ",CEP = " + empresa.Endereco.NrCep +
                            ",IdCidade = " + idCidade +
                            ",CNPJ = " + empresa.Cnpj +
                            ",InscEst = '" + empresa.InscricaoEstadual + "'" +
                            ",Obs = '" + empresa.Observacao + "'" +
                            ",Atuacao = '" + empresa.Atuacao + "'" +
                            ",Inativo = " + (empresa.Ativo ? 0 : 1) +
                            ",DtVigencia = @DtVigencia" +
                            ",DtAlteracao = @DtAlteracao " +
                            erpExternoStr + //",ErpExterno = " + empresa.IdEmpresaRm +
                            " WHERE IdProfissional = " + prestador.CodEacessoLegado + "AND Inativo = " + 0;

                    dbConnection.Execute(query, new { DtVigencia = empresa.DataVigencia, DtAlteracao = DateTime.Now }, transaction: tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
               
            }
        }

        public void AdicionarEmpresaDoPrestadorEAcesso(Empresa empresa, int idPrestadorEacesso, Prestador prestador)
        {
            var connectionStringEAcesso = _connectionStrings.Value.EacessoConnection;
            IDbConnection dbConnection = new SqlConnection(connectionStringEAcesso);
            dbConnection.Open();
            var tran = dbConnection.BeginTransaction();
            try
            {               
                _prestadorService.InserirEmpresaEAcesso(prestador, dbConnection, idPrestadorEacesso, tran);
                tran.Commit();
            }
            catch (Exception ex )
            {
                tran.Rollback();
                dbConnection.Close();
                throw ex;
            }
            tran.Dispose();
            dbConnection.Close();

        }

        public void SalvarEmpresaPrestador(Empresa empresa)
        {
            _empresaRepository.Adicionar(empresa);
            _unitOfWork.Commit();
        }

        public void Inativar(int id)
        {
            var model = _empresaRepository.BuscarPorId(id);

            if (model != null)
            {
                model.Ativo = !model.Ativo;
                _empresaRepository.Update(model);
                _unitOfWork.Commit();
            }
        }

        public Empresa BuscarPorCnpj(string cnpj)
        {
            var empresa = _empresaRepository.BuscarPorCnpj(cnpj);
            return empresa;
        }

        public string ObterCodEmpresaRm(int idPrestador)
        {
            var codigoRM = _empresaRepository.ObterCodEmpresaRm(idPrestador);
            return codigoRM;
        }
    }
}