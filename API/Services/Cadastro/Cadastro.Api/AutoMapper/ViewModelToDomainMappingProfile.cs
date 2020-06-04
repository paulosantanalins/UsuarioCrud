using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.NotaFiscalRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.TelefoneRoot.Entity;
using System;
using Utils;

namespace Cadastro.Api.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<PrestadorVM, Prestador>()
              .ForMember(x => x.IdPessoa, opt => opt.MapFrom(x => x.IdPessoa))
              .ForPath(x => x.Pessoa.Id, opt => opt.MapFrom(x => x.IdPessoa))
              .ForPath(x => x.Pessoa.Nome, opt => opt.MapFrom(x => x.Nome))
              .ForPath(x => x.Pessoa.Cpf, opt => opt.MapFrom(x => x.Cpf))
              .ForPath(x => x.Pessoa.Rg, opt => opt.MapFrom(x => x.Rg))
              .ForPath(x => x.Pessoa.DtNascimento, opt => opt.MapFrom(x => x.DataNascimento))
              .ForPath(x => x.Pessoa.NomeDoPai, opt => opt.MapFrom(x => x.Pai))
              .ForPath(x => x.Pessoa.NomeDaMae, opt => opt.MapFrom(x => x.Mae))
              .ForPath(x => x.Pessoa.Email, opt => opt.MapFrom(x => x.Email))
              .ForPath(x => x.Pessoa.EmailInterno, opt => opt.MapFrom(x => x.EmailInterno))
              .ForPath(x => x.Pessoa.IdNacionalidade, opt => opt.MapFrom(x => x.IdNacionalidade))
              .ForPath(x => x.Pessoa.CodEacessoLegado, opt => opt.MapFrom(x => x.CodEacessoLegado))
              .ForPath(x => x.Pessoa.IdEscolaridade, opt => opt.MapFrom(x => x.IdEscolaridade))
              .ForPath(x => x.Pessoa.IdExtensao, opt => opt.MapFrom(x => x.IdExtensao))
              .ForPath(x => x.Pessoa.IdGraduacao, opt => opt.MapFrom(x => x.IdGraduacao))
              .ForPath(x => x.Pessoa.IdEstadoCivil, opt => opt.MapFrom(x => x.IdEstadoCivil))
              .ForPath(x => x.Pessoa.IdSexo, opt => opt.MapFrom(x => x.IdSexo))
              .ForPath(x => x.Pessoa.IdEndereco, opt => opt.MapFrom(x => x.IdEndereco))
              .ForPath(x => x.Pessoa.IdTelefone, opt => opt.MapFrom(x => x.IdTelefone))
              .ForPath(x => x.Pessoa.Telefone, opt => opt.MapFrom(x => x.Telefone))
              .ForPath(x => x.Pessoa.Endereco, opt => opt.MapFrom(x => x.Endereco))
              //.ForPath(x => x.Pessoa.Usuario, opt => opt.MapFrom(x => Variables.UserName ?? "STFCORP"))
              .ForPath(x => x.Pessoa.DataAlteracao, opt => opt.MapFrom(x => DateTime.Now));



            CreateMap<EnderecoVM, Endereco>();

            CreateMap<TelefoneVM, Telefone>();

            CreateMap<PeriodoLimiteVM, PeriodoDiaPagamento>()
                //.ForMember(x => x.Usuario, opt => opt.MapFrom(x => Variables.UsuarioToken))
                .ForMember(x => x.DataAlteracao, opt => opt.MapFrom(x => DateTime.Now));

            CreateMap<HorasMesVM, HorasMes>()
                .ForMember(x => x.PeriodosDiaPagamento, opt => opt.MapFrom(x => x.DiasPagamento));

            CreateMap<PrestadorHoraVM, HorasMesPrestador>()
                .ForMember(x => x.IdPrestador, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => 0));
            
            CreateMap<CidadeVM, Cidade>()
                .ForPath(x => x.Estado.Id, opt => opt.MapFrom(x => x.IdEstado))
                .ForPath(x => x.Estado.IdPais, opt => opt.MapFrom(x => x.IdPais));
                
            CreateMap<EmpresaVM, Empresa>()
                .ForPath(x => x.Endereco, opt => opt.MapFrom(x => x));

            CreateMap<EmpresaVM, EmpresaPrestador>()
                 .ForPath(x => x.Empresa, opt => opt.MapFrom(x => x))
                 .ForPath(x => x.IdEmpresa, opt => opt.MapFrom(x => x.Id))
                 .ForPath(x => x.IdPrestador, opt => opt.MapFrom(x => x.IdPrestador))
                 .ForPath(x => x.Id, opt => opt.MapFrom(x => x.IdRelacional))
                 //.ForPath(x => x.Empresa.Usuario, opt => opt.MapFrom(x => Variables.UsuarioToken))
                 .ForPath(x => x.Empresa.DataAlteracao, opt => opt.MapFrom(x => DateTime.Now));

            CreateMap<EmpresaVM, Endereco>()
                .ForPath(x => x.Id, opt => opt.MapFrom(x => x.IdEndereco))
                .ForPath(x => x.IdCidade, opt => opt.MapFrom(x => x.IdCidade))
                .ForPath(x => x.NrEndereco, opt => opt.MapFrom(x => x.Numero))
                .ForPath(x => x.NmEndereco, opt => opt.MapFrom(x => x.DescricaoEndereco))
                .ForPath(x => x.NmCompEndereco, opt => opt.MapFrom(x => x.Complemento))
                .ForPath(x => x.NrCep, opt => opt.MapFrom(x => x.Cep))
                .ForPath(x => x.NmBairro, opt => opt.MapFrom(x => x.Bairro))                
                .ForPath(x => x.SgAbrevLogradouro, opt => opt.MapFrom(x => x.AbrevLogradouro));

            CreateMap<InativacaoPrestadorVM, InativacaoPrestador>()
                .ForPath(x => x.Id, opt => opt.MapFrom(x => x.Id))
                .ForPath(x => x.IdPrestador, opt => opt.MapFrom(x => x.IdPrestador))
                .ForPath(x => x.FlagIniciativaDesligamento, opt => opt.MapFrom(x => x.FlagIniciativaDesligamento))
                .ForPath(x => x.FlagRetorno, opt => opt.MapFrom(x => x.FlagRetorno))
                .ForPath(x => x.DataDesligamento, opt => opt.MapFrom(x => x.DataDesligamento))
                .ForPath(x => x.Motivo, opt => opt.MapFrom(x => x.Motivo));

            CreateMap<AbreviaturaLogradouroVM, AbreviaturaLogradouro>()
               .ForPath(x => x.Id, opt => opt.MapFrom(x => x.Id))
               .ForPath(x => x.Sigla, opt => opt.MapFrom(x => x.Sigla))
               .ForPath(x => x.Descricao, opt => opt.MapFrom(x => x.Descricao));

            CreateMap<DescontoPrestadorVM, DescontoPrestador>()
                 .ForPath(x => x.DescricaoDesconto, opt => opt.MapFrom(x => x.Observacao));

            CreateMap<ValorPrestadorVM, ValorPrestador>()
                .ForPath(x => x.Id, opt => opt.MapFrom(x => x.Id))
                .ForPath(x => x.IdPrestador, opt => opt.MapFrom(x => x.IdPrestador))
                .ForPath(x => x.DataAlteracao, opt => opt.MapFrom(x => x.DataAlteracao ?? DateTime.Now))
                .ForPath(x => x.DataReferencia, opt => opt.MapFrom(x => x.DataReferencia))
                .ForPath(x => x.IdMoeda, opt => opt.MapFrom(x => x.IdMoeda))
                .ForPath(x => x.IdTipoRemuneracao, opt => opt.MapFrom(x => x.IdTipoRemuneracao))
                .ForPath(x => x.Quantidade, opt => opt.MapFrom(x => x.Quantidade))
                .ForPath(x => x.ValorClt, opt => opt.MapFrom(x => x.ValorClt))
                .ForPath(x => x.ValorPericulosidade, opt => opt.MapFrom(x => x.ValorPericulosidade))
                .ForPath(x => x.ValorHora, opt => opt.MapFrom(x => x.ValorHora))
                .ForPath(x => x.ValorMes, opt => opt.MapFrom(x => x.ValorMes));
                //.ForPath(x => x.Usuario, opt => opt.MapFrom(x => Variables.UsuarioToken));


            CreateMap<PrestadorEnvioNfVM, PrestadorEnvioNf>()
                .ForPath(x => x.HorasMesPrestador, opt => opt.Ignore());
            CreateMap<ClienteServicoPrestadorVM, ClienteServicoPrestador>();
            CreateMap<ContratoPrestadorVM, ContratoPrestador>();
            CreateMap<ExtensaoContratoPrestadorVM, ExtensaoContratoPrestador>();
            CreateMap<DocumentoPrestadorVM, DocumentoPrestador>();
            CreateMap<ObservacaoPrestadorVM, ObservacaoPrestador>();
            CreateMap<DominioVM, Dominio>();
        }
    }
}
