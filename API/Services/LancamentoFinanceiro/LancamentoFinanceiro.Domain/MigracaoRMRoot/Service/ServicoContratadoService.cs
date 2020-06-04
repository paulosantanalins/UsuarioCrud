using System;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using LancamentoFinanceiro.Domain.Core.Notifications;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Validators;
using LancamentoFinanceiro.Domain.MigracaoRMRoot.Service.Interface;
using Microsoft.Extensions.Options;
using Utils;
using Utils.Connections;
using Utils.EacessoLegado.Service;
using Utils.Salesforce.Models;
using Utils.StfAnalitcsDW.Model;

namespace LancamentoFinanceiro.Domain.MigracaoRMRoot.Service
{
    public class ServicoContratadoService : IServicoContratadoService
    {
        private readonly IVariablesToken _variables;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IContratoRepository _contratoRepository;
        private readonly NotificationHandler _notificationHandler;
        private readonly IServicoContratadoRepository _servicoContratadoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly ICidadeRepository _cidadeRepository;
        private readonly IMapper _mapper;

        public ServicoContratadoService(IVariablesToken variables, IUnitOfWork unitOfWork,
            IContratoRepository contratoRepository, NotificationHandler notificationHandler,
            IServicoContratadoRepository servicoContratadoRepository, IClienteRepository clienteRepository,
            IOptions<ConnectionStrings> connectionStrings, ICidadeRepository cidadeRepository, IMapper mapper)
        {
            _variables = variables;
            _unitOfWork = unitOfWork;
            _contratoRepository = contratoRepository;
            _notificationHandler = notificationHandler;
            _servicoContratadoRepository = servicoContratadoRepository;
            _clienteRepository = clienteRepository;
            _connectionStrings = connectionStrings;
            _cidadeRepository = cidadeRepository;
            _mapper = mapper;
        }

        public int PersistirServicoEacesso(ViewServicoModel viewServico)
        {
            var servico = _mapper.Map<ServicoContratado>(viewServico);

            servico.EscopoServico = null;

            servico.DeParaServicos.Add(new DeParaServico
            {
                DescStatus = "MA",
                DescTipoServico = viewServico.SiglaTipoServico,
                NmServicoEacesso = viewServico.NomeServico,
                DescEscopo = viewServico.DescEscopo,
                DescSubTipoServico = viewServico.SubTipo,
                IdServicoEacesso = viewServico.IdServico,
                DtAlteracao = DateTime.Now,
                LgUsuario = "Eacesso"
            });
            viewServico.IdCliente = ObterClienteEacessoPorId(viewServico.IdCliente);
            servico.IdContrato = VerificarExistenciaContratoEacesso(viewServico.IdContrato, viewServico.IdCliente);

            var idServico = Persistir(servico, servico.IdCelula, viewServico.Markup);

            return idServico;
        }

