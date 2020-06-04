using Cliente.Domain.ClienteRoot.Dto;
using Cliente.Domain.ClienteRoot.Entity;
using Cliente.Domain.ClienteRoot.Repository;
using Cliente.Domain.ClienteRoot.Service.Interfaces;
using Cliente.Domain.Core.Notifications;
using Cliente.Domain.Interfaces;
using Dapper;
using Logger.Repository.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cliente.Domain.SharedRoot;
using Utils;
using Utils.Base;
using Utils.Connections;

namespace Cliente.Domain.ClienteRoot.Service
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly NotificationHandler _notificationHandler;
        private readonly ILogGenericoRepository _logGenericoRepository;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IVariablesToken _variables;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public ClienteService(IClienteRepository clienteRepository,
                              NotificationHandler notificationHandler,
                              ILogGenericoRepository logGenericoRepository,
                              IUnitOfWork unitOfWork,
                              IOptions<ConnectionStrings> connectionStrings,
                              IVariablesToken variables)
        {
            _clienteRepository = clienteRepository;
            _notificationHandler = notificationHandler;
            _logGenericoRepository = logGenericoRepository;
            _connectionStrings = connectionStrings;
            _unitOfWork = unitOfWork;
            _variables = variables;
        }

        public ClienteET ObterPorId(int idCliente)
        {
            var result = _clienteRepository.BuscarPorId(idCliente);
            return result;
        }

        public int? ObterClientePorIdSalesForce(string idSalesForce)
        {
            var result = _clienteRepository.ObterClientePorIdSalesForce(idSalesForce);
            return result;
        }

        public List<ComboDefaultDto> ObterClienteAtivoPorIdCelulaEAcesso(int idCelula)
        {
            List<ComboDefaultDto> clientes = new List<ComboDefaultDto>();
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                var query = "select DISTINCT c.idCliente as id, c.nomeFantasia as descricao" +
                    " from stfcorp.tblclientes c, stfcorp.tblClientesServicos s " +
                    "where c.idCliente = s.idCliente and DtInativacao is Null and idCelula=" + idCelula + "ORDER BY descricao;";
                clientes = dbConnection.Query<ComboDefaultDto>(query).ToList();
                dbConnection.Close();
            }
            return clientes;
        }

        public List<ComboDefaultDto> ObterTodosClientePorIdCelulaEAcesso(int idCelula)
        {
            List<ComboDefaultDto> clientes = new List<ComboDefaultDto>();
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                var query = "select DISTINCT c.idCliente as id, c.nomeFantasia as descricao from stfcorp.tblclientes c, stfcorp.tblClientesServicos s where c.idCliente = s.idCliente and idCelula=" + idCelula + "ORDER BY descricao;";
                clientes = dbConnection.Query<ComboDefaultDto>(query).ToList();
                dbConnection.Close();
            }
            return clientes;
        }

        public List<ComboLocalDto> ObterLocalTrabalhoIdClienteEAcesso(int idCliente)
        {
            List<ComboLocalDto> locais = new List<ComboLocalDto>();
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();

                var query = "select DISTINCT l.IdLocal as id, l.Nome as descricao, lo.Logradouro as tipo,l.Endereco as endereco, l.Numero as numero, " +
                  "l.CompEndereco as complemento, l.Bairro as bairro, l.cep as cep, cid.Cidade as cidade, e.Estado estado, p.Pais pais " +
                  "from stfcorp.TblClientesLocaisTrabalho l, stfcorp.tblCidades cid, stfcorp.tblClientes c, stfcorp.tblEstados e, stfcorp.tblpaises p, stfcorp.tbllogradouros lo " +
                  "where l.Inativo = 0 and cid.IdCidade = l.IdCidade and cid.SiglaEstado = e.SiglaEstado and cid.SiglaPais = p.SiglaPais and l.abrevlogradouro = lo.abrevlogradouro and " +
                  "l.IdCliente = " + idCliente;
                locais = dbConnection.Query<ComboLocalDto>(query).ToList();
                dbConnection.Close();
            }
            return locais;
        }

        public int PersistirCliente(ClienteET cliente)
        {
            if (string.IsNullOrEmpty(_variables.UserName))
            {
                _variables.UserName = "salesforce";
            }
            cliente.Usuario = _variables.UserName;
            _clienteRepository.Adicionar(cliente);
            _unitOfWork.Commit();
            return cliente.Id;
        }

        public int VerificarIdCliente()
        {
            var result = _clienteRepository.VerificarIdCliente() + 1 + 20000;
            return result;
        }

        public async Task<bool> ValidarCNPJ(string cnpj, string pais)
        {
            var cnpjValido = false;
            if (pais == "BRASIL" || pais == "BRAZIL")
            {
                cnpjValido = await ValidarCNPJ(cnpj);
            }
            else
            {
                cnpjValido = true;
            }

            return cnpjValido;
        }

        //private async Task<bool> VerificarExistenciaCliente(ClienteET cliente)
        //{
        //    var inserirCliente = await _clienteRepository.BuscarClientesPorCnpj(cliente.NrCnpj) == 0;
        //    if (inserirCliente && cliente.IdSalesforce != null)
        //    {
        //        inserirCliente = !await _clienteRepository.BuscarClientesPorIdSF(cliente.IdSalesforce);
        //    }

        //    return inserirCliente;
        //}

        private async Task<bool> ValidarCNPJ(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;
            int resto;
            string digito;
            string tempCnpj;

            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            if (cnpj.Length != 14)
                return false;

            tempCnpj = cnpj.Substring(0, 12);
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }

        public string ApenasNumeros(string str)
        {
            var apenasDigitos = new Regex(@"[^\d]");
            return apenasDigitos.Replace(str, "");
        }

        public FiltroGenericoDto<ClienteET> Filtrar(FiltroGenericoDto<ClienteET> filtro)
        {
            var result = _clienteRepository.Filtrar(filtro);
            return result;
        }

        public IEnumerable<ClienteET> ObterTodos()
        {
            return _clienteRepository.BuscarTodosReadOnly();
        }

        public FiltroGenericoDto<ClienteEacessoDto> ObterClientesEacesso(FiltroGenericoDto<ClienteEacessoDto> filtro)
        {
            filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                var query = @"SELECT DISTINCT IdCliente, NomeFantasia as NmFantasia, RazaoSocial, Cnpj FROM stfcorp.tblclientes cli;";
                var dados = dbConnection.Query<ClienteEacessoDto>(query).AsQueryable();

                if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
                {
                    dados = dados.Where(x =>
                        x.IdCliente.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                        (x.NmFantasia != null ? x.NmFantasia.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : false) ||
                        (x.RazaoSocial != null ? x.RazaoSocial.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : false) ||
                        (x.Cnpj != null ? x.Cnpj.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : false));
                }

                filtro.Total = dados.Count();
                filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);

                if (filtro.OrdemOrdenacao == "asc")
                    dados = dados.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
                else if (filtro.OrdemOrdenacao == "desc")
                    dados = dados.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
                else
                    dados = dados.OrderBy(x => x.NmFantasia).ThenBy(x => x.RazaoSocial);

                filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            }
            return filtro;
        }

        public ClienteEacessoDto ObterClienteEacesso(int idCliente)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            ClienteEacessoDto cliente = null;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                var query = $@"SELECT cli.IdCliente AS IdCliente, cli.nomeFantasia AS NmFantasia, cli.RazaoSocial AS RazaoSocial,
                                      cli.endereco AS Endereco, cli.cep AS Cep, cli.Numero AS Numero, 
                                      cli.CompEndereco AS Complemento, cli.Bairro AS Bairro, cli.InscrEstadual AS InscricaoEstadual, 
                                      cli.InscrMunicipal AS InscricaoMunicipal, 
                                      cli.cnpj as cnpj, cli.tipoClienteRM as tipoClienteRM,
                                      cli.Telefone AS Telefone,    
                                         cliCi.Cidade as Cidade,                                       
                                             cliEs.Estado as Estado,
                                                 cliLogra.Logradouro AS Tipo,   
                                                    cliSetorEcon.DescSetorEconomico AS SetorEconomico,
                                                        cliRamoAtv.DescRamoAtividade AS RamoAtividade,
                                                            cliCla.DescClassificacao AS Classificacao,  
                                                                co.IdCliente as IdCliente, co.IdContato as idContato, co.Nome as NomeContato, 
                                                                coTipoCtato.DescTipoContato as TipoContato, co.Inativo as Inativo,
                                                                     trab.IdCliente as IdCliente, trab.idLocal as IdLocal, 
                                                                     trab.IdLocalTrab as IdLocalTrabalho, trab.Nome as NomeTrabalho, 
                                                                     trab.Bairro as bairroTrabalho, trab.compEndereco as complementoEnderecoTrabalho,
                                                                         trabCi.SiglaEstado as siglaEstadoTrabalho, trabCi.Cidade as cidadeTrabalho, 
                                                                         trab.Inativo as Inativo,
                                                                                pa.Pais as paisTrabalho                                                                     
									 FROM stfcorp.tblclientes cli 
                                     LEFT JOIN stfcorp.tblClientesContatos co on cli.IdCliente = co.IdCliente 
									 LEFT JOIN stfcorp.TblClientesLocaisTrabalho trab on cli.IdCliente = trab.IdCliente
									 LEFT JOIN stfcorp.tblCidades trabCi on trab.IdCidade = trabCi.IdCidade 
									 LEFT JOIN stfcorp.tblCidades cliCi on cli.IdCidade = cliCi.IdCidade 
									 LEFT JOIN stfcorp.tblEstados cliEs on cliCi.SiglaEstado = cliEs.SiglaEstado 
                                     LEFT JOIN stfcorp.tblEstados trabEs on trabCi.SiglaEstado = trabEs.SiglaEstado     
									 LEFT JOIN stfcorp.tblPaises pa on trabCi.SiglaPais = pa.SiglaPais 
                                     LEFT JOIN stfcorp.tblLogradouros cliLogra on cli.AbrevLogradouro = cliLogra.AbrevLogradouro   
                                     LEFT JOIN stfcorp.tblSetoresEconomicos cliSetorEcon on cli.SiglaSetorEconomico = cliSetorEcon.SiglaSetorEconomico   
                                     LEFT JOIN stfcorp.tblRamosAtividades cliRamoAtv on cli.SiglaRamoAtividade = cliRamoAtv.SiglaRamoAtividade 
                                     LEFT JOIN stfcorp.tblTiposContatos coTipoCtato on co.SiglaTipoContato = coTipoCtato.SiglaTipoContato 
                                     LEFT JOIN stfcorp.tblClientesClassificacoes cliCla on cli.SiglaClassificacao = cliCla.SiglaClassificacao 
                                     WHERE cli.IdCliente = {idCliente};";

                var orderDictionary = new Dictionary<int, ClienteEacessoDto>();
                cliente = dbConnection.Query<ClienteEacessoDto, ContatoClienteEacessoDto, ClienteLocalTrabalhoEacessoDto, ClienteEacessoDto>
                    (query, (cli, contato, localTrabalho) =>
                    {
                        ClienteEacessoDto cliEntry;
                        if (!orderDictionary.TryGetValue(cli.IdCliente, out cliEntry))
                        {
                            cliEntry = cli;
                            orderDictionary.Add(cliEntry.IdCliente, cliEntry);
                        }

                        if (contato != null && !cliEntry.Contatos.Any(x => x.IdContato == contato.IdContato))
                            cliEntry.Contatos.Add(contato);

                        if (localTrabalho != null && !cliEntry.LocaisTrabalho.Any(x => x.IdLocalTrabalho == localTrabalho.IdLocalTrabalho))
                            cliEntry.LocaisTrabalho.Add(localTrabalho);

                        return cliEntry;

                    }, splitOn: "IdCliente").FirstOrDefault();
            }
            cliente.Contatos = cliente.Contatos.OrderBy(x => x.Inativo).ThenBy(x => x.TipoContato).ThenBy(x => x.NomeContato).ToList();
            cliente.LocaisTrabalho = cliente.LocaisTrabalho.OrderBy(x => x.Inativo).ThenBy(x => x.IdLocalTrabalho).ToList();
            return cliente;
        }

        public ClienteLocalTrabalhoEacessoDto ObterLocalTrabalhoEacesso(int idLocalTrabalho)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            ClienteLocalTrabalhoEacessoDto localTrabalho = null;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                var query = $@"SELECT trab.IdLocalTrab as IdLocalTrabalho, trab.idLocal as IdLocal, trab.Nome as NomeTrabalho, 
                                       trab.endereco as EnderecoTrabalho,
                                      trab.cep as cepTrabalho, trab.numero as numeroTrabalho, trab.Bairro as bairroTrabalho,
                                      trab.compEndereco as complementoEnderecoTrabalho,
                                        trabCi.Cidade as cidadeTrabalho,
                                            trabEs.Estado as EstadoTrabalho,
                                                trabLogra.Logradouro as abreviaturaLogradouroTrabalho 
									  FROM stfcorp.tblClientesLocaisTrabalho trab       									 
									  LEFT JOIN stfcorp.tblCidades trabCi on trab.IdCidade = trabCi.IdCidade 
                                      LEFT JOIN stfcorp.tblEstados trabEs on trabCi.SiglaEstado = trabEs.SiglaEstado 						  
                                      LEFT JOIN stfcorp.tblLogradouros trabLogra on trab.AbrevLogradouro = trabLogra.AbrevLogradouro  
                                      WHERE trab.IdLocalTrab = {idLocalTrabalho};";

                localTrabalho = dbConnection.Query<ClienteLocalTrabalhoEacessoDto>(query)
                    .FirstOrDefault();

            }
            return localTrabalho;
        }

        public ContatoClienteEacessoDto ObterContatoClienteEacesso(int idContato)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            ContatoClienteEacessoDto contato = null;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                var query = $@"SELECT co.IdContato, co.IdCliente, co.Nome as nomeContato, 
                                      co.Cargo as cargoContato,  co.endereco as enderecoContato,
                                      co.cep as cepContato, co.numero as numeroEnderecoContato, co.compEndereco as complementoEnderecoContato, 
                                      co.bairro as BairroContato, co.obs as observacaoContato, co.email as emailContato, co.Departamento as departamentoContato,
                                          coTipoCtato.DescTipoContato as TipoContato,
                                              coCi.cidade as cidadeContato,
                                                  coEs.estado as estadoContato,
                                                    coLogrd.Logradouro as abreviaturaLogradouroContato,
                                                        coTel.IdContato as IdContato, coTel.NumTelefone as NumTelefone
									  FROM stfcorp.tblClientesContatos co                                  									 
									  LEFT JOIN stfcorp.tblCidades coCi on co.IdCidade = coCi.IdCidade  
                                      LEFT JOIN stfcorp.tblEstados coEs on coCi.SiglaEstado = coEs.SiglaEstado 
                                      LEFT JOIN stfcorp.tblTiposContatos coTipoCtato on co.SiglaTipoContato = coTipoCtato.SiglaTipoContato 
                                      LEFT JOIN stfcorp.tblLogradouros coLogrd on co.AbrevLogradouro = coLogrd.AbrevLogradouro   
                                      LEFT JOIN stfcorp.tblClientesContatosTelefones coTel on co.IdContato = coTel.IdContato
                                      WHERE co.IdContato = {idContato};";

                var orderDictionary = new Dictionary<int, ContatoClienteEacessoDto>();
                contato = dbConnection.Query<ContatoClienteEacessoDto, TelefoneContatoClienteDto, ContatoClienteEacessoDto>
                    (query, (co, tel) =>
                    {
                        ContatoClienteEacessoDto coEntry;
                        if (!orderDictionary.TryGetValue(co.IdContato, out coEntry))
                        {
                            coEntry = co;
                            orderDictionary.Add(coEntry.IdContato, coEntry);
                        }

                        if (tel != null && !coEntry.Telefones.Any())
                            coEntry.Telefones.Add(tel);

                        return coEntry;

                    }, splitOn: "IdContato").FirstOrDefault();

                //contato = dbConnection.Query<ContatoClienteEacessoDto>(query).FirstOrDefault();
            }
            return contato;
        }

        public async Task<IEnumerable<MultiselectDto>> ObterClientesPorIds(List<int> ids)
        {
            var tasks = new List<Task<MultiselectDto>>();
            foreach (var id in ids)
            {
                tasks.Add(_clienteRepository.PopularComboClienteRepasse(id));
            }

            var results = await Task.WhenAll(tasks.ToArray());

            return results;
        }

        public IEnumerable<ClienteET> ObterSomenteAtivos()
        {
            return _clienteRepository.BuscarReadOnly(x => x.FlStatus == "A");
        }

        public IEnumerable<ClienteET> ObterSomenteInativos()
        {
            return _clienteRepository.Buscar(x => x.FlStatus == "I");
        }

        public bool VerificarExistenciaCliente(int idCliente)
        {
            var result = _clienteRepository.BuscarPorId(idCliente);
            return result != null;
        }

        public List<ClienteET> ObterClientesPorNome(string nome)
        {
            return _clienteRepository.Buscar(x => x.NmFantasia.Contains(nome) || x.NmRazaoSocial.Contains(nome)).ToList();
        }

        public ClienteET ObterClientePorIdEacesso(int idEAcesso)
        {
            var result = _clienteRepository.Buscar(x => x.IdEAcesso == idEAcesso).FirstOrDefault();
            return result;
        }

        public List<ClienteEacessoDto> ObterClientesEacessoPorIdCelula(int? id, bool isInativo)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                var query = $@"SELECT DISTINCT cli.IdCliente, cli.NomeFantasia as NmFantasia
                                      FROM stfcorp.tblClientesServicos cs                                									 
									  LEFT JOIN stfcorp.tblClientes cli on cs.IdCliente = cli.IdCliente                               
                                      WHERE cs.IdCelula = {id}";

                if (!isInativo)
                    query += " AND cs.in_inativo is null";

                var clientes = dbConnection.Query<ClienteEacessoDto>(query).ToList();
                return clientes;
            }
        }

        public List<ClienteEacessoDto> ObterClientesEacessoPorIdCelulaEvisualizacaoServico(int? id, bool isInativo, string login)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                var query = $@"SELECT DISTINCT cli.IdCliente, cli.NomeFantasia as NmFantasia
                                      FROM stfcorp.tblClientesServicos cs                                									 
									  LEFT JOIN stfcorp.tblClientes cli on cs.IdCliente = cli.IdCliente                               
                                      WHERE cs.IdCelula = {id} 
                                      AND CS.IdServico IN (SELECT IDSERVICO FROM [stfcorp].[VisualizarServicoTable]('{login}'))";

                if (!isInativo)
                    query += " AND cs.in_inativo is null";

                var clientes = dbConnection.Query<ClienteEacessoDto>(query).ToList();
                return clientes;
            }
        }
    }
}
