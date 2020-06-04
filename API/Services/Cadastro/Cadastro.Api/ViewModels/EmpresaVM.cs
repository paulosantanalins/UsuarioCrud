using System;

namespace Cadastro.Api.ViewModels
{
    public class EmpresaVM
    {
        public int Id { get; set; }
        public int IdPrestador { get; set; }
        public int? IdRelacional { get; set; }
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public DateTime? DataVigencia { get; set; }
        public string InscricaoEstadual { get; set; }        
        public string Atuacao { get; set; }
        public string Socios { get; set; }
        public string Observacao { get; set; }
        public bool Ativo { get; set; }
        public string AtivoDescricao  => Ativo ? "Ativo" : "Inativo";
        public int? IdEmpresaRm { get; set; }

        public int? IdEndereco { get; set; }
        public string DescricaoEndereco { get; set; }
        public string AbrevLogradouro { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string Complemento { get; set; }
        public string Numero { get; set; }
        public int IdCidade { get; set; }
        public string Referencia { get; set; }
        public string DescricaoCidade { get; set; }
        public int? IdEstado { get; set; }
        public string DescricaoEstado { get; set; }
        public int? IdPais { get; set; }
        public string DescricaoPais { get; set; }

        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
