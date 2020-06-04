using System;
using AutoMapper;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using Utils.EacessoLegado.Models;
using Utils.Salesforce.Models;
using Utils.StfAnalitcsDW.Model;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.AutoMapper
{
    public class DtoToDomainMapping : Profile
    {
        public DtoToDomainMapping()
        {
            CreateMap<ViewServicoModel, ServicoContratado>()
                .ForMember(dest => dest.DtFinal, opt => opt.MapFrom(src => src.DtFimVigencia ?? DateTime.Now))
                .ForMember(dest => dest.DtInicial, opt => opt.MapFrom(src => src.DtInicioVigencia))
                .ForMember(dest => dest.DescTipoCelula, opt => opt.MapFrom(src => src.DescTipoCelula == "COMERCIAL" ? "COM" : "TEC"))
                .ForMember(dest => dest.FlReembolso, opt => opt.MapFrom(src => src.FlReembolso == 1))
                .ForMember(dest => dest.QtdExtraReembolso, opt => opt.MapFrom(src => src.QtdExtraReembolso ?? 0))
                .ForMember(dest => dest.NmTipoReembolso, opt => opt.MapFrom(src => src.NmTipoReembolso == 0 ? null : src.NmTipoReembolso == 1 ? "Nota de serviço" : "Nota de débito"))
                .ForMember(dest => dest.FlHorasExtrasReembosaveis, opt => opt.MapFrom(src => src.FlHorasExtrasReembolsaveis == 1))
                .ForMember(dest => dest.VlRentabilidade, opt => opt.MapFrom(src => src.VlRentabilidadePrevista))
                .ForMember(dest => dest.FlReoneracao, opt => opt.MapFrom(src => src.FlReoneracao == 1))
                .ForMember(dest => dest.FlFaturaRecorrente, opt => opt.MapFrom(src => src.FlFaturaRecorrente == 1))
                .ForMember(dest => dest.IdProdutoRM, opt => opt.MapFrom(src => src.NrProdutoRM))
                .ForMember(dest => dest.FormaFaturamento, opt => opt.MapFrom(src => src.DescFormaFaturamento))
                .ForMember(dest => dest.IdContrato, opt => opt.Ignore());

            CreateMap<ClienteEacesso, AccountSalesObject>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SalesForceID))
                .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.SiglaSetorEconomico))
                .ForMember(dest => dest.Ramo__c, opt => opt.MapFrom(src => src.SiglaRamoAtividade))
                .ForMember(dest => dest.RazaoSocial__c, opt => opt.MapFrom(src => src.RazaoSocial))
                .ForMember(dest => dest.NomeFantasia__c, opt => opt.MapFrom(src => src.NomeFantasia))
                .ForMember(dest => dest.CNPJ__c, opt => opt.MapFrom(src => src.CNPJ))
                //Endereco
                .ForMember(src => src.CNPJ_CEP__c, opt => opt.MapFrom(x => x.Cep))
                .ForMember(src => src.CNPJ_Bairro__c, opt => opt.MapFrom(x => x.Bairro))
                .ForMember(src => src.CNPJ_Complemento__c, opt => opt.MapFrom(x => x.CompEndereco))
                .ForMember(src => src.CNPJ_Endereco_Numero__c, opt => opt.MapFrom(x => x.Numero))
                .ForMember(src => src.CNPJ_Logradouro__c, opt => opt.MapFrom(x => x.Endereco))
                .ForMember(src => src.TipoDeEndereco__c, opt => opt.MapFrom(x => x.AbrevLogradouro))
                .ForMember(src => src.CNPJ_Cidade__c, opt => opt.MapFrom(x => x.Cidade))
                .ForMember(src => src.CNPJ_Estado__c, opt => opt.MapFrom(x => x.SiglaEstado))
                .ForMember(src => src.PaisConta__c, opt => opt.MapFrom(x => x.SiglaPais))
                //valores mantidos
                .ForMember(dest => dest.Type, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Website, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.StatusConta__c, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Phone_2__c, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.ParentId, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Name, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.LastModifiedDate, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.IdGrupoCliente, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Fax, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.E_MAIL__c, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Description, opt => opt.UseDestinationValue())
                //tirar duvida
                .ForMember(dest => dest.InscricaoMunicipal__c, opt => opt.MapFrom(src => src.InscrMunicipal))
                .ForMember(dest => dest.InscricaoEstadual__c, opt => opt.MapFrom(src => src.InscrEstadual))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Telefone));

            CreateMap<AccountSalesObject, ClienteET>()
                .ForMember(dest => dest.FlTipoHierarquia, opt => opt.MapFrom(src => src.Type == "Filha" ? "F" : "M"))
                .ForMember(dest => dest.IdSalesforce, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NrInscricaoEstadual, opt => opt.MapFrom(src => src.InscricaoEstadual__c))
                .ForMember(dest => dest.NrInscricaoMunicipal, opt => opt.MapFrom(src => src.InscricaoMunicipal__c))
                .ForMember(dest => dest.NmEmail, opt => opt.MapFrom(src => src.E_MAIL__c))
                .ForMember(dest => dest.NmSite, opt => opt.MapFrom(src => src.Website))
                .ForMember(dest => dest.NrFax, opt => opt.MapFrom(src => src.Fax))
                .ForMember(dest => dest.FlStatus, opt => opt.MapFrom(src => src.StatusConta__c == "Ativo" ? "A" : "I"))
                .ForMember(dest => dest.NrTelefone, opt => opt.MapFrom(src => src.Phone.Trim()))
                .ForMember(dest => dest.NrTelefone2, opt => opt.MapFrom(src => src.Phone_2__c.Trim()))
                .ForMember(dest => dest.NmRazaoSocial, opt => opt.MapFrom(src => src.RazaoSocial__c))
                .ForMember(dest => dest.NmFantasia, opt => opt.MapFrom(src => src.NomeFantasia__c ?? ""))
                .ForMember(dest => dest.NrCnpj, opt => opt.MapFrom(src => src.CNPJ__c))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdCliente));

            CreateMap<AccountSalesObject, Endereco>()
                .ForMember(src => src.NrCep, opt => opt.MapFrom(x => x.CNPJ_CEP__c))
                .ForMember(src => src.NmBairro, opt => opt.MapFrom(x => x.CNPJ_Bairro__c))
                .ForMember(src => src.NmCompEndereco, opt => opt.MapFrom(x => x.CNPJ_Complemento__c))
                .ForMember(src => src.NrEndereco, opt => opt.MapFrom(x => x.CNPJ_Endereco_Numero__c))
                .ForMember(src => src.NmEndereco, opt => opt.MapFrom(x => x.CNPJ_Logradouro__c))
                .ForMember(src => src.SgAbrevLogradouro, opt => opt.MapFrom(x => x.TipoDeEndereco__c))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
