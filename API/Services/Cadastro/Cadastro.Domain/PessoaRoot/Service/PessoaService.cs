using Cadastro.Domain.CidadeRoot.Dto;
using Cadastro.Domain.CidadeRoot.Repository;
using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.DominioRoot.Repository;
using Cadastro.Domain.EmpresaGrupoRoot.Repository;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.NatcorpRoot.Dto;
using Cadastro.Domain.NatcorpRoot.Repository;
using Cadastro.Domain.PessoaRoot.Dto;
using Cadastro.Domain.PessoaRoot.Entity;
using Cadastro.Domain.PessoaRoot.Repository;
using Cadastro.Domain.PessoaRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Cadastro.Domain.TelefoneRoot.Entity;
using Cadastro.Domain.TelefoneRoot.Repository;
using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Utils.Base;
using Utils.Connections;
using Utils.Extensions;

namespace Cadastro.Domain.PessoaRoot.Service
{
    public class PessoaService : IPessoaService
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IDominioRepository _dominioRepository;
        private readonly IPaisRepository _paisRepository;
        private readonly ITelefoneRepository _telefoneRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICidadeRepository _cidadeRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IProfissionalNatcorpRepository _profissionalNatcorpRepository;
        private readonly IPessoaRepository _pessoaRepository;

        public PessoaService(
            IOptions<ConnectionStrings> connectionStrings,
            MicroServicosUrls microServicosUrls,
            IDominioRepository dominioRepository,
            IPaisRepository paisRepository,
            ITelefoneRepository telefoneRepository,
            IUnitOfWork unitOfWork,
            ICidadeRepository cidadeRepository,
            IEnderecoRepository enderecoRepository,
            IProfissionalNatcorpRepository profissionalNatcorpRepository,
            IPessoaRepository pessoaRepository
            )
        {
            _connectionStrings = connectionStrings;
            _microServicosUrls = microServicosUrls;
            _dominioRepository = dominioRepository;
            _paisRepository = paisRepository;
            _telefoneRepository = telefoneRepository;
            _unitOfWork = unitOfWork;
            _cidadeRepository = cidadeRepository;
            _enderecoRepository = enderecoRepository;
            _profissionalNatcorpRepository = profissionalNatcorpRepository;
            _pessoaRepository = pessoaRepository;
        }

