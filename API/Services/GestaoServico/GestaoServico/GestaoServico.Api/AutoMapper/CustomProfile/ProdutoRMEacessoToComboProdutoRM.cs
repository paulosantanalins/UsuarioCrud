using AutoMapper;
using GestaoServico.Api.ViewModels;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class ProdutoRMEacessoToComboProdutoRM : Profile
    {
        public ProdutoRMEacessoToComboProdutoRM()
        {
            CreateMap<ProdutoRMEacesso, ComboProdutoRM>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdPrd))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Descricao));
        }
    }
}
