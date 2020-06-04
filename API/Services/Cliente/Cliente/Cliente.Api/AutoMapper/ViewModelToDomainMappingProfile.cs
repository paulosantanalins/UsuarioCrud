using AutoMapper;
using Cliente.Api.ViewModels;
using Cliente.Domain.ClienteRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.Api.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ClienteVM, ClienteET>();
            CreateMap<CidadeVM, Cidade>();
            CreateMap<GrupoClienteVM, GrupoCliente>();
            CreateMap<EnderecoVM, Endereco>();
        }
    }
}
