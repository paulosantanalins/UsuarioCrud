using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class ClienteET : EntityBase
    {
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
    }
}
