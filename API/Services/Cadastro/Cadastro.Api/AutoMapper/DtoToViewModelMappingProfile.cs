using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.CidadeRoot.Dto;
using Cadastro.Domain.EmpresaGrupoRoot.Dto;

namespace Cadastro.Api.AutoMapper
{
    public class DtoToViewModelMappingProfile : Profile
    {
        public DtoToViewModelMappingProfile()
        {
            CreateMap<CidadeDto, CidadeGridVM>();

            CreateMap<DadosBancariosDaEmpresaGrupoDto, DadosFinanceiroEmpresaVM>()
                .ForMember(dest => dest.Conta, opt => opt.MapFrom(src => $"{src.Conta}-{(src.DigitoDaConta != null ? src.DigitoDaConta : "0")}"));
                
        }
    }
}
