using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Api.ViewModels.GestaoPortifolio;
using GestaoServico.Api.ViewModels.GestaoRespasse;
using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using GestaoServico.Domain.GestaoFilialRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using System.Linq;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Api.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<CategoriaContabil, CategoriaContabilVM>();
            CreateMap<TipoServico, TipoServicoVM>();
            CreateMap<ClassificacaoContabil, ClassificacaoContabilVM>().AfterMap((src, dest) => dest.DescCategoria = src.CategoriaContabil?.DescCategoria);
            CreateMap<PortfolioServico, PortfolioServicoVM>();
            CreateMap<Contrato, ContratoVM>()
                .ForMember(dest => dest.IdCelulaComercial, opt => opt.MapFrom(src => src.ServicoContratados.Any(x => x.DescTipoCelula == "COM") ? src.ServicoContratados.Where(x => x.DescTipoCelula == "COM").OrderByDescending(y => y.Id).FirstOrDefault().IdCelula : 0));

            CreateMap<ProdutoRMEacesso, ComboDefaultVM>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdPrd))
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao));      

            CreateMap<Contrato, ComboContratoVM>();
            CreateMap<Empresa, EmpresaVM>();
            CreateMap<Repasse, SolicitarRepasseVM>()
                .ForMember(x => x.IdCelOrigem, opt => opt.MapFrom(x => x.ServicoContratadoOrigem.IdCelula))
                .ForMember(x => x.IdCelDestino, opt => opt.MapFrom(x => x.ServicoContratadoDestino.IdCelula))
                .ForMember(x => x.IdClienteOrigem, opt => opt.MapFrom(x => x.ServicoContratadoOrigem.Contrato.ClientesContratos.FirstOrDefault().IdCliente))
                .ForMember(x => x.IdClienteDestino, opt => opt.MapFrom(x => x.ServicoContratadoDestino.Contrato.ClientesContratos.FirstOrDefault().IdCliente))
                ;
            CreateMap<ServicoContratado, ServicoContratadoVM>()
                .ForMember(x => x.IdPortifolioServico, opt => opt.MapFrom(x => x.EscopoServico.IdPortfolioServico))
                .ForMember(x => x.IdCliente, opt => opt.MapFrom(x => x.Contrato.ClientesContratos.FirstOrDefault().IdCliente))
                .ForMember(x => x.DataInicialContrato, opt => opt.MapFrom(x => x.Contrato.DtInicial))
                .ForMember(x => x.DataFinalContrato, opt => opt.MapFrom(x => x.Contrato.DtFinalizacao))
                .ForMember(x => x.IdMoedaContrato, opt => opt.MapFrom(x => x.Contrato.IdMoeda))
                .ForMember(x => x.IdCliente, opt => opt.MapFrom(x => x.Contrato.ClientesContratos.FirstOrDefault().IdCliente))
                .ForMember(dest => dest.IdCelulaComercial, opt => opt.MapFrom(src => src.Contrato.ServicoContratados.Any(x => x.DescTipoCelula == "COM") ? src.Contrato.ServicoContratados.Where(x => x.DescTipoCelula == "COM").OrderByDescending(y => y.Id).FirstOrDefault().IdCelula : 0))
                .ForMember(dest => dest.IdCelula, opt => opt.MapFrom(src => src.IdCelula))
                .ForMember(dest => dest.VlMarkup, opt => opt.MapFrom(src => src.VinculoMarkupServicosContratados.LastOrDefault().VlMarkup))
                .ForMember(dest => dest.DescEscopo, opt => opt.MapFrom(src => src.IdEscopoServico != null ? src.EscopoServico.NmEscopoServico : src.DeParaServicos.FirstOrDefault().DescEscopo))
                .ReverseMap();

            CreateMap<DeParaServico, DeParaViewModel>();
        }
    }
}
