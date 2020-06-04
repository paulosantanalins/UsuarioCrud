using System;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class EmpresaPrestadorDto
    {
        public int IdEmpresa { get; set; }
        public int IdPrestador { get; set; }
        public string IdEmpresaRm { get; set; }

        public string CNPJ { get; set; }
        public string RazaoSocial { get; set; }
        public DateTime DataVigencia { get; set; }
        public string Tipo { get; set; }
        public string Endereco { get; set; }
        public string Cep { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public int IdCidade { get; set; }
        public string NmCidade { get; set; }
        public string Referencia { get; set; }
        public string CCM { get; set; }
        public string InscricaoEstadual { get; set; }
        public string Observacao { get; set; }
        public string Atuacao { get; set; }
        public string Socios { get; set; }
        public bool Ativo { get; set; }
    }
}
