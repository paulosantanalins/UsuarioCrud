using Cadastro.Domain.PluginRoot.Dto;
using Cadastro.Domain.PluginRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utils;
using Utils.Connections;
using Utils.Extensions;

namespace Cadastro.Domain.PluginRoot.Service
{
    public class PluginRMService : IPluginRMService
    {
        private readonly IPrestadorService _prestadorService;
        private readonly IPrestadorRepository _prestadorRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IEmpresaService _empresaService;
        private readonly IHorasMesPrestadorService _horasMesPrestadorService;
        private readonly IDescontoPrestadorRepository _descontoPrestadorRepository;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IHorasMesPrestadorRepository _horasMesPrestadorRepository;
        private readonly ILogHorasMesPrestadorRepository _logHorasMesPrestadorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVariablesToken _variables;

        const string FORMATO_DATA_DB_RM = "yyyy-dd-MM HH:mm:ss";

        public PluginRMService(
            IPrestadorService prestadorService,
            IOptions<ConnectionStrings> connectionStrings,
            IPrestadorRepository prestadorRepository,
            IEmpresaRepository empresaRepository,
            IEmpresaService empresaService,
            IHorasMesPrestadorRepository horasMesPrestadorRepository,
            ILogHorasMesPrestadorRepository logHorasMesPrestadorRepository,
            IDescontoPrestadorRepository descontoPrestadorRepository,
            IHorasMesPrestadorService horasMesPrestadorService,
            IUnitOfWork unitOfWork, IVariablesToken variables)
        {
            _horasMesPrestadorService = horasMesPrestadorService;
            _prestadorRepository = prestadorRepository;
            _empresaService = empresaService;
            _empresaRepository = empresaRepository;
            _logHorasMesPrestadorRepository = logHorasMesPrestadorRepository;
            _horasMesPrestadorRepository = horasMesPrestadorRepository;
            _prestadorService = prestadorService;
            _descontoPrestadorRepository = descontoPrestadorRepository;
            _unitOfWork = unitOfWork;
            _variables = variables;
            _connectionStrings = connectionStrings;
        }

        #region ENVIAR EMPRESA RM
        public void EnviarEmpresaRM(int idPrestador, IDbConnection dbConnection, string idRepresentanteRM)
        {
            var prestadorDB = _prestadorService.BuscarPorId(idPrestador);
            prestadorDB.EmpresasPrestador = prestadorDB.EmpresasPrestador.OrderBy(x => x.Empresa.Ativo).ThenBy(x => x.DataAlteracao).ToList();

            foreach (var empresa in prestadorDB.EmpresasPrestador)
            {
                var codRm = ObterCodCfoNoRm(empresa.Empresa.Cnpj);

                if (string.IsNullOrEmpty(codRm))
                {
                    PluginEmpresaInsercaoDto pluginEmpresaInsercaoDto = PopularEmpresaPlugin(empresa.Empresa, prestadorDB);
                    pluginEmpresaInsercaoDto.OperacaoInt = empresa.Empresa.IdEmpresaRm.HasValue && empresa.Empresa.IdEmpresaRm.Value != 0 ? "A" : "I";
                    PopularCodCfoEmpresa(pluginEmpresaInsercaoDto.OperacaoInt, pluginEmpresaInsercaoDto, empresa.Empresa);

                    InserirIntFcfoImp(pluginEmpresaInsercaoDto, dbConnection);
                    InserirIntFcfoComplImp(pluginEmpresaInsercaoDto, dbConnection);
                    InserirIntFcfoContatoImp(pluginEmpresaInsercaoDto, dbConnection);
                    codRm = pluginEmpresaInsercaoDto.CodCfo;
                }

                if (!empresa.Empresa.IdEmpresaRm.HasValue || empresa.Empresa.IdEmpresaRm.Value == 0)
                {
                    AtualizarEmpresa(empresa.Empresa, codRm);
                    AtualizarEmpresaEAcesso(empresa.Empresa, codRm);
                }

                var nome = new string(TratarApostofre(empresa.Empresa.RazaoSocial)?.Take(44).ToArray());
                AtualizarTrprCompl(prestadorDB, codRm, nome, dbConnection, idRepresentanteRM);
            }
        }

        private static void InserirIntFcfoImp(PluginEmpresaInsercaoDto empresa, IDbConnection dbConnection)
        {
            string sQueryInsert = "INSERT INT_FCFO_IMP("
                                                + "codcoligada,"
                                                + "ativo,"
                                                + "idpais,"
                                                + "pagrec,"
                                                + "tipobairro,"
                                                + "tiporua,"
                                                + "tiporuaentrega,"
                                                + "tipobairroentrega,"
                                                + "idpaisentrega,"
                                                + "tiporuapgto,"
                                                + "tipobairropgto,"
                                                + "idpaispgto,"
                                                + "datainsercao_int,"
                                                + "datacriacao,"
                                                + "chaveorigem_int,"
                                                + "codcfo,"
                                                + "bairro,"
                                                + "cep,"
                                                + "cgccfo,"
                                                + "ci_uf,"
                                                + "paispagto,"
                                                + "numeropgto,"
                                                + "complementopgto,"
                                                + "bairropgto,"
                                                + "cidadepgto,"
                                                + "cidadeentrega,"
                                                + "paisentrega,"
                                                + "numeroentrega,"
                                                + "complementrega,"
                                                + "bairroentrega,"
                                                + "codmunicipioentrega,"
                                                + "ceppgto,"
                                                + "ruapgto,"
                                                + "numero,"
                                                + "rua,"
                                                + "pais,"
                                                + "cepentrega,"
                                                + "ruaentrega,"
                                                + "cidade,"
                                                + "codetd,"
                                                + "codmunicipio,"
                                                + "complemento,"
                                                + "inscrestadual,"
                                                + "nome,"
                                                + "status_int,"
                                                + "operacao_int,"
                                                + "PESSOAFISOUJUR,"
                                                + "CODCOLTCF,"
                                                + "NACIONALIDADE,"
                                                + "CODTCF,"
                                                + "nomefantasia,"
                                                + "CODETDENTREGA,"
                                                + "CODMUNICIPIOPGTO,"
                                                + "CODETDPGTO,"
                                                + "OPTANTEPELOSIMPLES ) VALUES ("

                                                + empresa.CodColigada + ","
                                                + (empresa.Ativo ? 1 : 0) + ","
                                                + empresa.IdPais + ","
                                                + empresa.PagRec + ","
                                                + empresa.TipoBairro + ","
                                                + empresa.TipoRua + ","
                                                + empresa.TipoRua + ","
                                                + empresa.TipoBairro + ","
                                                + empresa.IdPais + ","
                                                + empresa.TipoRua + ","
                                                + empresa.TipoBairro + ","
                                                + empresa.IdPais + ","
                                                + "'" + empresa.DataCriacao.ToString(FORMATO_DATA_DB_RM) + "',"
                                                + "'" + empresa.DataCriacao.ToString(FORMATO_DATA_DB_RM) + "',"
                                                + empresa.SeqlEACesso.Value + ","
                                                + "'" + empresa.CodCfo + "',"
                                                + "'" + empresa.Bairro + "',"
                                                + "'" + empresa.Cep + "',"
                                                + "'" + empresa.CgCcfo + "',"
                                                + "'" + empresa.CiUf + "',"
                                                + "'" + empresa.Pais + "',"
                                                + "'" + empresa.Numero + "',"
                                                + "'" + empresa.Complemento + "',"
                                                + "'" + empresa.Bairro + "',"
                                                + "'" + empresa.Cidade + "',"
                                                + "'" + empresa.Cidade + "',"
                                                + "'" + empresa.Pais + "',"
                                                + "'" + empresa.Numero + "',"
                                                + "'" + empresa.Complemento + "',"
                                                + "'" + empresa.Bairro + "',"
                                                + "'" + empresa.CodMunicipio + "',"
                                                + "'" + empresa.Cep + "',"
                                                + "'" + empresa.Rua + "',"
                                                + "'" + empresa.Numero + "',"
                                                + "'" + empresa.Rua + "',"
                                                + "'" + empresa.Pais + "',"
                                                + "'" + empresa.Cep + "',"
                                                + "'" + empresa.Rua + "',"
                                                + "'" + empresa.Cidade + "',"
                                                + "'" + empresa.CiUf + "',"
                                                + "'" + empresa.CodMunicipio + "',"
                                                + "'" + empresa.Complemento + "',"
                                                + "'" + empresa.InscrEstadual + "',"
                                                + "'" + empresa.Nome + "',"
                                                + "'" + empresa.StatusInt + "',"
                                                + "'" + empresa.OperacaoInt + "',"
                                                + "'" + empresa.PessoaFisOuJur + "',"
                                                + empresa.CodColTcf + ","
                                                + empresa.IdPais + ","
                                                + empresa.CodTcf + ","
                                                + "'" + empresa.Nome + "',"
                                                + "'" + empresa.CiUf + "',"
                                                + "'" + empresa.CodMunicipio + "',"
                                                + "'" + empresa.CiUf + "',"
                                                + empresa.OptantePeloSimples + ")";
            dbConnection.Query(sQueryInsert);
        }