        public void AtualizarMigracao()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(120),
                BaseAddress = new Uri(_microServicosUrls.UrlApiServico)
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync("api/Pessoa/atualizar-migracao").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
        }


        public Pessoa BuscarPorEmailInterno(string email)
        {
            var pessoa = _pessoaRepository.BuscarPrimeiro(x => x.EmailInterno == email);
            return pessoa;
        }

        public Pessoa BuscarPorIdEAcesso(int idEAcesso)
        {
            var pessoa = _pessoaRepository.BuscarPrimeiro(x => x.CodEacessoLegado == idEAcesso);
            return pessoa;
        }

        public int? ObterIdPessoa(int? idEacesso)
        {
            var idPessoa = _pessoaRepository.ObterIdPessoa(idEacesso);
            return idPessoa;
        }

        public UsuarioAdDto ObterUsuarioAd(string login)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var result = client.GetAsync($"{_microServicosUrls.UrlApiSeguranca}/api/authentication/FiltrarUsuariosADPorLogin/{login}").Result;
                    var content = result.Content.ReadAsStringAsync().Result;
                    var usuario = JsonConvert.DeserializeObject<UsuarioAdDto>(content);
                    return usuario;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void RealizarMigracaoProfissionaisNatcorp(DateTime? aPartirDe)
        {
            var dominiosNacionalidade = _dominioRepository.Buscar(z => z.ValorTipoDominio == SharedEnuns.Nacionalidade.VL_TIPO_DOMINIO.GetDescription()).ToList();
            var cidades = _cidadeRepository.BuscarTodos().ToList();
            var paises = _paisRepository.BuscarTodos().ToList();
            var dominiosEstadoCivil = _dominioRepository.Buscar(z => z.ValorTipoDominio == SharedEnuns.EstadoCivil.VL_TIPO_DOMINIO.GetDescription()).ToList();
            var dominiosSexo = _dominioRepository.Buscar(z => z.ValorTipoDominio == SharedEnuns.Sexo.VL_TIPO_DOMINIO.GetDescription()).ToList();
            var todosCPFsStfcorp = _pessoaRepository.ObterTodosCPFs();

            var profissionaisNatcorp = _profissionalNatcorpRepository.BuscarProfissionaisNatcorpParaMigracao(aPartirDe).ToList();
            var profNatcorpParaAdicionar = profissionaisNatcorp.Where(x => !todosCPFsStfcorp.Any(cpf => cpf == x.CPF)).ToList();
            var profNatcorpParaAtualizar = profissionaisNatcorp.Where(x => todosCPFsStfcorp.Any(cpf => cpf == x.CPF)).ToList();

            var profissionaisParaAdicionarStfcorp = MapearParaPessoasStfcorp(profNatcorpParaAdicionar, dominiosNacionalidade, cidades, paises, dominiosEstadoCivil, dominiosSexo, false);
            var pessoasparaAtualizarStfcorp = MapearParaPessoasStfcorp(profNatcorpParaAtualizar, dominiosNacionalidade, cidades, paises, dominiosEstadoCivil, dominiosSexo, true);

            MigrarPessoasParaStfcorp(profissionaisParaAdicionarStfcorp, pessoasparaAtualizarStfcorp);
        }

        private void MigrarPessoasParaStfcorp(List<Pessoa> profissionaisNatcorpParaAdicionar, List<Pessoa> profissionaisNatcorpParaEditar)
        {
            _pessoaRepository.AdicionarRange(profissionaisNatcorpParaAdicionar);

            foreach (var p in profissionaisNatcorpParaEditar)
            {
                _pessoaRepository.Update(p);

                if (p.Endereco != null && p.Telefone != null)
                    _enderecoRepository.Update(p.Endereco);

                if (p.Telefone != null)
                    _telefoneRepository.Update(p.Telefone);
            }

            _unitOfWork.Commit();

        }

        private List<Pessoa> MapearParaPessoasStfcorp(
            IEnumerable<ProfissionalNatcorpDto> profissionaisNatcorp,
            List<Dominio> dominiosNacionalidade,
            List<Cidade> cidades,
            List<Pais> paises,
            List<Dominio> dominiosEstadoCivil,
            List<Dominio> dominiosSexo,
            bool ehEdicao)
        {

            var listPessoasStfcorp = new List<Pessoa>();

            foreach (var p in profissionaisNatcorp)
            {
                var pessoa = ehEdicao ? _pessoaRepository.BuscarPessoaComIncludeEnderecoTelefone(p.CPF) : new Pessoa();
                var paisDoProfissional = paises.FirstOrDefault(y => y.SgPais == p.CodNacionalidade);

                pessoa.Matricula = p.MATRICULA;
                pessoa.Nome = p.Nome;
                pessoa.Email = p.EmailPessoal;
                //pessoa. CodEacessoLegado = TO_DO implementar job para update nisso                  
                pessoa.Cpf = p.CPF;
                pessoa.Rg = p.RG;
                pessoa.DtNascimento = p.DtNascimento;
                pessoa.NomeDoPai = p.NomePai;
                pessoa.NomeDaMae = p.NomeMae;
                pessoa.IdNacionalidade = (int?)dominiosNacionalidade.FirstOrDefault(z => z.IdValor == paisDoProfissional.Id)?.Id ?? null;
                pessoa.IdEstadoCivil = dominiosEstadoCivil.FirstOrDefault(z => z.IdValor == MapearEstadoCivilNatcorpParaIdDominioEstadoCivilStfcorp(p.CodEstadoCivil)).Id;
                pessoa.IdSexo = dominiosSexo.FirstOrDefault(z => z.IdValor == (int)ObterIdDominioSexo(p.Cod_Sexo)).Id;

                if (pessoa.Endereco != null)
                {
                    pessoa.Endereco = ehEdicao ? pessoa.Endereco : new Endereco();
                    pessoa.Endereco.IdCidade = cidades.FirstOrDefault(c => c.CodIBGE == p.EnderecoCodCidade)?.Id ?? null;
                    pessoa.Endereco.NomeCidade = cidades.FirstOrDefault(c => c.CodIBGE == p.EnderecoCodCidade)?.NmCidade ?? null;
                    pessoa.Endereco.NmEndereco = p.EnderecoLogradouro;
                    pessoa.Endereco.NrEndereco = p.EnderecoNumero;
                    pessoa.Endereco.NmBairro = p.EnderecoBairro;
                    pessoa.Endereco.NrCep = p.EnderecoCEP;
                    pessoa.Endereco.NmCompEndereco = p.EnderecoComplemento;
                }

                if (pessoa.Telefone != null)
                {
                    pessoa.Telefone = ehEdicao ? pessoa.Telefone : new Telefone();
                    pessoa.Telefone.Celular = p.TelefoneCelular;
                    pessoa.Telefone.NumeroResidencial = p.TelefoneResidencial;
                    pessoa.Telefone.DDD = p.TelefoneResidencialDDD;
                }

                listPessoasStfcorp.Add(pessoa);
            }
            return listPessoasStfcorp;
        }
       
        public Pessoa ObterPessoa(int id) => _pessoaRepository.BuscarPorId(id);

      

        private CidadeDto ObterCidadeEAcesso(string codigoIBGE)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                var query = $"SELECT IdCidade as Id, Cidade as NmCidade FROM tblCidades WHERE codigoIbge = @CodigoIBGE;";
                var result = dbConnection.Query<CidadeDto>(query, new { CodigoIBGE = codigoIBGE }).FirstOrDefault();
                return result;
            }
        }

        private string ObterSituacaoEAcesso(string codigoIBGE)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                var query = $"SELECT Cidade FROM tblCidades WHERE codigoIbge = @CodigoIBGE;";
                var result = dbConnection.Query<string>(query, new { CodigoIBGE = codigoIBGE }).FirstOrDefault()?.Trim()?.ToUpper();
                return result;
            }
        }

        private Pais ObterPaisDoProfissional(string siglaPais) => _paisRepository.BuscarPrimeiro(x => x.SgPais.Trim().ToUpper() == siglaPais.Trim().ToUpper());
        private int? MapearEstadoCivilNatcorpParaIdDominioEstadoCivilStfcorp(string siglaEstadoCivil)
        {
            switch (siglaEstadoCivil.Trim().ToUpper())
            {
                case "C": return (int)SharedEnuns.EstadoCivil.CASADO;
                case "S": return (int)SharedEnuns.EstadoCivil.SOLTEIRO;
                case "I": return (int)SharedEnuns.EstadoCivil.DIVORCIADO;
                case "V": return (int)SharedEnuns.EstadoCivil.VIUVO;
                case "O": return (int)SharedEnuns.EstadoCivil.AMASIADO;
                case "J": return (int)SharedEnuns.EstadoCivil.SEPARADO_JUDICIALMENTE;
                case "D": return (int)SharedEnuns.EstadoCivil.DESQUITADO;
                case "M": return (int)SharedEnuns.EstadoCivil.MARITAL;
                case "P": return (int)SharedEnuns.EstadoCivil.SEPARADO;
                case "E": return (int)SharedEnuns.EstadoCivil.UNIAO_ESTAVEL;
                default: return null;
            }
        }
        private Dominio ObterDominioNacionalidade(int idValorDominio)
        {
            return _dominioRepository.BuscarPrimeiro(x =>
                x.IdValor == idValorDominio
                && x.ValorTipoDominio.Trim().ToUpper() == SharedEnuns.Nacionalidade.VL_TIPO_DOMINIO.GetDescription()
            );
        }

        private SharedEnuns.Sexo ObterIdDominioSexo(string siglaSexo) => siglaSexo.Trim().ToUpper() == "M"
                ? SharedEnuns.Sexo.MASCULINO
                : SharedEnuns.Sexo.FEMININO;
    }
}
