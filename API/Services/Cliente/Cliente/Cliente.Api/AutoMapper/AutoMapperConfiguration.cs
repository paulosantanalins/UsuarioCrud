using AutoMapper;
using Cliente.Api.AutoMapper.CustomMaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.Api.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(ps =>
            {
                ps.AddProfile(new ClienteSFVMToClienteETCustomProfile());
                ps.AddProfile(new ClienteSFVMToEnderecoCustomProfile());
                ps.AddProfile(new AccountSalesObjectToEnderecoCustomProfile());
                ps.AddProfile(new AccountSalesObjectToClienteETCustomProfile());

                ps.AddProfile(new DomainToViewModelMappingProfile());
                ps.AddProfile(new ViewModelToDomainMappingProfile());
                ps.AddProfile(new ViewModelToDtoMappingProfile());
                ps.AddProfile(new DtoToViewModelMappingProfile());
            });
        }
    }
}
