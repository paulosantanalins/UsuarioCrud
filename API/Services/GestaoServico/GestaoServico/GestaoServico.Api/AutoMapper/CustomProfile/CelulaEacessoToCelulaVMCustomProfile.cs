using AutoMapper;
using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class CelulaEacessoToCelulaVMCustomProfile : Profile
    {
        public CelulaEacessoToCelulaVMCustomProfile()
        {
            CreateMap<CelulaEacesso, CelulaVM>()
                .ForMember(src => src.Id, opt => opt.MapFrom(x => x.IdCelula))
                .ForMember(src => src.DescCelula, opt => opt.MapFrom(x => x.Descricao));
        }
    }
}
