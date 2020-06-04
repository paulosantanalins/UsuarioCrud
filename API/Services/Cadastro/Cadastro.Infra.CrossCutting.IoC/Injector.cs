using Cadastro.Domain.CelulaRoot.Service;
using Cadastro.Domain.CelulaRoot.Service.Interfaces;
using Cadastro.Domain.CidadeRoot.Repository;
using Cadastro.Domain.CidadeRoot.Service;
using Cadastro.Domain.CidadeRoot.Service.Interfaces;
using Cadastro.Domain.DominioRoot.Repository;
using Cadastro.Domain.DominioRoot.Service;
using Cadastro.Domain.DominioRoot.Service.Interfaces;
using Cadastro.Domain.EmpresaGrupoRoot.Repository;
using Cadastro.Domain.EmpresaGrupoRoot.Service;
using Cadastro.Domain.EmpresaGrupoRoot.Service.Interfaces;
using Cadastro.Domain.EnderecoRoot.Repository;
using Cadastro.Domain.FilialRoot.Service;
using Cadastro.Domain.FilialRoot.Service.Interfaces;
using Cadastro.Domain.HorasMesRoot.Repository;
using Cadastro.Domain.HorasMesRoot.Service;
using Cadastro.Domain.HorasMesRoot.Service.Interfaces;
using Cadastro.Domain.IntegracaoRoot.Service;
using Cadastro.Domain.IntegracaoRoot.Service.Interfaces;
using Cadastro.Domain.LinkRoot.Repository;
using Cadastro.Domain.LinkRoot.Service;
using Cadastro.Domain.LinkRoot.Service.Interfaces;
using Cadastro.Domain.NatcorpRoot.Repository;
using Cadastro.Domain.NotaFiscalRoot.Repository;
using Cadastro.Domain.NotaFiscalRoot.Service;
using Cadastro.Domain.NotaFiscalRoot.Service.Interfaces;
using Cadastro.Domain.PessoaRoot.Repository;
using Cadastro.Domain.PessoaRoot.Service;
using Cadastro.Domain.PessoaRoot.Service.Interfaces;
using Cadastro.Domain.PluginRoot.Service;
using Cadastro.Domain.PluginRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.RmRoot.Service;
using Cadastro.Domain.RmRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Cadastro.Domain.TelefoneRoot.Repository;
using Cadastro.Infra.Data.SqlServer;
using Cadastro.Infra.Data.SqlServer.Repository;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso;
using Logger.Context;
using Logger.Repository;
using Logger.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Http;
using Utils;

