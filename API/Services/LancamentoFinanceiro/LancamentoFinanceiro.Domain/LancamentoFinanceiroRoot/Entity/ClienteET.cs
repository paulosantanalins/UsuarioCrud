using System.Collections.Generic;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity
{
    public class ClienteET : EntityBase
    {

        public ClienteET()
        {
            Enderecos = new HashSet<Endereco>();
        }
        public int? IdGrupoCliente { get; set; }
        public int? IdEAcesso { get; set; }
        public string NrCnpj { get; set; }
        public string NmFantasia { get; set; }
        public string NmRazaoSocial { get; set; }
        public string IdSalesforce { get; set; }

        public string FlTipoHierarquia { get; set; }
        public string FlStatus { get; set; }
        public string NrTelefone { get; set; }
        public string NrTelefone2 { get; set; }
        public string NrFax { get; set; }
        public string NmSite { get; set; }
        public string NmEmail { get; set; }
        public string NrInscricaoEstadual { get; set; }
        public string NrInscricaoMunicipal { get; set; }

        public virtual GrupoCliente GrupoCliente { get; set; }
        public virtual ICollection<Endereco> Enderecos { get; set; }
    }
}