        private static void InserirIntFcfoComplImp(PluginEmpresaInsercaoDto empresa, IDbConnection dbConnection)
        {
            string sQueryInsert = " INSERT INT_FCFOCOMPL_IMP ("
                                             + "STATUS_INT,"
                                             + "OPERACAO_INT,"
                                             + "DATAINSERCAO_INT,"
                                             + "CHAVEORIGEM_INT,"
                                             + "CODCOLIGADA,"
                                             + "CODCFO,"
                                             + "CIDORIGEM,"
                                             + "CAPLICATIVOORIGEM ) VALUES ("
                                             + "'" + empresa.StatusInt + "',"
                                             + "'" + empresa.OperacaoInt + "',"
                                             + "'" + empresa.DataCriacao.ToString(FORMATO_DATA_DB_RM) + "',"
                                             + empresa.SeqlEACesso.Value + ","
                                             + empresa.CodColigada + ","
                                             + "'" + empresa.CodCfo + "',"
                                             + "null,"
                                             + "'" + empresa.CAplicativoOrigem + "')";
            dbConnection.Query(sQueryInsert);
        }

        private static void InserirIntFcfoContatoImp(PluginEmpresaInsercaoDto empresa, IDbConnection dbConnection)
        {
            string sQueryInsert = " INSERT INT_FCFOCONTATO_IMP ("
                                                 + "STATUS_INT,"
                                                 + "OPERACAO_INT,"
                                                 + "DATAINSERCAO_INT,"
                                                 + "CHAVEORIGEM_INT,"
                                                 + "CODCOLIGADA,"
                                                 + "NOME,"
                                                 + "RUA,"
                                                 + "NUMERO,"
                                                 + "COMPLEMENTO,"
                                                 + "BAIRRO,"
                                                 + "CIDADE,"
                                                 + "CODETD,"
                                                 + "CEP,"
                                                 + "FUNCAO,"
                                                 + "TELEFONE,"
                                                 + "FAX,"
                                                 + "EMAIL,"
                                                 + "PAIS,"
                                                 + "IDCONTATO,"
                                                 + "CODCFO,"
                                                 + "RAMAL,"
                                                 + "DATANASCIMENTO,"
                                                 + "CODMUNICIPIO,"
                                                 + "ATIVO,"
                                                 + "CODAPLIC ) VALUES ("
                                                 + "'" + empresa.StatusInt + "',"
                                                 + "'" + empresa.OperacaoInt + "',"
                                                 + "'" + empresa.DataCriacao.ToString(FORMATO_DATA_DB_RM) + "',"
                                                 + empresa.SeqlEACesso.Value + ","
                                                 + empresa.CodColigada + ","
                                                 + "'" + empresa.NomeCont + "',"
                                                 + "'" + empresa.RuaCont + "',"
                                                 + "'" + empresa.NumeroCont + "',"
                                                 + "'" + empresa.ComplementoCont + "',"
                                                 + "'" + empresa.BairroCont + "',"
                                                 + "'" + empresa.CidadeCont + "',"
                                                 + "'" + empresa.CodEtdCont + "',"
                                                 + "'" + empresa.CepCont + "',"
                                                 + "'" + empresa.Funcao + "',"
                                                 + "'" + empresa.TelefoneCont + "',"
                                                 + "'" + empresa.CelularCont + "',"
                                                 + "'" + empresa.EmailCont + "',"
                                                 + "'" + empresa.PaisCont + "',"
                                                 + empresa.IdContato + ","
                                                 + "'" + empresa.CodCfo + "',"
                                                 + "'" + empresa.RamalCont + "',"
                                                 + "'" + empresa.DataNascimentoCont.ToString(FORMATO_DATA_DB_RM) + "',"
                                                 + "'" + empresa.IdCidadeCont + "',"
                                                 + (empresa.Ativo ? 1 : 0) + ","
                                                 + "'" + empresa.CodAplic + "')";
            dbConnection.Query(sQueryInsert);
        }

        public PluginEmpresaInsercaoDto PopularEmpresaPlugin(Empresa empresa, Prestador prestador)
        {
            var empresaPlugin = new PluginEmpresaInsercaoDto();

            empresaPlugin.IdPais = 1;
            empresaPlugin.TipoRua = ObterIdTipoRuaRm(empresa.Endereco.AbreviaturaLogradouro.Descricao);
            empresaPlugin.SeqlEACesso = ObterSequenceNoEacesso();
            empresaPlugin.Bairro = TratarApostofre(new string(empresa.Endereco.NmBairro?.Take(30).ToArray()));
            empresaPlugin.Cep = new string(empresa.Endereco.NrCep?.Take(9).ToArray());
            empresaPlugin.CgCcfo = new string(empresa.Cnpj?.Take(20).ToArray());
            empresaPlugin.CiUf = empresa.Endereco.Cidade.Estado.SgEstado;
            empresaPlugin.Pais = new string(TratarApostofre(empresa.Endereco.Cidade.Estado.Pais.NmPais)?.Take(20).ToArray());
            empresaPlugin.Numero = new string(empresa.Endereco.NrEndereco?.Take(8).ToArray());
            empresaPlugin.Complemento = new string(empresa.Endereco.NmCompEndereco?.Take(30).ToArray());
            empresaPlugin.Bairro = new string(TratarApostofre(empresa.Endereco.NmBairro)?.Take(30).ToArray());
            empresaPlugin.Cidade = new string(TratarApostofre(empresa.Endereco.Cidade.NmCidade.Trim())?.Take(32).ToArray());
            empresaPlugin.CodMunicipio = TratarApostofre(ObterCodigoMunicipioRm(empresa.Endereco.Cidade.NmCidade.Trim()));
            empresaPlugin.Rua = new string(TratarApostofre(empresa.Endereco.NmEndereco)?.Take(100).ToArray());
            empresaPlugin.InscrEstadual = string.IsNullOrEmpty(empresa.InscricaoEstadual) ? "ISENTO" : empresa.InscricaoEstadual;
            empresaPlugin.Nome = new string(TratarApostofre(empresa.RazaoSocial)?.Take(60).ToArray());
            empresaPlugin.OptantePeloSimples = ObterOptantePeloSimples(prestador.IdContratacao);

            empresaPlugin.NomeCont = new string(TratarApostofre(prestador.Pessoa.Nome)?.Take(50).ToArray());
            empresaPlugin.RuaCont = new string(TratarApostofre(prestador.Pessoa.Endereco.NmEndereco)?.Take(100).ToArray());
            empresaPlugin.NumeroCont = new string(prestador.Pessoa.Endereco.NrEndereco?.Take(8).ToArray());
            empresaPlugin.ComplementoCont = new string(prestador.Pessoa.Endereco.NmCompEndereco?.Take(30).ToArray());
            empresaPlugin.BairroCont = new string(TratarApostofre(prestador.Pessoa.Endereco.NmBairro)?.Take(30).ToArray());
            if (prestador.Pessoa.Endereco != null && prestador.Pessoa.Endereco.Cidade != null)
            {
                empresaPlugin.CidadeCont = new string(TratarApostofre(prestador.Pessoa.Endereco.Cidade.NmCidade.Trim())?.Take(32).ToArray());
                empresaPlugin.CodEtdCont = prestador.Pessoa.Endereco.Cidade.Estado.SgEstado;
                empresaPlugin.CodEtdCont = prestador.Pessoa.Endereco.Cidade.Estado.SgEstado;
                empresaPlugin.PaisCont = TratarApostofre(prestador.Pessoa.Endereco.Cidade.Estado.Pais.NmPais);
                empresaPlugin.IdCidadeCont = ObterCodigoMunicipioRm(TratarApostofre(prestador.Pessoa.Endereco.Cidade.NmCidade.Trim()));
            }
            else
            {
                empresaPlugin.CidadeCont = new string(TratarApostofre(empresa.Endereco.Cidade.NmCidade.Trim())?.Take(32).ToArray());
                empresaPlugin.CodEtdCont = empresa.Endereco.Cidade.Estado.SgEstado;
                empresaPlugin.CodEtdCont = empresa.Endereco.Cidade.Estado.SgEstado;
                empresaPlugin.PaisCont = TratarApostofre(empresa.Endereco.Cidade.Estado.Pais.NmPais);
                empresaPlugin.IdCidadeCont = ObterCodigoMunicipioRm(TratarApostofre(empresa.Endereco.Cidade.NmCidade.Trim()));
            }
            empresaPlugin.CepCont = new string(prestador.Pessoa.Endereco.NrCep?.Take(9).ToArray());
            empresaPlugin.TelefoneCont = new string(prestador.Pessoa.Telefone.NumeroResidencial?.Take(15).ToArray());
            empresaPlugin.CelularCont = new string(prestador.Pessoa.Telefone.Celular?.Take(15).ToArray());
            empresaPlugin.EmailCont = string.IsNullOrEmpty(prestador.Pessoa.EmailInterno) ? prestador.Pessoa.Email : prestador.Pessoa.EmailInterno;
            empresaPlugin.EmailCont = new string(empresaPlugin.EmailCont?.Take(80).ToArray());
            empresaPlugin.IdContato = 1;
            empresaPlugin.RamalCont = new string(prestador.Pessoa.Telefone.NumeroComercialRamal?.Take(6).ToArray());
            empresaPlugin.DataNascimentoCont = (DateTime)prestador.Pessoa.DtNascimento;

            return empresaPlugin;
        }

