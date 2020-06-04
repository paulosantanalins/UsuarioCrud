using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.NotaFiscalRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System;
using System.Globalization;
using System.Linq;
using Utils.Extensions;

namespace Cadastro.Api.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<PeriodoDiaPagamento, PeriodoLimiteVM>()
                .ForPath(x => x.DescDiaPagamento, opt => opt.MapFrom(x => x.DiaPagamento.DescricaoValor));

            CreateMap<HorasMes, HorasMesVM>()
                .ForMember(x => x.Periodo, opt => opt.MapFrom(x => new DateTime(x.Ano, x.Mes, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("pt")).ToUpper() + "/" + x.Ano))
                .ForPath(x => x.DiasPagamento, opt => opt.MapFrom(x => x.PeriodosDiaPagamento));

            CreateMap<Cidade, CidadeGridVM>()
                .ForPath(x => x.NmEstado, opt => opt.MapFrom(x => x.Estado.NmEstado))
                .ForPath(x => x.IdPais, opt => opt.MapFrom(x => x.Estado.IdPais))
                .ForPath(x => x.NmPais, opt => opt.MapFrom(x => x.Estado.Pais.NmPais));

            CreateMap<Cidade, CidadeVM>()
                .ForPath(x => x.NmEstado, opt => opt.MapFrom(x => x.Estado.NmEstado))
                .ForPath(x => x.IdPais, opt => opt.MapFrom(x => x.Estado.IdPais))
                .ForPath(x => x.NmPais, opt => opt.MapFrom(x => x.Estado.Pais.NmPais));

            CreateMap<Pais, PaisVM>();

            CreateMap<Pais, ComboDefaultVM>()
                .ForMember(x => x.Descricao, opt => opt.MapFrom(x => x.NmPais));

            CreateMap<Prestador, ComboDefaultVM>()
                .ForMember(x => x.Descricao, opt => opt.MapFrom(x => x.Pessoa.Nome));

            CreateMap<Prestador, ComboDescontoPrestadorVM>()
                .ForMember(x => x.Descricao, opt => opt.MapFrom(x => x.Pessoa.Nome))
                .ForMember(x => x.PodeReceberDesconto, opt => opt.MapFrom(x => x.HorasMesPrestador
                .Any(y => y.Situacao == SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_PENDENTE.GetDescription() ||
                          y.Situacao == SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_CADASTRADAS.GetDescription() ||
                          y.Situacao == SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_RECADASTRADAS.GetDescription() ||
                          y.Situacao == SharedEnuns.TipoSituacaoHorasMesPrestador.NOTA_FISCAL_SOLICITADA.GetDescription() ||
                          y.Situacao == SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_APROVADAS.GetDescription())));

            CreateMap<Estado, ComboDefaultVM>()
                .ForMember(x => x.Descricao, opt => opt.MapFrom(x => x.NmEstado));

            CreateMap<Cidade, ComboDefaultVM>()
                .ForMember(x => x.Descricao, opt => opt.MapFrom(x => x.NmCidade));

            CreateMap<Prestador, PrestadorVM>()
                .ForPath(x => x.IdPessoa, opt => opt.MapFrom(x => x.Pessoa.Id))
                .ForPath(x => x.Nome, opt => opt.MapFrom(x => x.Pessoa.Nome))
                .ForPath(x => x.Cpf, opt => opt.MapFrom(x => x.Pessoa.Cpf))
                .ForPath(x => x.Rg, opt => opt.MapFrom(x => x.Pessoa.Rg))
                .ForPath(x => x.DataNascimento, opt => opt.MapFrom(x => x.Pessoa.DtNascimento))
                .ForPath(x => x.Pai, opt => opt.MapFrom(x => x.Pessoa.NomeDoPai))
                .ForPath(x => x.Mae, opt => opt.MapFrom(x => x.Pessoa.NomeDaMae))
                .ForPath(x => x.Email, opt => opt.MapFrom(x => x.Pessoa.Email))
                .ForPath(x => x.EmailInterno, opt => opt.MapFrom(x => x.Pessoa.EmailInterno))
                .ForPath(x => x.IdNacionalidade, opt => opt.MapFrom(x => x.Pessoa.IdNacionalidade))
                .ForPath(x => x.IdEscolaridade, opt => opt.MapFrom(x => x.Pessoa.IdEscolaridade))
                .ForPath(x => x.IdExtensao, opt => opt.MapFrom(x => x.Pessoa.IdExtensao))
                .ForPath(x => x.IdGraduacao, opt => opt.MapFrom(x => x.Pessoa.IdGraduacao))
                .ForPath(x => x.IdEstadoCivil, opt => opt.MapFrom(x => x.Pessoa.IdEstadoCivil))
                .ForPath(x => x.IdSexo, opt => opt.MapFrom(x => x.Pessoa.IdSexo))
                .ForPath(x => x.IdEndereco, opt => opt.MapFrom(x => x.Pessoa.IdEndereco))
                .ForPath(x => x.IdTelefone, opt => opt.MapFrom(x => x.Pessoa.IdTelefone))

                .ForPath(x => x.Endereco, opt => opt.MapFrom(x => x.Pessoa.Endereco))
                .ForPath(x => x.Telefone, opt => opt.MapFrom(x => x.Pessoa.Telefone))

                .ForPath(x => x.DescricaoAreaFormacao, opt => opt.MapFrom(x => x.AreaFormacao.DescricaoValor))
                .ForPath(x => x.DescricaoCargo, opt => opt.MapFrom(x => x.Cargo.DescricaoValor))
                .ForPath(x => x.DescricaoContratacao, opt => opt.MapFrom(x => x.Contratacao.DescricaoValor))
                .ForPath(x => x.DescricaoDiaPagamento, opt => opt.MapFrom(x => x.DiaPagamento.DescricaoValor))
                .ForPath(x => x.DescricaoEscolaridade, opt => opt.MapFrom(x => x.Pessoa.Escolaridade.DescricaoValor))
                .ForPath(x => x.DescricaoEstadoCivil, opt => opt.MapFrom(x => x.Pessoa.EstadoCivil.DescricaoValor))
                .ForPath(x => x.DescricaoExtensao, opt => opt.MapFrom(x => x.Pessoa.Extensao.DescricaoValor))
                .ForPath(x => x.DescricaoGraduacao, opt => opt.MapFrom(x => x.Pessoa.Graduacao.DescricaoValor))
                .ForPath(x => x.DescricaoNacionalidade, opt => opt.MapFrom(x => x.Pessoa.Nacionalidade.DescricaoValor))
                .ForPath(x => x.DescricaoSexo, opt => opt.MapFrom(x => x.Pessoa.Sexo.DescricaoValor))
                .ForPath(x => x.DescricaoSituacao, opt => opt.MapFrom(x => x.SituacaoPrestador.DescricaoValor))
                .ForPath(x => x.Empresas, opt => opt.MapFrom(x => x.EmpresasPrestador.OrderByDescending(y => y.Empresa.Ativo)))
                .ForPath(x => x.ValoresPrestador, opt => opt.MapFrom(x => x.ValoresPrestador.OrderByDescending(y => y.DataAlteracao).ToList()))
                .ForPath(x => x.Inativacoes, opt => opt.MapFrom(x => x.InativacoesPrestador))
                .ForPath(x => x.ClientesServicosPrestador, opt => opt.MapFrom(x => x.ClientesServicosPrestador.OrderByDescending(y => y.Ativo)))
                .ForPath(x => x.DescricaoRemuneracao, opt => opt.MapFrom(x => x.TipoRemuneracao.DescricaoValor))
                .ForPath(x => x.DescricaoRemuneracao, opt => opt.MapFrom(x => x.TipoRemuneracao.DescricaoValor))
                .ForPath(x => x.ObservacoesPrestador, opt => opt.MapFrom(x => x.ObservacoesPrestador));

            CreateMap<PrestadorEnvioNf, PrestadorEnvioNfVM>()
                .ForPath(x => x.Responsavel, opt => opt.MapFrom(x => x.HorasMesPrestador.Prestador.EmpresasPrestador.Select(y => y.Empresa).Any() ?
                                    x.HorasMesPrestador.Prestador.EmpresasPrestador.Select(y => y.Empresa).FirstOrDefault().RazaoSocial :
                                    x.HorasMesPrestador.Prestador.Pessoa.Nome))
                .ForPath(x => x.DiaLimiteEnvioNF, opt => opt.MapFrom(x => x.HorasMesPrestador.Prestador.DiaPagamento.PeriodosDiaPagamento
                                    .FirstOrDefault(y => y.IdPeriodo == x.HorasMesPrestador.IdHorasMes).DiaLimiteEnvioNF))
                .ForPath(x => x.ValorNf, opt => opt.Ignore());

            CreateMap<Endereco, EnderecoVM>()
                .ForMember(x => x.DescricaoCidade, opt => opt.MapFrom(x => x.Cidade.NmCidade))
                .ForMember(x => x.DescricaoEstado, opt => opt.MapFrom(x => x.Cidade.Estado.SgEstado))
                .ForMember(x => x.DescricaoPais, opt => opt.MapFrom(x => x.Cidade.Estado.Pais.SgPais))
                .ForMember(x => x.DescricaoAbrevLogradouro, opt => opt.MapFrom(x => x.AbreviaturaLogradouro.Descricao))
                .ForMember(x => x.IdEstado, opt => opt.MapFrom(x => x.Cidade.IdEstado));

            CreateMap<EmpresaPrestador, EmpresaVM>()
               .ForPath(x => x.IdCidade, opt => opt.MapFrom(x => x.Empresa.Endereco.IdCidade))
               .ForPath(x => x.DescricaoCidade, opt => opt.MapFrom(x => x.Empresa.Endereco.Cidade.NmCidade))
               .ForPath(x => x.IdEstado, opt => opt.MapFrom(x => x.Empresa.Endereco.Cidade.Estado.Id))
               .ForPath(x => x.DescricaoEstado, opt => opt.MapFrom(x => x.Empresa.Endereco.Cidade.Estado.SgEstado))
               .ForPath(x => x.IdPais, opt => opt.MapFrom(x => x.Empresa.Endereco.Cidade.Estado.Pais.Id))
               .ForPath(x => x.DescricaoPais, opt => opt.MapFrom(x => x.Empresa.Endereco.Cidade.Estado.Pais.SgPais))
               .ForPath(x => x.Numero, opt => opt.MapFrom(x => x.Empresa.Endereco.NrEndereco))
               .ForPath(x => x.DescricaoEndereco, opt => opt.MapFrom(x => x.Empresa.Endereco.NmEndereco))
               .ForPath(x => x.Complemento, opt => opt.MapFrom(x => x.Empresa.Endereco.NmCompEndereco))
               .ForPath(x => x.Cep, opt => opt.MapFrom(x => x.Empresa.Endereco.NrCep))
               .ForPath(x => x.Bairro, opt => opt.MapFrom(x => x.Empresa.Endereco.NmBairro))
               .ForPath(x => x.AbrevLogradouro, opt => opt.MapFrom(x => x.Empresa.Endereco.SgAbrevLogradouro))
               .ForPath(x => x.IdEndereco, opt => opt.MapFrom(x => x.Empresa.IdEndereco))

               .ForPath(x => x.Usuario, opt => opt.MapFrom(x => x.Empresa.Usuario))
               .ForPath(x => x.DataAlteracao, opt => opt.MapFrom(x => x.Empresa.DataAlteracao))
               .ForPath(x => x.Id, opt => opt.MapFrom(x => x.Empresa.Id))
               .ForPath(x => x.IdEmpresaRm, opt => opt.MapFrom(x => x.Empresa.IdEmpresaRm))
               .ForPath(x => x.Cnpj, opt => opt.MapFrom(x => x.Empresa.Cnpj))
               .ForPath(x => x.RazaoSocial, opt => opt.MapFrom(x => x.Empresa.RazaoSocial))
               .ForPath(x => x.DataVigencia, opt => opt.MapFrom(x => x.Empresa.DataVigencia))
               .ForPath(x => x.InscricaoEstadual, opt => opt.MapFrom(x => x.Empresa.InscricaoEstadual))
               .ForPath(x => x.Atuacao, opt => opt.MapFrom(x => x.Empresa.Atuacao))
               .ForPath(x => x.Socios, opt => opt.MapFrom(x => x.Empresa.Socios))
               .ForPath(x => x.Socios, opt => opt.MapFrom(x => x.Empresa.Socios))
               .ForPath(x => x.Observacao, opt => opt.MapFrom(x => x.Empresa.Observacao))
               .ForPath(x => x.Ativo, opt => opt.MapFrom(x => x.Empresa.Ativo));

            CreateMap<Empresa, EmpresaVM>()
               .ForPath(x => x.IdCidade, opt => opt.MapFrom(x => x.Endereco.IdCidade))
               .ForPath(x => x.DescricaoCidade, opt => opt.MapFrom(x => x.Endereco.Cidade.NmCidade))
               .ForPath(x => x.IdEstado, opt => opt.MapFrom(x => x.Endereco.Cidade.Estado.Id))
               .ForPath(x => x.DescricaoEstado, opt => opt.MapFrom(x => x.Endereco.Cidade.Estado.SgEstado))
               .ForPath(x => x.IdPais, opt => opt.MapFrom(x => x.Endereco.Cidade.Estado.Pais.Id))
               .ForPath(x => x.DescricaoPais, opt => opt.MapFrom(x => x.Endereco.Cidade.Estado.Pais.SgPais))
               .ForPath(x => x.Numero, opt => opt.MapFrom(x => x.Endereco.NrEndereco))
               .ForPath(x => x.DescricaoEndereco, opt => opt.MapFrom(x => x.Endereco.NmEndereco))
               .ForPath(x => x.Complemento, opt => opt.MapFrom(x => x.Endereco.NmCompEndereco))
               .ForPath(x => x.Cep, opt => opt.MapFrom(x => x.Endereco.NrCep))
               .ForPath(x => x.Bairro, opt => opt.MapFrom(x => x.Endereco.NmBairro))
               //.ForPath(x => x.Referencia, opt => opt.MapFrom(x => x.Endereco.Referencia))
               .ForPath(x => x.AbrevLogradouro, opt => opt.MapFrom(x => x.Endereco.SgAbrevLogradouro))
               .ForPath(x => x.IdEndereco, opt => opt.MapFrom(x => x.Endereco.Id));

            CreateMap<ValorPrestador, ValorPrestadorVM>();

            CreateMap<ValorPrestadorBeneficio, ValorPrestadorBeneficioVM>()
                .ForMember(x => x.IdValorPrestador, opt => opt.MapFrom(x => x.IdValorPrestador))
                .ForMember(x => x.ValorBeneficio, opt => opt.MapFrom(x => x.ValorBeneficio))
                .ForMember(x => x.IdBeneficio, opt => opt.MapFrom(x => x.IdBeneficio))
                .ForMember(x => x.descricaoBeneficio, opt => opt.MapFrom(x => x.Beneficio.DescricaoValor));

            CreateMap<ClienteServicoPrestador, ClienteServicoPrestadorVM>();
            CreateMap<DocumentoPrestador, DocumentoPrestadorVM>();

            CreateMap<ContratoPrestador, ContratoPrestadorVM>()
                 .ForMember(x => x.Status, opt => opt.MapFrom(x => x.DataFim < DateTime.Now ? "Encerrado" : x.DataFim > DateTime.Now && x.DataInicio <= DateTime.Now ? "Vigente" : "Previsto"));

            CreateMap<ExtensaoContratoPrestador, ExtensaoContratoPrestadorVM>()
                 .ForMember(x => x.Status, opt => opt.MapFrom(x => x.DataFim < DateTime.Now ? "Encerrado" : x.DataFim > DateTime.Now && x.DataInicio <= DateTime.Now ? "Vigente" : "Previsto"));

            CreateMap<ObservacaoPrestador, ObservacaoPrestadorVM>()
                .ForMember(x => x.DescricaoTipoOcorrencia, opt => opt.MapFrom(x => x.TipoOcorrencia.DescricaoValor));


        }
    }
}
