using AutoMapper;
using FluentScheduler;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using GestaoServico.Infra.CrossCutting.IoC;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace GestaoServico.Api.Jobs
{
    public class CotacoesBACENJob : IJob
    {
        private StreamReader stream;
        private string fileMemory;
        public void Execute()
        {
            DownloadCSVBACEN();

            //var _repasseMigracaoService = RecuperarRepasseMigracaoService();

            //var repassesEacesso = _repasseMigracaoService.BuscarRepassesEAcesso("01-01-2019", "30-06-2019");
            //var repasses = Mapper.Map<List<Repasse>>(repassesEacesso);

            //_repasseMigracaoService.MigrarRepassesEacesso(repasses);
        }

        private void DownloadCSVBACEN()
        {

            var dataAtual = DateTime.Now;

            var dataDiaAnterior = dataAtual.AddDays(-1).ToString("yyyyMMdd");

            string link = "https://www4.bcb.gov.br/Download/fechamento/" + dataDiaAnterior + ".csv";

            using (var client = new WebClient())
            {
                byte[] dataBuffer = client.DownloadData(link);
                MemoryStream Memorystream = new MemoryStream(dataBuffer);
                StreamReader stream = new StreamReader(Memorystream);
                fileMemory = stream.ReadToEnd();
                stream.Close();
                stream.Dispose();
            }

            
        }
        
        //private static IRepasseMigracaoService RecuperarRepasseMigracaoService()
        //{
        //    //var repasseMigracaoService = Injector.ServiceProvider.GetService(typeof(IRepasseMigracaoService)) as IRepasseMigracaoService;
        //    //return repasseMigracaoService;
        //}
    }
}
