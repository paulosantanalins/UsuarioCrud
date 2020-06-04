using System;
using AutoMapper;
using Cadastro.Domain.DominioRoot.Dto;
using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;

namespace Cadastro.Api.AutoMapper
{
    public class DtoToDomainMappingProfile : Profile
    {
        public DtoToDomainMappingProfile()
        {
            CreateMap<EmpresaPrestadorDto, EmpresaPrestador>()
                .ForPath(x => x.Empresa.Ativo, opt => opt.MapFrom(x => x.Ativo))
                .ForPath(x => x.Empresa.IdEmpresaRm, opt => opt.MapFrom(x => x.IdEmpresaRm))
                .ForPath(x => x.Empresa.Atuacao, opt => opt.MapFrom(x => x.Atuacao))                
                .ForPath(x => x.Empresa.Cnpj, opt => opt.MapFrom(x => x.CNPJ))
                .ForPath(x => x.Empresa.DataVigencia, opt => opt.MapFrom(x => x.DataVigencia))
                .ForPath(x => x.Empresa.InscricaoEstadual, opt => opt.MapFrom(x => x.InscricaoEstadual))
                .ForPath(x => x.Empresa.Observacao, opt => opt.MapFrom(x => x.Observacao))
                .ForPath(x => x.Empresa.RazaoSocial, opt => opt.MapFrom(x => x.RazaoSocial))
                .ForPath(x => x.Empresa.Socios, opt => opt.MapFrom(x => x.Socios))
                .ForPath(x => x.Empresa.Endereco.NmBairro, opt => opt.MapFrom(x => x.Bairro))
                .ForPath(x => x.Empresa.Endereco.NrCep, opt => opt.MapFrom(x => x.Cep))
                .ForPath(x => x.Empresa.Endereco.NmCompEndereco, opt => opt.MapFrom(x => x.Complemento))
                .ForPath(x => x.Empresa.Endereco.NmEndereco, opt => opt.MapFrom(x => x.Endereco))
                .ForPath(x => x.Empresa.Endereco.IdCidade, opt => opt.MapFrom(x => x.IdCidade))
                .ForPath(x => x.Empresa.Endereco.NomeCidade, opt => opt.MapFrom(x => x.NmCidade))
                .ForPath(x => x.Empresa.Endereco.NrEndereco, opt => opt.MapFrom(x => x.Numero))
                .ForPath(x => x.Empresa.Endereco.SgAbrevLogradouro, opt => opt.MapFrom(x => x.Tipo));

            CreateMap<ValorPrestadorBeneficioEacessoDto, ValorPrestadorBeneficio>()
                .ForMember(x => x.Id, opt => opt.MapFrom(x => 0));

            CreateMap<ContratoPrestadorDto, ContratoPrestador>();

            CreateMap<DominioDto, Dominio>()
                .ForMember(dest => dest.ValorTipoDominio, opt => opt.MapFrom(src => src.GrupoDominio))
                .ForMember(dest => dest.DescricaoValor, opt => opt.MapFrom(src => src.ValorDominio))
                .ForMember(dest => dest.IdValor, opt => opt.MapFrom(src => src.IdValor))
                .ForMember(dest => dest.Ativo, opt => opt.MapFrom(src => true));

            CreateMap<PrestadorParaTransferenciaDto, TransferenciaPrestador>();

            CreateMap<FinalizacaoContratoDto, FinalizacaoContrato>()
                .ForMember(dest => dest.DataInclusao, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DataFimContrato,
                    opt => opt.MapFrom(src => src.FinalizarImediatamente ? DateTime.Now : src.UltimoDiaTrabalho))
                .ForMember(dest => dest.RetornoPermitido, opt => opt.MapFrom(src => !src.DesabilitarContratosFuturos));

            CreateMap<ValoresContratoPrestadorModel, ReajusteContrato>()
                .ForMember(dest => dest.QuantidadeHorasContrato, opt => opt.MapFrom(src => src.QuantidadeNova))
                .ForMember(dest => dest.ValorContrato, opt => opt.MapFrom(src => src.ValorNovo))
                .ForMember(dest => dest.DataReajuste, opt => opt.MapFrom(src => src.DataReajuste));
        }
    }
}
