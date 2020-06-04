
using AutoMapper;
using Cadastro.Domain.CelulaRoot.Entity;
using Cadastro.Domain.CidadeRoot.Repository;
using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.DominioRoot.ItensDominio;
using Cadastro.Domain.DominioRoot.Repository;
using Cadastro.Domain.DominioRoot.Service.Interfaces;
using Cadastro.Domain.EmailRoot.DTO;
using Cadastro.Domain.EmpresaGrupoRoot.Repository;
using Cadastro.Domain.EmpresaGrupoRoot.Service.Interfaces;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.HorasMesRoot.Repository;
using Cadastro.Domain.NotaFiscalRoot.Repository;
using Cadastro.Domain.PessoaRoot.Entity;
using Cadastro.Domain.PessoaRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Cadastro.Domain.TelefoneRoot.Entity;
using Cadastro.Domain.TelefoneRoot.Repository;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using Dapper;
using Logger.Context;
using Logger.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utils;
using Utils.Base;
using Utils.Connections;
using Utils.Extensions;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class PrestadorService : IPrestadorService
    {
        const string FORMATO_DATA_DB_EACESSO = "yyyy-MM-dd HH:mm:ss";

        private readonly IPrestadorRepository _prestadorRepository;
        private readonly IEmpresaPrestadorRepository _empresaPrestadorRepository;
        private readonly IEmpresaPrestadorService _empresaPrestadorService;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly ITelefoneRepository _telefoneRepository;
        private readonly ICidadeRepository _cidadeRepository;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly LinkBase _linksBaseUrls;
        private readonly IDominioRepository _dominioRepository;
        private readonly IDominioService _dominioService;
        private readonly IHorasMesPrestadorRepository _horasMesPrestadorRepository;
        private readonly IHorasMesRepository _horasMesRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentoPrestadorService _documentoPrestadorService;
        private readonly IContratoPrestadorService _contratoPrestadorService;
        private readonly IMinioService _minioService;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IInativacaoPrestadorRepository _inativacaoPrestadorRepository;
        private readonly IValorPrestadorRepository _valorPrestadorRepository;
        private readonly IEmpresaGrupoService _empresaGrupoService;
        private readonly ICelulaRepository _celulaRepository;
        private readonly ILogHorasMesPrestadorRepository _logHorasMesPrestadorRepository;
        private readonly IValorPrestadorBeneficioRepository _valorPrestadorBeneficioRepository;
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IClienteServicoPrestadorRepository _clienteServicoPrestadorRepository;
        private readonly ISocioEmpresaPrestadorRepository _socioEmpresaPrestadorRepository;
        private readonly IConfiguration _configuration;
        private readonly IPrestadorEnvioNfRepository _prestadorEnvioNfRepository;
        private readonly IObservacaoPrestadorRepository _observacaoPrestadorRepository;
        private readonly IExtensaoContratoPrestadorService _extensaoContratoPrestadorService;
        private readonly IVariablesToken _variables;

        public PrestadorService(
            IPrestadorRepository prestadorRepository,
            IEmpresaPrestadorRepository empresaPrestadorRepository,
            IEmpresaPrestadorService empresaPrestadorService,
            IEmpresaRepository empresaRepository,
            ICidadeRepository cidadeRepository,
            IHorasMesPrestadorRepository horasMesPrestadorRepository,
            IEnderecoRepository enderecoRepository,
            MicroServicosUrls microServicosUrls,
            IHorasMesRepository horasMesRepository,
            ITelefoneRepository telefoneRepository,
            ILogHorasMesPrestadorRepository logHorasMesPrestadorRepository,
            IDominioRepository dominioRepository,
            IDominioService dominioService,
            IPessoaRepository pessoaRepository,
            IOptions<ConnectionStrings> connectionStrings,
            IInativacaoPrestadorRepository inativacaoPrestadorRepository,
            IValorPrestadorRepository valorPrestadorRepository,
            LinkBase linksBaseUrls,
            IEmpresaGrupoService empresaGrupoService,
            ICelulaRepository celulaRepository,
            IValorPrestadorBeneficioRepository valorPrestadorBeneficioRepository,
            IConfiguration configuration,
            IPrestadorEnvioNfRepository prestadorEnvioNfRepository,
            IClienteServicoPrestadorRepository clienteServicoPrestadorRepository,
            ISocioEmpresaPrestadorRepository socioEmpresaPrestadorRepository,
            IUnitOfWork unitOfWork,
            IDocumentoPrestadorService documentoPrestadorService,
            IContratoPrestadorService contratoPrestadorService,
            IObservacaoPrestadorRepository observacaoPrestadorRepository,
            IExtensaoContratoPrestadorService extensaoContratoPrestadorService, 
            IVariablesToken variables)
        {
            _prestadorRepository = prestadorRepository;
            _empresaPrestadorRepository = empresaPrestadorRepository;
            _empresaPrestadorService = empresaPrestadorService;
            _empresaRepository = empresaRepository;
            _cidadeRepository = cidadeRepository;
            _enderecoRepository = enderecoRepository;
            _telefoneRepository = telefoneRepository;
            _dominioRepository = dominioRepository;
            _dominioService = dominioService;
            _linksBaseUrls = linksBaseUrls;
            _horasMesRepository = horasMesRepository;
            _microServicosUrls = microServicosUrls;
            _horasMesPrestadorRepository = horasMesPrestadorRepository;
            _pessoaRepository = pessoaRepository;
            _unitOfWork = unitOfWork;
            _documentoPrestadorService = documentoPrestadorService;
            _contratoPrestadorService = contratoPrestadorService;
            _connectionStrings = connectionStrings;
            _valorPrestadorRepository = valorPrestadorRepository;
            _clienteServicoPrestadorRepository = clienteServicoPrestadorRepository;
            _socioEmpresaPrestadorRepository = socioEmpresaPrestadorRepository;
            _inativacaoPrestadorRepository = inativacaoPrestadorRepository;
            _valorPrestadorRepository = valorPrestadorRepository;
            _empresaGrupoService = empresaGrupoService;
            _celulaRepository = celulaRepository;
            _valorPrestadorBeneficioRepository = valorPrestadorBeneficioRepository;
            _logHorasMesPrestadorRepository = logHorasMesPrestadorRepository;
            _configuration = configuration;
            _prestadorEnvioNfRepository = prestadorEnvioNfRepository;
            _observacaoPrestadorRepository = observacaoPrestadorRepository;
            _extensaoContratoPrestadorService = extensaoContratoPrestadorService;
            _variables = variables;
        }

        public int Adicionar(Prestador prestador)
        {
            foreach (var contratoPrestador in prestador.ContratosPrestador ?? Enumerable.Empty<ContratoPrestador>())
            {
                contratoPrestador.Id = 0;
                contratoPrestador.IdPrestador = prestador.Id;
                contratoPrestador.DataAlteracao = DateTime.Now;
                contratoPrestador.Usuario = _variables.UserName ?? "STFCORP";

                foreach (var extensao in contratoPrestador.ExtensoesContratoPrestador ?? Enumerable.Empty<ExtensaoContratoPrestador>())
                {
                    extensao.Id = 0;
                    extensao.IdContratoPrestador = contratoPrestador.Id;
                    extensao.DataAlteracao = DateTime.Now;
                    extensao.Usuario = _variables.UserName ?? "STFCORP";
                }
            }

            foreach (var documento in prestador.DocumentosPrestador ?? Enumerable.Empty<DocumentoPrestador>())
            {
                documento.Id = 0;
                documento.IdPrestador = prestador.Id;
                documento.DataAlteracao = DateTime.Now;
                documento.Usuario = _variables.UserName ?? "STFCORP";

                DomTipoDocumentoPrestador novoDominio = null;
                if (documento.IdTipoDocumentoPrestador.Equals((int)SharedEnuns.TipoDocumentoPrestador.OUTROS) &&
                !String.IsNullOrEmpty(documento.DescricaoTipoOutros))
                {
                    novoDominio = new DomTipoDocumentoPrestador
                    {
                        DescricaoValor = documento.DescricaoTipoOutros,
                        Ativo = true,
                        ValorTipoDominio = SharedEnuns.TipoDocumentoPrestador.VL_TIPO_DOMINIO.GetDescription(),
                    };
                    _dominioService.AddTrackingNoDominio(novoDominio);
                    documento.IdTipoDocumentoPrestador = novoDominio.Id;
                }
            }

            var valorPrestaodr = prestador.ValoresPrestador.FirstOrDefault();
            if (valorPrestaodr != null)
            {
                var horaMesId = _horasMesRepository.Buscar(x => x.Ano == valorPrestaodr.DataReferencia.Year &&
                                                                x.Mes == valorPrestaodr.DataReferencia.Month).First().Id;

                var horaMesPrestador = new HorasMesPrestador
                {
                    IdHorasMes = horaMesId,
                    Situacao = "HORAS PENDENTE",
                    SemPrestacaoServico = false,
                    Usuario = _variables.UserName,
                    DataAlteracao = DateTime.Now
                };

                prestador.HorasMesPrestador.Add(horaMesPrestador);
            }

            if (prestador.EmpresasPrestador.Any(x => x.IdEmpresa != 0))
            {
                var empresasJaCadastradas = prestador.EmpresasPrestador.Where(x => x.IdEmpresa != 0).ToList();
                prestador.EmpresasPrestador = prestador.EmpresasPrestador.Where(x => x.IdEmpresa == 0).ToList();
                _prestadorRepository.Adicionar(prestador);
                foreach (var empresaPrestador in empresasJaCadastradas)
                {
                    _empresaRepository.Update(empresaPrestador.Empresa);
                    _enderecoRepository.Update(empresaPrestador.Empresa.Endereco);
                    empresaPrestador.IdPrestador = prestador.Id;
                    _empresaPrestadorRepository.Adicionar(empresaPrestador);
                }

                _unitOfWork.Commit();
            }
            else
            {
                _prestadorRepository.Adicionar(prestador);
                _unitOfWork.Commit();
            }
            return prestador.Id;
        }

        public void AdicionarEmpresaPrestador(EmpresaPrestador empresaPrestador)
        {
            _empresaPrestadorRepository.Adicionar(empresaPrestador);
            _unitOfWork.Commit();
        }

        public Prestador BuscarPorId(int id)
        {
            var result = _prestadorRepository.BuscarPorIdComIncludes(id);
            return result;
        }

        public FiltroGenericoDtoBase<PrestadorDto> Filtrar(FiltroGenericoDtoBase<PrestadorDto> filtro)
        {
            var result = _prestadorRepository.Filtrar(filtro);
            return result;
        }

        public FiltroGenericoDtoBase<PrestadorDto> FiltrarHoras(FiltroGenericoDtoBase<PrestadorDto> filtro)
        {
            var result = _prestadorRepository.FiltrarHoras(filtro);
            return result;
        }

        public void AtualizarPrestador(Prestador prestador)
        {
            AtualizarTelefone(prestador);
            AtualizarEndereco(prestador);
            _pessoaRepository.Update(prestador.Pessoa);
            _prestadorRepository.Update(prestador);

            _unitOfWork.Commit();
        }

        private void AtualizarEndereco(Prestador prestador)
        {
            prestador.Pessoa.Endereco.Id = (int)prestador.Pessoa.IdEndereco;
            if (prestador.Pessoa.IdEndereco != 0)
            {
                _enderecoRepository.Update(prestador.Pessoa.Endereco);
            }
            else
            {
                _enderecoRepository.Adicionar(prestador.Pessoa.Endereco);
            }
        }

        private void AtualizarTelefone(Prestador prestador)
        {
            prestador.Pessoa.Telefone.Id = (int)prestador.Pessoa.IdTelefone;

            if (prestador.Pessoa.IdTelefone != 0)
            {
                _telefoneRepository.Update(prestador.Pessoa.Telefone);
            }
            else
            {
                _telefoneRepository.Adicionar(prestador.Pessoa.Telefone);
            }
        }

        private void AtualizarEmpresa(Prestador prestador)
        {
            var empresasAtualizadas = prestador.EmpresasPrestador.Where(x => x.IdEmpresa != 0).Select(x => x.Empresa).ToList();
            var enderecosAtualizados = empresasAtualizadas.Select(x => x.Endereco).ToList();
            _empresaRepository.UpdateList(empresasAtualizadas, false);
            _enderecoRepository.UpdateList(enderecosAtualizados, false);

            var empresasNovas = prestador.EmpresasPrestador.Where(x => x.IdEmpresa == 0).ToList();
            _empresaPrestadorRepository.AdicionarRange(empresasNovas);
        }

        private bool AtualizarDiaPagamentoPrestadorEAcesso(int diaPagamentoId, int idProfissionalEAcesso)
        {
            try
            {
                var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
                using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
                {
                    var diaPagto = _dominioRepository.BuscarPorId(diaPagamentoId).DescricaoValor;
                    string query = "update stfcorp.tblProfissionaisSistemasPagto set DiaPagto = " + diaPagto + ", Inativo = 0 where idProfissional = " + idProfissionalEAcesso;
                    var linhasAtualizadas = dbConnection.Execute(query);

                    if (linhasAtualizadas == 0)
                    {
                        query = "insert into stfcorp.tblProfissionaisSistemasPagto (idProfissional, idSistema, DiaPagto, Inativo) values" +
                            "(" + idProfissionalEAcesso + ",113," + diaPagto + ",0)";
                        dbConnection.Execute(query);
                    }

                    dbConnection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void AtualizarMigracao(int? idProfissionalEacesso)
        {
            List<EmpresaPrestador> empresasPrestador = new List<EmpresaPrestador>();
            List<Prestador> prestadores = new List<Prestador>();
            List<Cidade> cidades = new List<Cidade>();
            List<EmpresaPrestador> empresasPrestadorProntas = new List<EmpresaPrestador>();

            prestadores = _prestadorRepository.BuscarTodos().ToList();
            cidades = _cidadeRepository.BuscarTodos().ToList();

            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                empresasPrestador = ObterEmpresasPrestador(dbConnection, idProfissionalEacesso);

                foreach (var empresaPrestador in empresasPrestador)
                {
                    var prestador = prestadores.Find(x => x.CodEacessoLegado.Equals(empresaPrestador.IdPrestador));
                    if (prestador != null)
                    {
                        empresaPrestador.IdEmpresa = 0;
                        empresaPrestador.Prestador = null;
                        empresaPrestador.IdPrestador = prestador.Id;
                        empresaPrestador.Empresa.Usuario = "STFCORP";
                        ObterCidadeEmpresa(cidades, empresaPrestador);

                        empresasPrestadorProntas.Add(empresaPrestador);
                        if (empresasPrestadorProntas.Count == 100)
                        {
                            _empresaPrestadorRepository.AdicionarRange(empresasPrestadorProntas);
                            _unitOfWork.Commit();
                            empresasPrestadorProntas = new List<EmpresaPrestador>();
                        }

                    }
                }
                if (empresasPrestadorProntas.Count > 0)
                {
                    _empresaPrestadorRepository.AdicionarRange(empresasPrestadorProntas);
                    _unitOfWork.Commit();
                }
                dbConnection.Close();
            }
        }

        public void AtualizarMigracaoClienteServico(int? idProfissionalEacesso)
        {
            List<ClienteServicoPrestador> lista = new List<ClienteServicoPrestador>();

            var prestadores = _prestadorRepository.BuscarTodosSemTracking().Select(x => new ComboDefaultDto
            {
                Id = x.Id,
                Descricao = x.CodEacessoLegado.ToString()
            });

            var result = ObterClienteServicoPorPrestador(idProfissionalEacesso);

            foreach (var clienteServico in result)
            {
                var idPrestador = prestadores.FirstOrDefault(x => x.Descricao.Equals(clienteServico.IdPrestador.ToString()));
                if (idPrestador != null)
                {
                    clienteServico.IdPrestador = idPrestador.Id;
                    lista.Add(clienteServico);
                }

                if (lista.Count == 200)
                {
                    _clienteServicoPrestadorRepository.AdicionarRange(lista);
                    _unitOfWork.Commit();
                    lista = new List<ClienteServicoPrestador>();
                }
            }

            if (lista.Count > 0)
            {
                _clienteServicoPrestadorRepository.AdicionarRange(lista);
                _unitOfWork.Commit();
            }
        }

        private List<ClienteServicoPrestador> ObterClienteServicoPorPrestador(int? idProfissionalEacesso)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                var query = @"SELECT cs.idcelula AS IDCELULA, PF.idCliente AS IDCLIENTE, PF.idServico AS IDSERVICO, IdLocTrab AS IDLOCALTRABALHO, LEFT(pf.Descricao,245) AS DescricaoTrabalho
	                                ,Dtinicio AS DaTaINICIO, DtFim AS DataPrevisaoTermino, VlrCusto AS ValorCusto, VlrVenda AS ValorVENDA, VlrRepasse AS ValorREPASSE
	                                ,idProfissional AS IDPRESTADOR, case when pf.Inativo = 1 then 0 else 1 end as Ativo
                                    FROM stfcorp.tblclientesservicos cs, stfcorp.tblProfissionaisClientes PF INNER JOIN stfcorp.tblprofissionais P ON PF.IdProfissional = P.Id
                                    WHERE cs.idservico = pf.idservico and cs.idcliente = pf.idcliente and pf.idprofissional in 
                                    (SELECT pro.[Id] FROM stfcorp.tblCelulas cel
                                            INNER JOIN stfcorp.tblProfissionais pro ON cel.IdCelula = pro.Celula
                                            INNER JOIN stfcorp.tblProfissionaisTiposContratos contr ON contr.Id = pro.IdTipoContrato AND contr.AgrupProf IN('LTDA')  
                                            INNER JOIN stfcorp.tblCargos ON pro.IdCargo = tblCargos.id
                                            INNER JOIN stfcorp.tblGrupos ON cel.IdGrupo = tblGrupos.IdGrupo
                                            LEFT JOIN stfcorp.tblProfissionaisClientes PCL ON PCL.IdProfissional = PRO.ID AND PCL.Inativo = 0
                                            LEFT JOIN stfcorp.tblProfissionaisSistemasPagto sispag ON pro.id = sispag.idprofissional
                                            LEFT JOIN stfcorp.tblEmpresasGrupo empGrupo ON pro.CodEmprGrupo = empGrupo.IdEmpresa
                                            LEFT JOIN stfcorp.tblProfissionaisContratos proCont ON pro.id = proCont.idProfissional
                                            WHERE(proCont.idContrato = (select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional))
                                            or(isnull((select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional)), 0) = 0) )  
                                            and ISNULL(pro.Status, '') in ('', 'FE')
                                    ";

                if (idProfissionalEacesso.HasValue)
                {
                    query += "AND pro.Id = " + idProfissionalEacesso.Value + ")";
                }
                else
                {
                    query += ")";
                }

                var result = dbConnection.Query<ClienteServicoPrestador>(query).ToList();
                dbConnection.Close();
                return result;
            }
        }
        public void AtualizarMigracaoBeneficio(int? idProfissionalEacesso)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                var beneficios = GerarTiposBeneficios(dbConnection);
                var valoresPrestadorBeneficioEacesso = ObterBeneficiosEacesso(dbConnection, idProfissionalEacesso);
                var valoresPrestadorBeneficio = ObterBeneficiosEacessoMapeados(valoresPrestadorBeneficioEacesso, beneficios);
                dbConnection.Close();

                foreach (var listaBeneficio in valoresPrestadorBeneficio)
                {
                    _valorPrestadorBeneficioRepository.AdicionarRange(listaBeneficio);
                    _unitOfWork.Commit();
                }
            }
        }

        private List<List<ValorPrestadorBeneficio>> ObterBeneficiosEacessoMapeados(List<ValorPrestadorBeneficioEacessoDto> beneficiosEacesso, List<Dominio> tiposBeneficios)
        {
            ValorPrestadorBeneficio valorPrestadorBeneficio = new ValorPrestadorBeneficio();
            List<List<ValorPrestadorBeneficio>> beneficiosMatriz = new List<List<ValorPrestadorBeneficio>>();
            List<ValorPrestadorBeneficio> beneficiosLista = new List<ValorPrestadorBeneficio>();
            var count = 0;
            var valoresPrestadores = _valorPrestadorRepository.BuscarComIncludePrestador().ToList();

            foreach (var beneficio in beneficiosEacesso)
            {
                valorPrestadorBeneficio = Mapper.Map<ValorPrestadorBeneficio>(beneficio);
                valorPrestadorBeneficio.IdBeneficio = ObterIdTipoBeneficio(beneficio.NomeBeneficio, tiposBeneficios);
                valorPrestadorBeneficio.IdValorPrestador = ObterIdValorPrestador(beneficio, valoresPrestadores);

                if (valorPrestadorBeneficio.IdBeneficio != 0 && valorPrestadorBeneficio.IdValorPrestador != 0)
                {
                    beneficiosLista.Add(valorPrestadorBeneficio);
                    count++;
                    if (count > 200)
                    {
                        beneficiosMatriz.Add(beneficiosLista);
                        beneficiosLista = new List<ValorPrestadorBeneficio>();
                        count = 0;
                    }
                }
                else
                {
                    Console.Write("heelo");
                }
            }

            if (count > 0)
            {
                beneficiosMatriz.Add(beneficiosLista);
            }

            return beneficiosMatriz;
        }

        private int ObterIdValorPrestador(ValorPrestadorBeneficioEacessoDto beneficio, List<ValorPrestador> valoresPrestadores)
        {
            var valorPrestador = valoresPrestadores.FirstOrDefault(x =>
                                    x.Prestador.CodEacessoLegado.Value == beneficio.IdProfissional && x.IdRemuneracaoEacesso == beneficio.IdRemuneracao);
            if (valorPrestador != null)
            {
                return valorPrestador.Id;
            }
            else
            {
                return 0;
            }
        }

        private int ObterIdTipoBeneficio(string nomeBeneficio, List<Dominio> tiposBeneficios)
        {
            var beneficio = tiposBeneficios.FirstOrDefault(x => x.DescricaoValor.Equals(nomeBeneficio));
            if (beneficio != null)
            {
                return beneficio.Id;
            }
            else
            {
                return 0;
            }
        }

        private List<Dominio> GerarTiposBeneficios(IDbConnection dbConnection)
        {
            var beneficios = _dominioRepository.Buscar(x => x.ValorTipoDominio.Equals("BENEFICIO")).ToList();
            if (!beneficios.Any())
            {
                var idsBeneficiosUsados = ObterIdsBeneficiosUsados(dbConnection);
                CriarItensDominioBeneficio(dbConnection, idsBeneficiosUsados);
                beneficios = _dominioRepository.Buscar(x => x.ValorTipoDominio.Equals("BENEFICIO")).ToList();
            }
            return beneficios;
        }

        private void CriarItensDominioBeneficio(IDbConnection dbConnection, List<int> idsBeneficiosUsados)
        {
            var query = @"SELECT IdTipoAC AS IDVALOR, Nome AS DESCRICAOVALOR, 1 AS ATIVO, 'BENEFICIO' AS VALORTIPODOMINIO FROM 
                                stfcorp.tblProfissionaisRemuneracoesAjudaTipo where idTipoAc in (
                                " + string.Join(",", idsBeneficiosUsados) +
                                        ")";
            var beneficios = dbConnection.Query<Dominio>(query).ToList();
            _dominioRepository.AdicionarRange(beneficios);
            _unitOfWork.Commit();
        }

        private List<ValorPrestadorBeneficioEacessoDto> ObterBeneficiosEacesso(IDbConnection dbConnection, int? idProfissionalEacesso)
        {
            var query = @"SELECT a.idremuneracao, a.idprofissional, at.Nome AS nomeBeneficio, Valor AS ValorBeneficio
                            FROM stfcorp.tblprofissionaisremuneracoesajuda a, stfcorp.tblprofissionaisremuneracoesajudatipo at
                            WHERE a.idtipoac = at.idtipoac and IdProfissional IN (
	                            SELECT pro.[Id] FROM stfcorp.tblCelulas cel  
                                    INNER JOIN stfcorp.tblProfissionais pro ON cel.IdCelula = pro.Celula  
                                    INNER JOIN stfcorp.tblProfissionaisTiposContratos contr ON contr.Id = pro.IdTipoContrato AND contr.AgrupProf IN('LTDA')  
                                    INNER JOIN stfcorp.tblCargos ON pro.IdCargo = tblCargos.id  
                                    INNER JOIN stfcorp.tblGrupos ON cel.IdGrupo = tblGrupos.IdGrupo  
                                        LEFT JOIN stfcorp.tblProfissionaisClientes PCL ON PCL.IdProfissional = PRO.ID AND PCL.Inativo = 0  
                                        LEFT JOIN stfcorp.tblProfissionaisSistemasPagto sispag ON pro.id = sispag.idprofissional  
                                        LEFT JOIN stfcorp.tblEmpresasGrupo empGrupo ON pro.CodEmprGrupo = empGrupo.IdEmpresa  
                                        LEFT JOIN stfcorp.tblProfissionaisContratos proCont ON pro.id = proCont.idProfissional 
                                WHERE (proCont.idContrato = (select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional))  
                                    or (isnull((select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional)),0) = 0) )  
                                    and ISNULL(pro.Status, '') in ('', 'FE')";

            if (idProfissionalEacesso.HasValue)
            {
                query += "AND pro.Id = " + idProfissionalEacesso.Value + ")";
            }
            else
            {
                query += ")";
            }

            var valoresPrestadorBeneficio = dbConnection.Query<ValorPrestadorBeneficioEacessoDto>(query).ToList();
            return valoresPrestadorBeneficio;
        }

        private List<int> ObterIdsBeneficiosUsados(IDbConnection dbConnection)
        {
            var query = @"SELECT IdTipoAC FROM stfcorp.tblprofissionaisremuneracoesajuda
                              WHERE IdProfissional IN (
                                    SELECT pro.[Id] FROM stfcorp.tblCelulas cel
                                    INNER JOIN stfcorp.tblProfissionais pro ON cel.IdCelula = pro.Celula  
                                    INNER JOIN stfcorp.tblProfissionaisTiposContratos contr ON contr.Id = pro.IdTipoContrato AND contr.AgrupProf IN('LTDA')  
                                    INNER JOIN stfcorp.tblCargos ON pro.IdCargo = tblCargos.id  
                                    INNER JOIN stfcorp.tblGrupos ON cel.IdGrupo = tblGrupos.IdGrupo  
                                    LEFT JOIN stfcorp.tblProfissionaisClientes PCL ON PCL.IdProfissional = PRO.ID AND PCL.Inativo = 0  
                                    LEFT JOIN stfcorp.tblProfissionaisSistemasPagto sispag ON pro.id = sispag.idprofissional  
                                    LEFT JOIN stfcorp.tblEmpresasGrupo empGrupo ON pro.CodEmprGrupo = empGrupo.IdEmpresa  
                                    LEFT JOIN stfcorp.tblProfissionaisContratos proCont ON pro.id = proCont.idProfissional 
                                    WHERE (proCont.idContrato = (select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional))  
                                    or (isnull((select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional)),0) = 0) )  
                                    and ISNULL(pro.Status, '') in ('', 'FE') AND PRO.ID = 2146557379 " +
                                 ") " +
                                 "GROUP BY IdTipoAC order by idtipoac";
            var idsBeneficios = dbConnection.Query<int>(query).ToList();
            return idsBeneficios;
        }

        private static void ObterCidadeEmpresa(List<Cidade> cidades, EmpresaPrestador empresaPrestador)
        {
            if (empresaPrestador.Empresa.Endereco != null && empresaPrestador.Empresa.Endereco.NomeCidade != null)
            {
                var cidade = cidades.Find(x => x.NmCidade.Trim().ToUpper().Equals(empresaPrestador.Empresa.Endereco.NomeCidade.Trim().ToUpper()));
                if (cidade != null)
                {
                    empresaPrestador.Empresa.Endereco.IdCidade = cidade.Id;
                }
                else
                {
                    empresaPrestador.Empresa.Endereco.IdCidade = null;
                }
            }
            else
            {
                empresaPrestador.Empresa.Endereco.IdCidade = null;
            }
        }

        public void RealizarMigracao(int? idProfissionalEacesso)
        {
            List<Prestador> prestadores = new List<Prestador>();
            List<Endereco> enderecos = new List<Endereco>();
            List<Telefone> telefones = new List<Telefone>();
            List<Prestador> prestadoresProntos = new List<Prestador>();

            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                prestadores = ObterPrestadores(dbConnection, idProfissionalEacesso);
                enderecos = ObterEnderecos(dbConnection);
                telefones = ObterTelefones(dbConnection);

                int count = 0;
                foreach (var prestador in prestadores)
                {
                    try
                    {
                        PreencherEndereco(enderecos, prestador);
                        PreencherTelefone(telefones, prestador);
                        PreencherValoresDominio(dbConnection, prestador);

                        prestador.ValoresPrestador = null;

                        prestador.DataDesligamento = prestador.DataDesligamento.HasValue ? (prestador.DataDesligamento.Value.Year == 2501 ?
                            (new DateTime(2001, 7, 30)) : (prestador.DataDesligamento.Value)) :
                            prestador.DataDesligamento;

                        prestadoresProntos.Add(prestador);

                        if (prestadoresProntos.Count == 50)
                        {
                            _prestadorRepository.AdicionarRange(prestadoresProntos);
                            _unitOfWork.Commit();
                            prestadoresProntos = new List<Prestador>();
                        }

                        count++;
                    }
                    catch (Exception ex)
                    {
                        AdicionarLogGenerico(ex, "PRESTADOR count: " + count, prestador.CodEacessoLegado.ToString());
                        continue;
                    }
                }
                if (prestadoresProntos.Count > 0)
                {
                    _prestadorRepository.AdicionarRange(prestadoresProntos);
                    _unitOfWork.Commit();
                }
                dbConnection.Close();
            }
        }

        private static void AdicionarLogGenerico(Exception ex, string nmOrigem, string descLogGenerico)
        {
            var _logContext = new LogGenericoContext();
            _logContext.LogGenericos.Add(new LogGenerico
            {
                NmTipoLog = "MIGRACAO",
                NmOrigem = nmOrigem,
                DtHoraLogGenerico = DateTime.Now,
                DescLogGenerico = descLogGenerico + " - " + ex?.Message,
                DescExcecao = ex?.StackTrace.Substring(0, 600)
            });
            _logContext.SaveChanges();
        }

        private static void PreencherTelefone(List<Telefone> telefones, Prestador prestador)
        {
            var telefone = telefones.FirstOrDefault(x => x.IdProfissional == prestador.CodEacessoLegado);
            if (telefone != null)
            {
                prestador.Pessoa.Telefone = telefone;
            }
        }

        private void PreencherEndereco(List<Endereco> enderecos, Prestador prestador)
        {
            var endereco = enderecos.FirstOrDefault(x => x.IdProfissional == prestador.CodEacessoLegado);
            if (endereco != null)
            {
                if (endereco.IdCidade.HasValue)
                {
                    endereco.IdCidade = _cidadeRepository.ObterIdPeloNome(endereco.NomeCidade);
                }
                prestador.Pessoa.Endereco = endereco;
            }
        }

        private void PreencherValoresDominio(IDbConnection dbConnection, Prestador prestador)
        {
            prestador.IdAreaFormacao = prestador.IdAreaFormacao.HasValue ? _dominioRepository.ObterIdPeloCodValor(prestador.IdAreaFormacao.Value, "AREA_FORMACAO") : prestador.IdAreaFormacao;
            prestador.IdCargo = _dominioRepository.ObterIdPeloCodValor(prestador.IdCargo, "CARGO").Value;
            prestador.IdContratacao = _dominioRepository.ObterIdPeloCodValor(prestador.IdContratacao, "CONTRATACAO").Value;
            prestador.IdDiaPagamento = prestador.IdDiaPagamento.HasValue ? _dominioRepository.ObterIdPeloCodValor(prestador.IdDiaPagamento.Value, "DIA_PAGAMENTO") : prestador.IdDiaPagamento;

            prestador.IdSituacao = (prestador.IdSituacao == 1 || prestador.IdSituacao == 2 || prestador.IdSituacao == 3) ? prestador.IdSituacao : 1;
            prestador.IdSituacao = prestador.IdSituacao.HasValue ? _dominioRepository.ObterIdPeloCodValor(prestador.IdSituacao.Value, "SITUACAO_PRESTADOR") : prestador.IdSituacao;

            var pessoa = _pessoaRepository.Buscar(x => x.CodEacessoLegado == Convert.ToInt32(prestador.CodEacessoLegado)).FirstOrDefault();

            if (pessoa != null)
            {
                prestador.Pessoa = pessoa;
            }
            else
            {
                var profissional = ObterEmailsProfissionalEacesso(dbConnection, Convert.ToInt32(prestador.CodEacessoLegado));
                prestador.Pessoa.IdEscolaridade = prestador.Pessoa.IdEscolaridade.HasValue ? _dominioRepository.ObterIdPeloCodValor(prestador.Pessoa.IdEscolaridade.Value, "ESCOLARIDADE") : prestador.Pessoa.IdEscolaridade;
                prestador.Pessoa.IdEstadoCivil = prestador.Pessoa.IdEstadoCivil.HasValue ? _dominioRepository.ObterIdPeloCodValor((int)prestador.Pessoa.IdEstadoCivil, "ESTADOCIVIL").Value : prestador.Pessoa.IdEstadoCivil;
                prestador.Pessoa.IdExtensao = prestador.Pessoa.IdExtensao.HasValue ? _dominioRepository.ObterIdPeloCodValor(prestador.Pessoa.IdExtensao.Value, "EXTENSAO") : prestador.Pessoa.IdExtensao;
                prestador.Pessoa.IdGraduacao = prestador.Pessoa.IdGraduacao.HasValue ? _dominioRepository.ObterIdPeloCodValor(prestador.Pessoa.IdGraduacao.Value, "GRADUACAO") : prestador.Pessoa.IdGraduacao;
                prestador.Pessoa.IdNacionalidade = prestador.Pessoa.IdNacionalidade.HasValue ? _dominioRepository.ObterIdPeloCodValor(prestador.Pessoa.IdNacionalidade.Value, "NACIONALIDADE") : prestador.Pessoa.IdNacionalidade;
                prestador.Pessoa.IdSexo = prestador.Pessoa.IdNacionalidade.HasValue ? _dominioRepository.ObterIdPeloCodValor((int)prestador.Pessoa.IdSexo, "SEXO").Value : prestador.Pessoa.IdSexo;
                prestador.Pessoa.Nome = profissional != null ? profissional.Nome : string.Empty;
                prestador.Pessoa.Email = profissional != null ? profissional.Email : string.Empty;
                prestador.Pessoa.EmailInterno = profissional != null ? profissional.EmailInterno : string.Empty;
            }

            prestador.IdTipoRemuneracao = ObterTipoRemuneracao(dbConnection, prestador.CodEacessoLegado.Value);
            prestador.IdTipoRemuneracao = prestador.IdTipoRemuneracao.HasValue ? _dominioRepository.ObterIdPeloCodValor(prestador.IdTipoRemuneracao.Value, "TIPO_REMUNERACAO") : prestador.IdTipoRemuneracao;
        }

        public List<Prestador> ObterPrestadores(IDbConnection dbConnection, int? idProfissionalEacesso)
        {
            string sQuery = "SELECT pro.[Id] as CodEacessoLegado ,empGrupo.[ERPExterno] as IdEmpresaGrupo ,pro.[Celula] as IdCelula ,pro.[Nome] as Nome ,pro.[CPF] as Cpf ,pro.[RG] as Rg ,pro.[IdFilial] as IdFilial " +
                                 ",pro.[DtAdmissao] as DataInicio ,pro.[DtNascimento] as DataNascimento ,pro.[NomePai] as Pai ,pro.[NomeMae] as Mae " +
                                 ",case when datepart(year,proCont.DtTermino) < 1950 then null when datepart(year,proCont.DtTermino) > 2025 then null else proCont.DtTermino end as DataValidadeContrato " +
                                 ",case when pro.[DtDesligamento] is not null then pro.[DtDesligamento] else (case when pro.Inativo = 0 then null else '2018-12-31' end) end as DataDesligamento " +
                                 ",pro.[Email] as Email ,pro.[Email_Int] as EmailInterno ,pro.[Nacionalidade] as IdNacionalidade ,pro.[IdCargo] as IdCargo ,pro.[IdTipoContrato] as IdContratacao " +
                                 ",pro.[instrucao] as IdEscolaridade ,pro.[IdGraduacao] as idGraduacao ,pro.[Instrucao1] as idExtensao ,pro.[IdGraduacao1] as idArea ,pro.[IdEstadoCivil] as idEstadoCivil ,pro.[Sexo] as idSexo ,pro.[Situacao] as idSituacao " +
                                 ",case sispag.diaPagto when 5 then 1 when 10 then 2 when 15 then 3 end as idDiaPagamento, cast(pro.[UltimaAlteracao] as smalldatetime) as DataUltimaAlteracaoEacesso " +
                                 ",pro.erp_externo as IdRepresentanteRmTRPR, pro.CodProdutoFornec as IdProdutoRm, pro.dtAdmissaoClt as dataInicioCLT " +
                         "FROM stfcorp.tblCelulas cel " +
                             "INNER JOIN stfcorp.tblProfissionais pro ON cel.IdCelula = pro.Celula " +
                             "INNER JOIN stfcorp.tblProfissionaisTiposContratos contr ON contr.Id = pro.IdTipoContrato AND contr.AgrupProf IN('LTDA') " +
                             "INNER JOIN stfcorp.tblCargos ON pro.IdCargo = tblCargos.id " +
                             "INNER JOIN stfcorp.tblGrupos ON cel.IdGrupo = tblGrupos.IdGrupo " +
                             "LEFT JOIN stfcorp.tblProfissionaisSistemasPagto sispag ON pro.id = sispag.idprofissional " +
                             "LEFT JOIN stfcorp.tblEmpresasGrupo empGrupo ON pro.CodEmprGrupo = empGrupo.IdEmpresa " +
                             "LEFT JOIN stfcorp.tblProfissionaisContratos proCont ON pro.id = proCont.idProfissional " +
                         "WHERE (proCont.idContrato = (select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional)) " +
                         "or (isnull((select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional)),0) = 0) ) " +
                         "and ISNULL(pro.Status, '') in ('', 'FE')";

            sQuery = AdicionarClausulaIdProfissionalEacesso(idProfissionalEacesso, sQuery, "pro.Id");

            var prestadores = dbConnection.Query<Prestador>(sQuery).AsList();
            var count = prestadores.Count;
            return prestadores.OrderByDescending(x => x.DataInicio).ToList();
        }

        private static string AdicionarClausulaIdProfissionalEacesso(int? idProfissionalEacesso, string sQuery, string coluna)
        {
            if (idProfissionalEacesso.HasValue)
            {
                sQuery += " and " + coluna + " in (" + idProfissionalEacesso.Value + ")";
            }

            return sQuery;
        }

        private static List<Endereco> ObterEnderecos(IDbConnection dbConnection)
        {
            string sQuery = "SELECT pro.[Id] as IdProfissional, pro.[IdCidade] as IdCidade, cid.Cidade as NomeCidade ,pro.[AbrevLogradouro] as SgAbrevLogradouro ,pro.[Endereco] as NmEndereco ,pro.[Num] as NrEndereco, pro.[Complemento] as NmCompEndereco " +
                                ",pro.[Bairro] as NmBairro ,pro.[CEP] as nrCep " +
                            "FROM stfcorp.tblCelulas cel " +
                                "INNER JOIN stfcorp.tblProfissionais pro ON cel.IdCelula = pro.Celula " +
                                "INNER JOIN stfcorp.tblProfissionaisTiposContratos contr ON contr.Id = pro.IdTipoContrato AND contr.AgrupProf IN('LTDA') " +
                                "INNER JOIN stfcorp.tblCargos ON pro.IdCargo = tblCargos.id " +
                                "INNER JOIN stfcorp.tblGrupos ON cel.IdGrupo = tblGrupos.IdGrupo " +
                                "LEFT JOIN stfcorp.tblProfissionaisClientes PCL ON PCL.IdProfissional = PRO.ID AND PCL.Inativo = 0 " +
                                "LEFT JOIN stfcorp.tblCidades cid ON cid.IdCidade = pro.idCidade " +
                            "WHERE ISNULL(pro.Status, '') in ('', 'FE')";
            var enderecos = dbConnection.Query<Endereco>(sQuery).AsList();
            return enderecos;
        }

        private static List<Telefone> ObterTelefones(IDbConnection dbConnection)
        {
            string sQuery = "SELECT pro.[Id] as IdProfissional, pro.[NumeroNextel] as NumeroNextel, pro.[IdNextel] as IdNextel, pro.[DDD] as DDD, pro.[celular] as Celular, pro.[TelCom] as NumeroComercial " +
                                ",pro.[ramaltelcom] as NumeroComercialRamal, pro.[TelRes] as NumeroResidencial " +
                            "FROM stfcorp.tblCelulas cel " +
                                "INNER JOIN stfcorp.tblProfissionais pro ON cel.IdCelula = pro.Celula " +
                                "INNER JOIN stfcorp.tblProfissionaisTiposContratos contr ON contr.Id = pro.IdTipoContrato AND contr.AgrupProf IN('LTDA') " +
                                "INNER JOIN stfcorp.tblCargos ON pro.IdCargo = tblCargos.id " +
                                "INNER JOIN stfcorp.tblGrupos ON cel.IdGrupo = tblGrupos.IdGrupo " +
                                "LEFT JOIN stfcorp.tblProfissionaisClientes PCL ON PCL.IdProfissional = PRO.ID AND PCL.Inativo = 0 " +
                            "WHERE ISNULL(pro.Status, '') in ('', 'FE')";
            var telefones = dbConnection.Query<Telefone>(sQuery).AsList();
            return telefones;
        }

        private static List<EmpresaPrestador> ObterEmpresasPrestador(IDbConnection dbConnection, int? idProfissionalEacesso)
        {
            try
            {
                string sQuery = @"SELECT proe.Id as IdEmpresa, proe.IdProfissional as IdPrestador, proe.CNPJ, proe.RazaoSocial, proe.DtVigencia as DataVigencia,
                                        proe.AbrevLogradouro as Tipo, proe.Endereco, proe.CEP, proe.Num as Numero, proe.Complemento, proe.Bairro, proe.IdCidade,
                                        cid.Cidade as NmCidade, proe.CCM, proe.InscEst as InscricaoEstadual, proe.Obs as Observacao,
		                                proe.Atuacao, soc.Socios, case proe.inativo when 1 then 0 when 0 then 1 end as Ativo,
                                        proe.ERPExterno as IdEmpresaRm 
                                    FROM stfcorp.tblCelulas cel
                                    INNER JOIN stfcorp.tblProfissionais pro ON cel.IdCelula = pro.Celula
                                    INNER JOIN stfcorp.tblProfissionaisEmpresas proe ON proe.IdProfissional = pro.Id
                                    INNER JOIN stfcorp.tblProfissionaisTiposContratos contr ON contr.Id = pro.IdTipoContrato AND contr.AgrupProf IN('LTDA') 
							        LEFT JOIN stfcorp.tblProfissionaisClientes PCL ON PCL.IdProfissional = PRO.ID AND PCL.Inativo = 0  
                                    LEFT JOIN stfcorp.tblCidades cid ON cid.IdCidade = proe.idCidade
                                    LEFT JOIN (SELECT Main.IdEmpresa,
			                                    LEFT(Main.Socios,Len(Main.Socios)-1) As Socios
		                                      FROM(SELECT DISTINCT ST2.IdEmpresa, 
				                                    (SELECT ST1.NomeSocio + ',' AS [text()]
					                                    FROM  [stfcorp].[tblProfissionaisEmprSocios] ST1
					                                    WHERE ST1.IdEmpresa = ST2.IdEmpresa
					                                    ORDER BY ST1.IdEmpresa
					                                    FOR XML PATH ('')
				                                    ) [Socios]
				                                    FROM  [stfcorp].[tblProfissionaisEmprSocios] ST2
		                                    ) [Main]) as soc ON soc.IdEmpresa = proe.Id
                                    WHERE ISNULL(pro.Status, '') in ('', 'FE')";

                sQuery = AdicionarClausulaIdProfissionalEacesso(idProfissionalEacesso, sQuery, "pro.Id");

                var empresasPrestador = dbConnection.Query<EmpresaPrestadorDto>(sQuery).AsList();
                var listaEmpresas = Mapper.Map<List<EmpresaPrestador>>(empresasPrestador);
                return listaEmpresas;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static List<ValorPrestador> ObterValores(IDbConnection dbConnection, List<Prestador> prestadores)
        {
            string sQuery = "SELECT idprofissional as IdProfissional, vrltda as ValorMes, vrhora as ValorHora, dtalteracao as DataAlteracao, 'STFCORP' as Usuario " +
                            "FROM stfcorp.tblprofissionaisremuneracoes WHERE idprofissional in (" + string.Join(",", prestadores.Select(x => x.CodEacessoLegado.Value)) +
                            ")";
            var valores = dbConnection.Query<ValorPrestador>(sQuery).AsList();
            return valores;
        }

        private static int ObterTipoRemuneracao(IDbConnection dbConnection, int idProfissional)
        {
            string sQuery = "SELECT pr.dtalteracao as Data, pr.idtiporemuneracao as Id FROM stfcorp.tblProfissionaisRemuneracoes pr, stfcorp.tblprofissionais pro " +
                                "WHERE pro.id = pr.idprofissional and ISNULL(pro.Status, '') in ('','FE') and pro.id = " + idProfissional;
            var idTipoRemuneracao = dbConnection.Query<RetornoContratacaoEacessoDto>(sQuery);
            if (idTipoRemuneracao.Any())
                return idTipoRemuneracao.OrderByDescending(x => x.Data).FirstOrDefault().Id;
            else
                return 0;
        }

        public FiltroGenericoDtoBase<AprovarPagamentoDto> FiltrarAprovarPagamento(FiltroGenericoDtoBase<AprovarPagamentoDto> filtro)
        {
            var result = _prestadorRepository.FiltrarAprovarPagamento(filtro);
            return result;
        }

        public DadosPagamentoPrestadorDto ObterDadosPagementoPorId(int id, int IdHorasMes)
        {
            var result = _prestadorRepository.ObterDadosPagementoPorId(id, IdHorasMes);
            return result;
        }

        public List<PrestadorMigradoDto> ObterPrestadoresMigrados()
        {
            var result = _prestadorRepository.BuscarTodos();

            var prestadores = result.Select(x =>
                new PrestadorMigradoDto
                {
                    Id = x.Id,
                    IdEacesso = x.CodEacessoLegado,
                    IdEndereco = x.Pessoa.IdEndereco,
                    IdEmpresaGrupo = x.IdEmpresaGrupo,
                    IdTelefone = x.Pessoa.IdTelefone,
                    IdCelula = x.IdCelula,
                    DataUltimaAlteracao = x.DataUltimaAlteracaoEacesso
                }
            ).ToList();

            return prestadores;
        }

        public void AdicionarPrestadoresNovos(List<Prestador> prestadoresNovos)
        {
            List<Endereco> enderecos = new List<Endereco>();
            List<Telefone> telefones = new List<Telefone>();
            List<ValorPrestador> valores = new List<ValorPrestador>();

            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                enderecos = ObterEnderecos(dbConnection);
                telefones = ObterTelefones(dbConnection);
                valores = ObterValores(dbConnection, prestadoresNovos);

                int count = 0;
                foreach (var prestador in prestadoresNovos)
                {
                    PreencherEndereco(enderecos, prestador);
                    PreencherTelefone(telefones, prestador);
                    PreencherValoresDominio(dbConnection, prestador);

                    prestador.ValoresPrestador = valores.Where(x => x.IdProfissional == prestador.CodEacessoLegado.Value).ToList();

                    try
                    {
                        prestador.DataDesligamento = prestador.DataDesligamento.HasValue ? (prestador.DataDesligamento.Value.Year == 2501 ?
                            (new DateTime(2001, 7, 30)) : (prestador.DataDesligamento.Value)) :
                            prestador.DataDesligamento;

                        Adicionar(prestador);
                        count++;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dbConnection.Close();
            }
        }

        public void AtualizarPrestadorVindosEacesso(List<Prestador> prestadoresAtualizados, List<PrestadorMigradoDto> prestadoresAntigos)
        {
            List<Endereco> enderecos = new List<Endereco>();
            List<Telefone> telefones = new List<Telefone>();
            List<ValorPrestador> valores = new List<ValorPrestador>();

            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                enderecos = ObterEnderecos(dbConnection);
                telefones = ObterTelefones(dbConnection);
                valores = ObterValores(dbConnection, prestadoresAtualizados);

                int count = 0;
                foreach (var prestador in prestadoresAtualizados)
                {
                    PreencherEndereco(enderecos, prestador);
                    PreencherTelefone(telefones, prestador);
                    PreencherValoresDominio(dbConnection, prestador);

                    prestador.ValoresPrestador = valores.Where(x => x.IdProfissional == prestador.CodEacessoLegado.Value).ToList();

                    try
                    {
                        prestador.DataDesligamento = prestador.DataDesligamento.HasValue ? (prestador.DataDesligamento.Value.Year == 2501 ?
                            (new DateTime(2001, 7, 30)) : (prestador.DataDesligamento.Value)) :
                            prestador.DataDesligamento;

                        var prestadorAntigo = prestadoresAntigos.FirstOrDefault(x => x.IdEacesso == prestador.CodEacessoLegado);
                        prestador.Id = prestadorAntigo.Id;
                        prestador.Pessoa.IdEndereco = prestadorAntigo.IdEndereco.Value;
                        prestador.Pessoa.IdTelefone = prestadorAntigo.IdTelefone.Value;
                        prestador.IdCelula = prestadorAntigo.IdCelula.Value;
                        prestador.HorasMesPrestador = null;

                        AtualizarPrestador(prestador);
                        count++;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dbConnection.Close();
            }
        }

        public DadosFinanceiroDto ObterDadosFinanceiroColigada(string idBanco, int idColigada)
        {

            var connectionStringRm = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRm))
            {
                string sQuery = @"SELECT FCXA.CODCOLIGADA, FCXA.CODCXA, FCXA.DESCRICAO formapagamento,FCXA.NUMBANCO, FCXA.NUMAGENCIA Agencia,
                                    FCXA.NROCONTA numeroConta, GBAN.NOMEREDUZIDO NomeBanco 
                                    FROM GBANCO GBAN, FCXA INNER JOIN FCONTA ON (FCXA.CODCOLCONTA = FCONTA.CODCOLIGADA AND FCXA.NROCONTA = FCONTA.NROCONTA
                                        AND FCXA.NUMBANCO = FCONTA.NUMBANCO AND FCXA.NUMAGENCIA = FCONTA.NUMAGENCIA)
                                    WHERE 
                                        FCXA.NUMBANCO = GBAN.NUMBANCO
                                        AND CODCXA LIKE '1-%'
                                        AND FCXA.CODCOLIGADA  = @idColigada
                                        AND FCXA.NUMBANCO like @idBanco
                                        AND FCXA.ATIVA = 1
                                        AND FCONTA.TIPOCONTA =1";

                List<DadosFinanceiroDto> dadosFinanceirosDto = dbConnection.Query<DadosFinanceiroDto>(sQuery, new { idColigada, idBanco }).AsList();
                DadosFinanceiroDto dadosFinanceiroDto;
                if (idBanco.Equals("%%"))
                {
                    dadosFinanceiroDto = dadosFinanceirosDto.Any(x => x.NomeBanco.Equals("ITAU")) ?
                                            dadosFinanceirosDto.FirstOrDefault(x => x.NomeBanco.Equals("ITAU")) : dadosFinanceirosDto.FirstOrDefault();
                }
                else
                {
                    dadosFinanceiroDto = dadosFinanceirosDto.FirstOrDefault();
                    if (dadosFinanceiroDto == null)
                    {
                        idBanco = "%%";
                        dadosFinanceiroDto = dbConnection.Query<DadosFinanceiroDto>(sQuery, new { idColigada, idBanco }).FirstOrDefault();
                    }
                }
                dbConnection.Close();
                return dadosFinanceiroDto;
            }
        }

        public DadosFinanceiroDto ObterDadosFinanceiroRMPrestador(string idProfissional, int? idEmpresaGrupo)
        {

            var connectionStringRm = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRm))
            {
                var dadosFinanceiro = ObterDadosFinanceiroRM(dbConnection, idProfissional, idEmpresaGrupo);
                dbConnection.Close();
                return dadosFinanceiro;
            }
        }

        private DadosFinanceiroDto ObterDadosFinanceiroRM(IDbConnection dbConnection, string cnpj, int? idEmpresaGrupo)
        {
            DadosFinanceiroDto dadosFinanceiroDto = new DadosFinanceiroDto();
            try
            {
                string sQuery = @"SELECT
		                                upper(CONST.descricao) as formaPagamento, 
		                                ISNULL(DADOSPGTO.NUMEROBANCO,0) As banco,
		                                ISNULL(CAST(DADOSPGTO.CODIGOAGENCIA AS VARCHAR),'')  AS agencia,
		                                CASE DADOSPGTO.FORMAPAGAMENTO WHEN 'P' THEN 4 ELSE 3 END AS tipoConta,
		                                CASE WHEN ISNULL(DADOSPGTO.CONTACORRENTE, '') = '' THEN '' 
		                                ELSE 
			                                DADOSPGTO.CONTACORRENTE + CASE WHEN ISNULL(DADOSPGTO.DIGITOCONTA,'') = '' THEN '' ELSE '-' + DADOSPGTO.DIGITOCONTA END
		                                END AS NumeroConta,
		                                CASE DADOSPGTO.ATIVO WHEN 1 THEN 0 ELSE 1 END AS Inativo
                                FROM
                                        GCONSIST CONST,
		                                DBO.FCFO FCFO,
		                                DBO.FDADOSPGTO DADOSPGTO
                                WHERE
                                        CONST.CODINTERNO = DADOSPGTO.FORMAPAGAMENTO AND CONST.CODTABELA = 'FORMAPAGAM' AND CONST.CODCOLIGADA  = 0 AND CONST.APLICACAO = 'F' AND 
                                        DADOSPGTO.ATIVO = 1 AND
		                                SUBSTRING(FCFO.CGCCFO,1,2) + SUBSTRING(FCFO.CGCCFO,4,3) + SUBSTRING(FCFO.CGCCFO,8,3)+ SUBSTRING(FCFO.CGCCFO,12,4)  + SUBSTRING(FCFO.CGCCFO,17,2) COLLATE DATABASE_DEFAULT
			                                = @cnpj AND DADOSPGTO.CODCOLIGADA = @idEmpresaGrupo AND
		                                FCFO.CODCFO = DADOSPGTO.CODCFO
                                ORDER BY
		                                DADOSPGTO.RECMODIFIEDON DESC";

                dadosFinanceiroDto = dbConnection.Query<DadosFinanceiroDto>(sQuery, new { cnpj, idEmpresaGrupo }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                dbConnection.Close();
                throw new Exception(ex.Message);
            }

            return dadosFinanceiroDto;
        }

        public string ObterContaCaixaNoRm(int idColigada, string banco)
        {
            int? bancoId = null;
            if (!String.IsNullOrEmpty(banco))
            {
                bancoId = Int32.Parse(banco);
            }

            var connectionStringRm = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRm))
            {
                string sQuery = @"SELECT FCXA.CODCXA
                                    FROM GBANCO GBAN, FCXA INNER JOIN FCONTA ON (FCXA.CODCOLCONTA = FCONTA.CODCOLIGADA AND FCXA.NROCONTA = FCONTA.NROCONTA
                                        AND FCXA.NUMBANCO = FCONTA.NUMBANCO AND FCXA.NUMAGENCIA = FCONTA.NUMAGENCIA)
                                    WHERE 
                                        FCXA.NUMBANCO = GBAN.NUMBANCO
                                        AND CODCXA LIKE '1-%'
                                        AND FCXA.CODCOLIGADA  = @idColigada
                                        AND GBAN.NUMBANCO = @bancoId
                                        AND FCXA.ATIVA = 1
                                        AND FCONTA.TIPOCONTA =1";

                string contaCaixa = dbConnection.Query<string>(sQuery, new { idColigada, bancoId }).FirstOrDefault();
                dbConnection.Close();
                return contaCaixa;
            }
        }

        public bool VerificarCpfProfissinalInativoExiste(string cpf)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                string sQuery = "SELECT * FROM [stfcorp].[tblProfissionais]" +
                           "WHERE INATIVO = 1 and CPF = @cpf";
                var valores = dbConnection.Query<ValorPrestador>(sQuery, new { cpf }).AsList();

                dbConnection.Close();

                return valores.ToList().Any();
            }

        }

        public bool VerificarDataInaticacaoPrestador(int idProfissional, DateTime dtDesligamento)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            var prestador = BuscarPorId(idProfissional);
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                string sQuery = "SELECT * FROM [stfcorp].[tblProfissionaisInativacao]" +
                           "WHERE IdProfissional = " + prestador.CodEacessoLegado + " and dtDesligamento = @dtDesligamento";
                var inativacoes = dbConnection.Query<InativacaoPrestador>(sQuery, new { idProfissional, dtDesligamento }).AsList();

                dbConnection.Close();

                return inativacoes.ToList().Any();
            }

        }

        public bool VerificarCpfProfissinalExiste(string cpf)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                string sQuery = "SELECT * FROM [stfcorp].[tblProfissionais]" +
                           "WHERE INATIVO = 0 and CPF = @cpf";
                var valores = dbConnection.Query<ValorPrestador>(sQuery, new { cpf }).AsList();

                dbConnection.Close();

                return valores.ToList().Any();
            }

        }

        public bool VerificarCpfProfissinalCandidatoExiste(string cpf)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                string sQuery = "SELECT * FROM [stfcorp].[tblProfissionaisCandidato]" +
                           "WHERE CPF = @cpf";
                var valores = dbConnection.Query<ValorPrestador>(sQuery, new { cpf }).AsList();

                dbConnection.Close();

                return valores.ToList().Any();
            }

        }

        public void SolicitarNF(int idHorasMesPrestador)
        {
            var horasMesPrestador = _horasMesPrestadorRepository.BuscarPorIdComIncludes(idHorasMesPrestador);
            var periodoVigente = _horasMesRepository.BuscarPeriodoVigente();
            List<ValorParametroEmailDTO> parametros = DefinirParametros(periodoVigente, horasMesPrestador);

            var email = new EmailDTO
            {
                IdTemplate = 3,
                RemetenteNome = "Gestão de Contratos",
                Para = ObterDestinatarioEmailParaSolicitarNF(horasMesPrestador),
                ValoresParametro = parametros
            };

            EnviarEmail(email);
            AtualizarSituacao(idHorasMesPrestador, SharedEnuns.TipoSituacaoHorasMesPrestador.NOTA_FISCAL_SOLICITADA.GetDescription());
        }

        private string ObterDestinatarioEmailParaSolicitarNF(HorasMesPrestador horasMesPrestador)
        {
            var destinatario = "";
            switch (Variables.EnvironmentName.ToUpper())
            {
                case "DEVELOPMENT":
                    destinatario = "clbatista@stefanini.com";
                    break;
                case "STAGING":
                    destinatario = "clbatista@stefanini.com";
                    break;
                case "PRODUCTION":
                    destinatario = DefinirEmailsDestinatarios(horasMesPrestador.Prestador.Pessoa.EmailInterno, horasMesPrestador.Prestador.Pessoa.Email, horasMesPrestador.Prestador.EmailParceiro);
                    break;
            }
            return destinatario;
        }


        public void AtualizarSituacao(int idHorasMesPrestador, string situacaoNova)
        {
            var horasMesPrestador = _horasMesPrestadorRepository.BuscarPorId(idHorasMesPrestador);

            LogHorasMesPrestador log = new LogHorasMesPrestador
            {
                SituacaoAnterior = horasMesPrestador.Situacao,
                SituacaoNova = situacaoNova,
                DataAlteracao = DateTime.Now,
                Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName,
                IdHorasMesPrestador = idHorasMesPrestador
            };
            _logHorasMesPrestadorRepository.Adicionar(log);

            horasMesPrestador.Situacao = situacaoNova;
            _horasMesPrestadorRepository.Update(horasMesPrestador);

            _unitOfWork.Commit();
        }

        public void AtualizarSituacao(int idHorasMesPrestador, string situacaoNova, string usuarioAlteracao)
        {
            var horasMesPrestador = _horasMesPrestadorRepository.BuscarPorId(idHorasMesPrestador);

            LogHorasMesPrestador log = new LogHorasMesPrestador
            {
                SituacaoAnterior = horasMesPrestador.Situacao,
                SituacaoNova = situacaoNova,
                DataAlteracao = DateTime.Now,
                Usuario = usuarioAlteracao,
                IdHorasMesPrestador = idHorasMesPrestador
            };
            _logHorasMesPrestadorRepository.Adicionar(log, usuarioAlteracao);

            horasMesPrestador.Situacao = situacaoNova;
            _horasMesPrestadorRepository.Update(horasMesPrestador, usuarioAlteracao);

            _unitOfWork.Commit();
        }

        private void EnviarEmail(EmailDTO email)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_microServicosUrls.UrlEnvioEmail);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/Email", content).Result;

            var responseString = response.Content.ReadAsStringAsync().Result;
        }

        private List<ValorParametroEmailDTO> DefinirParametros(HorasMes periodoVigente, HorasMesPrestador horasMesPrestador)
        {
            var diaPagamento = horasMesPrestador.Prestador.DiaPagamento.DescricaoValor;
            var parametros = new List<ValorParametroEmailDTO>();
            var parametroNome = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroRazaoSocial]",
                ParametroValor = horasMesPrestador.Prestador.EmpresasPrestador.Select(x => x.Empresa).Where(x => x.Ativo).OrderByDescending(x => x.DataVigencia).FirstOrDefault().RazaoSocial
            };
            parametros.Add(parametroNome);

            var parametroPeriodo = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroPeriodo]",
                ParametroValor = new DateTime(periodoVigente.Ano, periodoVigente.Mes, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("pt")).ToUpper() + "/" + periodoVigente.Ano
            };
            parametros.Add(parametroPeriodo);

            var mesLimite = new DateTime(periodoVigente.Ano, periodoVigente.Mes, 1).AddMonths(1).ToString("MMMM", CultureInfo.CreateSpecificCulture("pt"));
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            var parametroMesLimite = new ValorParametroEmailDTO
            {
                ParametroNome = "[MesLimite]",
                ParametroValor = ti.ToTitleCase(mesLimite)

            };
            parametros.Add(parametroMesLimite);

            var parametroDiaLimite = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroDataLimite]",
                ParametroValor = periodoVigente.PeriodosDiaPagamento.FirstOrDefault(x => x.DiaPagamento.DescricaoValor.Equals(diaPagamento)).DiaLimiteEnvioNF.ToString("dd/MM/yyyy 23:59")
            };
            parametros.Add(parametroDiaLimite);

            var parametroValor = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroValorNota]",
                ParametroValor = String.Format("{0:0.0#}", ObterValor(horasMesPrestador, false)).Replace(".", ",")
            };
            parametros.Add(parametroValor);

            var empresaGrupo = horasMesPrestador.Prestador.IdEmpresaGrupo.HasValue ?
                                    _empresaGrupoService.BuscarNoRMPorId(horasMesPrestador.Prestador.IdEmpresaGrupo.Value, horasMesPrestador.Prestador.PagarPelaMatriz, horasMesPrestador.Prestador.IdFilial)
                                    : null;

            var parametroNomeColigada = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroNomeEmpresaGrupo]",
                ParametroValor = empresaGrupo != null ? empresaGrupo.Nome : ""
            };
            parametros.Add(parametroNomeColigada);

            var parametroCnpjColigada = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroCnpjEmpresaGrupo]",
                ParametroValor = empresaGrupo != null ? empresaGrupo.Cnpj : ""
            };
            parametros.Add(parametroCnpjColigada);

            var parametroInscricaoColigada = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroInscricaoEstadualEmpresaGrupo]",
                ParametroValor = empresaGrupo != null ? empresaGrupo.InscricaoEstadual : ""
            };
            parametros.Add(parametroInscricaoColigada);

            var parametroProdutoRmPrestador = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroProdutoRmPrestador]",
                ParametroValor = ObterDescricaoProdutoRmPorId(horasMesPrestador)
            };
            parametros.Add(parametroProdutoRmPrestador);

            var token = horasMesPrestador.PrestadoresEnvioNf.Where(x => x.IdHorasMesPrestador == horasMesPrestador.Id).FirstOrDefault().Token;
            var parametroUrl = new ValorParametroEmailDTO();
            switch (Variables.EnvironmentName.ToUpper())
            {
                case "DEVELOPMENT":
                    parametroUrl.ParametroNome = "{uri}";
                    parametroUrl.ParametroValor = _linksBaseUrls.Development + "anexar-nota-fiscal/" + token;
                    parametros.Add(parametroUrl);
                    break;
                case "PRODUCTION":
                    parametroUrl.ParametroNome = "{uri}";
                    parametroUrl.ParametroValor = _linksBaseUrls.Production + "anexar-nota-fiscal/" + token;
                    parametros.Add(parametroUrl);
                    break;
                case "STAGING":
                    parametroUrl.ParametroNome = "{uri}";
                    parametroUrl.ParametroValor = _linksBaseUrls.Staging + "anexar-nota-fiscal/" + token;
                    parametros.Add(parametroUrl);
                    break;
                default:
                    break;
            }
            return parametros;
        }

        private string ObterDescricaoProdutoRmPorId(HorasMesPrestador horasMesPrestador)
        {
            var idColigada = horasMesPrestador.Prestador.IdEmpresaGrupo.Value;
            var idProdutoRm = horasMesPrestador.Prestador.IdProdutoRm;

            var client = new HttpClient
            {
                BaseAddress = new Uri(_microServicosUrls.UrlApiServico)
            };

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/ProdutoRM/" + idProdutoRm + " / " + idColigada).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<string>(responseString);
            return result;
        }

        public decimal ObterValor(HorasMesPrestador horasMesPrestador, bool considerarDescontos)
        {
            if (horasMesPrestador != null)
            {
                var valorAtual = horasMesPrestador.Prestador.ValoresPrestador.ToList().OrderByDescending(x => x.DataAlteracao).FirstOrDefault();

                if (valorAtual != null)
                {
                    if (horasMesPrestador.SemPrestacaoServico)
                    {
                        return 0;
                    }
                    else
                    {
                        decimal totalPagamentoMensal = horasMesPrestador.Prestador.TipoRemuneracao.DescricaoValor.Equals("MENSALISTA") ? _prestadorRepository.ObterValorMensalista(valorAtual, horasMesPrestador) :
                            (horasMesPrestador.Prestador.TipoRemuneracao.DescricaoValor.Equals("HORISTA") ?
                                (valorAtual.ValorHora * (horasMesPrestador.Horas ?? 0)) : 0);

                        decimal totalHoraAdicional = 0;
                        if (horasMesPrestador.Extras.HasValue)
                        {
                            totalHoraAdicional = horasMesPrestador.Extras.Value * valorAtual.ValorHora;
                        }

                        var valorTotal = totalPagamentoMensal + totalHoraAdicional;

                        if (considerarDescontos)
                        {
                            valorTotal = valorTotal - CalcularDescontos(horasMesPrestador);
                        }
                        return valorTotal;
                    }
                }
                else
                {
                    return 0;
                }
            }

            return 0;
        }

        private decimal CalcularDescontos(HorasMesPrestador horasMesPrestador)
        {
            var descontos = horasMesPrestador.DescontosPrestador.Sum(x => x.ValorDesconto);
            return descontos;
        }

        public void InativarPrestador(InativacaoPrestador inativacaoPrestador)
        {
            var prestador = _prestadorRepository.BuscarPorId(inativacaoPrestador.IdPrestador);

            if (inativacaoPrestador.Id == 0)
            {
                inativacaoPrestador.CodEacessoLegado = prestador.CodEacessoLegado;
                if (prestador.CodEacessoLegado.HasValue)
                {
                    InserirProfissionaisInativacaoEAcesso(inativacaoPrestador, prestador);
                }
                prestador.DataDesligamento = inativacaoPrestador.DataDesligamento;
                _prestadorRepository.Update(prestador);
                _inativacaoPrestadorRepository.Adicionar(inativacaoPrestador);
                _unitOfWork.Commit();
                EnviarEmailInativacao(prestador, inativacaoPrestador.DataDesligamento);
            }
            else
            {
                if (prestador.CodEacessoLegado.HasValue)
                {
                    AtualizarProfissionaisInativacaoEAcesso(inativacaoPrestador, prestador);
                }
                _inativacaoPrestadorRepository.Update(inativacaoPrestador);
                _unitOfWork.Commit();
            }
        }

        private void EnviarEmailInativacao(Prestador prestador, DateTime? dataDesligamentoInativaao)
        {
            var numeroContratacao = _dominioRepository.BuscarPorId(prestador.IdContratacao).DescricaoValor;

            var celula = _celulaRepository.BuscarPorIdAtivasComInclude(prestador.IdCelula);

            DateTime dataDesligamento = dataDesligamentoInativaao.GetValueOrDefault();

            List<ValorParametroEmailDTO> parametros = DefinirParametrosInativarPrestador(numeroContratacao, prestador.Pessoa.Nome, dataDesligamento, celula);
            var email = new EmailDTO
            {
                IdTemplate = 4,
                RemetenteNome = "Gestão de Contratos",
                Para = ObterDestinatarioEmailInativacao(),
                ValoresParametro = parametros
            };

            EnviarEmail(email);
        }

        private string ObterDestinatarioEmailInativacao()
        {
            var destinatario = "";
            switch (Variables.EnvironmentName.Trim().ToUpper())
            {
                case "DEVELOPMENT":
                    destinatario = "clbatista@stefanini.com";
                    break;
                case "STAGING":
                    destinatario = "clbatista@stefanini.com";
                    break;
                case "PRODUCTION":
                    destinatario = "acessos@stefanini.com";
                    break;
            }
            return destinatario;
        }

        private List<ValorParametroEmailDTO> DefinirParametrosInativarPrestador(string numeroContratacao, string nomeProfissional, DateTime dataDesligamento, Celula celula)
        {
            var parametros = new List<ValorParametroEmailDTO>();
            var parametroNumContratacao = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroContratacao]",
                ParametroValor = numeroContratacao.ToString()
            };
            parametros.Add(parametroNumContratacao);

            var parametroProfissional = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroProfissional]",
                ParametroValor = nomeProfissional
            };
            parametros.Add(parametroProfissional);

            var parametroDataDesligamento = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroDataDesligamento]",
                ParametroValor = dataDesligamento.ToString("dd/MM/yyyy")
            };
            parametros.Add(parametroDataDesligamento);

            var parametroCelula = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroCelula]",
                ParametroValor = celula.Id.ToString()
            };

            parametros.Add(parametroCelula);

            var parametroGerente = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroGerente]",
                ParametroValor = celula.Pessoa.Nome
            };
            parametros.Add(parametroGerente);

            var parametroGrupo = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroGrupo]",
                ParametroValor = celula.Grupo.Descricao
            };
            parametros.Add(parametroGrupo);



            return parametros;
        }

        public void ReativarPrestador(InativacaoPrestador inativacaoPrestador)
        {
            var prestador = _prestadorRepository.BuscarPorId(inativacaoPrestador.IdPrestador);
            var reativarPrestador = _inativacaoPrestadorRepository.BuscarPorId(inativacaoPrestador.Id);
            reativarPrestador.Motivo = inativacaoPrestador.Motivo;

            if (prestador.CodEacessoLegado.HasValue)
                AtualizarProfissionaisReativacaoEAcesso(reativarPrestador, prestador.CodEacessoLegado, prestador.IdCelula);

            prestador.DataDesligamento = null;
            _prestadorRepository.Update(prestador);
            _inativacaoPrestadorRepository.Update(reativarPrestador);
            _unitOfWork.Commit();
            //EnviarEmailInativacao(prestador); TODO criar o template email qdo for definido.
        }

        private void InserirProfissionaisInativacaoEAcesso(InativacaoPrestador inativacaoPrestador, Prestador prestador)
        {
            try
            {
                var idContratacao = _dominioRepository.BuscarPorId(prestador.IdContratacao).IdValor;
                var idTipoRemuneracao = prestador.IdTipoRemuneracao.HasValue ? _dominioRepository.BuscarPorId(prestador.IdTipoRemuneracao.Value).IdValor : 1;

                var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
                using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
                {
                    string origemMotivo = SharedEnuns.PrestadorInativacaoOrdemMotivo.EMPRESA.GetDescription();
                    if (inativacaoPrestador.FlagIniciativaDesligamento == 1)
                    {
                        origemMotivo = SharedEnuns.PrestadorInativacaoOrdemMotivo.PROFISSIONAL.GetDescription();
                    }

                    dbConnection.Open();
                    AtualizarProfissionaisDataDesligamentoEAcesso(prestador.CodEacessoLegado.GetValueOrDefault(), 1, prestador.IdCelula, inativacaoPrestador.DataDesligamento, dbConnection);
                    InserirTabelaProfissionaisInativacao(inativacaoPrestador, prestador, idContratacao, idTipoRemuneracao, dbConnection, origemMotivo);
                    AtualizarTabelaProfissionaisDesligamentoEAcesso(dbConnection, prestador.CodEacessoLegado);
                    AtualizarTabelaProfissionaisSistemasPagtoEAcesso(dbConnection, prestador.CodEacessoLegado);
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static void InserirTabelaProfissionaisInativacao(InativacaoPrestador inativacaoPrestador, Prestador prestador, int idContratacao, int idTipoRemuneracao, IDbConnection dbConnection, string origemMotivo)
        {
            string sQueryInsert =
                " INSERT [stfcorp].[tblProfissionaisInativacao]  ("
                + " idprofissional,"
                + " dtdesligamento,"
                + " idtipocontrato,"
                + " idtiporemuneracao,"
                + " ds_motivo,"
                + " in_retorno,"
                + " nm_Resp,"
                + " origem_motivo,"
                + " idcelula"
                + " ) VALUES ("
                + "'" + inativacaoPrestador.CodEacessoLegado + "', "
                + "@DataDesligamento, "
                + "'" + idContratacao + "', "
                + "'" + idTipoRemuneracao + "', "
                + "'" + inativacaoPrestador.Motivo + "', "
                + "'" + inativacaoPrestador.FlagRetorno + "', "
                + "'" + inativacaoPrestador.Responsavel + "', "
                + "'" + origemMotivo + "', "
                + "'" + prestador.IdCelula + "'"
                + ")";
            dbConnection.Query(sQueryInsert, new { DataDesligamento = inativacaoPrestador.DataDesligamento });
        }

        private void AtualizarTabelaProfissionaisDesligamentoEAcesso(IDbConnection dbConnection, int? codEacessoLegado)
        {
            try
            {
                string sQueryUpdate = @"UPDATE [stfcorp].tblProfissionaisDesligamento SET Status = 'FE' WHERE Status NOT IN ('FE','CC') AND IdProf = " + codEacessoLegado;
                dbConnection.Execute(sQueryUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void AtualizarTabelaProfissionaisSistemasPagtoEAcesso(IDbConnection dbConnection, int? codEacessoLegado)
        {
            try
            {
                string sQueryUpdate = @"UPDATE [stfcorp].[tblProfissionaisSistemasPagto] SET Inativo = 1 WHERE IdProfissional = " + codEacessoLegado;
                dbConnection.Execute(sQueryUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void AtualizarTabelalProfissionaisClientesEAcesso(IDbConnection dbConnection, int? codEacessoLegado)
        {
            try
            {
                string sQueryUpdate = @"UPDATE [stfcorp].[tblProfissionaisClientes] SET Inativo = 1 WHERE IdProfissional = " + codEacessoLegado;
                dbConnection.Execute(sQueryUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void AtualizarTabelaProfissionaisEmpresasEAcesso(IDbConnection dbConnection, int? codEacessoLegado)
        {
            try
            {
                string sQueryUpdate = @"UPDATE [stfcorp].[tblProfissionaisEmpresas] SET Inativo = 1 WHERE IdProfissional = " + codEacessoLegado;
                dbConnection.Execute(sQueryUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void AtualizarTabelaProfissionaisFormaPagtoEAcesso(IDbConnection dbConnection, int? codEacessoLegado)
        {
            try
            {
                string sQueryUpdate = @"UPDATE [stfcorp].[tblProfissionaisFormaPagto] SET In_Inativo = 1, padrao = 0 WHERE IdProfissional = " + codEacessoLegado;
                dbConnection.Execute(sQueryUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void AtualizarProfissionaisInativacaoEAcesso(InativacaoPrestador inativacaoPrestador, Prestador prestador)
        {
            try
            {
                var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
                using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
                {
                    dbConnection.Open();
                    string sQueryUpdate = "UPDATE [stfcorp].[tblProfissionaisInativacao] SET [ds_motivo] = " + "'" + inativacaoPrestador.Motivo + "'"
                                        + " , [in_retorno] = " + "'" + inativacaoPrestador.FlagRetorno + "'"
                                        + " , [nm_Resp] = " + "'" + inativacaoPrestador.Responsavel + "'"
                                        + " , [Liberado] = " + "'" + inativacaoPrestador.FlagIniciativaDesligamento + "'"
                                        + " WHERE idprofissional = " + "'" + inativacaoPrestador.CodEacessoLegado + "'"
                                        + "       AND idcelula = " + "'" + prestador.IdCelula + "'";

                    dbConnection.Execute(sQueryUpdate);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void AtualizarProfissionaisReativacaoEAcesso(InativacaoPrestador inativacaoPrestador, int? codEacessoLegado, int idCelula)
        {
            try
            {
                var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
                using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
                {
                    dbConnection.Open();
                    AtualizarProfissionaisDataDesligamentoEAcesso(codEacessoLegado.GetValueOrDefault(), 0, idCelula, null, dbConnection);
                    AtualizarTabelalProfissionaisClientesEAcesso(dbConnection, codEacessoLegado);
                    AtualizarTabelaProfissionaisEmpresasEAcesso(dbConnection, codEacessoLegado);
                    AtualizarTabelaProfissionaisFormaPagtoEAcesso(dbConnection, codEacessoLegado);
                    AtualizarTabelaProfissionaisSistemasPagtoEAcesso(dbConnection, codEacessoLegado);
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void AtualizarProfissionaisDataDesligamentoEAcesso(int idProfissional, int inativarPrestador, int idCelula, DateTime? dataDesligamento, IDbConnection dbConnection)
        {
            try
            {
                string sQueryUpdate = String.Empty;
                if (dataDesligamento == null)
                {
                    sQueryUpdate = "UPDATE [stfcorp].[tblProfissionais] SET [DtDesligamento] = NULL, [DTCadastro] = GETDATE(), [Inativo] = " + inativarPrestador
                                    + " WHERE Id = " + idProfissional + " AND Celula = " + idCelula;
                }
                else
                {
                    sQueryUpdate = "UPDATE [stfcorp].[tblProfissionais] SET [DtDesligamento] = @DataDesligamento,"
                                    + " [Inativo] = " + inativarPrestador + " WHERE Id = " + idProfissional + " AND Celula = " + idCelula;

                }
                dbConnection.Execute(sQueryUpdate, new { DataDesligamento = dataDesligamento });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AtualizarMigracaoInativacaoPrestador(int? idProfissionalEacesso)
        {
            List<InativacaoPrestador> inativacoesPrestador = new List<InativacaoPrestador>();
            List<InativacaoPrestador> inativacoesPrestadorProntos = new List<InativacaoPrestador>();
            List<Prestador> prestadores = new List<Prestador>();

            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                inativacoesPrestador = ObterProfissionaisInativacao(dbConnection, idProfissionalEacesso);
                dbConnection.Close();
            }

            prestadores = _prestadorRepository.BuscarTodos().ToList();
            foreach (var inativacaoPrestador in inativacoesPrestador)
            {
                try
                {
                    var prestador = prestadores.Find(x => x.CodEacessoLegado.Equals(inativacaoPrestador.CodEacessoLegado));

                    if (prestador != null)
                    {
                        inativacaoPrestador.IdPrestador = prestador.Id;
                        inativacoesPrestadorProntos.Add(inativacaoPrestador);
                    }

                    if (inativacoesPrestadorProntos.Count == 100)
                    {
                        _inativacaoPrestadorRepository.AdicionarRange(inativacoesPrestadorProntos);
                        _unitOfWork.Commit();
                        inativacoesPrestadorProntos = new List<InativacaoPrestador>();
                    }

                }
                catch (Exception ex)
                {
                    var mensagem = ex.InnerException != null ? ex.InnerException.Message : "";
                    AdicionarLogGenerico(ex, "PRESTADOR INATIVACAO", mensagem + " - " + inativacaoPrestador.CodEacessoLegado.ToString());
                    continue;
                }
            }
            if (inativacoesPrestadorProntos.Count > 0)
            {
                _inativacaoPrestadorRepository.AdicionarRange(inativacoesPrestadorProntos);
                _unitOfWork.Commit();
            }
        }

        private static List<InativacaoPrestador> ObterProfissionaisInativacao(IDbConnection dbConnection, int? idProfissionalEacesso)
        {
            string sQuery = @"SELECT [idprofissional] as CodEacessoLegado
                                      ,[dtdesligamento] as DataDesligamento
                                      ,isnull([ds_motivo],'') as Motivo
                                      ,[idcelula] as IdCelula
                                      ,CASE WHEN [in_retorno] = 'S' THEN 1 
	                                         WHEN [in_retorno] = '1' THEN 1 
	  	                                     WHEN [in_retorno] = 'N' THEN 0
			                                 WHEN [in_retorno] = NULL THEN 0  
	                                  END as FlagRetorno
                                      ,isnull([nm_Resp],'') as Responsavel
                                      ,CASE WHEN [origem_motivo] = 'E' THEN 0 
	                                        WHEN [origem_motivo] = 'P' THEN 1  
	                                   END as FlagIniciativaDesligamento
                                      ,[LoginLiberacao] as Usuario
                                FROM [stfcorp].[tblProfissionaisInativacao] WHERE idprofissional in (
                                    SELECT pro.[Id] FROM stfcorp.tblCelulas cel
                                            INNER JOIN stfcorp.tblProfissionais pro ON cel.IdCelula = pro.Celula
                                            INNER JOIN stfcorp.tblProfissionaisTiposContratos contr ON contr.Id = pro.IdTipoContrato AND contr.AgrupProf IN('LTDA')  
                                            INNER JOIN stfcorp.tblCargos ON pro.IdCargo = tblCargos.id
                                            INNER JOIN stfcorp.tblGrupos ON cel.IdGrupo = tblGrupos.IdGrupo
                                            LEFT JOIN stfcorp.tblProfissionaisClientes PCL ON PCL.IdProfissional = PRO.ID AND PCL.Inativo = 0
                                            LEFT JOIN stfcorp.tblProfissionaisSistemasPagto sispag ON pro.id = sispag.idprofissional
                                            LEFT JOIN stfcorp.tblEmpresasGrupo empGrupo ON pro.CodEmprGrupo = empGrupo.IdEmpresa
                                            LEFT JOIN stfcorp.tblProfissionaisContratos proCont ON pro.id = proCont.idProfissional
                                            WHERE(proCont.idContrato = (select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional))
                                            or(isnull((select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional)), 0) = 0) )  
                                            and ISNULL(pro.Status, '') in ('', 'FE')";

            if (idProfissionalEacesso.HasValue)
            {
                sQuery += " AND pro.Id = " + idProfissionalEacesso.Value + ") and datepart(year,dtdesligamento) <= 2070 and datepart(year,dtdesligamento) >= 1900";
            }
            else
            {
                sQuery += ") and datepart(year,dtdesligamento) <= 2070 and datepart(year,dtdesligamento) >= 1900";
            }

            var inativacoesPrestador = dbConnection.Query<InativacaoPrestador>(sQuery).AsList();
            return inativacoesPrestador;
        }

        public void AdicionarEAcesso(int idPrestador, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var prestador = _prestadorRepository.BuscarPorIdComIncludes(idPrestador);
            var idPrestadorEacesso = InserirPrestadorEAcesso(prestador, dbConnection, dbTransaction);
            InserirContatoEAcesso(prestador, dbConnection, idPrestadorEacesso, dbTransaction);
            InserirEmpresaEAcesso(prestador, dbConnection, idPrestadorEacesso, dbTransaction);
            InserirRemuneracaoEAcesso(prestador, dbConnection, dbTransaction, idPrestadorEacesso);
            if (prestador.ClientesServicosPrestador.Any())
            {
                InserirClienteServicoEAcesso(prestador, dbConnection, dbTransaction, idPrestadorEacesso);
            }
            AtualizarCodEacessoPrestador(idPrestador, idPrestadorEacesso);

        }

        public void AtualizarEAcesso(int idPrestador, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var prestador = _prestadorRepository.BuscarPorIdComIncludes(idPrestador);
            var connectionStringEAcesso = _connectionStrings.Value.EacessoConnection;
            var idPrestadorEacesso = AtualizarPrestadorEAcesso(prestador, dbConnection, dbTransaction);
            AtualizarContatoEAcesso(prestador, dbConnection, idPrestadorEacesso, dbTransaction);
        }

        private void AtualizarCodEacessoPrestador(int idPrestador, int idPrestadorEacesso)
        {
            var prestador = _prestadorRepository.BuscarPorIdComIncludes(idPrestador);
            prestador.CodEacessoLegado = idPrestadorEacesso;
            prestador.Pessoa.CodEacessoLegado = idPrestadorEacesso;
            _prestadorRepository.Update(prestador);
            _unitOfWork.Commit();
        }

        private void CadastrarPessoa(int idPrestadorEacesso, Prestador prestador)
        {
            var pessoa = new Pessoa
            {
                CodEacessoLegado = idPrestadorEacesso,
                Nome = prestador.Pessoa.Nome,
                Email = prestador.Pessoa.Email
            };
            _pessoaRepository.Adicionar(pessoa);
            _unitOfWork.Commit();
        }

        private int? AtualizarPrestadorEAcesso(Prestador prestador, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var idEmpresaGrupoEacesso = ObterIdEmpresaGrupoEacesso(dbConnection, prestador.IdEmpresaGrupo, dbTransaction);
            var idFilialEacesso = ObterIdFilialEacesso(dbConnection, prestador.IdEmpresaGrupo, prestador.IdFilial, dbTransaction);
            var idCliente = prestador.ClientesServicosPrestador.FirstOrDefault(x => x.Ativo);

            var query = $@"UPDATE stfcorp.tblProfissionais SET
                 celula = {prestador.IdCelula}
                ,situacao = {(prestador.IdSituacao.HasValue ? prestador.SituacaoPrestador.IdValor : 1)}
                ,idtipocontrato = {prestador.Contratacao.IdValor}
                ,idcargo = {prestador.Cargo.IdValor}
                ,dtadmissao = @DataInicio
                ,inativo = 0
                ,integracao = 0
                ,idestadocivil = {prestador.Pessoa.EstadoCivil.IdValor}
                ,codemprgrupo = {idEmpresaGrupoEacesso}
                ,nome = '{prestador.Pessoa.Nome}'
                ,sexo = {prestador.Pessoa.Sexo.IdValor}
                ,dtnascimento = @DataNascimento
                ,cpf = '{prestador.Pessoa.Cpf}'
                ,rg = '{prestador.Pessoa.Rg}'
                ,estrangeiro = 0
                ,Nacionalidade = {prestador.Pessoa.Nacionalidade.IdValor}
                ,DescansoPJ = 0
                ,nomepai ='{prestador.Pessoa.NomeDoPai}'
                ,nomemae = '{prestador.Pessoa.NomeDaMae}'
                ,instrucao = {prestador.Pessoa.Escolaridade.IdValor}
                ,idgraduacao = {(prestador.Pessoa.IdGraduacao.HasValue ? prestador.Pessoa.Graduacao.IdValor : 0)}
                ,instrucao1 = {(prestador.Pessoa.IdExtensao.HasValue ? prestador.Pessoa.Extensao.IdValor : 0)}
                ,idGraduacao1 = {(prestador.IdAreaFormacao.HasValue ? prestador.AreaFormacao.IdValor : 0)}
                ,IdFilial = {idFilialEacesso}
                ,Contato = 0
                ,PagEspecial = 0
                ,CVisita = 0 
                ,PesqDesenv = 0
                ,CodProdutoFornec = {prestador.IdProdutoRm}
                ,Erp_Externo = {prestador.IdRepresentanteRmTRPR}
                ,IdCliente = {(idCliente == null ? "NULL" : idCliente.IdCliente.ToString())}
                 WHERE  id = {prestador.CodEacessoLegado};";

            dbConnection.Execute(query, new { DataInicio = prestador.DataInicio, DataNascimento = prestador.Pessoa.DtNascimento }, transaction: dbTransaction);

            return prestador.CodEacessoLegado;
        }

        private int InserirPrestadorEAcesso(Prestador prestador, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var codEacessoDisponivel = ObterCodFuncionarioDisponivelEAcesso(dbConnection, dbTransaction);
            var idEmpresaGrupoEacesso = ObterIdEmpresaGrupoEacesso(dbConnection, prestador.IdEmpresaGrupo, dbTransaction);
            var idFilialEacesso = ObterIdFilialEacesso(dbConnection, prestador.IdEmpresaGrupo, prestador.IdFilial, dbTransaction);


            var dtCadastro = DateTime.Now;
            var dtAdmissao = prestador.DataInicio;
            var dtNasc = prestador.Pessoa.DtNascimento;
            var idCliente = prestador.ClientesServicosPrestador.FirstOrDefault(x => x.Ativo);

            var query = $@"INSERT stfcorp.tblProfissionais (
                                id, celula, situacao, idtipocontrato, idcargo ,DtCadastro, DtAdmissao, inativo, integracao,
                                idestadocivil, codemprgrupo, nome, sexo,dtnascimento, cpf, rg, Estrangeiro, Nacionalidade,
                                DescansoPJ, nomepai, nomemae, instrucao, idgraduacao, instrucao1, idGraduacao1, IdFilial, 
                                Contato, PagEspecial, CVisita, PesqDesenv, CodProdutoFornec, Erp_Externo, IdCliente
                         )
                         VALUES (
                            {codEacessoDisponivel} ,{prestador.IdCelula}, 
                            {(prestador.IdSituacao.HasValue ? prestador.SituacaoPrestador.IdValor : 1)},
                            {prestador.Contratacao.IdValor}, {prestador.Cargo.IdValor}, @DtCadastro, @DtAdmissao,
                            0, 0, {prestador.Pessoa.EstadoCivil.IdValor}, {idEmpresaGrupoEacesso},
                            '{TratarApostofre(prestador.Pessoa.Nome)}', {prestador.Pessoa.Sexo.IdValor}, @DtNasc,'{prestador.Pessoa.Cpf}',
                            '{prestador.Pessoa.Rg}', 0,{prestador.Pessoa.Nacionalidade.IdValor}, 0, '{TratarApostofre(prestador.Pessoa.NomeDoPai)}',
                            '{TratarApostofre(prestador.Pessoa.NomeDaMae)}', {prestador.Pessoa.Escolaridade.IdValor},
                            {(prestador.Pessoa.IdGraduacao.HasValue ? prestador.Pessoa.Graduacao.IdValor : 0)},
                            {(prestador.Pessoa.IdExtensao.HasValue ? prestador.Pessoa.Extensao.IdValor : 0)},
                            {(prestador.IdAreaFormacao.HasValue ? prestador.AreaFormacao.IdValor : 0)},
                            {idFilialEacesso}, 0, 0, 0, 0, {prestador.IdProdutoRm},
                            {prestador.IdRepresentanteRmTRPR}, {(idCliente == null ? "NULL" : idCliente.IdCliente.ToString())}
                        )";
            dbConnection.Execute(query, new { DtCadastro = dtCadastro, DtAdmissao = dtAdmissao, DtNasc = dtNasc }, transaction: dbTransaction);
            return codEacessoDisponivel;
        }

        private void InserirContatoEAcesso(Prestador prestador, IDbConnection dbConnection, int idPrestadorEacesso, IDbTransaction dbTransaction)
        {
            var idCidade = ObterCidadeEmpresaEAcesso(TratarApostofre(prestador.Pessoa.Endereco.Cidade.NmCidade), dbConnection, dbTransaction);
            var query = @"UPDATE stfcorp.tblprofissionais SET AbrevLogradouro = '" + prestador.Pessoa.Endereco.SgAbrevLogradouro + "', " +
                            "Endereco = '" + TratarApostofre(prestador.Pessoa.Endereco.NmEndereco) + "', Num = '" + prestador.Pessoa.Endereco.NrEndereco + "', " +
                            "Complemento = '" + TratarApostofre(prestador.Pessoa.Endereco.NmCompEndereco) + "', Bairro = '" + TratarApostofre(prestador.Pessoa.Endereco.NmBairro) + "', " +
                            "CEP = '" + prestador.Pessoa.Endereco.NrCep +
                            "', idCidade = " + idCidade + ", " +
                            "Email = '" + prestador.Pessoa.Email + "', Email_Int = '" + prestador.Pessoa.EmailInterno + "', " +
                            "TelCom = '" + prestador.Pessoa.Telefone.NumeroComercial + "', RamalTelCom = '" + prestador.Pessoa.Telefone.NumeroComercialRamal + "', " +
                            "Celular = '" + prestador.Pessoa.Telefone.Celular + "', TelRes = '" + prestador.Pessoa.Telefone.NumeroResidencial + "', " +
                            "NumeroNextel = '" + prestador.Pessoa.Telefone.NumeroNextel + "', IdNextel = '" + prestador.Pessoa.Telefone.IdNextel + "' " +
                            "WHERE id = " + idPrestadorEacesso;
            dbConnection.Query(query, null, transaction: dbTransaction);
        }

        private void AtualizarContatoEAcesso(Prestador prestador, IDbConnection dbConnection, int? idPrestadorEacesso, IDbTransaction dbTransaction)
        {
            var idCidade = ObterCidadeEmpresaEAcesso(prestador.Pessoa.Endereco.Cidade.NmCidade, dbConnection, dbTransaction);
            var query = @"UPDATE stfcorp.tblprofissionais SET AbrevLogradouro = '" + prestador.Pessoa.Endereco.SgAbrevLogradouro + "', " +
                            "Endereco = '" + prestador.Pessoa.Endereco.NmEndereco + "', Num = '" + prestador.Pessoa.Endereco.NrEndereco + "', " +
                            "Complemento = '" + prestador.Pessoa.Endereco.NmCompEndereco + "', Bairro = '" + prestador.Pessoa.Endereco.NmBairro + "', " +
                            "CEP = '" + prestador.Pessoa.Endereco.NrCep +
                            "', idCidade = " + idCidade + ", " +
                            "Email = '" + prestador.Pessoa.Email + "', Email_Int = '" + prestador.Pessoa.EmailInterno + "', " +
                            "TelCom = '" + prestador.Pessoa.Telefone.NumeroComercial + "', RamalTelCom = '" + prestador.Pessoa.Telefone.NumeroComercialRamal + "', " +
                            "Celular = '" + prestador.Pessoa.Telefone.Celular + "', TelRes = '" + prestador.Pessoa.Telefone.NumeroResidencial + "', " +
                            "NumeroNextel = '" + prestador.Pessoa.Telefone.NumeroNextel + "', IdNextel = '" + prestador.Pessoa.Telefone.IdNextel + "' " +
                            "WHERE id = " + idPrestadorEacesso;
            dbConnection.Query(query, null, transaction: dbTransaction);
        }

        public void InserirEmpresaEAcesso(Prestador prestador, IDbConnection dbConnection, int idPrestadorEacesso, IDbTransaction dbTransaction)
        {
            foreach (var empresa in prestador.EmpresasPrestador.Select(x => x.Empresa))
            {
                var query = "";
                var idEmpresaRm = empresa.IdEmpresaRm ?? 0;
                var idCidade = ObterCidadeEmpresaEAcesso(empresa.Endereco.Cidade.NmCidade, dbConnection, dbTransaction);
                if (!_empresaPrestadorService.VerificaExisteEmpresaNoEacesso(empresa.Cnpj, idPrestadorEacesso, dbConnection, dbTransaction))
                {
                    query = @"INSERT stfcorp.tblprofissionaisempresas (
                                        IdProfissional, RazaoSocial, AbrevLogradouro, Endereco, Num, Complemento, Bairro, CEP, IdCidade,
                                        CNPJ, InscEst, Obs, Atuacao, Inativo, DtVigencia, DtAlteracao, ErpExterno
                                 )
                                 VALUES (
                                    " + idPrestadorEacesso + ",'" + TratarApostofre(empresa.RazaoSocial) + "','" + empresa.Endereco.SgAbrevLogradouro + "','" + TratarApostofre(empresa.Endereco.NmEndereco) + "'," +
                                    "'" + empresa.Endereco.NrEndereco + "','" + TratarApostofre(empresa.Endereco.NmCompEndereco) + "','" + TratarApostofre(empresa.Endereco.NmBairro) + "','" + empresa.Endereco.NrCep + "'," +
                                    "" + idCidade + ",'"
                                    + empresa.Cnpj + "','" + TratarApostofre(empresa.InscricaoEstadual) + "'," +
                                    "'" + TratarApostofre(empresa.Observacao) + "','" + empresa.Atuacao + "'," + (empresa.Ativo ? 0 : 1) + "," +
                                    "@DataVigencia," +
                                    "@DataAtual," + idEmpresaRm +
                                ")";
                }
                else
                {
                    query = "UPDATE stfcorp.tblprofissionaisempresas set IdProfissional = " + idPrestadorEacesso + ", RazaoSocial = '" + TratarApostofre(empresa.RazaoSocial) + "'," +
                        "AbrevLogradouro = '" + empresa.Endereco.SgAbrevLogradouro + "', Endereco = '" + TratarApostofre(empresa.Endereco.NmEndereco) + "', " +
                        "Num = '" + empresa.Endereco.NrEndereco + "', Complemento = '" + empresa.Endereco.NmCompEndereco + "', Bairro = '" + TratarApostofre(empresa.Endereco.NmBairro) + "'," +
                        "CEP = '" + empresa.Endereco.NrCep + "', IdCidade = " + idCidade + ", CNPJ = '" + empresa.Cnpj + "', InscEst = '" + empresa.InscricaoEstadual + "'," +
                        "Obs = '" + TratarApostofre(empresa.Observacao) + "', Atuacao = '" + TratarApostofre(empresa.Atuacao) + "', Inativo = " + (empresa.Ativo ? 0 : 1) + "," +
                        "DtVigencia = @DataVigencia," +
                        "DtAlteracao = @DataAtual, ErpExterno = " + idEmpresaRm + " " +
                        "WHERE cnpj = '" + empresa.Cnpj + "'";
                }
                dbConnection.Execute(query, new { DataVigencia = empresa.DataVigencia, DataAtual = DateTime.Now }, dbTransaction);
            }
        }

        private void InserirRemuneracaoEAcesso(Prestador prestador, IDbConnection dbConnection, IDbTransaction dbTransaction, int idPrestadorEacesso)
        {
            foreach (var valor in prestador.ValoresPrestador)
            {
                var valorHora = Arredondar(valor.ValorHora).ToString().Replace(".", ",").Replace(",", ".");
                var valorMes = Arredondar(valor.ValorMes).ToString().Replace(".", ",").Replace(",", ".");
                var query = "INSERT stfcorp.tblProfissionaisRemuneracoes (IdProfissional, IdTipoRemuneracao, IdMoeda, DtAlteracao, QtdHoras, VrHora, VrMes, VrLTDA, VrCLT) VALUES ( " +
                                "" + idPrestadorEacesso + "," + ObterIdValorTipoRemuneracao(valor.IdTipoRemuneracao) + ",1, @DataAtual," +
                                "" + valor.Quantidade + "," + valorHora + "," + valorMes + "," + (ObterIdValorTipoRemuneracao(valor.IdTipoRemuneracao) == 1 ? valorMes : valorHora) + "," +
                                "0)";

                dbConnection.Execute(query, new { DataAtual = DateTime.Now }, transaction: dbTransaction);

            }
        }
        private void InserirClienteServicoEAcesso(Prestador prestador, IDbConnection dbConnection, IDbTransaction dbTransaction, int idPrestadorEacesso)
        {
            foreach (var clienteServicoPrestador in prestador.ClientesServicosPrestador)
            {
                StringBuilder stringBuilder = new StringBuilder("");
                stringBuilder.Append(@"INSERT INTO [stfcorp].[tblProfissionaisClientes] (
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
                stringBuilder.Append("'" + idPrestadorEacesso + "'");
                stringBuilder.Append(",@DataInicio");
                stringBuilder.Append(",@DataPrevisaoTermino");
                stringBuilder.Append(",'" + clienteServicoPrestador.IdCliente + "'");
                stringBuilder.Append("," + clienteServicoPrestador.IdServico + "");
                stringBuilder.Append(",'" + TratarApostofre(clienteServicoPrestador.DescricaoTrabalho) + "'");
                stringBuilder.Append(",@ValorCusto");
                stringBuilder.Append(",@ValorRepasse");
                stringBuilder.Append(",@ValorVenda");
                stringBuilder.Append("," + clienteServicoPrestador.IdLocalTrabalho + "");
                stringBuilder.Append("," + 0 + "");
                if (clienteServicoPrestador.Ativo == true)
                {
                    stringBuilder.Append("," + 0);
                }
                else
                {
                    stringBuilder.Append("," + 1);
                }
                stringBuilder.Append(")");

                dbConnection.Execute(stringBuilder.ToString(),
                    new
                    {
                        DataPrevisaoTermino = clienteServicoPrestador.DataPrevisaoTermino ?? null,
                        DataInicio = clienteServicoPrestador.DataInicio,
                        ValorCusto = clienteServicoPrestador.ValorCusto,
                        ValorRepasse = clienteServicoPrestador.ValorRepasse,
                        ValorVenda = clienteServicoPrestador.ValorVenda,
                    },
                    transaction: dbTransaction);
            }
        }

        public void InserirRemuneracaoEAcesso(ValorPrestador valor)
        {
            var prestador = _prestadorRepository.BuscarPorIdComIncludes(valor.IdPrestador);
            var connectionStringEAcesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEAcesso))
            {
                dbConnection.Open();
                var valorHora = Arredondar(valor.ValorHora).ToString().Replace(".", ",").Replace(",", ".");
                var valorMes = Arredondar(valor.ValorMes).ToString().Replace(".", ",").Replace(",", ".");
                var query = "INSERT [stfcorp].[tblProfissionaisRemuneracoes] (IdProfissional, IdTipoRemuneracao, IdMoeda, DtAlteracao, QtdHoras, VrHora, VrMes, VrLTDA, VrCLT) VALUES ( " +
                                "" + prestador.CodEacessoLegado + "," + ObterIdValorTipoRemuneracao(valor.IdTipoRemuneracao) + ",1, @DataAtual," +
                                "" + valor.Quantidade + "," + valorHora + "," + valorMes + "," + (ObterIdValorTipoRemuneracao(valor.IdTipoRemuneracao) == 1 ? valorMes : valorHora) + "," +
                                "0)";

                dbConnection.Execute(query, new { DataAtual = DateTime.Now });

            }
        }


        private int ObterIdValorTipoRemuneracao(int idTipoRemuneracao)
        {
            //var tiposRemuneracao = _dominioRepository.BuscarDominiosPorTipo("TIPO_REMUNERACAO", true);
            var tiposRemuneracao = _dominioRepository.Buscar(x => x.ValorTipoDominio == "TIPO_REMUNERACAO" && x.Ativo == true).Select(x => new ComboDefaultDto
            {
                Id = x.Id,
                Descricao = x.DescricaoValor,
                Ativo = x.Ativo,
                IdValor = x.IdValor,
            }).ToList();

            var tipoRemuneracao = tiposRemuneracao.FirstOrDefault(x => x.Id == idTipoRemuneracao);
            if (tipoRemuneracao != null)
            {
                return tipoRemuneracao.IdValor;
            }
            else
            {
                return 1;
            }
        }

        private decimal Arredondar(decimal valor)
        {
            decimal inteiro = Math.Truncate(valor * 100);
            var result = (inteiro / 100);
            return result;
        }

        private int ObterCodFuncionarioDisponivelEAcesso(IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var query = "SELECT MAX(id) from stfcorp.tblprofissionais";
            var id = dbConnection.Query<int>(query, null, transaction: dbTransaction).FirstOrDefault();
            return id + 1;
        }

        private int ObterIdEmpresaGrupoEacesso(IDbConnection dbConnection, int? idEmpresaGrupo, IDbTransaction dbTransaction)
        {
            var query = "SELECT idEmpresa from stfcorp.tblEmpresasGrupo where erpExterno = " + idEmpresaGrupo;
            var id = dbConnection.Query<int>(query, null, dbTransaction).FirstOrDefault();
            return id;
        }

        private int ObterIdFilialEacesso(IDbConnection dbConnection, int? idEmpresaGrupo, int? idFilial, IDbTransaction dbTransaction)
        {
            var idEmpresaGrupoEacesso = ObterIdEmpresaGrupoEacesso(dbConnection, idEmpresaGrupo, dbTransaction);

            var query = "SELECT idFilial from stfcorp.tblfiliais where erpExterno = " + idFilial + "and idEmpresa = " + idEmpresaGrupoEacesso;
            var id = dbConnection.Query<int>(query, null, transaction: dbTransaction).FirstOrDefault();
            return id;
        }

        public int? ObterCidadeEmpresaEAcesso(string cidade, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var query = "SELECT idCidade from stfcorp.tblcidades where Cidade = '" + cidade + "'";
            int? id = dbConnection.Query<int>(query, null, transaction: dbTransaction).FirstOrDefault();
            return id.HasValue ? id : 23;
        }

        public void PersistirPrestadorRemuneracao(ValorPrestador valorPrestador)
        {
            var prestador = _prestadorRepository.BuscarPorId(valorPrestador.IdPrestador);

            if (valorPrestador.Id == 0)
            {
                var horaMesId = _horasMesRepository.Buscar(x => x.Ano == valorPrestador.DataReferencia.Year &&
                                                                x.Mes == valorPrestador.DataReferencia.Month)
                    .First().Id;

                var horaMesPrestador = new HorasMesPrestador
                {
                    IdHorasMes = horaMesId,
                    Situacao = "HORAS PENDENTE",
                    SemPrestacaoServico = false,
                    Usuario = _variables.UserName,
                    DataAlteracao = DateTime.Now
                };

                prestador.HorasMesPrestador.Add(horaMesPrestador);

                valorPrestador.DataAlteracao = DateTime.Now;
                prestador.DataUltimaAlteracaoEacesso = valorPrestador.DataAlteracao;
                _valorPrestadorRepository.Adicionar(valorPrestador);
                prestador.IdTipoRemuneracao = valorPrestador.IdTipoRemuneracao;
                _prestadorRepository.Update(prestador);
                _unitOfWork.Commit();
            }
            else
            {
                prestador.DataUltimaAlteracaoEacesso = valorPrestador.DataAlteracao;
                _prestadorRepository.Update(prestador);
                _unitOfWork.Commit();

                using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
                {
                    foreach (var valor in prestador.ValoresPrestador)
                    {
                        var valorHora = Arredondar(valor.ValorHora).ToString().Replace(".", ",").Replace(",", ".");
                        var valorMes = Arredondar(valor.ValorMes).ToString().Replace(".", ",").Replace(",", ".");
                        var query = "UPDATE stfcorp.tblProfissionaisRemuneracoes SET" +
                            ",IdMoeda = 1" +
                            ",DtAlteracao = @DataAtual" +
                            ",QtdHoras = " + valor.Quantidade +
                            ",VrHora = " + valorHora +
                            ",VrMes =  " + valorMes +
                            ",VrLTDA = " + (ObterIdValorTipoRemuneracao(valor.IdTipoRemuneracao) == 1 ? valorMes : valorHora) +
                            ",VrCLT = 0" +
                            " WHERE IdProfissional = " + prestador.CodEacessoLegado +
                            " INATIVO = 0 ";
                        dbConnection.Execute(query, new { DataAtual = DateTime.Now });
                    }
                }
            }
        }

        public void AtualizarMigracaoRemuneracaoPrestador(int? idProfissionalEacesso)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                List<ValorPrestador> itensParaAdicionar = new List<ValorPrestador>();
                var count = 0;
                var prestadores = _prestadorRepository.Buscar(x => x.CodEacessoLegado.HasValue).ToList();
                var valoresPrestador = ObterProfissionaisRemuneracao(dbConnection, idProfissionalEacesso);
                foreach (var valorPrestador in valoresPrestador)
                {
                    var prestador = prestadores.FirstOrDefault(x => x.CodEacessoLegado.Value == valorPrestador.IdProfissional);
                    if (prestador != null)
                    {
                        valorPrestador.IdPrestador = prestador.Id;
                        itensParaAdicionar.Add(valorPrestador);
                        count++;
                        if (count > 200)
                        {
                            _valorPrestadorRepository.AdicionarRange(itensParaAdicionar);
                            _unitOfWork.Commit();
                            count = 0;
                            itensParaAdicionar = new List<ValorPrestador>();
                        }
                    }
                }
                if (count > 0)
                {
                    _valorPrestadorRepository.AdicionarRange(itensParaAdicionar);
                    _unitOfWork.Commit();
                    count = 0;
                    itensParaAdicionar = new List<ValorPrestador>();
                }
            }
        }

        private static List<ValorPrestador> ObterProfissionaisRemuneracao(IDbConnection dbConnection, int? idProfissionalEacesso)
        {
            dbConnection.Open();
            string sQuery = @"SELECT [idprofissional] as IdProfissional      
                                    ,isnull([IdMoeda],1) as IdMoeda
                                    ,[dtalteracao] as DataAlteracao      
                                    ,[dtalteracao] as DataReferencia
                                    ,case when [idtiporemuneracao] = 1 then 3707 else 3708 end as IdTipoRemuneracao
                                    ,isnull([qtdhoras],0) as Quantidade
                                    ,isnull([vrclt],0) as ValorClt
                                    ,isnull([VrPericulosidade],0) as ValorPericulosidade
                                    ,isnull([vrhora],0) as ValorHora
                                    ,isnull([vrmes],0) as ValorMes
		                            ,isnull([vrcarco],0) as ValorPropriedadeIntelectual
                                    ,idRemuneracao as idRemuneracaoEacesso
                            FROM [stfcorp].[tblProfissionaisRemuneracoes]
                            WHERE IdProfissional IN (
                                        SELECT pro.[Id] FROM stfcorp.tblCelulas cel  
                                            INNER JOIN stfcorp.tblProfissionais pro ON cel.IdCelula = pro.Celula  
                                            INNER JOIN stfcorp.tblProfissionaisTiposContratos contr ON contr.Id = pro.IdTipoContrato AND contr.AgrupProf IN('LTDA')  
                                            INNER JOIN stfcorp.tblCargos ON pro.IdCargo = tblCargos.id  
                                            INNER JOIN stfcorp.tblGrupos ON cel.IdGrupo = tblGrupos.IdGrupo  
                                                LEFT JOIN stfcorp.tblProfissionaisClientes PCL ON PCL.IdProfissional = PRO.ID AND PCL.Inativo = 0  
                                                LEFT JOIN stfcorp.tblProfissionaisSistemasPagto sispag ON pro.id = sispag.idprofissional  
                                                LEFT JOIN stfcorp.tblEmpresasGrupo empGrupo ON pro.CodEmprGrupo = empGrupo.IdEmpresa  
                                                LEFT JOIN stfcorp.tblProfissionaisContratos proCont ON pro.id = proCont.idProfissional 
                                            WHERE (proCont.idContrato = (select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional))  
                                                or (isnull((select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional)),0) = 0) )  
                                                and ISNULL(pro.Status, '') in ('', 'FE')";

            if (idProfissionalEacesso.HasValue)
            {
                sQuery += "AND pro.Id = " + idProfissionalEacesso.Value + ") and (inativo = 0 or inativo is null)";
            }
            else
            {
                sQuery += ") and (inativo = 0 or inativo is null)";
            }

            var valoresPrestador = dbConnection.Query<ValorPrestador>(sQuery).AsList();
            dbConnection.Close();
            return valoresPrestador;
        }

        public List<ValorPrestadorBeneficio> ObterValoresPrestadorBeneficios(int idValorPrestador)
        {
            var result = _valorPrestadorBeneficioRepository.BuscarValoresPrestadorBeneficio(idValorPrestador);

            return result;
        }

        public List<RelatorioPrestadoresDto> BuscarRelatorioPrestadores(int empresaId, int filialId, FiltroGenericoDtoBase<RelatorioPrestadoresDto> filtro)
        {
            var result = _empresaPrestadorRepository.BuscarRelatorioPrestadores(empresaId, filialId, filtro);
            var empresas = _empresaGrupoService.BuscarTodasNoRM();

            var listaServicos = new List<string>();
            var listaCelulas = new List<string>();

            foreach (var item in result)
            {
                listaServicos.Add(item.IdServico.ToString());

                listaCelulas.Add(item.IdCelula.ToString());
            }

            var listaClienteServicos = ObterClienteServicoEAcesso(listaServicos);

            var listaDiretorias = ObterDiretoriaEAcesso(listaCelulas);

            foreach (RelatorioPrestadoresDto prestador in result)
            {
                var empresasFiltradas = empresas.Where(x => x.Id.Equals(prestador.IdEmpresaGrupo));
                if (empresasFiltradas.Any())
                {
                    prestador.Empresa = empresasFiltradas.First().Descricao;
                }
                else
                {
                    prestador.Empresa = "";
                }

                var clienteServico = listaClienteServicos.Where(x => x.IdCliente == prestador.IdCliente && x.IdServico == prestador.IdServico).FirstOrDefault();
                if (clienteServico != null)
                {
                    prestador.Custo = clienteServico.Custo ?? "";
                    prestador.Cliente = clienteServico.Cliente ?? "";
                    prestador.Servico = clienteServico.Servico ?? "";
                }

                var diretoria = listaDiretorias.Where(x => x.IdCelula == prestador.IdCelula).FirstOrDefault();
                if (diretoria != null)
                {
                    prestador.Diretoria = diretoria.Diretor ?? "";
                }
            }

            return result;
        }

        public void ReabrirLancamento(int idHorasMesPrestador)
        {
            var horasMesPrestador = _horasMesPrestadorRepository.BuscarPorId(idHorasMesPrestador);

            LogHorasMesPrestador log = new LogHorasMesPrestador
            {
                SituacaoAnterior = horasMesPrestador.Situacao,
                SituacaoNova = SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_PENDENTE.GetDescription(),
                DataAlteracao = DateTime.Now,
                Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName,
                IdHorasMesPrestador = idHorasMesPrestador
            };
            _logHorasMesPrestadorRepository.Adicionar(log);

            horasMesPrestador.Horas = null;
            horasMesPrestador.Extras = null;
            horasMesPrestador.Situacao = SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_PENDENTE.GetDescription();
            _horasMesPrestadorRepository.Update(horasMesPrestador);

            RemoverRegistroDeNfSeArquivoAindaNaoTiverSidoEnviado(horasMesPrestador);

            _unitOfWork.Commit();
        }

        private string DefinirEmailsDestinatarios(string emailInterno, string emailExterno, string emailParceiro)
        {
            if (!String.IsNullOrEmpty(emailParceiro))
            {
                return emailParceiro;
            }
            else if (!String.IsNullOrEmpty(emailInterno) && !String.IsNullOrEmpty(emailExterno))
            {
                return emailInterno.ToUpper().Equals(emailExterno.ToUpper()) ? emailExterno : emailExterno + "," + emailInterno;
            }
            else if (!String.IsNullOrEmpty(emailInterno) && String.IsNullOrEmpty(emailExterno))
            {
                return emailInterno;
            }
            else if (String.IsNullOrEmpty(emailInterno) && !String.IsNullOrEmpty(emailExterno))
            {
                return emailExterno;
            }
            return "";
        }

        public string ObterBancoNoRm(int? idBanco)
        {
            if (idBanco.HasValue)
            {
                var connectionStringRm = _connectionStrings.Value.RMConnection;
                using (IDbConnection dbConnection = new SqlConnection(connectionStringRm))
                {
                    var query = "select nomereduzido from gbanco where numeroOficial = " + idBanco;
                    var result = dbConnection.Query<string>(query).FirstOrDefault();
                    dbConnection.Close();
                    return result;
                }
            }
            else
            {
                return "";
            }
        }

        public FiltroGenericoDtoBase<ConciliacaoPagamentoDto> FiltrarConciliacaoPagamentos(FiltroGenericoDtoBase<ConciliacaoPagamentoDto> filtro, bool resumo)
        {
            const int TODOS = 1;

            var result = _prestadorRepository.FiltrarConciliacaoPagamentos(filtro, resumo);
            ObterDadosPagamentoNoRm(result);
            if (!resumo)
            {
                if (filtro.Id != TODOS)
                {
                    result.Valores = result.Valores.Where(x => x.Divergente).ToList();
                }

                filtro.Total = result.Valores.Count();
                filtro.Valores = result.Valores.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            }
            else
            {
                filtro.Valores = result.Valores.ToList();
            }

            return result;
        }

        private void ObterDadosPagamentoNoRm(FiltroGenericoDtoBase<ConciliacaoPagamentoDto> result)
        {
            foreach (var item in result.Valores)
            {
                PopularDadosBancarios(item);
                PopularContaCaixa(item);
                item.DadosFinanceiro = (String.IsNullOrEmpty(item.Conta) ||
                                        //String.IsNullOrEmpty(item.ContaCaixa) ||
                                        String.IsNullOrEmpty(item.Banco) ||
                                        String.IsNullOrEmpty(item.Agencia) ? false : true);
                item.Divergente = (!item.Conciliado || !item.CentroCusto || !item.DadosFinanceiro || !item.Fechado) ? true : false;
                if (!String.IsNullOrEmpty(item.Banco))
                {
                    if (item.Banco.ToUpper() == "33")
                    {
                        item.BancoEmpresa = "SANTANDER";
                        item.ContaEmpresa = "13000032-9";
                        item.CodigoBancoEmpresa = "033";
                    }
                    else
                    {
                        item.BancoEmpresa = "ITAU";
                        item.ContaEmpresa = "43800-7";
                        item.CodigoBancoEmpresa = "341";
                    }
                }

                item.EmpresasPrestador = null;
            }
        }

        private void PopularDadosBancarios(ConciliacaoPagamentoDto x)
        {
            var dadosFinanceiroDto = new DadosFinanceiroDto();
            var empresaAtiva = x.EmpresasPrestador.Select(e => e.Empresa).FirstOrDefault(a => a.Ativo);
            if (empresaAtiva != null)
            {
                x.Empresa = empresaAtiva.RazaoSocial;
                x.NrCnpj = empresaAtiva.Cnpj;
                dadosFinanceiroDto = ObterDadosFinanceiroRMPrestador(empresaAtiva.Cnpj, x.IdEmpresaGrupo);
            }
            else
            {
                var ultimaEmpresa = x.EmpresasPrestador.Select(e => e.Empresa).OrderByDescending(o => o.Id).FirstOrDefault();
                if (ultimaEmpresa != null)
                {
                    x.Empresa = ultimaEmpresa.RazaoSocial;
                    x.NrCnpj = ultimaEmpresa.Cnpj;
                    dadosFinanceiroDto = ObterDadosFinanceiroRMPrestador(ultimaEmpresa.Cnpj, x.IdEmpresaGrupo);
                }
            }
            if (dadosFinanceiroDto != null)
            {
                x.Agencia = dadosFinanceiroDto.Agencia;
                x.Banco = dadosFinanceiroDto.Banco.ToString();
                x.Conta = dadosFinanceiroDto.NumeroConta;
            }
        }

        private void PopularContaCaixa(ConciliacaoPagamentoDto x)
        {
            x.ContaCaixa = ObterContaCaixaNoRm(x.IdEmpresaGrupo.Value, x.Banco);
        }

        private string TratarApostofre(string texto)
        {
            if (texto != null)
            {
                texto = texto.Replace("'", "''");
            }
            return texto;
        }

        public void AtualizarMigracaoPrestadorPessoa()
        {
            var prestadores = _prestadorRepository.Buscar(x => x.CodEacessoLegado.HasValue);
            var pessoas = _pessoaRepository.BuscarTodos();

            var count = 0;
            var connectionStringStfcorp = _connectionStrings.Value.DefaultConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringStfcorp))
            {
                dbConnection.Open();
                foreach (var prestador in prestadores)
                {
                    var pessoa = pessoas.Where(x => x.CodEacessoLegado == prestador.CodEacessoLegado.Value).FirstOrDefault();
                    if (pessoa != null)
                    {
                        var query = "update tblprestador set idPessoa = " + pessoa.Id + " where idprestador = " + prestador.Id;
                        dbConnection.Execute(query);
                    }
                    count++;
                }
                dbConnection.Close();
            }
        }


        public void AtualizarMigracaoSociosEmpresaPrestador()
        {
            var sociosEAcesso = ObterSociosEmpresaPrestadorEAcesso();
            var sociosStfCorp = _socioEmpresaPrestadorRepository.BuscarTodos();



            var sociosParaAdicionar = ObterSociosParaPersistir(sociosEAcesso, sociosStfCorp, false);
            var sociosParaAtualizar = ObterSociosParaPersistir(sociosEAcesso, sociosStfCorp, true);

            _socioEmpresaPrestadorRepository.AdicionarRange(sociosParaAdicionar);
            _socioEmpresaPrestadorRepository.UpdateRange(sociosParaAtualizar);

            _unitOfWork.Commit();
        }

        private List<SocioEmpresaPrestador> ObterSociosParaPersistir(
            IEnumerable<SocioEmpresaPrestadorEAcessoDto> sociosEacesso,
            IEnumerable<SocioEmpresaPrestador> sociosStfCorp,
            bool isUpdate)
        {
            return sociosEacesso.Where(sEacesso => !sociosStfCorp.Any(sStfCorp => sStfCorp?.IdEAcesso == sEacesso.Id))
                .Select(socioEacesso => new SocioEmpresaPrestador
                {
                    Id = isUpdate ? _socioEmpresaPrestadorRepository.BuscarPorIdEAcesso(socioEacesso.Id).Id : 0,
                    NomeSocio = socioEacesso.NomeSocio,
                    CpfSocio = socioEacesso.CpfSocio,
                    RgSocio = socioEacesso.RgSocio,
                    Profissao = socioEacesso.Profissao,
                    TipoPessoa = socioEacesso.TipoPessoa,
                    IdNacionalidade = socioEacesso.Nacionalidade,
                    IdEstadoCivil = socioEacesso.EstadoCivil,
                    Participacao = socioEacesso.Participacao,
                    IdEAcesso = socioEacesso.Id,
                    IdEmpresa = _empresaRepository.BuscarPorCnpj(socioEacesso.CnpjEmpresa)?.Id ?? null,
                    IdEmpresaEacesso = socioEacesso.IdEmpresaEacesso,
                    Usuario = "STFCORP",
                    DataAlteracao = DateTime.Now
                }).ToList();
        }


        private List<SocioEmpresaPrestadorEAcessoDto> ObterSociosEmpresaPrestadorEAcesso()
        {
            var connectionStringEAcesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEAcesso))
            {

                var query = @"SELECT Código as Id,
                            IdEmpresa as IdEmpresaEacesso,
                            NomeSocio,
                            CpfSocio, 
                            RgSocio,
                            Profissao,
                            Nacionalidade,
                            EstadoCivil,
                            Participacao, 
                            (SELECT DescricaoTipoPessoa from tblFornecedoresTipoPessoa WHERE IdTipoPessoa = TipoPessoa) as TipoPessoa,
                            (SELECT CNPJ from tblProfissionaisEmpresas WHERE Id = IdEmpresa) as CnpjEmpresa 
                            from tblProfissionaisEmprSocios pes";
                return dbConnection.Query<SocioEmpresaPrestadorEAcessoDto>(query).AsList();
            }
        }




        public void AtualizarMigracaoPrestadorInicioCLT()
        {
            var count = 0;
            List<Prestador> prestadores = new List<Prestador>();

            var connectionStringEAcesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEAcesso))
            {
                dbConnection.Open();
                prestadores = ObterPrestadores(dbConnection, null);
                dbConnection.Close();
            }

            var connectionStringStfcorp = _connectionStrings.Value.DefaultConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringStfcorp))
            {
                dbConnection.Open();
                foreach (var prestador in prestadores)
                {
                    var query = "";
                    if (prestador.DataInicioCLT.HasValue)
                    {
                        query = "update tblprestador set dtInicioCLT = " + prestador.DataInicioCLT.Value.ToString("yyyy/MM/dd") + " where cdeacessolegado = " + prestador.CodEacessoLegado;
                    }
                    else
                    {
                        query = "update tblprestador set dtInicioCLT = null where cdeacessolegado = " + prestador.CodEacessoLegado;
                    }
                    dbConnection.Execute(query);
                    count++;
                }
                dbConnection.Close();
            }
        }

        public void PersistirPrestadorObservacao(ObservacaoPrestador observacaoPrestador)
        {
            using (var connectionEacesso = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                connectionEacesso.Open();
                var tran = connectionEacesso.BeginTransaction();
                try
                {
                    if (observacaoPrestador.Id == 0)
                    {
                        _observacaoPrestadorRepository.Adicionar(observacaoPrestador);
                    }
                    else
                    {
                        var observacaoPrestadorBD = _observacaoPrestadorRepository.BuscarPorId(observacaoPrestador.Id);
                        observacaoPrestadorBD.IdTipoOcorencia = observacaoPrestador.IdTipoOcorencia;
                        observacaoPrestadorBD.Observacao = observacaoPrestador.Observacao;
                        observacaoPrestadorBD.Status = observacaoPrestador.Status;
                        _observacaoPrestadorRepository.Update(observacaoPrestadorBD);
                    }

                    InserirObsEacesso(observacaoPrestador, connectionEacesso, tran);

                    tran.Commit();
                    _unitOfWork.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                }
                finally
                {
                    connectionEacesso.Close();
                }
            }
        }

        private void InserirObsEacesso(ObservacaoPrestador observacaoPrestador, SqlConnection connectionEacesso,
            SqlTransaction tran)
        {
            var idProfissional = _prestadorRepository.BuscarPorId(observacaoPrestador.IdPrestador)
                .CodEacessoLegado;
            var valorDominio = _dominioRepository.BuscarPorId(observacaoPrestador.IdTipoOcorencia).IdValor;
            var query = $@"insert into tblProfissionaisObservacoes 
                                            (IdProfissional, DtObs, TipoObs, Obs, Fechada, Inativo)
	                                        values ({idProfissional}, 
                                                    GETDATE(), 
                                                    {valorDominio}, 
                                                    '{observacaoPrestador.Observacao}', 
                                                    {(observacaoPrestador.Status ? 1 : 0)}, 
                                                    0);";

            var command = new SqlCommand(query, connectionEacesso, tran);

            command.ExecuteNonQuery();
        }


        public void AtualizarMigracaoObservacaoPrestador(int? idProfissionalEacesso)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                List<ObservacaoPrestador> itensParaAdicionar = new List<ObservacaoPrestador>();
                var count = 0;
                var prestadores = _prestadorRepository.Buscar(x => x.CodEacessoLegado.HasValue).ToList();
                var ObservacoesPrestador = ObterProfissionaisObservacao(dbConnection, idProfissionalEacesso);
                var tiposOcorrencia = _dominioRepository.Buscar(x => x.ValorTipoDominio.Equals("OCORRENCIA_OBS")).ToList();
                foreach (var observacaoPrestador in ObservacoesPrestador)
                {
                    var prestador = prestadores.FirstOrDefault(x => x.CodEacessoLegado.Value == observacaoPrestador.IdProfissional);
                    if (prestador != null)
                    {
                        observacaoPrestador.IdPrestador = prestador.Id;
                        observacaoPrestador.Usuario = prestador.Usuario;
                        var tpOcorrencia = tiposOcorrencia.Find(x => x.IdValor == observacaoPrestador.IdTipoOcorencia);

                        if (tpOcorrencia != null)
                        {
                            observacaoPrestador.IdTipoOcorencia = tpOcorrencia.Id;

                            itensParaAdicionar.Add(observacaoPrestador);
                            count++;
                            if (count > 0)
                            {
                                _observacaoPrestadorRepository.AdicionarRange(itensParaAdicionar);
                                _unitOfWork.Commit();
                                count = 0;
                                itensParaAdicionar = new List<ObservacaoPrestador>();
                            }
                        }
                    }
                }
                if (count > 0)
                {
                    _observacaoPrestadorRepository.AdicionarRange(itensParaAdicionar);
                    _unitOfWork.Commit();
                    count = 0;
                    itensParaAdicionar = new List<ObservacaoPrestador>();
                }
            }
        }

        public void FazerUploadContratoPrestadorParaOMinIO(string nomeAnexo, string caminhoContrato, string arquivoBase64)
        {
            _contratoPrestadorService.UploadContratoParaMinIO(nomeAnexo,
                caminhoContrato,
                arquivoBase64);
        }


        public void FazerUploadExtensaoContratoPrestadorParaOMinIO(string nomeAnexo, string caminhoContrato, string arquivoBase64)
        {
            _extensaoContratoPrestadorService.UploadExtensaoContratoParaMinIO(
                nomeAnexo,
                caminhoContrato,
                arquivoBase64);
        }

        public void FazerUploadDocumentoPrestadorParaOMinIO(string nomeAnexo, string caminhoDocumento, string arquivoBase64)
        {
            _documentoPrestadorService.UploadDocumentoParaMinIO(
                nomeAnexo,
                caminhoDocumento,
                 arquivoBase64);

        }

        private static List<ObservacaoPrestador> ObterProfissionaisObservacao(IDbConnection dbConnection, int? idProfissionalEacesso)
        {
            dbConnection.Open();
            string sQuery = @"SELECT [idprofissional] as IdProfissional
                                    ,[DtObs] as DataAlteracao 
                                    ,[TipoObs] as IdTipoOcorencia
                                    ,[Obs] as Observacao
                                    ,[Fechada] as Status
                            FROM [stfcorp].[tblProfissionaisObservacoes]
                            WHERE IdProfissional IN (
                                        SELECT pro.[Id] FROM stfcorp.tblCelulas cel  
                                            INNER JOIN stfcorp.tblProfissionais pro ON cel.IdCelula = pro.Celula  
                                            INNER JOIN stfcorp.tblProfissionaisTiposContratos contr ON contr.Id = pro.IdTipoContrato AND contr.AgrupProf IN('LTDA')  
                                            INNER JOIN stfcorp.tblCargos ON pro.IdCargo = tblCargos.id  
                                            INNER JOIN stfcorp.tblGrupos ON cel.IdGrupo = tblGrupos.IdGrupo  
                                                LEFT JOIN stfcorp.tblProfissionaisClientes PCL ON PCL.IdProfissional = PRO.ID AND PCL.Inativo = 0  
                                                LEFT JOIN stfcorp.tblProfissionaisSistemasPagto sispag ON pro.id = sispag.idprofissional  
                                                LEFT JOIN stfcorp.tblEmpresasGrupo empGrupo ON pro.CodEmprGrupo = empGrupo.IdEmpresa  
                                                LEFT JOIN stfcorp.tblProfissionaisContratos proCont ON pro.id = proCont.idProfissional 
                                            WHERE (proCont.idContrato = (select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional))  
                                                or (isnull((select max(idContrato) from stfcorp.tblProfissionaisContratos sProCont where (pro.id = sProCont.idProfissional)),0) = 0) )  
                                                and ISNULL(pro.Status, '') in ('', 'FE')";

            if (idProfissionalEacesso.HasValue)
            {
                sQuery += "AND pro.Id = " + idProfissionalEacesso.Value + ") and (inativo = 0 or inativo is null)";
            }
            else
            {
                sQuery += ") and (inativo = 0 or inativo is null)";
            }

            var observacaoPrestador = dbConnection.Query<ObservacaoPrestador>(sQuery).AsList();
            dbConnection.Close();
            return observacaoPrestador;
        }

        /// <summary>
        /// Obtem os nomes do Cliente e Serviço do Eacesso
        /// </summary>
        /// <param name="listaServicos"></param>
        /// <returns>Ids dos Serviços</returns>
        public IEnumerable<ClienteServicoEacesso> ObterClienteServicoEAcesso(List<string> listaServicos)
        {
            List<ClienteServicoEacesso> listaClientesServicos = new List<ClienteServicoEacesso>();
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            var servico = "";

            foreach (var id in listaServicos)
            {
                servico += servico == string.Empty ? id : "," + id;
            }

            if (!string.IsNullOrEmpty(servico))
            {
                using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
                {
                    dbConnection.Open();
                    var query = @"  SELECT 
                                ( 
	                                CASE CS.SiglaTipoServico
	                                WHEN 'ACA' THEN 'Admin.' 
	                                WHEN 'ACC' THEN 'Comercial'
	                                ELSE 'Custo'
	                                END
                                )	AS Custo, CS.IdCelula, CL.IdDiretoria, D.Diretor, CS.IdCliente, 
                                CS.IdServico, C.NomeFantasia as Cliente, CS.Nome as Servico
                                FROM stfcorp.tblClientesServicos CS 
                                INNER JOIN stfcorp.tblClientes C ON C.IdCliente = CS.IdCliente
                                INNER JOIN STFCORP.tblCelulas CL ON CL.IdCelula = CS.IdCelula
								LEFT JOIN STFCORP.tblDiretorias D ON D.IdDiretoria = CL.IdDiretoria
                                WHERE CS.IdServico IN(" + servico + ")";
                    listaClientesServicos = dbConnection.Query<ClienteServicoEacesso>(query).ToList();
                    dbConnection.Close();
                }
            }
            return listaClientesServicos;
        }
        private void RemoverRegistroDeNfSeArquivoAindaNaoTiverSidoEnviado(HorasMesPrestador horasMesPrestador)
        {
            var listaNfsDasHoraMesPrestador = _prestadorEnvioNfRepository.BuscarPorIdHorasMesPrestador(horasMesPrestador.Id);
            var notaFiscalAindaNaoFoiEnviada = listaNfsDasHoraMesPrestador.Any(x => x.CaminhoNf == null);
            if (notaFiscalAindaNaoFoiEnviada)
            {
                _prestadorEnvioNfRepository.Remove(listaNfsDasHoraMesPrestador.FirstOrDefault(x => x.CaminhoNf == null));
            }
        }

        public IEnumerable<DiretoriaPrestadorDto> ObterDiretoriaEAcesso(List<string> listaCelulas)
        {
            List<DiretoriaPrestadorDto> listaDiretorias = new List<DiretoriaPrestadorDto>();
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            var celulas = "";

            foreach (var id in listaCelulas)
            {
                celulas += celulas == string.Empty ? id : "," + id;
            }

            if (!string.IsNullOrEmpty(celulas))
            {
                using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
                {
                    dbConnection.Open();
                    var query = @"  SELECT 
	                                CL.IdCelula, CL.IdDiretoria, D.Diretor
                                FROM STFCORP.tblCelulas CL
								LEFT JOIN STFCORP.tblDiretorias D ON D.IdDiretoria = CL.IdDiretoria
                                WHERE CL.IdCelula IN(" + celulas + ")";
                    listaDiretorias = dbConnection.Query<DiretoriaPrestadorDto>(query).ToList();
                    dbConnection.Close();
                }
            }
            return listaDiretorias;
        }

        public void RealizarMigracaoCltPj(int idProfissionalEacesso)
        {
            RealizarMigracao(idProfissionalEacesso);
            AtualizarMigracao(idProfissionalEacesso);
            AtualizarMigracaoInativacaoPrestador(idProfissionalEacesso);
            AtualizarMigracaoClienteServico(idProfissionalEacesso);
            AtualizarMigracaoRemuneracaoPrestador(idProfissionalEacesso);
            AtualizarMigracaoBeneficio(idProfissionalEacesso);
            AtualizarMigracaoObservacaoPrestador(idProfissionalEacesso);
        }

        private PrestadorDto ObterEmailsProfissionalEacesso(IDbConnection dbConnection, int idProfissional)
        {
            string sQuery = @"SELECT Id, Nome, Email, Email_Int AS EmailInterno 
                              FROM stfcorp.tblprofissionais pro WHERE pro.id = " + idProfissional;
            return dbConnection.Query<PrestadorDto>(sQuery).FirstOrDefault();
        }
    }
}