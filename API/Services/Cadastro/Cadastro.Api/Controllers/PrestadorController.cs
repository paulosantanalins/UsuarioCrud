using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.DominioRoot.Service.Interfaces;
using Cadastro.Domain.EmpresaGrupoRoot.Service.Interfaces;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.PluginRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Cadastro.Domain.TelefoneRoot.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Utils;
using Utils.Base;
using Utils.Connections;
using Utils.Extensions;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PrestadorController : Controller
    {
        private readonly IPrestadorService _prestadorService;
        private readonly IPluginRMService _pluginRMService;
        private readonly IDominioService _dominioService;
        private readonly IHorasMesPrestadorService _horasMesPrestadorService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValorPrestadorService _valorPrestadorService;
        protected readonly IEmpresaGrupoService _empresaGrupoService;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IVariablesToken _variablesToken;

        public PrestadorController(
            IOptions<ConnectionStrings> connectionStrings,
            IPrestadorService prestadorService,
            IPluginRMService pluginRMService,
            IDominioService dominioService,
            IEmpresaGrupoService empresaGrupoService,
            IValorPrestadorService valorPrestadorService,
            IHorasMesPrestadorService horasMesPrestadorService,
            IUnitOfWork unitOfWork, IVariablesToken variablesToken)
        {
            _connectionStrings = connectionStrings;
            _prestadorService = prestadorService;
            _pluginRMService = pluginRMService;
            _dominioService = dominioService;
            _horasMesPrestadorService = horasMesPrestadorService;
            _unitOfWork = unitOfWork;
            _variablesToken = variablesToken;
            _valorPrestadorService = valorPrestadorService;
            _empresaGrupoService = empresaGrupoService;
        }

        [HttpPost("Filtrar")]
        public IActionResult Filtrar([FromBody] FiltroGenericoViewModelBase<PrestadorVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDtoBase<PrestadorDto>>(filtro);
            var resultBD = _prestadorService.Filtrar(filtroDto);
            var resultVM = Mapper.Map<FiltroGenericoViewModelBase<PrestadorDto>>(resultBD);
            return Ok(resultVM);
        }

        [HttpPost("Filtrar-Horas")]
        public IActionResult FiltrarHoras([FromBody] FiltroGenericoViewModelBase<PrestadorVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDtoBase<PrestadorDto>>(filtro);
            var resultBD = _prestadorService.FiltrarHoras(filtroDto);
            var resultVM = Mapper.Map<FiltroGenericoViewModelBase<PrestadorDto>>(resultBD);
            return Ok(resultVM);
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId([FromRoute] int id)
        {
            var resultBD = _prestadorService.BuscarPorId(id);

            foreach (var valorPrestador in resultBD.ValoresPrestador)
            {
                valorPrestador.PermiteExcluir = _valorPrestadorService.ValidaExcluir(valorPrestador);
            }

            var resultVM = Mapper.Map<PrestadorVM>(resultBD);

            resultVM.ObservacoesPrestador = Mapper.Map<List<ObservacaoPrestadorVM>>(resultBD.ObservacoesPrestador);

            resultVM.Empresas = resultVM.Empresas.OrderByDescending(x => x.Ativo).ThenBy(x => x.DataVigencia).ToList();
            resultVM.ContratosPrestador = resultVM.ContratosPrestador.OrderBy(x => x.Id).ToList();

            DadosFinanceiroDto dadosFinanceiroDto = ObterEmpresa(resultBD);
            if (dadosFinanceiroDto != null)
            {
                ObterDadosFinceriroRM(resultVM, dadosFinanceiroDto);
            }


            var tiposDeDocumentoPrestador = _dominioService.BuscarDominios(SharedEnuns.TipoDocumentoPrestador.VL_TIPO_DOMINIO.GetDescription());
            resultVM.DocumentosPrestador = resultVM.DocumentosPrestador.Select(x =>
                 new DocumentoPrestadorVM
                 {
                     Id = x.Id,
                     IdPrestador = x.IdPrestador,
                     CaminhoDocumento = x.CaminhoDocumento,
                     DescricaoTipoOutros = x.DescricaoTipoOutros,
                     NomeAnexo = x.NomeAnexo,
                     Tipo = tiposDeDocumentoPrestador?.FirstOrDefault(d => d.Id.Equals(x.IdTipoDocumentoPrestador))?.DescricaoValor ?? "",
                     IdTipoDocumentoPrestador = x.IdTipoDocumentoPrestador,
                     DataAlteracao = x.DataAlteracao,
                     Usuario = x.Usuario
                 }).ToList();



            return Ok(new { dados = resultVM, notifications = "", success = true });
        }

        private DadosFinanceiroDto ObterEmpresa(Prestador resultBD)
        {
            var empresaAtiva = resultBD.EmpresasPrestador.Select(x => x.Empresa).FirstOrDefault(x => x.Ativo);
            if (empresaAtiva != null)
            {
                return _prestadorService.ObterDadosFinanceiroRMPrestador(empresaAtiva.Cnpj, resultBD.IdEmpresaGrupo);
            }
            else
            {
                var ultimaEmpresa = resultBD.EmpresasPrestador.Select(x => x.Empresa).OrderByDescending(x => x.Id).FirstOrDefault();
                if (ultimaEmpresa != null)
                {
                    return _prestadorService.ObterDadosFinanceiroRMPrestador(ultimaEmpresa.Cnpj, resultBD.IdEmpresaGrupo);
                }
                else
                {
                    return null;
                }
            }
        }

        private void ObterDadosFinceriroRM(PrestadorVM resultVM, DadosFinanceiroDto dadosFinanceiroDto)
        {

            resultVM.DadosFinanceiro = new DadosFinanceiroVM
            {
                Agencia = dadosFinanceiroDto.Agencia,
                Banco = _prestadorService.ObterBancoNoRm(dadosFinanceiroDto.Banco),
                FormaPagamento = dadosFinanceiroDto.FormaPagamento,
                NumeroConta = dadosFinanceiroDto.NumeroConta,
                TipoConta = ObterTipoConta(dadosFinanceiroDto.TipoConta)
            };
            ObterDadosFinanceirosEmpresa(resultVM);
        }

        private void ObterDadosFinanceirosEmpresa(PrestadorVM resultVM)
        {
            if (!String.IsNullOrEmpty(resultVM.DadosFinanceiro.Banco))
            {
                var dadosBancariosDaEmpresaGrupoDto =
                   _empresaGrupoService.BuscarDadosBancariosDaEmpresaDoGrupoPorIdEmpresaGrupoEBanco(resultVM.IdEmpresaGrupo, resultVM.DadosFinanceiro.Banco);

                if (dadosBancariosDaEmpresaGrupoDto == null)
                {
                    resultVM.DadosFinanceiroEmpresa = new DadosFinanceiroEmpresaVM();
                }
                else
                {
                    resultVM.DadosFinanceiroEmpresa = Mapper.Map<DadosFinanceiroEmpresaVM>(dadosBancariosDaEmpresaGrupoDto);
                }
            }
        }

        private string ObterTipoConta(int tipoConta)
        {
            switch (tipoConta)
            {
                case 1:
                    return "CORRENTE - FISICA";
                case 2:
                    return "POUPANCA - FISICA";
                case 3:
                    return "CORRENTE - JURIDICA";
                case 4:
                    return "POUPANCA - JURIDICA";
                default:
                    return "";
            }
        }



        [HttpPost]
        public IActionResult Persistir([FromBody] PrestadorVM prestadorVM)
        {
            SqlConnection eacessoConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection);
            eacessoConnection.Open();
            SqlTransaction eacessoTran = eacessoConnection.BeginTransaction();
            var rmConnection = new SqlConnection(_connectionStrings.Value.RMIntegracaoConnection);
            rmConnection.Open();
            var rmTran = rmConnection.BeginTransaction();

            using (IDbContextTransaction tran = _unitOfWork.BeginTran())
            {
                try
                {
                    if (prestadorVM.Id == 0)
                    {
                        foreach (var contrato in prestadorVM.ContratosPrestador)
                        {
                            if (!string.IsNullOrEmpty(contrato.ArquivoBase64))
                            {
                                var tipoArquivo = contrato.NomeAnexo.Split('.')[contrato.NomeAnexo.Split('.').Length - 1];
                                contrato.CaminhoContrato = $"{Guid.NewGuid().ToString()}.{tipoArquivo}";
                                _prestadorService.FazerUploadContratoPrestadorParaOMinIO(contrato.NomeAnexo, contrato.CaminhoContrato, contrato.ArquivoBase64);

                                foreach (ExtensaoContratoPrestadorVM extensao in contrato.ExtensoesContratoPrestador)
                                {
                                    if (!string.IsNullOrEmpty(extensao.ArquivoBase64))
                                    {
                                        var extensaoTipoArquivo = extensao.NomeAnexo.Split('.')[extensao.NomeAnexo.Split('.').Length - 1];
                                        extensao.CaminhoContrato = $"{Guid.NewGuid().ToString()}.{extensaoTipoArquivo}";
                                        _prestadorService.FazerUploadExtensaoContratoPrestadorParaOMinIO(extensao.NomeAnexo, extensao.CaminhoContrato, extensao.ArquivoBase64);
                                    }
                                }
                            }
                        }

                        foreach (var documento in prestadorVM.DocumentosPrestador)
                        {
                            if (!string.IsNullOrEmpty(documento.ArquivoBase64))
                            {
                                var tipoArquivo = documento.NomeAnexo.Split('.')[documento.NomeAnexo.Split('.').Length - 1];
                                documento.CaminhoDocumento = $"{Guid.NewGuid().ToString()}.{tipoArquivo}";
                                _prestadorService.FazerUploadDocumentoPrestadorParaOMinIO(documento.NomeAnexo, documento.CaminhoDocumento, documento.ArquivoBase64);
                            }
                        }

                        var prestador = Mapper.Map<Prestador>(prestadorVM);
                        prestador.Pessoa.Usuario = _variablesToken.UserName ?? "STFCORP";
                        prestador.EmpresasPrestador = Mapper.Map<ICollection<EmpresaPrestador>>(prestadorVM.Empresas);
                        prestador.EmpresasPrestador.ToList().ForEach(x => x.Empresa.Usuario = _variablesToken.UsuarioToken);
                        prestador.Pessoa.Endereco = Mapper.Map<Endereco>(prestadorVM.Endereco);
                        prestador.Pessoa.Telefone = Mapper.Map<Telefone>(prestadorVM.Telefone);
                        prestador.ValoresPrestador.ToList().ForEach(x => x.Usuario = _variablesToken.UsuarioToken);


                        var valorPrestadoMaisRecente = prestador.ValoresPrestador
                            .ToList()
                            .OrderByDescending(x => x.DataAlteracao)
                            .FirstOrDefault();

                        if (valorPrestadoMaisRecente != null)
                            prestador.IdTipoRemuneracao = valorPrestadoMaisRecente.IdTipoRemuneracao;

                        var id = _prestadorService.Adicionar(prestador);
                        _pluginRMService.EnviarPrestadorRM(id, "I", true);
                        _prestadorService.AdicionarEAcesso(id, eacessoConnection, eacessoTran);


                    }
                    else
                    {
                        var prestador = Mapper.Map<Prestador>(prestadorVM);
                        prestador.Pessoa.Usuario = _variablesToken.UserName ?? "STFCORP";
                        prestador.EmpresasPrestador = Mapper.Map<ICollection<EmpresaPrestador>>(prestadorVM.Empresas);
                        prestador.EmpresasPrestador.ToList().ForEach(x => x.Empresa.Usuario = _variablesToken.UsuarioToken);
                        prestador.ValoresPrestador = Mapper.Map<ICollection<ValorPrestador>>(prestadorVM.ValoresPrestador);
                        prestador.ValoresPrestador.ToList().ForEach(x => x.Usuario = _variablesToken.UsuarioToken);
                        prestador.IdTipoRemuneracao = prestadorVM.ValoresPrestador?.ToList()?.OrderByDescending(x => x.DataReferencia)?.FirstOrDefault()?.IdTipoRemuneracao;
                        _prestadorService.AtualizarPrestador(prestador);

                        if (prestador.CodEacessoLegado != null)
                            _prestadorService.AtualizarEAcesso(prestador.Id, eacessoConnection, eacessoTran);

                        _pluginRMService.EnviarPrestadorRM(prestadorVM.Id, "A", false);
                    }
                    tran.Commit();
                    eacessoTran.Commit();
                    rmTran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    eacessoTran.Rollback();
                    rmTran.Rollback();
                    throw ex;
                }              
                return Ok(true);
            }

        }


        [HttpPost("dia-pagamento")]
        public IActionResult DiaPagamento([FromBody] PrestadorVM prestadorVM)
        {
            var prestador = _prestadorService.BuscarPorId(prestadorVM.Id);
            prestador.IdDiaPagamento = prestadorVM.IdDiaPagamento;

            _prestadorService.AtualizarPrestador(prestador);

            return Ok(true);
        }

        [HttpPost("enviar-horas")]
        public IActionResult EnviarHoras([FromBody] PrestadorHoraVM prestadorHoraVM)
        {
            var prestadorHoraBD = Mapper.Map<HorasMesPrestador>(prestadorHoraVM);
            _horasMesPrestadorService.SalvarHoras(prestadorHoraBD);
            return Ok(prestadorHoraBD.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_APROVADAS.GetDescription()));
        }

        [HttpPost("filtrar-aprovar-pagamento")]
        public IActionResult FiltrarAprovarPagamento([FromBody] FiltroGenericoViewModelBase<PrestadorVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDtoBase<AprovarPagamentoDto>>(filtro);
            var resultBD = _prestadorService.FiltrarAprovarPagamento(filtroDto);
            var resultVM = Mapper.Map<FiltroGenericoViewModelBase<AprovarPagamentoDto>>(resultBD);
            return Ok(resultVM);
        }

        [HttpPut("aprovar-pagamento")]
        public IActionResult AprovarPagamento([FromBody] AprovarPagamentoVM aprovarPagamento)
        {
            _horasMesPrestadorService.AprovarPagamento((int)aprovarPagamento.IdHoraMesPrestador);
            return Ok(true);
        }

        [HttpPut("negar-aprovar-pagamento")]
        public IActionResult NegarAprovarPagamento([FromBody] AprovarPagamentoVM aprovarPagamento)
        {
            _horasMesPrestadorService.NegarPagamento((int)aprovarPagamento.IdHoraMesPrestador, aprovarPagamento.Motivo);
            return Ok(true);
        }

        [HttpPut("dados-pagamento")]
        public IActionResult ObterDadosPagementoPorId([FromBody] AprovarPagamentoVM aprovarPagamento)
        {
            var resultBD = _prestadorService.ObterDadosPagementoPorId(aprovarPagamento.Id, aprovarPagamento.IdHorasMes);
            var resultVM = Mapper.Map<DadosPagamentoPrestadorDto>(resultBD);
            return Ok(new { dados = resultVM, notifications = "", success = true });
        }

        [HttpGet("verificar-cpf/{cpf}/{idPrestador}")]
        public IActionResult VerificarCpfExiste([FromRoute] string cpf, [FromRoute] int idPrestador)
        {
            bool validarCPF;
            string notificao;
            this.ValidarCPF(cpf, idPrestador, out validarCPF, out notificao);

            return Ok(new { dados = validarCPF, notifications = notificao, success = true });
        }

        [HttpPost("verificar-data-inativacao/")]
        public IActionResult VerificarDataInativacao([FromBody] InativacaoPrestador inativacao)
        {
            bool validarDataInativacao = _prestadorService.VerificarDataInaticacaoPrestador(inativacao.IdPrestador, inativacao.DataDesligamento.Value);
            string notificao = "";
            if (validarDataInativacao)
            {
                notificao = "ProfissinalExisteInativacao";
            }
            return Ok(new { dados = validarDataInativacao, notifications = notificao, success = true });
        }

        [HttpGet("solicitar-nf/{idHorasMesPrestador}")]
        public IActionResult SolicitarNF([FromRoute] int idHorasMesPrestador)
        {
            _prestadorService.SolicitarNF(idHorasMesPrestador);
            return Ok();
        }

        private void ValidarCPF(string cpf, int idPrestador, out bool validarCPF, out string notificao)
        {
            validarCPF = Domain.SharedRoot.ValidarCPF.IsCpf(cpf);
            notificao = String.Empty;
            if (validarCPF)
            {
                validarCPF = ValidarCpfBaseDados(cpf, idPrestador, ref notificao);
            }
            else
            {
                validarCPF = true;
                notificao = "InvalidoCpf";
            }
        }

        private bool ValidarCpfBaseDados(string cpf, int idPrestador, ref string notificao)
        {
            bool validaCpfInativo = _prestadorService.VerificarCpfProfissinalInativoExiste(cpf);
            bool validarCPF = _prestadorService.VerificarCpfProfissinalExiste(cpf);

            if (idPrestador != 0)
            {
                var prestador = _prestadorService.BuscarPorId(idPrestador);
                if (prestador.Pessoa.Cpf.Equals(cpf))
                {
                    return false;
                }
            }

            if (validaCpfInativo)
            {
                notificao = "ProfissinalExisteInativoCpf";
                return validaCpfInativo;
            }
            if (validarCPF)
            {
                notificao = "ProfissinalExisteCpf";
            }
            else
            {
                validarCPF = _prestadorService.VerificarCpfProfissinalCandidatoExiste(cpf);
                if (validarCPF)
                {
                    notificao = "ProfissinalCandidatoExisteCpf";
                }
            }

            return validarCPF;
        }


        [HttpPost("dados-prestador-inativacao")]
        public IActionResult PersistirDadosPrestadorInativacao([FromBody] InativacaoPrestadorVM inativacaoPrestadorVM)
        {
            var inativacao = Mapper.Map<InativacaoPrestador>(inativacaoPrestadorVM);
            _prestadorService.InativarPrestador(inativacao);

            return Ok(new { dados = inativacao, success = true });
        }

        [HttpPost("reativar-prestador")]
        public IActionResult ReativarPrestador([FromBody] InativacaoPrestadorVM inativacaoPrestadorVM)
        {
            var inativacao = Mapper.Map<InativacaoPrestador>(inativacaoPrestadorVM);
            _prestadorService.ReativarPrestador(inativacao);
            return Ok(true);
        }

        [HttpPost("dados-prestador-remuneracao")]
        public IActionResult persistirDadosPrestadorRemuneracao([FromBody] ValorPrestadorVM valorPrestadorVM)
        {
            var valorPrestador = Mapper.Map<ValorPrestador>(valorPrestadorVM);
            valorPrestador.Usuario = _variablesToken.UsuarioToken;
            _prestadorService.PersistirPrestadorRemuneracao(valorPrestador);
            _prestadorService.InserirRemuneracaoEAcesso(valorPrestador);
            return Ok(new { dados = valorPrestador, success = true });
        }

        [HttpGet("obter-beneficios-prestador/{idValorPrestador}")]
        public IActionResult obterBeneficiosPrestador([FromRoute] int idValorPrestador)
        {
            var resultBD = _prestadorService.ObterValoresPrestadorBeneficios(idValorPrestador);
            List<ValorPrestadorBeneficioVM> resultVM = Mapper.Map<List<ValorPrestadorBeneficioVM>>(resultBD);
            var valorPrestadorBeneficioTotal = new ValorPrestadorBeneficioVM()
            {
                descricaoBeneficio = "Total",
                ValorBeneficio = resultVM.Sum(x => x.ValorBeneficio)
            };

            resultVM.Add(valorPrestadorBeneficioTotal);


            return Ok(new { dados = resultVM, notifications = "", success = true });
        }


        [HttpPost("relatorio-prestadores-ativos/{empresaId}/{filialId}")]
        public IActionResult ObterRelatorioPrestadoresAtivos([FromRoute] int empresaId, [FromRoute]int filialId , [FromBody] FiltroGenericoDto<RelatorioPrestadoresDto> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDtoBase<RelatorioPrestadoresDto>>(filtro);
            var lista = _prestadorService.BuscarRelatorioPrestadores(empresaId, filialId, filtroDto).OrderBy(x => x.IdCelula).OrderBy(x => x.Nome).ToList();

            return Ok(lista);
        }


        [HttpGet("reabrir-lancamento/{idHorasMesPrestador}")]
        public IActionResult ReabrirLancamento([FromRoute] int idHorasMesPrestador)
        {
            _prestadorService.ReabrirLancamento(idHorasMesPrestador);
            return Ok();
        }

        [HttpPost("filtrar-conciliacao-pagamentos")]
        public IActionResult FiltrarConciliacaoPagamentos([FromBody] FiltroGenericoViewModelBase<PrestadorVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDtoBase<ConciliacaoPagamentoDto>>(filtro);
            var resultBD = _prestadorService.FiltrarConciliacaoPagamentos(filtroDto, false);
            var resultVM = Mapper.Map<FiltroGenericoViewModelBase<ConciliacaoPagamentoDto>>(resultBD);

            var coligadasRm = _empresaGrupoService.BuscarTodasNoRM();
            foreach (ConciliacaoPagamentoDto item in resultVM.Valores)
            {
                var empresaGrupoRmDto = coligadasRm.ToList().Find(c => c.Id == item.IdEmpresaGrupo);
                item.EmpresaGrupo = empresaGrupoRmDto.Nome;
            }

            return Ok(resultVM);
        }

        [HttpPost("resumo-conciliacao-pagamentos-excel")]
        public IActionResult ResumoConciliacaoPagamentosExcel([FromBody] FiltroGenericoViewModelBase<PrestadorVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDtoBase<ConciliacaoPagamentoDto>>(filtro);
            var resultBD = _prestadorService.FiltrarConciliacaoPagamentos(filtroDto, true);
            List<ResumoConciliacaoPagamentoExcelDto> resultVM = popularResumoConciliacaoPagamentoExcelDto(resultBD);

            return Ok(resultVM);
        }

        private List<ResumoConciliacaoPagamentoExcelDto> popularResumoConciliacaoPagamentoExcelDto(FiltroGenericoDtoBase<ConciliacaoPagamentoDto> resultBD)
        {
            List<ResumoConciliacaoPagamentoExcelDto> resumoConciliacaoPagamentoExcelDtoLista = new List<ResumoConciliacaoPagamentoExcelDto>();
            var coligadasRm = _empresaGrupoService.BuscarTodasNoRM();

            resultBD.Valores.ForEach(x =>
            {
                var resumoConciliacaoPagamentoExcelDto = new ResumoConciliacaoPagamentoExcelDto
                {
                    Celula = x.IdCelula.ToString(),
                    DescricaoDiretoria = x.Diretoria,
                    IdEmpresaGrupo = x.IdEmpresaGrupo.ToString(),
                    DescriçaoEmpresaGrupo = coligadasRm.ToList().Find(c => c.Id == x.IdEmpresaGrupo).Descricao,
                    Empresa = x.Empresa,
                    CNPJ = x.NrCnpj,
                    Prestador = x.Nome,
                    ValorBrutoStfcorp = x.ValorStfcorp.ToString(),
                    ValorBrutoRm = x.ValorRm.ToString(),
                    ValorLiquidoRm = x.ValorRmComDesconto.ToString(),
                    TipoContratacao = x.Contratacao,
                    DiaPagamento = x.DiaPagamento.ToString(),
                    CodigoCentroCusto = x.CodigoCentroCusto,
                    ContaCaixa = x.ContaCaixa,
                    Banco = x.Banco,
                    Agencia = x.Agencia,
                    Conta = x.Conta,
                    Conciliado = x.Conciliado ? "Sim" : "Não",
                    Fechado = x.StatusRm == "Q" ? "Sim" : "Não"
                };
                resumoConciliacaoPagamentoExcelDtoLista.Add(resumoConciliacaoPagamentoExcelDto);
            });
            return resumoConciliacaoPagamentoExcelDtoLista;
        }

        [HttpPost("resumo-conciliacao-pagamentos")]
        public IActionResult ResumoConciliacaoPagamentos([FromBody] FiltroGenericoViewModelBase<PrestadorVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDtoBase<ConciliacaoPagamentoDto>>(filtro);
            var resultBD = _prestadorService.FiltrarConciliacaoPagamentos(filtroDto, true);
            List<ResumoConciliacaoPagamentoDto> resultVM = popularResumoConciliacaoPagamentoDto(resultBD);

            return Ok(resultVM);
        }

        private List<ResumoConciliacaoPagamentoDto> popularResumoConciliacaoPagamentoDto(FiltroGenericoDtoBase<ConciliacaoPagamentoDto> resultBD)
        {
            List<ResumoConciliacaoPagamentoDto> resumoConciliacaoPagamentoDtoLista = new List<ResumoConciliacaoPagamentoDto>();
            var coligadasRm = _empresaGrupoService.BuscarTodasNoRM();
            resultBD.Valores.ForEach(x =>
            {
                var existeIdEmpresaGrupo = resumoConciliacaoPagamentoDtoLista.Find(r => r.IdEmpresaGrupo == x.IdEmpresaGrupo);
                if (existeIdEmpresaGrupo == null)
                {
                    var registros = resultBD.Valores.FindAll(r => r.IdEmpresaGrupo == x.IdEmpresaGrupo);

                    var bancos = registros.Select(r => r.CodigoBancoEmpresa).Distinct().ToList();


                    var primeiroRegistro = true;
                    bancos.ToList().ForEach(b =>
                    {
                        if (!String.IsNullOrEmpty(b))
                        {
                            var resumoConciliacaoPagamentoDto = new ResumoConciliacaoPagamentoDto
                            {
                                Coligada = String.Empty,
                                Banco = b,
                                ValorTotalBruto = registros.FindAll(f => f.CodigoBancoEmpresa == b).Sum(s => s.ValorRm),
                                ValorTotalLiquido = registros.FindAll(f => f.CodigoBancoEmpresa == b).Sum(s => s.ValorRmComDesconto),
                                NrCnpj = x.NrCnpj,
                                IdEmpresaGrupo = x.IdEmpresaGrupo
                            };
                            if (primeiroRegistro)
                            {
                                var empresaGrupoRmDto = coligadasRm.ToList().Find(c => c.Id == x.IdEmpresaGrupo);
                                resumoConciliacaoPagamentoDto.Coligada = empresaGrupoRmDto.Nome;
                                primeiroRegistro = false;
                            }
                            resumoConciliacaoPagamentoDtoLista.Add(resumoConciliacaoPagamentoDto);
                        }
                    });
                    var resumoConciliacaoPagamentoDtoVazio = new ResumoConciliacaoPagamentoDto
                    {
                        Coligada = String.Empty,
                        Banco = String.Empty,
                        ValorTotalBruto = null,
                        ValorTotalLiquido = null
                    };
                    resumoConciliacaoPagamentoDtoLista.Add(resumoConciliacaoPagamentoDtoVazio);
                }
            });
            return resumoConciliacaoPagamentoDtoLista;
        }

        [HttpPost("remover-remuneracao-prestador")]
        public IActionResult RemoverRemuneracaoPrestador([FromBody] int idRemuneracao)
        {
            var resultBD = _valorPrestadorService.BuscarPorId(idRemuneracao);
            var permiteExcluir = _valorPrestadorService.ValidaExcluir(resultBD);
            if (permiteExcluir)
            {
                _valorPrestadorService.RemoverValorPrestador(resultBD);
            }
            return Ok();
        }

        [HttpPost("inativar-remuneracao-prestador")]
        public IActionResult InativarRemuneracaoPrestador([FromBody] int idRemuneracao)
        {
            var resultBD = _valorPrestadorService.BuscarPorId(idRemuneracao);
            _valorPrestadorService.InativarRemuneracaoPrestador(resultBD);
            return Ok();
        }

        [HttpPost("dados-prestador-observacao")]
        public IActionResult persistirDadosPrestadorObservacao([FromBody] ObservacaoPrestadorVM observacaoPrestadorVM)
        {
            var observacaoPrestador = Mapper.Map<ObservacaoPrestador>(observacaoPrestadorVM);
            _prestadorService.PersistirPrestadorObservacao(observacaoPrestador);
            return Ok(new { dados = observacaoPrestador, success = true });
        }

        [HttpPost("verifica-integracao-prestador-rm")]
        public IActionResult VerificarIntegracaoPrestadorRm([FromBody] PrestadorHoraVM prestadorHoraVM)
        {
            bool integrado;
            if (Variables.EnvironmentName.Equals("Production"))
            {
                var prestadorHoraBD = Mapper.Map<HorasMesPrestador>(prestadorHoraVM);
                integrado = _horasMesPrestadorService.VerificarIntegracaoPrestadorRm(prestadorHoraBD);
            }
            else
            {
                integrado = true;
            }
            return Ok(integrado);
        }
    }
}