        private int VerificarExistenciaContratoEacesso(string contrato, int idCliente)
        {
            var id = int.TryParse(contrato, out var idContrato);
            if (id)
            {
                var contratoDB = _contratoRepository.BuscarPorId(idContrato);
                if (contratoDB != null)
                {
                    return idContrato;
                }
            }
            var idContratoDefault = _contratoRepository.VerificarContratoDefaultPorIdCliente(idCliente);
            
            return idContratoDefault != 0 ? idContratoDefault : CriarContratoPadraoEacesso(contrato);
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
                    VlMarkup = vlMarkup ?? 0
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

        private int CriarContratoPadraoEacesso(string contrato)
        {
            _variables.UserName = "EACESSO";
            var newContrato = new Contrato
            {
                IdMoeda = 1,
                DtInicial = new DateTime(1990, 1, 1),
                DtFinalizacao = new DateTime(2020, 1, 1),
                DescStatusSalesForce = contrato ?? "",
                DescContrato = "Contrato default",
                LgUsuario = "Eacesso",
                DtAlteracao = DateTime.Now.Date,
            };

            _contratoRepository.Adicionar(newContrato);
            _unitOfWork.Commit();
            return newContrato.Id;
        }

        private int ObterClienteEacessoPorId(int idCliente)
        {
            var clienteExistente = _clienteRepository.BuscarPorId(idCliente) != null;
            if (clienteExistente)
            {
                return idCliente;
            }

            var pendente = false;
            var salesObject = new AccountSalesObject();
            var clienteEacessoService = new ClienteEacessoService(_connectionStrings.Value.EacessoConnection);
            var clienteEacesso = clienteEacessoService.ObterClienteEacessoPorId(idCliente);
            if (clienteEacesso != null)
            {
                salesObject = _mapper.Map<AccountSalesObject>(clienteEacesso);
            }
            if (salesObject.CNPJ__c != null && salesObject.CNPJ__c.Trim() != "")
            {
                salesObject.CNPJ__c = ApenasNumeros(salesObject.CNPJ__c);
            }
            else
            {
                pendente = true;
                //await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui cnpj", DescExcecao = "" });
                salesObject.CNPJ__c = ApenasNumeros("NWM9709244W4");
            }

            ClienteET clienteStfCorp = _mapper.Map<ClienteET>(salesObject);
            if (clienteStfCorp.Id == 0)
            {
                clienteStfCorp.Id = VerificarIdCliente();
            }

            var endereco = _mapper.Map<Endereco>(salesObject);

            if (clienteStfCorp.NrTelefone != null)
            {
                clienteStfCorp.NrTelefone = clienteStfCorp.NrTelefone.Replace(" ", "");
                if (clienteStfCorp.NrTelefone.Contains('/'))
                {
                    var numeros = clienteStfCorp.NrTelefone.Split("/");
                    clienteStfCorp.NrTelefone = numeros[0];
                    clienteStfCorp.NrTelefone2 = numeros[0].Substring(0, 2) + numeros[1];
                }
            }
            if (clienteStfCorp.NrTelefone2 != null)
            {
                clienteStfCorp.NrTelefone = clienteStfCorp.NrTelefone2.Replace(" ", "");
            }

            if (clienteStfCorp.NmRazaoSocial == null || !clienteStfCorp.NmRazaoSocial.Any())
            {
                clienteStfCorp.NmRazaoSocial = "";
                pendente = true;
                //await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui uma razão social", DescExcecao = "" });
            }
            if (clienteStfCorp.NmFantasia == null || !clienteStfCorp.NmFantasia.Any())
            {
                clienteStfCorp.NmFantasia = "";
                pendente = true;
                //await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui um nome fantasia", DescExcecao = "" });
            }
            endereco.IdCidade = VerificarEndereco(salesObject.CNPJ_Cidade__c);

            clienteStfCorp.Enderecos.Add(endereco);
            if (endereco.IdCidade == null)
            {
                pendente = true;
                //await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui uma cidade valida", DescExcecao = "" });
            }

            if (!ValidarCNPJ(clienteStfCorp.NrCnpj, "BRASIL"))
            {
                pendente = true;
                //await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui um cnpj valido", DescExcecao = "" });
            }

            clienteStfCorp.FlStatus = pendente ? "P" : "A";

            salesObject.IdStfCorp = PersistirCliente(clienteStfCorp);
            return salesObject.IdStfCorp;
        }

        private string ApenasNumeros(string str)
        {
            var apenasDigitos = new Regex(@"[^\d]");
            return apenasDigitos.Replace(str, "");
        }

        private int VerificarIdCliente()
        {
            var result = _clienteRepository.VerificarIdCliente() + 1 + 20000;
            return result;
        }

        private int? VerificarEndereco(string cidade)
        {
            if (cidade != null)
            {
                var cidadeDb = _cidadeRepository.BuscarUnicoPorParametro(x => x.NmCidade.ToUpper() == cidade.ToUpper());
                if (cidadeDb != null)
                {
                    return cidadeDb.Id;
                }
            }
            return null;
        }

        public bool ValidarCNPJ(string cnpj, string pais)
        {
            bool cnpjValido;
            if (pais == "BRASIL" || pais == "BRAZIL")
            {
                cnpjValido = ValidarCNPJ(cnpj);
            }
            else
            {
                cnpjValido = true;
            }

            return cnpjValido;
        }

        private bool ValidarCNPJ(string cnpj)
        {
            var multiplicador1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var soma = 0;

            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            if (cnpj.Length != 14)
                return false;

            var tempCnpj = cnpj.Substring(0, 12);
            for (var i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            var resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            var digito = resto.ToString();
            tempCnpj += digito;
            soma = 0;

            for (var i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito += resto;
            return cnpj.EndsWith(digito);
        }

        private int PersistirCliente(ClienteET cliente)
        {
            if (string.IsNullOrEmpty(_variables.UserName))
            {
                _variables.UserName = "salesforce";
            }
            cliente.LgUsuario = _variables.UserName;
            _clienteRepository.Adicionar(cliente);
            _unitOfWork.Commit();
            return cliente.Id;
        }
    }
}
