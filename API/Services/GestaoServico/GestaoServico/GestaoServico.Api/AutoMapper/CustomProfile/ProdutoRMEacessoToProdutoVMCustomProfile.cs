using AutoMapper;
using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class ProdutoRMEacessoToProdutoVMCustomProfile : Profile
    {
        public ProdutoRMEacessoToProdutoVMCustomProfile()
        {
            CreateMap<ProdutoRMEacesso, ProdutoVM>().ReverseMap();
        }
    }
}
