using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class SocioEmpresaPrestador : EntityBase
    {       
        public string NomeSocio { get; set; }
        public string CpfSocio { get; set; }
        public string RgSocio { get; set; }
        public string Profissao { get; set; }
        public string TipoPessoa { get; set; }
        public int? IdNacionalidade { get; set; }
        public int? IdEstadoCivil { get; set; }
        public decimal? Participacao { get; set; }
        public int IdEAcesso { get; set; }
        public Empresa Empresa { get; set; }
        public int? IdEmpresa { get; set; }
        public int IdEmpresaEacesso { get; set; }
    }
}
