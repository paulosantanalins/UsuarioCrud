using AutoMapper;
using GestaoServico.Api.AutoMapper.CustomProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(ps =>
            {
                ps.AddProfile(new DomainToViewModelMappingProfile());
                ps.AddProfile(new DtoToViewModelMappingProfile());
                ps.AddProfile(new ViewModelToDomainMappingProfile());
                ps.AddProfile(new ViewModelToDTOMappingProfile());
                ps.AddProfile(new SalesObjectToDomainMapping());

                #region Custom Profile
                ps.AddProfile(new ServicoIntegracaoVMParaServicoContratadoCustomProfile());
                ps.AddProfile(new EmpresaEacessoToEmpresaVMCustomProfile());
                ps.AddProfile(new FilialEacessoToFilialVMCustomProfile());
                ps.AddProfile(new CelulaEacessoToCelulaVMCustomProfile());
                ps.AddProfile(new ProdutoRMEacessoToProdutoVMCustomProfile());
                ps.AddProfile(new ProdutoRMEacessoToComboProdutoRM());
                ps.AddProfile(new ServicoContratadoToMultiselectViewModelCustomProfile());
                ps.AddProfile(new ProfissionalEacessoToMultiselectViewModel());
                ps.AddProfile(new ProfissionalEacessoToProfissionalEAcessoMultiSelectViewModel());
                ps.AddProfile(new EpmRepasseToRepasseCustomProfile());
                ps.AddProfile(new FiltroAprovarRepasseVMToFiltroAprovarRepasseDTOCustomProfile());
                ps.AddProfile(new ViewServiceModelToServicoContratadoCustomProfile());
                #endregion
            });
        }
    }
}
