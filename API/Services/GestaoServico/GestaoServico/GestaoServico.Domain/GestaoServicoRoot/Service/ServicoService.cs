
using GestaoServico.Domain.GestaoCelulaRoot.Entity;
using GestaoServico.Domain.GestaoContratoRoot.Entity;
using GestaoServico.Domain.GestaoContratoRoot.Repository;
using GestaoServico.Domain.GestaoServicoRoot.Entity;
using GestaoServico.Domain.GestaoServicoRoot.Repository;
using GestaoServico.Domain.GestaoServicoRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoVinculoClienteRoot.Entity;
using Logger.Model;
using Logger.Repository.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GestaoServico.Domain.GestaoServicoRoot.Service
{
    public class ServicoService : IServicoService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IContratoRepository _contratoRepository;
        private readonly ILogGenericoRepository _logGenericoRepository;

        public ServicoService(IServiceRepository serviceRepository,
                              IContratoRepository contratoRepository,
                              ILogGenericoRepository logGenericoRepository)
        {
            _serviceRepository = serviceRepository;
            _contratoRepository = contratoRepository;
            _logGenericoRepository = logGenericoRepository;
        }

        public async Task PersistirService(Servico servico)
        {
            await _serviceRepository.PersistirServico(servico);
        }

        public async Task CriarServicos(VinculoClienteServico vinculoClienteServico, string codContrato, string siglaTipoServico, int idCelula, string cnpjFilial, int idCliente, int? idCelulaDelivery)
        {
            Contrato contrato = null;
            TipoServico tipoServico = null;
            Filial filial = null;
            ServicoPai servicoPai = null;

            var vinculoExistente = await VerificarExistenciaServico(vinculoClienteServico);

            if (codContrato != null && codContrato != "")
            {
                contrato = await _contratoRepository.ObterContratoPorCodigo(codContrato);
            }

            if (siglaTipoServico != null && siglaTipoServico != "")
            {
                tipoServico = await _serviceRepository.ObterTipoServicoPorSigla(siglaTipoServico);
            }

            if (cnpjFilial != null && cnpjFilial != "")
            {
                filial = await _contratoRepository.ObterFilialComEmpresa(cnpjFilial);
                if (filial != null)
                {
                    vinculoClienteServico.IdFilial = filial.Id;
                    vinculoClienteServico.IdEmpresa = filial.IdEmpresa;
                }
            }
            if (vinculoExistente == null)
            {
                servicoPai = CriarServicoPai(vinculoClienteServico);
            }
            else
            {

                servicoPai = vinculoExistente.Servico.ServicoPai;
            }
            var combinada = CriarCombinada();

            //Comercial
            servicoPai.Servicos.Add(await CriarServico(vinculoClienteServico, idCelula, contrato, tipoServico, combinada, false));

            if (idCelulaDelivery != null && idCelulaDelivery != 0)
            {
                //Tecnica
                servicoPai.Servicos.Add(await CriarServico(vinculoClienteServico, idCelulaDelivery, contrato, tipoServico, combinada, true));
            }
            try
            {
                servicoPai.DescServicoPai = "TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT";
                await _serviceRepository.PersistirServicoPai(servicoPai);
                var logInserirServico = new { Tipo = vinculoExistente == null ? "Inclusao" : "Alteracao", NomeServico = vinculoClienteServico.NmClienteServico, SalesId = vinculoClienteServico.IdSalesForce, };
                await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.TRANSACAO.ToString(), NmOrigem = Origem.SALESFORCE.ToString(), DescLogGenerico = logInserirServico.Tipo + " de dados Servico: " + JsonConvert.SerializeObject(logInserirServico), DescExcecao = "", DtHoraLogGenerico = DateTime.Now });
            }
            catch (Exception e)
            {
                var logInserirServico = new { Tipo = vinculoExistente == null ? "Inclusao" : "Alteracao", NomeServico = vinculoClienteServico.NmClienteServico, SalesId = vinculoClienteServico.IdSalesForce, };
                await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.TRANSACAO.ToString(), NmOrigem = Origem.SALESFORCE.ToString(), DescLogGenerico = "Falha " + logInserirServico.Tipo + " de dados Servico: " + JsonConvert.SerializeObject(logInserirServico), DescExcecao = e.StackTrace, DtHoraLogGenerico = DateTime.Now });
            }

        }

        private async Task<VinculoClienteServico> VerificarExistenciaServico(VinculoClienteServico vinculoClienteServico)
        {
            if (vinculoClienteServico.IdSalesForce == null)
            {
                return null;
            }

            var vinculoRelacionado = await _serviceRepository.VerificarExistenciaDeServicoPorIdSalesForce(vinculoClienteServico.IdSalesForce);
            return vinculoRelacionado;

        }

        private  VinculoCombinadaServico CriarCombinada()
        {
            VinculoCombinadaServico combinada = new VinculoCombinadaServico();
            combinada.FlStatus = "A";
            combinada.DtMigracao = new DateTime();
            return combinada;
        }

        private async Task<Servico> CriarServico(VinculoClienteServico vinculoClienteServico, int? idCelula, Contrato contrato, TipoServico tipoServico, VinculoCombinadaServico combinada, bool tecnica)
        {
            Servico servico = new Servico();
            servico.FlMigrado = "N";
            servico.FlStatus = "A";
            servico.VinculoCelulaServicos.Add(new VinculoCelulaServico() { IdCelula = idCelula.Value });
            if (contrato != null)
            {
                servico.VinculoContratoServicos.Add(new VinculoContratoServico() { IdContrato = contrato.Id });
            }
            if (tipoServico != null)
            {
                if (!tecnica)
                {
                    servico.VinculoServicoTipoServicos.Add(new VinculoServicoTipoServico() { IdTipoServico = tipoServico.Id });
                }
                else
                {
                    //buscar no banco servico tipo ACO
                    var servicoAco = await _serviceRepository.ObterTipoServicoPorSigla("ACO");
                    servico.VinculoServicoTipoServicos.Add(new VinculoServicoTipoServico() { IdTipoServico = servicoAco.Id });
                }
            }
            
            servico.VinculoClienteServicos.Add(vinculoClienteServico);
            servico.VinculoCombinadaServicos.Add(combinada);
            return servico;
        }

        private ServicoPai CriarServicoPai(VinculoClienteServico vinculoClienteServico)
        {
            var servicoPai = new ServicoPai() { NmServicoPai = vinculoClienteServico.NmClienteServico, DescServicoPai = vinculoClienteServico.DescClienteServico };

            return servicoPai;
        }
    }
}