namespace Cadastro.Infra.CrossCutting.IoC
{
    public class Injector
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static void RegisterServices(IServiceCollection services)
        {
            //UnitOfWork
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Domain Service
            services.AddScoped<ICidadeService, CidadeService>();
            services.AddScoped<IEmpresaGrupoService, EmpresaGrupoService>();
            services.AddScoped<IEnderecoService, EnderecoService>();
            services.AddScoped<IPrestadorService, PrestadorService>();
            services.AddScoped<IDominioService, DominioService>();
            services.AddScoped<IHorasMesService, HorasMesService>();
            services.AddScoped<IHorasMesPrestadorService, HorasMesPrestadorService>();
            services.AddScoped<IEmpresaService, EmpresaService>();
            services.AddScoped<IEmpresaPrestadorService, EmpresaPrestadorService>();
            services.AddScoped<IPluginRMService, PluginRMService>();
            services.AddScoped<IFilialService, FilialService>();
            services.AddScoped<INotaFiscalService, NotaFiscalService>();
            services.AddScoped<IInativacaoPrestadorService, InativacaoPrestadorService>();
            services.AddScoped<IDescontoPrestadorService, DescontoPrestadorService>();
            services.AddScoped<IMinioService, MinioService>();
            services.AddScoped<IClienteServicoPrestadorService, ClienteServicoPrestadorService>();
            services.AddScoped<ILinkService, LinkService>();
            services.AddScoped<IRmService, RmService>();
            services.AddScoped<IValorPrestadorService, ValorPrestadorService>();
            services.AddScoped<IPessoaService, PessoaService>();
            services.AddScoped<IContratoPrestadorService, ContratoPrestadorService>();
            services.AddScoped<IGrupoService, GrupoService>();
            services.AddScoped<IDocumentoPrestadorService, DocumentoPrestadorService>();
            services.AddScoped<IIntegracaoService,  IntegracaoService>();
            services.AddScoped<ITransferenciaCltPjService, TransferenciaCltPjService>();
            services.AddScoped<ITransferenciaPrestadorService, TransferenciaPrestadorService>();
            services.AddScoped<IFinalizacaoContratoService, FinalizacaoContratoService>();
            services.AddScoped<IReajusteContratoService, ReajusteContratoService>();
            services.AddScoped<IViewProfissionaisService, ViewProfissionaisService>();

            //Infra Data            
            services.AddScoped<IEstadoRepository, EstadoRepository>();
            services.AddScoped<IPaisRepository, PaisRepository>();
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddScoped<ICidadeRepository, CidadeRepository>();
            services.AddScoped<IPrestadorRepository, PrestadorRepository>();
            services.AddScoped<IEmpresaPrestadorRepository, EmpresaPrestadorRepository>();
            services.AddScoped<IDominioRepository, DominioRepository>();
            services.AddScoped<IHorasMesRepository, HorasMesRepository>();
            services.AddScoped<ICelulaRepository, CelulaRepository>();
            services.AddScoped<IHorasMesPrestadorRepository, HorasMesPrestadorRepository>();
            services.AddScoped<ITelefoneRepository, TelefoneRepository>();
            services.AddScoped<IPeriodoDiaPagamentoRepository, PeriodoDiaPagamentoRepository>();
            services.AddScoped<ILogHorasMesPrestadorRepository, LogHorasMesPrestadorRepository>();
            services.AddScoped<IEmpresaRepository, EmpresaRepository>();
            services.AddScoped<IAbreviaturaLogradouroRepository, AbreviaturaLogradouroRepository>();
            services.AddScoped<IPrestadorEnvioNfRepository, PrestadorEnvioNfRepository>();
            services.AddScoped<IInativacaoPrestadorRepository, InativacaoPrestadorRepository>();
            services.AddScoped<IDescontoPrestadorRepository, DescontoPrestadorRepository>();
            services.AddScoped<IValorPrestadorRepository, ValorPrestadorRepository>();
            services.AddScoped<IValorPrestadorBeneficioRepository, ValorPrestadorBeneficioRepository>();
            services.AddScoped<IClienteServicoPrestadorRepository, ClienteServicoPrestadorRepository>();
            services.AddScoped<ILinkRepository, LinkRepository>();
            services.AddScoped<IPessoaRepository, PessoaRepository>();
            services.AddScoped<IContratoPrestadorRepository, ContratoPrestadorRepository>();
            services.AddScoped<IObservacaoPrestadorRepository, ObservacaoPrestadorRepository>();
            services.AddScoped<IGrupoRepository, GrupoRepository>();
            services.AddScoped<IDocumentoPrestadorRepository, DocumentoPrestadorRepository>();
            services.AddScoped<IExtensaoContratoPrestadorService, ExtensaoContratoPrestadorService>();
            services.AddScoped<IExtensaoContratoPrestadorRepository, ExtensaoContratoPrestadorRepository>();
            services.AddScoped<ITransferenciaPrestadorRepository, TransferenciaPrestadorRepository>();
            services.AddScoped<ISocioEmpresaPrestadorRepository, SocioEmpresaPrestadorRepository>();
            services.AddScoped<ITransferenciaCltPjRepository, TransferenciaCltPjRepository>();
            services.AddScoped<ILogTransferenciaPrestadorRepository, LogTransferenciaPrestadorRepository>();
            services.AddScoped<IFinalizacaoContratoRepository, FinalizacaoContratoRepository>();
            services.AddScoped<IProfissionalNatcorpRepository, ProfissionalNatcorpRepository>();
            services.AddScoped<IReajusteContratoRepository, ReajusteContratoRepository>();

            //Globais
            services.AddScoped<Variables>();

            //log
            services.AddScoped<LogGenericoContext>();
            services.AddScoped<ILogGenericoRepository, LogGenericoRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

            services.AddScoped<IVariablesToken, VariablesToken>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
