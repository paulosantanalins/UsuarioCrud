using AutoMapper;
using Cliente.Api.ViewModels;
using Cliente.Domain.ClienteRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Cliente.Api.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<ClienteET, ClienteVM>();
            CreateMap<Cidade, CidadeVM>();
            CreateMap<GrupoCliente, GrupoClienteVM>();
            CreateMap<Endereco, EnderecoVM>().AfterMap((src, dest) => dest.Cliente = null);
            //CreateMap<Usuario, UsuarioVM>();
            CreateMap<FiltroGenericoViewModel<ClienteVM>, FiltroGenericoDto<ClienteET>>().ReverseMap();
        }
    }
}
