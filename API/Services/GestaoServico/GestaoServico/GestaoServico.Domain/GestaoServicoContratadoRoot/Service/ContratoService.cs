using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Repository;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using GestaoServico.Domain.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utils;
using Utils.Base;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Service
{
    public class ContratoService : IContratoService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IVariablesToken _variables;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IContratoRepository _contratoRepository;

        public ContratoService(IContratoRepository contratoRepository,
                               IUnitOfWork unitOfWork,
                               MicroServicosUrls microServicosUrls,
                               IVariablesToken variables)
        {
            _contratoRepository = contratoRepository;
            _microServicosUrls = microServicosUrls;
            _unitOfWork = unitOfWork;
            _variables = variables;
        }
        public bool VerificarExistenciaPorNumeroContrato(string nrContrato)
        {
            var result = _contratoRepository.VerificarExistenciaPorNumeroContrato(nrContrato);

            return result;
        }

        public Contrato ObterContratoPorNumeroContrato(string nrContrato)
        {
            var result = _contratoRepository.ObterContratoPorNumeroContrato(nrContrato);
            return result;
        }

        public FiltroGenericoDto<ContratoDto> Filtrar(FiltroGenericoDto<ContratoDto> filtroDto)
        {
            var result = _contratoRepository.Filtrar(filtroDto);
            return result;
        }

        public IEnumerable<Contrato> ObterTodos()
        {
            return _contratoRepository.BuscarTodosReadOnly();
        }

        public ICollection<Contrato> ObterContratosPorCliente(int idCliente)
        {
            var result = _contratoRepository.BuscarContratos(idCliente);
            return result;
        }

        public ICollection<MultiselectDto> PopularComboClientePorCelula(int idCelula)
        {
            var result = _contratoRepository.BuscarIdsClientePorIdCelula(idCelula);
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(120);
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiCliente);            

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/cliente/ObterClientesPorIds", content).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<MultiselectDto>>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                return null;
            }
        }

        public Contrato ObterContratoComServicosContratados(int idContrato)
        {
            return _contratoRepository.BuscarContratoComServicosContratados(idContrato);
        }

        public void PersistirInformacoesContrato(int idContrato, int idMoeda, int idCelula, DateTime dtInicial, DateTime? dtFinalizacao)
        {
            var contratoDb = _contratoRepository.BuscarContratoComVinculos(idContrato);

            if (!contratoDb.ServicoContratados.Any(x => x.DescTipoCelula == "COM"))
            {
                contratoDb.ServicoContratados.Add(new ServicoContratado
                {
                    IdContrato = contratoDb.Id,
                    IdCelula = idCelula,
                    DtInicial = dtInicial,
                    DtFinal = dtFinalizacao.Value,
                    DescTipoCelula = "COM",
                    Usuario = _variables.UserName,
                    DataAlteracao = DateTime.Now,
                    //vao sair 
                    IdEscopoServico = 1
                });
            }
            contratoDb.DtFinalizacao = dtFinalizacao;
            contratoDb.DtInicial = dtInicial;
            contratoDb.IdMoeda = idMoeda;
            _contratoRepository.Update(contratoDb);
            _unitOfWork.Commit();
        }

        public int VerificarExistenciaContratoEacesso(string contrato, int idCliente)
        {
            var id = int.TryParse(contrato, out int idContrato);
            if (id)
            {
                var contratoDB = _contratoRepository.BuscarPorId(idContrato);
                if (contratoDB != null)
                {
                    return idContrato;
                }
            }
            var idContratoDefault = _contratoRepository.VerificarContratoDefaultPorIdCliente(idCliente);
            if (idContratoDefault != 0)
            {
                return idContratoDefault;
            }
            else
            {
                return CriarContratoPadraoEacesso(contrato, idCliente);
            }
        }

        private int CriarContratoPadraoEacesso(string contrato, int idCliente)
        {
            _variables.UserName = "EACESSO";
            var newContrato = new Contrato
            {
                IdMoeda = 1,
                DtInicial = new DateTime(1990, 1, 1),
                DtFinalizacao = new DateTime(2020, 1, 1),
                DescStatusSalesForce = contrato ?? "",
                DescContrato = "Contrato default",
                Usuario = "Eacesso",
                DataAlteracao = DateTime.Now.Date,
            };

            _contratoRepository.Adicionar(newContrato);
            _unitOfWork.Commit();
            return newContrato.Id;
        }
    }
}