        private void AtualizarEmpresa(Empresa empresa, string codCfo)
        {
            empresa.IdEmpresaRm = Int32.Parse(codCfo);
            _empresaRepository.Update(empresa);
            _unitOfWork.Commit();
        }

        private void AtualizarEmpresaEAcesso(Empresa empresa, string codCfo)
        {
            var idEmpresaRm = Int32.Parse(codCfo);
            var connectionStringEAcesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEAcesso))
            {
                var query = "UPDATE STFCORP.TBLPROFISSIONAISEMPRESAS SET ERPEXTERNO = " + idEmpresaRm + " WHERE CNPJ = '" + empresa.Cnpj +"'";
                dbConnection.Execute(query);
                dbConnection.Close();
            }
        }

        private int ObterIdTipoRuaRm(string descricao)
        {
            int? id = null;
            var connectionStringRM = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRM))
            {
                var query = "SELECT CODIGO FROM DTIPORUA WHERE DESCRICAO = '" + descricao + "'";
                id = dbConnection.Query<int>(query).FirstOrDefault();
                dbConnection.Close();
            }
            return id ?? 1;
        }

        private string ObterCodCfoEmpresaNoRm()
        {
            int ultimoValor = 0;
            var connectionStringRm = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRm))
            {
                var query = "SELECT MAX(CAST(codcfo AS INT)) FROM DBO.fcfo";
                var incrementoPlugin = dbConnection.Query<int>(query).FirstOrDefault();
                query = "SELECT VALAUTOINC FROM DBO.gAutoInc WHERE CODAUTOINC = 'CODCFO' AND CODCOLIGADA = 0";
                var tempcodautoinc = dbConnection.Query<int>(query).FirstOrDefault();

                if (incrementoPlugin > tempcodautoinc)
                {
                    query = "UPDATE DBO.gAutoInc SET VALAUTOINC = " + incrementoPlugin + " WHERE CODAUTOINC = 'CODCFO' AND CODCOLIGADA = 0";
                    dbConnection.Execute(query);
                }

                query = "SELECT MAX(VALAUTOINC) FROM DBO.gAutoInc WHERE CODAUTOINC = 'CODCFO' AND CODCOLIGADA = 0";
                ultimoValor = dbConnection.Query<int>(query).FirstOrDefault() + 1;
                query = "UPDATE DBO.gAutoInc SET VALAUTOINC = " + ultimoValor + " WHERE CODAUTOINC = 'CODCFO' AND CODCOLIGADA = 0";
                dbConnection.Execute(query);

                dbConnection.Close();
            }
            var cod = "000000" + ultimoValor;
            return cod.Substring(cod.Length - 6);
        }

        private int ObterOptantePeloSimples(int idTipoContrato)
        {
            if (idTipoContrato == 1591745077 || idTipoContrato == 1591745075)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private string ObterCodigoMunicipioRm(string municipio)
        {
            string id = "";
            var connectionStringRM = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRM))
            {
                var query = "SELECT CODMUNICIPIO FROM GMUNICIPIO WHERE NOMEMUNICIPIO = '" + municipio.TrimEnd() + "'";
                id = dbConnection.Query<string>(query).FirstOrDefault();
                dbConnection.Close();
            }
            return id == null ? "" : id;
        }

        private void PopularCodCfoEmpresa(string operacao, PluginEmpresaInsercaoDto pluginEmpresaInsercaoDto, Empresa empresa)
        {
            if (operacao == "I")
            {
                pluginEmpresaInsercaoDto.CodCfo = ObterCodCfoEmpresaNoRm();
            }
            else
            {
                var cod = "000000" + empresa.IdEmpresaRm;
                pluginEmpresaInsercaoDto.CodCfo = cod.Substring(cod.Length - 6);
            }
        }
        #endregion

        #region SOLICITAR PAGAMENTO
        public void SolicitarPagamentoRM(int idHorasMesPrestador)
        {
            var solicitacaoPlugin = PopularPagamentoPlugin(idHorasMesPrestador);
            InserirSolicitarPagamentoRM(solicitacaoPlugin);
            AtualizarSituacao(idHorasMesPrestador, solicitacaoPlugin.SeqlEACesso.Value);
        }

        private void AtualizarSituacao(int idHorasMesPrestador, int idChaveOrigemIntRm)
        {
            var horasMesPrestador = _horasMesPrestadorRepository.BuscarPorId(idHorasMesPrestador);

            LogHorasMesPrestador log = new LogHorasMesPrestador
            {
                SituacaoAnterior = horasMesPrestador.Situacao,
                SituacaoNova = SharedEnuns.TipoSituacaoHorasMesPrestador.AGUARDANDO_INTEGRACAO.GetDescription(),
                DataAlteracao = DateTime.Now,
                Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName,
                IdHorasMesPrestador = idHorasMesPrestador
            };
            _logHorasMesPrestadorRepository.Adicionar(log);

            horasMesPrestador.IdChaveOrigemIntRm = idChaveOrigemIntRm;
            horasMesPrestador.Situacao = SharedEnuns.TipoSituacaoHorasMesPrestador.AGUARDANDO_INTEGRACAO.GetDescription();
            _horasMesPrestadorRepository.Update(horasMesPrestador);

            _unitOfWork.Commit();
        }

        private void InserirSolicitarPagamentoRM(PluginSolicitarPagamentoDto solicitacao)
        {
            var connectionStringIntegracaoRM = _connectionStrings.Value.RMIntegracaoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringIntegracaoRM))
            {
                dbConnection.Open();
                InserirIntTmovImp(solicitacao, dbConnection);
                InserirIntTmovHistoricoImp(solicitacao, dbConnection);
                InserirIntTmovComplImp(solicitacao, dbConnection);
                InserirIntTitMMovImp(solicitacao, dbConnection);
                InserirIntTitMMovComplImp(solicitacao, dbConnection);
                InserirIntTitMMovRatCcuImp(solicitacao, dbConnection);
                InserirIntTitMMovHistoricoImp(solicitacao, dbConnection);
                dbConnection.Close();
            }
        }

        private static void InserirIntTmovImp(PluginSolicitarPagamentoDto solicitacao, IDbConnection dbConnection)
        {
            string sQueryInsert = " INSERT INT_TMOV_IMP ("
                                    + "STATUS_INT,"
                                    + "OPERACAO_INT,"
                                    + "DATAINSERCAO_INT,"
                                    + "CHAVEORIGEM_INT,"
                                    + "SERIE,"
                                    + "NUMEROMOV,"
                                    + "codfilial,"
                                    + "codcoligada,"
                                    + "datacriacao,"
                                    + "dataemissao,"
                                    + "dataentrega,"
                                    + "valorbruto,"
                                    + "codcfo,"
                                    + "codtmv,"
                                    + "nordem,"
                                    + "status,"
                                    + "CODCOLCFO,"
                                    + "CODLOC,"
                                    + "codcpg,"
                                    + "CODTB1FLX ) " +
                                    "VALUES ("
                                    + "'" + solicitacao.StatusInt + "',"
                                    + "'" + solicitacao.OperacaoInt + "',"
                                    + "'" + solicitacao.DataInsercaoInt.ToString(FORMATO_DATA_DB_RM) + "',"
                                    + solicitacao.SeqlEACesso.Value + ","
                                    + "'" + solicitacao.Serie + "',"
                                    + "'" + solicitacao.NumeroMov + "',"
                                    + solicitacao.CodFilial.Value + ","
                                    + solicitacao.CodColigada.Value + ","
                                    + "'" + solicitacao.DataCriacao.ToString(FORMATO_DATA_DB_RM) + "',"
                                    + "'" + solicitacao.DataEmissao.ToString(FORMATO_DATA_DB_RM) + "',"
                                    + "'" + solicitacao.DataEntrega.ToString(FORMATO_DATA_DB_RM) + "',"
                                    + solicitacao.ValorBruto.ToString().Replace(",", ".") + ","
                                    + "'" + solicitacao.CodCfo + "',"
                                    + "'" + solicitacao.CodTmv + "',"
                                    + "null,"
                                    + "'" + solicitacao.Status + "',"
                                    + solicitacao.CodColCfo + ","
                                    + "'" + solicitacao.CodLoc + "',"
                                    + "'" + solicitacao.CodCpg + "',"
                                    + "'" + solicitacao.CodContabil + "')";
            dbConnection.Query(sQueryInsert);
        }

        private static void InserirIntTmovHistoricoImp(PluginSolicitarPagamentoDto solicitacao, IDbConnection dbConnection)
        {
            string sQueryInsert = " INSERT INT_TMOVHISTORICO_IMP ("
                                    + "STATUS_INT,"
                                    + "OPERACAO_INT,"
                                    + "DATAINSERCAO_INT,"
                                    + "CHAVEORIGEM_INT,"
                                    + "codcoligada,"
                                    + "historicocurto) "
                                    + "VALUES ( "
                                    + "'" + solicitacao.StatusInt + "',"
                                    + "'" + solicitacao.OperacaoInt + "',"
                                    + "'" + solicitacao.DataInsercaoInt.ToString(FORMATO_DATA_DB_RM) + "',"
                                    + solicitacao.SeqlEACesso.Value + ","
                                    + solicitacao.CodColigada.Value + ","
                                    + "'" + solicitacao.HistoricoCurto + "')";
            dbConnection.Query(sQueryInsert);
        }

        private static void InserirIntTmovComplImp(PluginSolicitarPagamentoDto solicitacao, IDbConnection dbConnection)
        {
            string sQueryInsert = " INSERT INT_TMOVCOMPL_IMP ("
                                    + "STATUS_INT,"
                                    + "OPERACAO_INT,"
                                    + "DATAINSERCAO_INT,"
                                    + "CHAVEORIGEM_INT,"
                                    + "codcoligada,"
                                    + "MGRUPO,"
                                    + "MDIRETORIA,"
                                    + "MVP,"
                                    + "MEXECUTOR) " +
                                    "VALUES ( "
                                    + "'" + solicitacao.StatusInt + "',"
                                    + "'" + solicitacao.OperacaoInt + "',"
                                    + "'" + solicitacao.DataInsercaoInt.ToString(FORMATO_DATA_DB_RM) + "',"
                                    + solicitacao.SeqlEACesso.Value + ","
                                    + solicitacao.CodColigada.Value + ","
                                    + solicitacao.CelulaPrestador.IdGrupo.Value + ","
                                    + solicitacao.CelulaPrestador.IdCelulaSuperior.Value + ","
                                    + "null,"
                                    + "'ADMIN')";
            dbConnection.Query(sQueryInsert);
        }

        private static void InserirIntTitMMovImp(PluginSolicitarPagamentoDto solicitacao, IDbConnection dbConnection)
        {
            string sQueryInsert = " INSERT INT_TITMMOV_IMP ("
                                    + "STATUS_INT,"
                                    + "OPERACAO_INT,"
                                    + "DATAINSERCAO_INT,"
                                    + "CHAVEORIGEM_INT,"
                                    + "CODCOLIGADA,"
                                    + "idprd,"
                                    + "dataentrega,"
                                    + "precounitario,"
                                    + "quantidade,"
                                    + "codund,"
                                    + "NSEQITMMOV,"
                                    + "CODRPR,"
                                    + "CAMPOLIVRE) " +
                                    "VALUES ( "
                                    + "'" + solicitacao.StatusInt + "',"
                                    + "'" + solicitacao.OperacaoInt + "',"
                                    + "'" + solicitacao.DataInsercaoInt.ToString(FORMATO_DATA_DB_RM) + "',"
                                    + solicitacao.SeqlEACesso.Value + ","
                                    + solicitacao.CodColigada.Value + ","
                                    + solicitacao.IdProduto + ","
                                    + "'" + solicitacao.DataEntrega.ToString(FORMATO_DATA_DB_RM) + "',"
                                    + solicitacao.ValorBruto.ToString().Replace(",", ".") + ","
                                    + solicitacao.Quantidade + ","
                                    + "'" + solicitacao.Codund + "',"
                                    + solicitacao.Contador + ","
                                    + "'" + solicitacao.CodProf + "',"
                                    + "null)";
            dbConnection.Query(sQueryInsert);
        }

        private static void InserirIntTitMMovComplImp(PluginSolicitarPagamentoDto solicitacao, IDbConnection dbConnection)
        {
            string sQueryInsert = " INSERT INT_TITMMOVCOMPL_IMP ("
                                    + "STATUS_INT,"
                                    + "OPERACAO_INT,"
                                    + "DATAINSERCAO_INT,"
                                    + "CHAVEORIGEM_INT,"
                                    + "CODCOLIGADA,"
                                    + "IDFICHA,"
                                    + "IDDETALHE,"
                                    + "IEMPRESTIMO,"
                                    + "IDESCVIAGEM,"
                                    + "IDESCMULTATRANSITO,"
                                    + "INOTEBOOKCOMPRA,"
                                    + "IPENSAOALIMENTICIA,"
                                    + "IDESCSINISTRO,"
                                    + "NSEQITMMOV,"
                                    + "IDESCTELMOV) " +
                                    "VALUES ( "
                                    + "'" + solicitacao.StatusInt + "',"
                                    + "'" + solicitacao.OperacaoInt + "',"
                                    + "'" + solicitacao.DataInsercaoInt.ToString(FORMATO_DATA_DB_RM) + "',"
                                    + solicitacao.SeqlEACesso.Value + ","
                                    + solicitacao.CodColigada + ","
                                    + "null" + ","
                                    + solicitacao.IdDetalhe + ","
                                    + solicitacao.MadEmprestimo.ToString().Replace(",", ".") + ","
                                    + solicitacao.MDescAdViagem.ToString().Replace(",", ".") + ","
                                    + solicitacao.MMultaTransito.ToString().Replace(",", ".") + ","
                                    + solicitacao.MNotebookCompra.ToString().Replace(",", ".") + ","
                                    + solicitacao.MPensaoAlimenticia.ToString().Replace(",", ".") + ","
                                    + solicitacao.MDescSinistro.ToString().Replace(",", ".") + ","
                                    + solicitacao.Contador + ","
                                    + solicitacao.MDescTelMov.ToString().Replace(",", ".") + ")";
            dbConnection.Query(sQueryInsert);
        }

        private static void InserirIntTitMMovRatCcuImp(PluginSolicitarPagamentoDto solicitacao, IDbConnection dbConnection)
        {
            string sQueryInsert = " INSERT INT_TITMMOVRATCCU_IMP ("
                                    + "STATUS_INT,"
                                    + "OPERACAO_INT,"
                                    + "DATAINSERCAO_INT,"
                                    + "CHAVEORIGEM_INT,"
                                    + "CODCOLIGADA,"
                                    + "CODCCUSTO,"
                                    + "PERCENTUAL,"
                                    + "VALOR,"
                                    + "NSEQITMMOV) " +
                                    "VALUES ( "
                                    + "'" + solicitacao.StatusInt + "',"
                                    + "'" + solicitacao.OperacaoInt + "',"
                                    + "'" + solicitacao.DataInsercaoInt.ToString(FORMATO_DATA_DB_RM) + "',"
                                    + solicitacao.SeqlEACesso.Value + ","
                                    + solicitacao.CodColigada + ","
                                    + "'" + solicitacao.CodCCusto + "',"
                                    + "null,"
                                    + solicitacao.ValorBruto.ToString().Replace(",", ".") + ","
                                    + solicitacao.Contador + ")";
            dbConnection.Query(sQueryInsert);
        }

        private static void InserirIntTitMMovHistoricoImp(PluginSolicitarPagamentoDto solicitacao, IDbConnection dbConnection)
        {
            string sQueryInsert = " INSERT INT_TITMMOVHISTORICO_IMP ("
                                    + "STATUS_INT,"
                                    + "OPERACAO_INT,"
                                    + "DATAINSERCAO_INT,"
                                    + "CHAVEORIGEM_INT,"
                                    + "CODCOLIGADA,"
                                    + "HISTORICOCURTO,"
                                    + "NSEQITMMOV) " +
                                    "VALUES ( "
                                    + "'" + solicitacao.StatusInt + "',"
                                    + "'" + solicitacao.OperacaoInt + "',"
                                    + "'" + solicitacao.DataInsercaoInt.ToString(FORMATO_DATA_DB_RM) + "',"
                                    + solicitacao.SeqlEACesso.Value + ","
                                    + solicitacao.CodColigada + ","
                                    + "'" + solicitacao.Historico + "',"
                                    + solicitacao.Contador + ")";
            dbConnection.Query(sQueryInsert);
        }

        private PluginSolicitarPagamentoDto PopularPagamentoPlugin(int id)
        {
            var lancamentoMes = _horasMesPrestadorService.BuscarPorIdComInclude(id);

            var prestadorPlugin = new PluginSolicitarPagamentoDto();

            prestadorPlugin.SeqlEACesso = ObterSequenceNoEacesso();
            prestadorPlugin.NumeroMov = ObterNumeroMovNoRm(lancamentoMes.Prestador.IdEmpresaGrupo);
            prestadorPlugin.CodFilial = lancamentoMes.Prestador.PagarPelaMatriz ? 1 : (lancamentoMes.Prestador.IdFilial == 0 ? 1 : lancamentoMes.Prestador.IdFilial);
            prestadorPlugin.CodColigada = lancamentoMes.Prestador.IdEmpresaGrupo;
            prestadorPlugin.DataEntrega = new DateTime(lancamentoMes.HorasMes.Ano, lancamentoMes.HorasMes.Mes, Int32.Parse(lancamentoMes.Prestador.IdDiaPagamento.HasValue ? lancamentoMes.Prestador.DiaPagamento.DescricaoValor : 10.ToString())).AddMonths(1);
            prestadorPlugin.ValorBruto = CalcularValorBruto(lancamentoMes);
            prestadorPlugin.CodCfo = ObterCodCfoNoRm(lancamentoMes.Prestador.EmpresasPrestador);
            prestadorPlugin.ChaveOrigemInt = null; //epagamento.pagamento.id
            prestadorPlugin.CodLoc = ObterCodLocNoRm(lancamentoMes.Prestador.IdEmpresaGrupo, lancamentoMes.Prestador.IdFilial);
            prestadorPlugin.CodCpg = MontarCodCPG(lancamentoMes.Prestador.DiaPagamento.DescricaoValor);
            prestadorPlugin.CodContabil = ObterCodigoContabil(lancamentoMes);

            prestadorPlugin.HistoricoCurto = $@"BASE E-PAG - PERIODO PAGTO REF {lancamentoMes.HorasMes.Mes}/{lancamentoMes.HorasMes.Ano}
            {lancamentoMes.ObservacaoSemPrestacaoServico}";

            prestadorPlugin.CelulaPrestador = lancamentoMes.Prestador.Celula;

            prestadorPlugin.IdProduto = lancamentoMes.Prestador.IdProdutoRm;
            prestadorPlugin.CodProf = "000000" + lancamentoMes.Prestador.IdRepresentanteRmTRPR;
            prestadorPlugin.CodProf = prestadorPlugin.CodProf.Substring(prestadorPlugin.CodProf.Length - 6);

            prestadorPlugin.IdDetalhe = Int32.Parse("1" + id);

            var clienteServicoAtivo = lancamentoMes.Prestador.ClientesServicosPrestador.FirstOrDefault(x => x.Ativo);
            if (clienteServicoAtivo == null)
            {
                clienteServicoAtivo = lancamentoMes.Prestador.ClientesServicosPrestador.OrderByDescending(x => x.DataAlteracao).FirstOrDefault();
            }

            if (clienteServicoAtivo != null)
            {
                prestadorPlugin.CodCCusto = ("000" + clienteServicoAtivo.IdCelula).Substring((("000" + clienteServicoAtivo.IdCelula).Length) - 4) + "." +
                            ("0000" + clienteServicoAtivo.IdCliente).Substring((("0000" + clienteServicoAtivo.IdCliente).Length) - 5) + "." +
                                ("00000" + clienteServicoAtivo.IdServico).Substring((("00000" + clienteServicoAtivo.IdServico).Length) - 6);
            }

            prestadorPlugin.Historico = "PAGTO REF: " + lancamentoMes.HorasMes.Mes + "/" + lancamentoMes.HorasMes.Ano + " - ID CONTRATAÇÃO: " + lancamentoMes.Prestador.Contratacao.IdValor +
                            " - VALOR BRUTO: " + CalcularValorBruto(lancamentoMes);

            prestadorPlugin.MadEmprestimo = _descontoPrestadorRepository.ObterValorDescontoPorIdHorasMesPrestador(id, SharedEnuns.TipoDesconto.EMPRESTIMO.GetDescription());
            prestadorPlugin.MDescAdViagem = _descontoPrestadorRepository.ObterValorDescontoPorIdHorasMesPrestador(id, SharedEnuns.TipoDesconto.DESC_AD_VIAGEM.GetDescription());
            prestadorPlugin.MMultaTransito = _descontoPrestadorRepository.ObterValorDescontoPorIdHorasMesPrestador(id, SharedEnuns.TipoDesconto.MULTA_TRANSITO.GetDescription());
            prestadorPlugin.MNotebookCompra = _descontoPrestadorRepository.ObterValorDescontoPorIdHorasMesPrestador(id, SharedEnuns.TipoDesconto.COMPRA_NOTEBOOK.GetDescription());
            prestadorPlugin.MPensaoAlimenticia = _descontoPrestadorRepository.ObterValorDescontoPorIdHorasMesPrestador(id, SharedEnuns.TipoDesconto.PENSAO_ALIMENTICIA.GetDescription());
            prestadorPlugin.MDescSinistro = _descontoPrestadorRepository.ObterValorDescontoPorIdHorasMesPrestador(id, SharedEnuns.TipoDesconto.SINISTRO.GetDescription());
            prestadorPlugin.MDescTelMov = _descontoPrestadorRepository.ObterValorDescontoPorIdHorasMesPrestador(id, SharedEnuns.TipoDesconto.TELEFONIA_CELULAR.GetDescription());

            return prestadorPlugin;
        }

        private string ObterCodLocNoRm(int? idEmpresaGrupo, int idFilial)
        {
            var connectionStringRM = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRM))
            {
                var query = "SELECT CODLOC FROM DBO.TLOC WHERE " +
                    "codcoligada = " + idEmpresaGrupo.Value + " and codfilial = " + idFilial;
                var codLoc = dbConnection.Query<string>(query).FirstOrDefault();
                dbConnection.Close();

                return codLoc;
            }
        }

        private string MontarCodCPG(string diaPagamento)
        {
            int diaPagamentoInt = Int32.Parse(diaPagamento);
            var result = "F" + (diaPagamentoInt > 9 ? diaPagamento : "0" + diaPagamentoInt);
            return result;
        }

        private string ObterCodCfoNoRm(ICollection<EmpresaPrestador> empresas)
        {
            var empresaAtual = empresas.Select(x => x.Empresa).OrderByDescending(x => x.Id).FirstOrDefault(x => x.Ativo);
            if (empresaAtual != null)
            {
                string cnpj = empresaAtual.Cnpj;
                string codCfo = "";
                var connectionStringRM = _connectionStrings.Value.RMConnection;
                using (IDbConnection dbConnection = new SqlConnection(connectionStringRM))
                {
                    var query = "SELECT CODCFO FROM DBO.FCFO FCFO WHERE " +
                        "SUBSTRING(FCFO.CGCCFO, 1, 2) + SUBSTRING(FCFO.CGCCFO, 4, 3) + SUBSTRING(FCFO.CGCCFO, 8, 3) + SUBSTRING(FCFO.CGCCFO, 12, 4) + SUBSTRING(FCFO.CGCCFO, 17, 2) " +
                        "COLLATE DATABASE_DEFAULT = '" + cnpj + "'";
                    codCfo = dbConnection.Query<string>(query).FirstOrDefault();
                    dbConnection.Close();
                }
                return codCfo;
            }
            else
            {
                return "";
            }
        }

        private string ObterCodCfoNoRm(string cnpj)
        {
            string codCfo = "";
            var connectionStringRM = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRM))
            {
                var query = "SELECT CODCFO FROM DBO.FCFO FCFO WHERE " +
                    "SUBSTRING(FCFO.CGCCFO, 1, 2) + SUBSTRING(FCFO.CGCCFO, 4, 3) + SUBSTRING(FCFO.CGCCFO, 8, 3) + SUBSTRING(FCFO.CGCCFO, 12, 4) + SUBSTRING(FCFO.CGCCFO, 17, 2) " +
                    "COLLATE DATABASE_DEFAULT = '" + cnpj + "'";
                codCfo = dbConnection.Query<string>(query).FirstOrDefault();
                dbConnection.Close();
            }
            return codCfo;
        }

        private string ObterNumeroMovNoRm(int? idEmpresaGrupo)
        {
            string numeroMov = "";
            var connectionStringRM = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRM))
            {
                var query = "SELECT VALAUTOINC FROM DBO.gAutoInc WHERE CODCOLIGADA = " + idEmpresaGrupo.Value + " AND CODAUTOINC = 'OC 000000' AND CODSISTEMA = 'T'";
                var sequence = dbConnection.Query<int>(query).FirstOrDefault();
                query = "UPDATE DBO.gAutoInc SET VALAUTOINC = " + (sequence + 1) + " WHERE CODCOLIGADA = " + idEmpresaGrupo.Value + " AND CODAUTOINC = 'OC 000000' AND CODSISTEMA = 'T'";
                dbConnection.Execute(query);
                dbConnection.Close();

                numeroMov = ("000000" + sequence);
                numeroMov = numeroMov.Substring(numeroMov.Length - 6);
            }
            return numeroMov;
        }

        private int? ObterSequenceNoEacesso()
        {
            int? sequence = null;
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                var query = "UPDATE stfcorp.tblSequenceRM SET IDSeq = IDSeq + 1";
                dbConnection.Execute(query);
                query = "SELECT MAX(IDSeq) FROM stfcorp.tblSequenceRM";
                sequence = dbConnection.Query<int>(query).FirstOrDefault();
                dbConnection.Close();
            }
            return sequence;
        }

        private string ObterCodigoContabil(HorasMesPrestador lancamentoMes)
        {
            var idContratacao = lancamentoMes.Prestador.Contratacao.IdValor;

            var idsLtda = new List<int> { 2, 7, 1591745075, 1591745077 };
            var idsEasyEpg = new List<int> { 4, 1591745073 };
            var idsImpacto = new List<int> { 1591745078, 1591745081 };
            var idsMax = new List<int> { 1591745076, 1591745080 };

            string subTipoCompra = "";
            subTipoCompra = ObterSubTipoCompra(idContratacao, idsLtda, idsEasyEpg, idsImpacto, idsMax, subTipoCompra);

            string codigoContabil = ObterCodigoContabilNoEacesso(subTipoCompra);

            return codigoContabil;
        }

        private static string ObterSubTipoCompra(int idContratacao, List<int> idsLtda, List<int> idsEasyEpg, List<int> idsImpacto, List<int> idsMax, string subTipoCompra)
        {
            if (idsLtda.Any(x => x == idContratacao))
            {
                subTipoCompra = "LTDA";
            }
            else if (idsEasyEpg.Any(x => x == idContratacao))
            {
                subTipoCompra = "EASY-EPG";
            }
            else if (idsImpacto.Any(x => x == idContratacao))
            {
                subTipoCompra = "IMPACTO";
            }
            else if (idsMax.Any(x => x == idContratacao))
            {
                subTipoCompra = "MAX";
            }

            return subTipoCompra;
        }

        private string ObterCodigoContabilNoEacesso(string subTipoCompra)
        {
            string codigoContabil = "";
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                var query = "SELECT Cod_Contabil FROM stfcorp.tblComprasCodigosSub WHERE SubCodigoTipoCompra = '" + subTipoCompra + "' AND CodigoTipoCompra = 'TERC'";
                codigoContabil = dbConnection.Query<string>(query).AsList().FirstOrDefault();
                dbConnection.Close();
            }

            return codigoContabil;
        }

        private decimal CalcularValorBruto(HorasMesPrestador horasMes)
        {
            if (horasMes != null)
            {
                var valorAtual = horasMes.Prestador.ValoresPrestador.ToList().OrderByDescending(x => x.DataAlteracao).FirstOrDefault();

                if (valorAtual != null)
                {
                    if (horasMes.SemPrestacaoServico)
                    {
                        return 0;
                    }
                    else
                    {
                        decimal totalPagamentoMensal = horasMes.Prestador.TipoRemuneracao.DescricaoValor.Equals("MENSALISTA") ? _prestadorRepository.ObterValorMensalista(valorAtual, horasMes) :
                            (horasMes.Prestador.TipoRemuneracao.DescricaoValor.Equals("HORISTA") ?
                                (valorAtual.ValorHora * (horasMes.Horas ?? 0)) : 0);

                        decimal totalHoraAdicional = 0;
                        if (horasMes.Extras.HasValue)
                        {
                            totalHoraAdicional = horasMes.Extras.Value * valorAtual.ValorHora;
                        }

                        return totalPagamentoMensal + totalHoraAdicional;
                    }
                }
                else
                {
                    return 0;
                }
            }

            return 0;
        }
        #endregion

        #region ENVIAR PRESTADOR RM
        public PluginPrestadorInsercaoDto PopularPrestadorPlugin(int id)
        {
            var prestadorDB = _prestadorService.BuscarPorId(id);
            var prestadorPlugin = new PluginPrestadorInsercaoDto();

            prestadorPlugin.SeqlEACesso = ObterSequenceNoEacesso();
            prestadorPlugin.CodColigada = prestadorDB.IdEmpresaGrupo.ToString();
            prestadorPlugin.Nome = TratarApostofre(new string(prestadorDB.Pessoa.Nome?.Take(40).ToArray()));
            prestadorPlugin.Email = prestadorDB.Pessoa.EmailInterno == null ?
                new string(prestadorDB.Pessoa.Email?.Take(40).ToArray()) : new string(prestadorDB.Pessoa.EmailInterno?.Take(40).ToArray());

            PreencherEnderecoPrestador(prestadorDB, prestadorPlugin);
            PreencherTelefonePrestador(prestadorDB, prestadorPlugin);
            PreencherTabelaComplementarPrestador(prestadorDB, prestadorPlugin);

            return prestadorPlugin;
        }

        private static void PreencherTabelaComplementarPrestador(Prestador prestadorDB, PluginPrestadorInsercaoDto prestadorPlugin)
        {
            var empresa = prestadorDB.EmpresasPrestador.Select(x => x.Empresa).Where(x => x.Ativo).OrderByDescending(x => x.DataAlteracao).FirstOrDefault();
            prestadorPlugin.IdCliente = ("000000" + empresa.IdEmpresaRm).Substring(("000000" + empresa.IdEmpresaRm).Length - 6);
            prestadorPlugin.Cliente = empresa.RazaoSocial.Length > 44 ? empresa.RazaoSocial.Substring(0, 44) : empresa.RazaoSocial;
            prestadorPlugin.IdProfissional = prestadorDB.Id; //Prof.Id;
        }

        private static void PreencherTelefonePrestador(Prestador prestadorDB, PluginPrestadorInsercaoDto prestadorPlugin)
        {
            if (prestadorDB.Pessoa.Telefone != null)
            {
                prestadorPlugin.Telefone = new string(prestadorDB.Pessoa.Telefone.NumeroResidencial?.Take(15).ToArray());
                prestadorPlugin.Celular = new string(prestadorDB.Pessoa.Telefone.Celular?.Take(15).ToArray());
                prestadorPlugin.Fax = new string(prestadorDB.Pessoa.Telefone.NumeroNextel?.Take(15).ToArray());
            }
        }

        private void PreencherEnderecoPrestador(Prestador prestadorDB, PluginPrestadorInsercaoDto prestadorPlugin)
        {
            if (prestadorDB.Pessoa.Endereco != null)
            {
                prestadorPlugin.Rua = TratarApostofre(prestadorDB.Pessoa.Endereco.NmEndereco);
                prestadorPlugin.Complemento = TratarApostofre(new string(prestadorDB.Pessoa.Endereco.NmCompEndereco?.Take(30).ToArray()));
                prestadorPlugin.Bairro = TratarApostofre(new string(prestadorDB.Pessoa.Endereco.NmBairro?.Take(30).ToArray()));
                prestadorPlugin.Cep = new string(prestadorDB.Pessoa.Endereco.NrCep?.Take(9).ToArray());

                if (prestadorDB.Pessoa.Endereco.Cidade != null)
                {
                    prestadorPlugin.Cidade = TratarApostofre(new string(prestadorDB.Pessoa.Endereco.Cidade.NmCidade?.Take(32).ToArray()).TrimEnd());
                    if (prestadorDB.Pessoa.Endereco.Cidade.Estado != null)
                    {
                        prestadorPlugin.CodEtd = new string(prestadorDB.Pessoa.Endereco.Cidade.Estado.SgEstado?.Take(2).ToArray());
                        if (prestadorDB.Pessoa.Endereco.Cidade.Estado.Pais != null)
                        {
                            prestadorPlugin.Pais = TratarApostofre(new string(prestadorDB.Pessoa.Endereco.Cidade.Estado.Pais.NmPais?.Take(20).ToArray()));
                        }
                    }
                }
            }
        }

        private string ObterCodCfoPrestadorNoRm()
        {
            string cod = null;
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                var query = "UPDATE stfcorp.tbSequenceRepresentanteRM SET ID = ID + 1";
                dbConnection.Execute(query);
                query = "SELECT MAX(ID) FROM stfcorp.tbSequenceRepresentanteRM";
                cod = dbConnection.Query<string>(query).FirstOrDefault();
                dbConnection.Close();
            }
            cod = "000000" + cod;
            return cod.Substring(cod.Length - 6);
        }

        public void EnviarPrestadorRM(int idPrestador, string operacao, bool atualizaEmpresa)
        {
            PluginPrestadorInsercaoDto pluginPrestadorInsercaoDto = PopularPrestadorPlugin(idPrestador);
            pluginPrestadorInsercaoDto.OperacaoInt = operacao;
            PopularCodCfo(operacao, pluginPrestadorInsercaoDto, idPrestador);

            var connectionStringIntegracaoRM = _connectionStrings.Value.RMIntegracaoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringIntegracaoRM))
            {
                dbConnection.Open();
                InserirIntTrprImp(pluginPrestadorInsercaoDto, dbConnection);
                InserirIntTrprComplImp(pluginPrestadorInsercaoDto, dbConnection);
                if (atualizaEmpresa)
                {
                    EnviarEmpresaRM(idPrestador, dbConnection, pluginPrestadorInsercaoDto.CodCfo);
                }
                dbConnection.Close();
            }

            if (operacao == "I")
            {
                AtualizarPrestador(idPrestador, pluginPrestadorInsercaoDto.CodCfo);
            }
        }

        private void AtualizarPrestador(int idPrestador, string codCfo)
        {
            var prestadorDb = _prestadorRepository.BuscarPorId(idPrestador);
            var semZeroEsquerda = Int32.Parse(codCfo);
            prestadorDb.IdRepresentanteRmTRPR = semZeroEsquerda.ToString();
            _unitOfWork.Commit();
        }

        private void PopularCodCfo(string operacao, PluginPrestadorInsercaoDto pluginEmpresaInsercaoDto, int idPrestador)
        {
            if (operacao == "I")
            {
                pluginEmpresaInsercaoDto.CodCfo = ObterCodCfoPrestadorNoRm();
            }
            else
            {
                var prestadorDb = _prestadorRepository.BuscarPorId(idPrestador);
                var cod = "000000" + prestadorDb.IdRepresentanteRmTRPR;
                pluginEmpresaInsercaoDto.CodCfo = cod.Substring(cod.Length - 6);
            }
        }

        private static void InserirIntTrprImp(PluginPrestadorInsercaoDto prestador, IDbConnection dbConnection)
        {
            string sQueryInsert = "INSERT INT_TRPR_IMP  ("
                                   + "STATUS_INT,"
                                   + "OPERACAO_INT,"
                                   + "DATAINSERCAO_INT,"
                                   + "CHAVEORIGEM_INT,"
                                   + "CODCOLIGADA,"
                                   + "CODRPR,"
                                   + "NOME,"
                                   + "RUA,"
                                   + "COMPLEMENTO,"
                                   + "BAIRRO,"
                                   + "CIDADE,"
                                   + "CODETD,"
                                   + "CEP,"
                                   + "TELEFONE,"
                                   + "CELULAR,"
                                   + "FAX,"
                                   + "RUAPGTO,"
                                   + "COMPLEMENTOPGTO,"
                                   + "BAIRROPGTO,"
                                   + "CIDADEPGTO,"
                                   + "CODETDPGTO,"
                                   + "CEPPGTO,"
                                   + "INATIVO,"
                                   + "EMAIL,"
                                   + "PAIS ) VALUES ("
                                   + "'" + prestador.StatusInt + "',"
                                   + "'" + prestador.OperacaoInt + "',"
                                   + "'" + prestador.DataInsercao.ToString(FORMATO_DATA_DB_RM) + "',"
                                   + prestador.SeqlEACesso.Value + ","
                                   + "'" + prestador.CodColigada + "',"
                                   + "'" + prestador.CodCfo + "',"
                                   + "'" + prestador.Nome + "',"
                                   + "'" + prestador.Rua + "',"
                                   + "'" + prestador.Complemento + "',"
                                   + "'" + prestador.Bairro + "',"
                                   + "'" + prestador.Cidade + "',"
                                   + "'" + prestador.CodEtd + "',"
                                   + "'" + prestador.Cep + "',"
                                   + "'" + prestador.Telefone + "',"
                                   + "'" + prestador.Celular + "',"
                                   + "'" + prestador.Fax + "',"
                                   + "'" + prestador.Rua + "',"
                                   + "'" + prestador.Complemento + "',"
                                   + "'" + prestador.Bairro + "',"
                                   + "'" + prestador.Cidade + "',"
                                   + "'" + prestador.CodEtd + "',"
                                   + "'" + prestador.Cep + "',"
                                   + (prestador.Inativo ? 1 : 0) + ","
                                   + "'" + prestador.Email + "',"
                                   + "'" + prestador.Pais + "'" +
                                   ")";

            dbConnection.Query(sQueryInsert);
        }

        private void InserirIntTrprComplImp(PluginPrestadorInsercaoDto prestador, IDbConnection dbConnection)
        {
            string sQueryInsert = " INSERT INT_TRPRCOMPL_IMP ("
                           + "STATUS_INT,"
                           + "OPERACAO_INT,"
                           + "DATAINSERCAO_INT,"
                           + "CHAVEORIGEM_INT,"
                           + "CODCOLIGADA,"
                           + "CODRPR,"
                           + "CCRCODFORNEC,"
                           + "CCRNOMEFORNEC,"
                           + "CCRCIDORIGEM ) VALUES ("
                           + "'" + prestador.StatusInt + "',"
                           + "'" + prestador.OperacaoInt + "',"
                           + "'" + prestador.DataInsercao.ToString(FORMATO_DATA_DB_RM) + "',"
                           + prestador.SeqlEACesso.Value + ","
                           + "'" + prestador.CodColigada + "',"
                           + "'" + prestador.CodCfo + "',"
                           + "'" + prestador.IdCliente + "',"
                           + "'" + TratarApostofre(prestador.Cliente) + "',"
                           + prestador.IdProfissional + ")";
            dbConnection.Query(sQueryInsert);
        }

        private void AtualizarTrprCompl(Prestador prestador, string codFornec, string nome, IDbConnection dbConnection, string idRepresentanteRM)
        {
            string sQueryInsert = " INSERT INT_TRPRCOMPL_IMP ("
                                + "STATUS_INT,"
                                + "OPERACAO_INT,"
                                + "DATAINSERCAO_INT,"
                                + "CHAVEORIGEM_INT,"
                                + "CODCOLIGADA,"
                                + "CODRPR,"
                                + "CCRCODFORNEC,"
                                + "CCRNOMEFORNEC,"
                                + "CCRCIDORIGEM ) VALUES ("
                                + "'I',"
                                + "'A',"
                                + "'" + DateTime.Now.ToString(FORMATO_DATA_DB_RM) + "',"
                                + ObterSequenceNoEacesso() + ","
                                + prestador.IdEmpresaGrupo.Value + ","
                                + "'" + ((prestador.IdRepresentanteRmTRPR == null || prestador.IdRepresentanteRmTRPR.Equals(0)) ? idRepresentanteRM : prestador.IdRepresentanteRmTRPR) + "',"
                                + "'" + codFornec + "',"
                                + "'" + nome + "',"
                                + prestador.Id + ")";
            dbConnection.Query(sQueryInsert);
        }
        #endregion

        private string TratarApostofre(string texto)
        {
            if (texto != null)
            {
                texto = texto.Replace("'", "''");
            }
            return texto;
        }

        public void AtualizarSituacaoApartirDoRm()
        {
            var pagamentosPendentes = _horasMesPrestadorService.BuscarLancamentosComPagamentoSolicitado();

            foreach (var lancamento in pagamentosPendentes)
            {
                VerificarExecucaoPluginParaPagamentoPJ(lancamento);
            }
        }

        private void VerificarExecucaoPluginParaPagamentoPJ(HorasMesPrestador horasMesPrestador)
        {
            var connectionStringIntegracaoRM = _connectionStrings.Value.RMIntegracaoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringIntegracaoRM))
            {
                var query = "select status_int as Status, idmov as IdMov, chaveorigem_int as IdChaveOrigemIntRm from INT_TMOV_IMP where chaveorigem_int = " + horasMesPrestador.IdChaveOrigemIntRm;
                var situacaoRm = dbConnection.Query<IntTmovDto>(query).FirstOrDefault();
                dbConnection.Close();

                if (situacaoRm == null)
                {
                    return;
                }
                else if (situacaoRm.Status.Equals("E"))
                {
                    if (!horasMesPrestador.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.ERRO_SOLICITAR_PAGAMENTO.GetDescription()))
                    {
                        _prestadorService.AtualizarSituacao(horasMesPrestador.Id, SharedEnuns.TipoSituacaoHorasMesPrestador.ERRO_SOLICITAR_PAGAMENTO.GetDescription(), "STFCORP");
                    }
                }
                else if (situacaoRm.Status.Equals("S"))
                {
                    if (horasMesPrestador.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.AGUARDANDO_INTEGRACAO.GetDescription()))
                    {
                        _prestadorService.AtualizarSituacao(horasMesPrestador.Id, SharedEnuns.TipoSituacaoHorasMesPrestador.AGUARDANDO_ENTRADA_DA_NF.GetDescription(), "STFCORP");
                    }

                    VerificarNotaAnexadaNoRm(situacaoRm, horasMesPrestador);
                }
            }
        }

        private void VerificarNotaAnexadaNoRm(IntTmovDto situacaoRm, HorasMesPrestador horasMesPrestador)
        {
            var connectionStringRM = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRM))
            {
                var query = "select status from TMOV where idmov = " + situacaoRm.IdMov + " and codcoligada = " + horasMesPrestador.Prestador.IdEmpresaGrupo.Value;
                var result = dbConnection.Query<string>(query).FirstOrDefault();
                dbConnection.Close();

                if (result == null)
                {
                    if (!horasMesPrestador.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.CANCELADO.GetDescription()))
                    {
                        _prestadorService.AtualizarSituacao(horasMesPrestador.Id, SharedEnuns.TipoSituacaoHorasMesPrestador.CANCELADO.GetDescription());
                    }
                    return;
                }

                switch (result)
                {
                    case "C":
                        if (!horasMesPrestador.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.CANCELADO.GetDescription()))
                        {
                            _prestadorService.AtualizarSituacao(horasMesPrestador.Id, SharedEnuns.TipoSituacaoHorasMesPrestador.CANCELADO.GetDescription(), "STFCORP");
                        }
                        break;
                    case "F":
                        if (!horasMesPrestador.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.AGUARDANDO_PAGAMENTO.GetDescription()))
                        {
                            _prestadorService.AtualizarSituacao(horasMesPrestador.Id, SharedEnuns.TipoSituacaoHorasMesPrestador.AGUARDANDO_PAGAMENTO.GetDescription(), "STFCORP");
                        }
                        VerificarPagamentoConcluidoNoRmParaPj(situacaoRm, horasMesPrestador);
                        break;
                }
            }
        }

        private void VerificarPagamentoConcluidoNoRmParaPj(IntTmovDto situacaoRm, HorasMesPrestador horasMesPrestador)
        {
            var connectionStringRM = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRM))
            {
                var query = "select idmovdestino from tmovrelac where idmovorigem = " + situacaoRm.IdMov + " and codcolorigem = " + horasMesPrestador.Prestador.IdEmpresaGrupo.Value;
                var idMovDestino = dbConnection.Query<int>(query).FirstOrDefault();

                query = "select status from TMOV where idmov = " + idMovDestino + " and codcoligada = " + horasMesPrestador.Prestador.IdEmpresaGrupo.Value;
                var result = dbConnection.Query<string>(query).FirstOrDefault();
                dbConnection.Close();

                if (result != null && result.Equals("Q"))
                {
                    _prestadorService.AtualizarSituacao(horasMesPrestador.Id, SharedEnuns.TipoSituacaoHorasMesPrestador.PAGAMENTO_REALIZADO.GetDescription(), "STFCORP");
                    return;
                }
            }
        }

        public IEnumerable<LogErroRMDto> ObterLogErroRm(int idInt)
        {
            var rmConnection = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(rmConnection))
            {
                var query = "select tabela, dataErro, descErro as DescricaoErro from int_logerro_imp where id_int = " + idInt;
                var log = dbConnection.Query<LogErroRMDto>(query).ToList();
                dbConnection.Close();
                return log;
            }
        }
    }
}
