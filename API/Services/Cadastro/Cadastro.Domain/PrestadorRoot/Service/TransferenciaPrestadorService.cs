using Cadastro.Domain.EmailRoot.DTO;
using Cadastro.Domain.EmpresaGrupoRoot.Service.Interfaces;
using Cadastro.Domain.FilialRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utils;
using Utils.Base;
using static Cadastro.Domain.SharedRoot.SharedEnuns;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class TransferenciaPrestadorService : ITransferenciaPrestadorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICelulaRepository _celulaRepository;
        private readonly IPrestadorRepository _prestadorRepository;
        private readonly ILogTransferenciaPrestadorRepository _logTransferenciaPrestadorRepository;
        private readonly IFilialService _filialService;
        private readonly IEmpresaGrupoService _empresaGrupoService;
        private readonly IClienteServicoPrestadorRepository _clienteServicoPrestadorRepository;
        private readonly ITransferenciaPrestadorRepository _transferenciaPrestadorRepository;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly LinkBase _linkBase;

        public TransferenciaPrestadorService(
            ICelulaRepository celulaRepository,
            IPrestadorRepository prestadorRepository,
            ILogTransferenciaPrestadorRepository logTransferenciaPrestadorRepository,
            IFilialService filialService,
            IEmpresaGrupoService empresaGrupoService,
            IClienteServicoPrestadorRepository clienteServicoPrestadorRepository,
            ITransferenciaPrestadorRepository transferenciaPrestadorRepository,
            IUnitOfWork unitOfWork,
            MicroServicosUrls microServicosUrls,
            LinkBase linkBase)
        {
            _celulaRepository = celulaRepository;
            _prestadorRepository = prestadorRepository;
            _logTransferenciaPrestadorRepository = logTransferenciaPrestadorRepository;
            _filialService = filialService;
            _empresaGrupoService = empresaGrupoService;
            _clienteServicoPrestadorRepository = clienteServicoPrestadorRepository;
            _transferenciaPrestadorRepository = transferenciaPrestadorRepository;
            _unitOfWork = unitOfWork;
            _microServicosUrls = microServicosUrls;
            _linkBase = linkBase;
        }

        public IEnumerable<KeyValueDto> ObterPrestadoresPorCelula(int idCelula)
        {
            var result = _prestadorRepository.ObterPrestadoresPorCelula(idCelula);

            return result;
        }

        public ResponseTransferenciaPrestador ObterPrestadorParaTransferencia(int idPrestador, bool validar)
        {
            var prestador = _prestadorRepository.ObterPrestadorParaTransferencia(idPrestador);

            if (_transferenciaPrestadorRepository.ExisteTransferenciaAberta(idPrestador) && validar)
            {
                return new ResponseTransferenciaPrestador { Message = "TRANSFERENCIA_ABERTA" };
            }

            var filial = _filialService.BuscarFilialNoRm(prestador.IdFilial, prestador.IdEmpresaGrupo.GetValueOrDefault());

            var empresaGrupo = _empresaGrupoService.BuscarNoRMPorId(prestador.IdEmpresaGrupo.GetValueOrDefault());

            var servicoCliente = prestador.ClientesServicosPrestador.FirstOrDefault();

            if (servicoCliente == null)
            {
                return new ResponseTransferenciaPrestador { Message = "PRESTADOR_SEM_CLIENTE" };
            }

            var cliente =
                _prestadorRepository.ObterClienteAtivoPorIdCelulaEAcesso(prestador.IdCelula, servicoCliente.IdCliente);

            var servico = ObterServicoPorId(servicoCliente.IdServico);

            var localTrabalho =
                _prestadorRepository.ObterLocalTrabalhoPorId(servicoCliente.IdLocalTrabalho, servicoCliente.IdCliente);

            return new ResponseTransferenciaPrestador
            {
                PrestadorParaTransferenciaDto = new PrestadorParaTransferenciaDto
                {
                    EmpresaGrupo = empresaGrupo.Descricao,
                    IdEmpresaGrupo = empresaGrupo.Id,
                    Filial = filial.Descricao,
                    IdFilial = filial.Id,
                    Celula = $"{prestador.IdCelula} - {prestador.Celula.Descricao}",
                    IdCelula = prestador.IdCelula,
                    Cliente = cliente.Descricao,
                    IdCliente = cliente.Id,
                    Servico = servico.Descricao,
                    IdServico = servico.Id,
                    IdLocalTrabalho = localTrabalho.Id,
                    LocalDeTrabalho = localTrabalho.Descricao,
                    IdPrestador = prestador.Id
                }
            };
        }

        public string SolicitarTransferenciaPrestador(TransferenciaPrestador transferenciaPrestador)
        {
            var prestador = _prestadorRepository.ObterPrestadorParaTransferencia(transferenciaPrestador.IdPrestador);

            var validacao = ValidarTransferencia(transferenciaPrestador, prestador);

            if (!string.IsNullOrEmpty(validacao)) return validacao;

            _transferenciaPrestadorRepository.Adicionar(transferenciaPrestador);

            AddLogTransferencia(SituacoesTransferenciaEnum.NovaSolicitacao, transferenciaPrestador.Id, string.Empty);
            VerificarStatusTransferencia(transferenciaPrestador, prestador);

            _unitOfWork.Commit();

            return string.Empty;
        }

        public string AtualizarTransferenciaPrestador(TransferenciaPrestador transferenciaPrestador)
        {
            var prestador = _prestadorRepository.ObterPrestadorParaTransferencia(transferenciaPrestador.IdPrestador);

            var validacao = ValidarTransferencia(transferenciaPrestador, prestador);

            if (!string.IsNullOrEmpty(validacao)) return validacao;

            VerificarStatusTransferencia(transferenciaPrestador, prestador);
            AddLogTransferencia(SituacoesTransferenciaEnum.SolicitacaoEditada, transferenciaPrestador.Id, string.Empty);

            _transferenciaPrestadorRepository.Update(transferenciaPrestador);

            _unitOfWork.Commit();

            return string.Empty;
        }

        public IEnumerable<PeriodoTransferenciaPrestadorDto> BuscarPeriodosComTransferenciasCadastradas() => _transferenciaPrestadorRepository.BuscarPeriodosComTransferenciaCadastrada();

        public void AprovarTransferencia(AprovarTransferenciaPrestadorDto aprovacaoDto)
        {
            var transfBD = _transferenciaPrestadorRepository.BuscarPorId(aprovacaoDto.IdTransf);
            transfBD.DataTransferencia = aprovacaoDto.Data;
            transfBD.Situacao = SituacoesTransferenciaEnum.Aprovado.GetHashCode();
            _transferenciaPrestadorRepository.Update(transfBD);

            AddLogTransferencia((SituacoesTransferenciaEnum)transfBD.Situacao, transfBD.Id, string.Empty);

            _unitOfWork.Commit();
        }

        public void NegarTransferencia(NegarTransferenciaPrestadorDto negacaoDto)
        {
            var transfBD = _transferenciaPrestadorRepository.BuscarPorId(negacaoDto.IdTransf);
            transfBD.Situacao = SituacoesTransferenciaEnum.Negado.GetHashCode();
            _transferenciaPrestadorRepository.Update(transfBD);

            AddLogTransferencia((SituacoesTransferenciaEnum)transfBD.Situacao, transfBD.Id, negacaoDto.Motivo);

            if (_unitOfWork.Commit())
            {
                var celulaOrigem = _prestadorRepository.BuscarPorId(transfBD.IdPrestador).IdCelula;
                var emailResponsavelDaCelulaOrigem = _celulaRepository.BuscarPorId(celulaOrigem).EmailResponsavel;

                var email = new EmailDTO
                {
                    IdTemplate = 10,
                    RemetenteNome = "Gestão de Transferências",
                    Para = ObterDestinatarioEmail(emailResponsavelDaCelulaOrigem),
                    ValoresParametro = DefinirParametrosEmailNegacao(negacaoDto, _prestadorRepository.BuscarPorIdComIncludes(transfBD.IdPrestador), DateTime.Now)
                };

                EnviarEmail(email);
            }
        }

        public FiltroComPeriodo<TransferenciaPrestadorDto> FiltrarTransferencia(FiltroComPeriodo<TransferenciaPrestadorDto> filtro)
        {
            var result = _transferenciaPrestadorRepository.FiltrarTransferencias(filtro);
            return result;
        }

        public PrestadorParaTransferenciaDto ConsultarTransferencia(int idTransferencia)
        {
            var result = _transferenciaPrestadorRepository.ConsultarTransferencia(idTransferencia);
            return result;
        }

        public IEnumerable<LogTransferenciaPrestador> ObterLogsTransferencia(int idTransf) => _logTransferenciaPrestadorRepository.Buscar(x => x.IdTransferenciaPrestador == idTransf);

        private void AddLogTransferencia(SituacoesTransferenciaEnum status, int idTransferencia, string motivo)
        {
            var log = new LogTransferenciaPrestador
            {
                Status = status,
                IdTransferenciaPrestador = idTransferencia,
                MotivoNegacao = motivo
            };

            _logTransferenciaPrestadorRepository.Adicionar(log);
        }


        private string ValidarTransferencia(TransferenciaPrestador transferenciaPrestador, Prestador prestador)
        {
            if (prestador == null) return "Prestador não encontrado.";

            if (prestador.IdEmpresaGrupo != transferenciaPrestador.IdEmpresaGrupo &&
                prestador.IdEmpresaGrupo.GetValueOrDefault().VerificarEmpresaNaoPodeTransferir())
                return "TRANSFERENCIA_COLIGADA_INVALIDA";

            return string.Empty;
        }

        private void VerificarStatusTransferencia(TransferenciaPrestador transferenciaPrestador, Prestador prestador)
        {
            if (prestador.IdCelula != transferenciaPrestador.IdCelula.GetValueOrDefault())
            {
                EnviarEmailTransferencia(transferenciaPrestador, prestador);
                transferenciaPrestador.Situacao = SituacoesTransferenciaEnum.AguardandoAprovacao.GetHashCode();
                if (prestador.IdSituacao == transferenciaPrestador.Situacao)
                {
                    AddLogTransferencia(SituacoesTransferenciaEnum.SolicitacaoEditada, transferenciaPrestador.Id, string.Empty);

                }
                else
                {
                    AddLogTransferencia(SituacoesTransferenciaEnum.AguardandoAprovacao, transferenciaPrestador.Id, string.Empty);
                }
            }
            else
            {
                transferenciaPrestador.Situacao = SituacoesTransferenciaEnum.Aprovado.GetHashCode();
                var teste = ObterLogsTransferencia(transferenciaPrestador.Id).OrderBy(x => x.DataAlteracao).LastOrDefault();
                if (prestador.IdSituacao == transferenciaPrestador.Situacao)
                {
                    AddLogTransferencia(SituacoesTransferenciaEnum.SolicitacaoEditada, transferenciaPrestador.Id, string.Empty);

                }
                else
                {
                    AddLogTransferencia(SituacoesTransferenciaEnum.Aprovado, transferenciaPrestador.Id, string.Empty);

                }
            }
        }

        private void EnviarEmailTransferencia(TransferenciaPrestador transferenciaPrestador, Prestador prestador)
        {
            var empresaGrupo =
             _empresaGrupoService.BuscarNoRMPorId(transferenciaPrestador.IdEmpresaGrupo.GetValueOrDefault());

            var filial = _filialService.BuscarFilialNoRm(transferenciaPrestador.IdFilial.GetValueOrDefault(),
                transferenciaPrestador.IdEmpresaGrupo.GetValueOrDefault());

            var cliente = _prestadorRepository.ObterClienteAtivoPorIdCelulaEAcesso(
                transferenciaPrestador.IdCelula.GetValueOrDefault(),
                transferenciaPrestador.IdCliente.GetValueOrDefault());

            var servico = ObterServicoPorId(transferenciaPrestador.IdServico.GetValueOrDefault());

            var localTrabalho = _prestadorRepository.ObterLocalTrabalhoPorId(
                transferenciaPrestador.IdLocalTrabalho.GetValueOrDefault(),
                transferenciaPrestador.IdCliente.GetValueOrDefault());

            if (empresaGrupo == null || filial == null || cliente == null || servico == null || localTrabalho == null) return;

            var parametros = DefinirParametrosEmail(transferenciaPrestador, prestador, empresaGrupo.Descricao,
                filial.Descricao, cliente.Descricao, servico.Descricao, localTrabalho.Descricao);

            var email = new EmailDTO
            {
                IdTemplate = 9,
                RemetenteNome = "Gestão de Transferências",
                Para = ObterDestinatarioEmail(prestador.Celula.EmailResponsavel),
                ValoresParametro = parametros
            };

            EnviarEmail(email);
        }

        private IEnumerable<ValorParametroEmailDTO> DefinirParametrosEmailNegacao(NegarTransferenciaPrestadorDto negacaoDto, Prestador prestador, DateTime data)
        {
            var parametros = new List<ValorParametroEmailDTO>();

            var parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[NOMEPROFISSIONAL]",
                ParametroValor = prestador.Pessoa.Nome
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[DATADODIA]",
                ParametroValor = data.ToString("dd/MM/yyyy")
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[MOTIVO]",
                ParametroValor = negacaoDto.Motivo
            };
            parametros.Add(parametro);

            return parametros;
        }

        private IEnumerable<ValorParametroEmailDTO> DefinirParametrosEmail(
            TransferenciaPrestador transferenciaPrestador, Prestador prestador, string empresaGrupo, string filial,
            string cliente, string servico, string localTrabalho)
        {
            var parametros = new List<ValorParametroEmailDTO>();

            var parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[NOMEPROFISSIONAL]",
                ParametroValor = prestador.Pessoa.Nome
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[DATADODIA]",
                ParametroValor = transferenciaPrestador.DataTransferencia.ToString("dd/MM/yyyy")
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[CELULAORIGEM]",
                ParametroValor = prestador.IdCelula.ToString()
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[EMPRESAGRUPO]",
                ParametroValor = empresaGrupo
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[FILIAL]",
                ParametroValor = filial
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[CELULADESTINO]",
                ParametroValor = transferenciaPrestador.IdCelula.ToString()
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[CLIENTE]",
                ParametroValor = cliente
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[SERVICO]",
                ParametroValor = servico
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[LOCALDETRABALHO]",
                ParametroValor = localTrabalho
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[LINK]",
                ParametroValor = ObterLinkAprovacaoTransferencia()
            };
            parametros.Add(parametro);

            return parametros;
        }

        private string ObterDestinatarioEmail(string emailCorreto)
        {
            var destinatario = "";
            switch (Variables.EnvironmentName.ToUpper())
            {
                case "DEVELOPMENT":
                    destinatario = "gmguilherme@latam.stefanini.com,ybligorio@latam.stefanini.com,ybligorio@stefanini.com,clbatista@stefanini.com";
                    break;
                case "STAGING":
                    destinatario = "gmguilherme@latam.stefanini.com,ybligorio@latam.stefanini.com,ybligorio@stefanini.com,clbatista@stefanini.com";
                    break;
                case "PRODUCTION":
                    destinatario = emailCorreto;
                    break;
            }
            return destinatario;
        }

        private string ObterLinkAprovacaoTransferencia()
        {
            var destinatario = "";
            switch (Variables.EnvironmentName.ToUpper())
            {
                case "DEVELOPMENT":
                    destinatario = $"{_linkBase.Development}stfcorp/gestao-de-terceiros/aprovar-transferencias/consultar-para-aprovar";
                    break;
                case "STAGING":
                    destinatario = $"{_linkBase.Staging}stfcorp/gestao-de-terceiros/aprovar-transferencias/consultar-para-aprovar";
                    break;
                case "PRODUCTION":
                    destinatario = $"{_linkBase.Production}stfcorp/gestao-de-terceiros/aprovar-transferencias/consultar-para-aprovar";
                    break;
            }
            return destinatario;
        }

        public void EfetivarTransferenciasDePrestadoresJob()
        {
            var transfsParaEfetivar = _transferenciaPrestadorRepository.Buscar(x => x.Situacao == SituacoesTransferenciaEnum.Aprovado.GetHashCode()
                             && x.DataTransferencia.Date <= DateTime.Now.Date).ToList();

            var idPrestadores = transfsParaEfetivar.Select(x => x.IdPrestador).Distinct();
            var prestadoresParaAlterar = _prestadorRepository.ObterPrestadoresParaTransferencia(idPrestadores.ToArray());

            foreach (var p in prestadoresParaAlterar)
            {
                var transfDoPrestador = transfsParaEfetivar.LastOrDefault(x => x.IdPrestador == p.Id);
                var clienteServicoPrestador = p.ClientesServicosPrestador.FirstOrDefault();

                p.IdEmpresaGrupo = transfDoPrestador.IdEmpresaGrupo;
                p.IdFilial = (int)transfDoPrestador.IdFilial;
                p.IdCelula = (int)transfDoPrestador.IdCelula;
                p.DataAlteracao = DateTime.Now;
                p.Usuario = "STFCORP";

                if (transfDoPrestador.IdCelula != clienteServicoPrestador.IdCelula
                 || transfDoPrestador.IdCliente != clienteServicoPrestador.IdCliente
                 || transfDoPrestador.IdServico != clienteServicoPrestador.IdServico)
                {
                    clienteServicoPrestador.Ativo = false;
                    var novoClienteServicoPrestador = CriarNovoParaCspParaTransferenciaDePrestador(clienteServicoPrestador, transfDoPrestador);
                    _clienteServicoPrestadorRepository.Update(clienteServicoPrestador);
                    _clienteServicoPrestadorRepository.Adicionar(novoClienteServicoPrestador);
                }
                else
                {
                    clienteServicoPrestador.IdLocalTrabalho = (int)transfDoPrestador.IdLocalTrabalho;
                }

                transfDoPrestador.Situacao = SituacoesTransferenciaEnum.Efetivado.GetHashCode();
                _clienteServicoPrestadorRepository.Update(clienteServicoPrestador);
                _prestadorRepository.Update(p);
                _transferenciaPrestadorRepository.Update(transfDoPrestador);
            }
            _unitOfWork.Commit();
        }

        private ClienteServicoPrestador CriarNovoParaCspParaTransferenciaDePrestador(ClienteServicoPrestador cspAnterior, TransferenciaPrestador transf)
        {
            var novoClienteServicoPrestador = cspAnterior.Clone();
            novoClienteServicoPrestador.Prestador = null;
            novoClienteServicoPrestador.Id = 0;
            novoClienteServicoPrestador.Ativo = true;
            novoClienteServicoPrestador.IdCelula = (int)transf.IdCelula;
            novoClienteServicoPrestador.IdCliente = (int)transf.IdCliente;
            novoClienteServicoPrestador.IdServico = (int)transf.IdServico;
            novoClienteServicoPrestador.IdLocalTrabalho = (int)transf.IdLocalTrabalho;
            novoClienteServicoPrestador.DataAlteracao = DateTime.Now;
            novoClienteServicoPrestador.Usuario = "STFCORP";
            return novoClienteServicoPrestador;
        }

        private void EnviarEmail(EmailDTO email)
        {
            var client = new HttpClient { BaseAddress = new Uri(_microServicosUrls.UrlEnvioEmail) };


            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/Email", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
        }

        private ComboClienteServicoDto ObterServicoPorId(int idServico)
        {
            var client = new HttpClient { BaseAddress = new Uri(_microServicosUrls.UrlApiServico) };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync($"api/ServicoContratados/obter-servico-por-id/{idServico}").Result;

            var jsonString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<ComboClienteServicoDto>(jsonString);
            return result;
        }
    }
}
