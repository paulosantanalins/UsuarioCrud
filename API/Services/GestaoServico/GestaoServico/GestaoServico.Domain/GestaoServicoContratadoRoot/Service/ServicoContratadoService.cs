using Dapper;
using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Repository;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Validators;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Dto;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Repository;
using GestaoServico.Domain.Interfaces;
using GestaoServico.Domain.OperacaoMigradaRoot.DTO;
using GestaoServico.Domain.OperacaoMigradaRoot.Service.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Utils;
using Utils.Base;
using Utils.Connections;
using Utils.Relatorios.Models;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Service
{
    public class ServicoContratadoService : IServicoContratadoService
    {
        protected readonly IUnitOfWork _unitOfWork;
        private readonly IVariablesToken _variables;
        private readonly NotificationHandler _notificationHandler;
        private readonly IServicoContratadoRepository _servicoContratadoRepository;
        protected readonly IContratoRepository _contratoRepository;
        protected readonly IContratoService _contratoService;
        protected readonly IDeParaServicoRepository _deParaServicoRepository;
        private readonly IOperacaoMigradaService _operacaoMigradaService;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public ServicoContratadoService(
            IUnitOfWork unitOfWork,
            IVariablesToken variables,
            IServicoContratadoRepository pacoteServicoRepository,
            IOperacaoMigradaService operacaoMigradaService,
            NotificationHandler notificationHandler,
            IContratoRepository contratoRepository,
            IContratoService contratoService,
            IDeParaServicoRepository deParaServicoRepository,
            IOptions<ConnectionStrings> connectionStrings,
            MicroServicosUrls microServicosUrls
        )
        {
            _unitOfWork = unitOfWork;
            _variables = variables;
            _servicoContratadoRepository = pacoteServicoRepository;
            _notificationHandler = notificationHandler;
            _contratoRepository = contratoRepository;
            _operacaoMigradaService = operacaoMigradaService;
            _contratoService = contratoService;
            _deParaServicoRepository = deParaServicoRepository;
            _connectionStrings = connectionStrings;
            _microServicosUrls = microServicosUrls;
        }

        public ServicoContratado BuscarPorId(int id)
        {
            var result = _servicoContratadoRepository.BuscarComInclude(id);
            return result;
        }

        public FiltroGenericoDto<ServicoContratadoDto> Filtrar(FiltroGenericoDto<ServicoContratadoDto> filtroDto)
        {
            var result = _servicoContratadoRepository.Filtrar(filtroDto);
            return result;
        }

        public FiltroGenericoDto<ServicoContratado> FiltrarAccordions(FiltroGenericoDto<ServicoContratado> filtroDto)
        {
            var result = _servicoContratadoRepository.FiltrarAccordions(filtroDto);
            return result;
        }

        public int Persistir(ServicoContratado servicoContratado, int idCelula, decimal? vlMarkup)
        {
            servicoContratado.Contrato = null;
            var mensagens = new ServicoContratadoValidator().Validate(servicoContratado).Errors;
            if (mensagens.Count > 0)
            {
                foreach (var mensagem in mensagens)
                {
                    _notificationHandler.AddMensagem(mensagem.PropertyName, mensagem.ErrorMessage);
                }
                return 0;
            }
            if (!servicoContratado.DeParaServicos.Any() || servicoContratado.DeParaServicos.Count == 0)
            {
                var idExistente = _servicoContratadoRepository.Validar(servicoContratado);
                if (idExistente != 0)
                {
                    _notificationHandler.AddMensagem("DUPLICIDADE", "PERSISTIR_SERVICO_CONTRATADO_DUPLICIDADE");
                    _notificationHandler.AddMensagem("ID", idExistente.ToString());
                    return idExistente;
                }
            }
            
            if (servicoContratado.Id == 0)
            {
                if (!servicoContratado.DeParaServicos.Any() || servicoContratado.DeParaServicos.Count == 0)
                {
                    //PersistirServicoComercial(servicoContratado);
                    servicoContratado.DescTipoCelula = "TEC";
                }
                else
                {
                    _variables.UserName = "EACESSO";
                }
                servicoContratado.VinculoMarkupServicosContratados.Add(new VinculoMarkupServicoContratado
                {
                    DtInicioVigencia = servicoContratado.DtInicial,
                    DtFimVigencia = servicoContratado.DtFinal,
                    IdServicoContratado = servicoContratado.Id,
                    VlMarkup = vlMarkup ?? 0,
                    //Usuario = _variables.GetUserName(),
                    //DataAlteracao = DateTime.Now
                });
                _servicoContratadoRepository.Adicionar(servicoContratado);
            }
            else
            {
                _servicoContratadoRepository.Update(servicoContratado);
            }
            _unitOfWork.Commit();
            return servicoContratado.Id;
        }

        private void PersistirServicoComercial(ServicoContratado servicoContratado)
        {
            var servicoComercial = (ServicoContratado)servicoContratado.Clone();
            servicoComercial.DescTipoCelula = "COM";
            var idServicoComercialExcluido = _servicoContratadoRepository.VerificarServicoContratadoComercialUnicoPorContrato(servicoContratado.IdContrato);
            servicoComercial.IdCelula = _servicoContratadoRepository.ObterCelulaComercialVigenteContrato(servicoContratado.IdContrato) ?? 0;
            if (idServicoComercialExcluido.HasValue)
            {
                var servicoParaRemovido = _servicoContratadoRepository.BuscarPorId(idServicoComercialExcluido.Value);
                _servicoContratadoRepository.Remover(servicoParaRemovido);
            }
            _servicoContratadoRepository.Adicionar(servicoComercial);
        }

        public List<ServicoContratado> PreencherComboServicoContratado()
        {
            var result = _servicoContratadoRepository.BuscarTodos().ToList();
            return result;
        }

        public List<MultiselectDto> PreencherComboServicoContratadoPorCelulaCliente(int idCelula, int idCliente)
        {
            var result = _servicoContratadoRepository.PreencherComboServicoContratadoPorCelulaCliente(idCelula, idCliente).ToList();
            return result;
        }

        public List<MultiselectDto> ObterServicoContratadoPorCliente(int idCliente)
        {
            var result = _servicoContratadoRepository.ObterServicoContratadoPorCliente(idCliente).ToList();
            return result;
        }

        public List<MultiselectDto> ObterServicoContratadoPorCelula(int idCelula)
        {
            var result = _servicoContratadoRepository.ObterServicoContratadoPorCelula(idCelula).ToList();
            return result;
        }

        public List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicoContratadoPorClienteRelatorioRentabilidade(int idCelula, int idCliente)
        {
            var result = _servicoContratadoRepository.ObterServicoContratadoPorCelulaClienteRelatorioRentabilidade(idCelula, idCliente).ToList();
            return result;
        }

        public List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicoContratadoPorCelulaRelatorioRentabilidade(int idCelula)
        {
            var result = _servicoContratadoRepository.ObterServicoContratadoPorCelulaRelatorioRentabilidade(idCelula).ToList();
            return result;
        }

        public List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicoContratadoPorCelulaRelatorioDiretoria(List<int> idsCelula)
        {
            var result = _servicoContratadoRepository.ObterServicoContratadoPorCelulasRelatorioDiretoria(idsCelula).ToList();
            return result;
        }

        public void MigrarServicoEacesso(ServicoContratado servico)
        {
            _servicoContratadoRepository.Update(servico);
            var depara = _deParaServicoRepository.Buscar(x => x.IdServicoContratado == servico.Id).FirstOrDefault();
            depara.DescStatus = "AP";
            _deParaServicoRepository.Update(depara);
            _unitOfWork.Commit();
        }

        public int PersistirServicoEacesso(ServicoContratado servico, int idServicoEacesso, string nomeServico, int idCliente, decimal markup, string idContrato, string siglaTipoServico, string descEscopo, string siglaSubServico = null)
        {
            servico.EscopoServico = null;
            servico.DeParaServicos.Add(new DeParaServico
            {
                DescStatus = "MA",
                DescTipoServico = siglaTipoServico,
                NmServicoEacesso = nomeServico,
                IdServicoEacesso = idServicoEacesso,
                DescEscopo = descEscopo,
                DescSubTipoServico = siglaSubServico,
                DataAlteracao = DateTime.Now,
                Usuario = "Eacesso"
            });
            idCliente = ObterClienteEacessoPorId(idCliente);
            servico.IdContrato = _contratoService.VerificarExistenciaContratoEacesso(idContrato, idCliente);

            var idServicoPersistido = Persistir(servico, servico.IdCelula, markup);
            return idServicoPersistido;
        }

        private int ObterClienteEacessoPorId(int idCliente)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(120);
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiCliente);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/cliente/" + idCliente + "/obter-cliente-migrado/").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return Int32.Parse(responseString);
        }

        public ServicoContratado ObterServicoComercialAtivoPorContrato(int idContrato)
        {
            var result = _servicoContratadoRepository.ObterServicoComercialVigenteContrato(idContrato);
            return result;
        }

        public void Criar(ServicoContratado servicoContratado)
        {
            _servicoContratadoRepository.Adicionar(servicoContratado);
            _unitOfWork.Commit();
        }

        public void PersistirServicoMigrado(ServicoMigracaoDTO servicoMigradoDTO)
        {
            ServicoContratado servicoContratado = CriarVinculoContrato(servicoMigradoDTO, _variables.UserName);

            CriarVinculoDePara(servicoMigradoDTO, servicoContratado, _variables.UserName);
            CriarVinculoMarkup(servicoMigradoDTO, servicoContratado, _variables.UserName);
            CriarVinculoCelulaComercial(servicoMigradoDTO, servicoContratado, _variables.UserName);

            PopularCamposServicoContratado(servicoMigradoDTO, servicoContratado);

            _servicoContratadoRepository.Adicionar(servicoContratado);
            _operacaoMigradaService.AtualizarStatus(servicoMigradoDTO.IdsServicosAgrupados);

            _unitOfWork.Commit();
        }

        private static ServicoContratado CriarVinculoContrato(ServicoMigracaoDTO servicoMigradoDTO, string usuario)
        {
            ServicoContratado servicoContratado = new ServicoContratado();
            servicoContratado.Contrato = new Contrato
            {
                DescStatusSalesForce = "",
                IdMoeda = 1,
                NrAssetSalesForce = servicoMigradoDTO.DescricaoContrato,
                DescContrato = servicoMigradoDTO.DescricaoOperacao,
                DtInicial = servicoMigradoDTO.DtInicioContrato.Value,
                DtFinalizacao = servicoMigradoDTO.DtFimContrato,
                DataAlteracao = DateTime.Now,
                Usuario = usuario
            };
            foreach (var idCliente in servicoMigradoDTO.IdsClientes)
            {
                if (!servicoContratado.Contrato.ClientesContratos.Any(x => x.IdCliente == idCliente))
                {
                    servicoContratado.Contrato.ClientesContratos.Add(new ClienteContrato
                    {
                        IdCliente = idCliente,
                        DataAlteracao = DateTime.Now,
                        Usuario = usuario
                    });
                }
            }
            return servicoContratado;
        }

        private static void PopularCamposServicoContratado(ServicoMigracaoDTO servicoMigradoDTO, ServicoContratado servicoContratado)
        {
            servicoContratado.DescricaoServicoContratado = servicoMigradoDTO.DescricaoServico;
            servicoContratado.IdEscopoServico = servicoMigradoDTO.IdEscopo;
            servicoContratado.IdCelula = servicoMigradoDTO.IdCelula;
            servicoContratado.DescTipoCelula = servicoMigradoDTO.IdTipoCelula == 1 ? "COM" : servicoMigradoDTO.IdTipoCelula == 3 ? "TEC" : "OUT";
            servicoContratado.DtInicial = servicoMigradoDTO.DtInicioContrato.Value;
            servicoContratado.DtFinal = servicoMigradoDTO.DtFimContrato;
            servicoContratado.FormaFaturamento = servicoMigradoDTO.DescricaoFormaFaturamento;
            servicoContratado.VlRentabilidade = servicoMigradoDTO.RentabilidadePrevista;
            servicoContratado.NmTipoReembolso = servicoMigradoDTO.DescricaoTipoReembolso;
            servicoContratado.VlKM = servicoMigradoDTO.ValorKmRodado;
            servicoContratado.FlReembolso = servicoMigradoDTO.IsDespesasReembolsaveis ?? false;
            servicoContratado.FlHorasExtrasReembosaveis = servicoMigradoDTO.IsHeReembolsaveis ?? false;
            servicoContratado.FlFaturaRecorrente = servicoMigradoDTO.IsFaturaRecorrente ?? false;
            servicoContratado.FlReoneracao = servicoMigradoDTO.IsReeoneracao ?? false;
            servicoContratado.IdEmpresa = servicoMigradoDTO.IdColigada;
            servicoContratado.IdFilial = servicoMigradoDTO.IdFilial;
            servicoContratado.IdProdutoRM = servicoMigradoDTO.CodProdutoRm;
            servicoContratado.DataAlteracao = DateTime.Now;
            servicoContratado.IdGrupoDelivery = servicoMigradoDTO.IdGrupoDelivery;
            servicoContratado.IdFrenteNegocio = servicoMigradoDTO.IdFrenteNegocio;
        }

        private static void CriarVinculoCelulaComercial(ServicoMigracaoDTO servicoMigradoDTO, ServicoContratado servicoContratado, string usuario)
        {
            foreach (var idServicoComercial in servicoMigradoDTO.CelulasServicosComerciaisAgrupados)
            {
                servicoContratado.VinculoServicoCelulaComercial.Add(new VinculoServicoCelulaComercial
                {
                    DataInicial = servicoMigradoDTO.DtInicioContrato.Value,
                    IdCelulaComercial = idServicoComercial,
                    DataAlteracao = DateTime.Now,
                    Usuario = usuario
                });
            }
        }

        private static void CriarVinculoMarkup(ServicoMigracaoDTO servicoMigradoDTO, ServicoContratado servicoContratado, string usuario)
        {
            servicoContratado.VinculoMarkupServicosContratados.Add(new VinculoMarkupServicoContratado
            {
                DtInicioVigencia = servicoMigradoDTO.DtInicioContrato.Value,
                VlMarkup = servicoMigradoDTO.MarkUp ?? 0,
                DataAlteracao = DateTime.Now,
                Usuario = usuario
            });
        }

        private static void CriarVinculoDePara(ServicoMigracaoDTO servicoMigradoDTO, ServicoContratado servicoContratado, string usuario)
        {
            foreach (var idServico in servicoMigradoDTO.IdsServicosAgrupados)
            {
                servicoContratado.DeParaServicos.Add(new DeParaServico
                {
                    DescStatus = "MA",
                    DescTipoServico = servicoMigradoDTO.IdTipoCelula == 1 ? "COM" : servicoMigradoDTO.IdTipoCelula == 3 ? "TEC" : "OUT",
                    IdServicoEacesso = idServico,
                    DataAlteracao = DateTime.Now,
                    Usuario = usuario
                });
            }
        }
    }
}
