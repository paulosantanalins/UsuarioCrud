﻿using AutoMapper;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using GestaoServico.Infra.CrossCutting.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.EacessoLegado.Models;
using Utils.EacessoLegado.Service;
using Utils.StfAnalitcsDW.Model;

namespace GestaoServico.Api.AutoMapper.Resolvers
{
    public class ServicoOrigemResolve : IValueResolver<EAcessoRepasse, Repasse, int>
    {
        public int Resolve(EAcessoRepasse source, Repasse destination, int destMember, ResolutionContext context)
        {
            return 0;
            var servicosEacessoService = new ClienteServicoEacessoService("Data Source=10.161.69.101\\CORP_H;Initial Catalog=STFCORP_FENIX;User ID=stfcorp_fenix;Password=\"noRPH|dt*;\"");
            var serviceSevicoContratadoDePara = RecuperarImportacaoServiceDePara();
            var serviceSevicoContratado = RecuperarImportacaoServicoContratadoService();

            try
            {
                var idOrigem = serviceSevicoContratadoDePara.BuscarIdServicoContratadoPorIdServicoEacesso(source.IdServicoContratadoOrigem);
                if (idOrigem != 0)
                    return idOrigem;
                else
                {

                    var servicoEacesso = ObterServicoEacesso(source.IdServicoContratadoOrigem, servicosEacessoService);
                    var servico = Mapper.Map<ServicoContratado>(servicoEacesso);
                    return serviceSevicoContratado.PersistirServicoEacesso(servico, servicoEacesso.IdServico, servicoEacesso.NomeServico, servicoEacesso.IdCliente, servicoEacesso.Markup, servicoEacesso.IdContrato, servicoEacesso.DescEscopo, servicoEacesso.SubTipo);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private ViewServicoModel ObterServicoEacesso(int idServicoEacesso, ClienteServicoEacessoService servicoEacessoService)
        {
            return servicoEacessoService.ObterServicoPorId(idServicoEacesso);
        }

        private static IDeParaServicoService RecuperarImportacaoServiceDePara()
        {
            var importacaoService = Injector.ServiceProvider.GetService(typeof(IDeParaServicoService)) as IDeParaServicoService;
            return importacaoService;
        }

        private static IServicoContratadoService RecuperarImportacaoServicoContratadoService()
        {
            var importacaoService = Injector.ServiceProvider.GetService(typeof(IServicoContratadoService)) as IServicoContratadoService;
            return importacaoService;
        }
    }
}
