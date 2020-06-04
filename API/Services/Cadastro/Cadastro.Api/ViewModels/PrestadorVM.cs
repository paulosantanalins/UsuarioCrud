using System;
using System.Collections.Generic;

namespace Cadastro.Api.ViewModels
{
    public class PrestadorVM
    {
        public PrestadorVM()
        {
       
            Telefone = new TelefoneVM();
            Endereco = new EnderecoVM();
            DadosFinanceiro = new DadosFinanceiroVM();
            DadosFinanceiroEmpresa = new DadosFinanceiroEmpresaVM();
            Empresas = new List<EmpresaVM>();
            ValoresPrestador = new List<ValorPrestadorVM>();
            ClientesServicosPrestador = new List<ClienteServicoPrestadorVM>();
            ContratosPrestador = new List<ContratoPrestadorVM>();
            ObservacoesPrestador = new List<ObservacaoPrestadorVM>();
        }

        public int Id { get; set; }
        public int IdEmpresaGrupo { get; set; }
        public string DescricaoEmpresaGrupo { get; set; }
        public int? CodEacessoLegado { get; set; }
        public int IdCelula { get; set; }
        public int? IdEndereco { get; set; }
        public int? IdTelefone { get; set; }

        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Rg { get; set; }
        public int IdFilial { get; set; }
        public DateTime? DataUltimaAlteracaoEacesso { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataInicioCLT { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Pai { get; set; }
        public string Mae { get; set; }
        public string ProdutoRm { get; set; }
        public DateTime? DataValidadeContrato { get; set; }
        public DateTime? DataDesligamento { get; set; }
        public string Email { get; set; }
        public string EmailInterno { get; set; }
        public string EmailParceiro { get; set; }
        public string Usuario { get; set; }

        public int? IdNacionalidade { get; set; }
        public string DescricaoNacionalidade { get; set; }
        public int IdCargo { get; set; }
        public string DescricaoCargo { get; set; }
        public int IdContratacao { get; set; }
        public string DescricaoContratacao { get; set; }
        public int? IdEscolaridade { get; set; }
        public string DescricaoEscolaridade { get; set; }
        public int? IdExtensao { get; set; }
        public string DescricaoExtensao { get; set; }
        public int? IdGraduacao { get; set; }
        public string DescricaoGraduacao { get; set; }
        public int IdEstadoCivil { get; set; }
        public string DescricaoEstadoCivil { get; set; }
        public int IdSexo { get; set; }
        public string DescricaoSexo { get; set; }
        public int? IdSituacao { get; set; }
        public string DescricaoSituacao { get; set; }
        public int? IdDiaPagamento { get; set; }
        public string DescricaoDiaPagamento { get; set; }
        public int? IdAreaFormacao { get; set; }
        public string DescricaoAreaFormacao { get; set; }
        public int? IdRemuneracao { get; set; }
        public string DescricaoRemuneracao { get; set; }
        public string IdRepresentanteRmTRPR { get; set; }
        public int IdProdutoRm { get; set; }
        public int? IdTipoRemuneracao { get; set; }
        public bool PagarPelaMatriz { get; set; }
        public int IdPessoa { get; set; }
        public EnderecoVM Endereco { get; set; }
        public TelefoneVM Telefone { get; set; }
        public List<EmpresaVM> Empresas { get; set; }
        public DadosFinanceiroVM DadosFinanceiro { get; set; }
        public DadosFinanceiroEmpresaVM DadosFinanceiroEmpresa { get; set; }
        public List<InativacaoPrestadorVM> Inativacoes { get; set; }
        public List<ContratoPrestadorVM> ContratosPrestador { get; set; }
        public List<DocumentoPrestadorVM> DocumentosPrestador { get; set; }
        public List<ValorPrestadorVM> ValoresPrestador { get; set; }
        public List<ClienteServicoPrestadorVM> ClientesServicosPrestador { get; set; }
        public List<ObservacaoPrestadorVM> ObservacoesPrestador { get; set; }
    }
}
