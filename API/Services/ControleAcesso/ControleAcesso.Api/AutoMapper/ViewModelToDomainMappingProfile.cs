using AutoMapper;
using ControleAcesso.Api.ViewModels;
using ControleAcesso.Api.ViewModels.ControleAcesso;
using ControleAcesso.Domain.BroadcastRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<BroadcastVM, Broadcast>();
            CreateMap<Perfil, PerfilVM>().ReverseMap();
            CreateMap<VinculoPerfilFuncionalidade, VinculoPerfilFuncionalidadeVM>().ReverseMap();
            CreateMap<GrupoVM, Grupo>()
            .ForMember(x => x.DescGrupo, opt => opt.MapFrom(x => x.Descricao));
        }
    }
}
