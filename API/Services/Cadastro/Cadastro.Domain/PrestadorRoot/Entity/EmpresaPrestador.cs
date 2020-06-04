using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class EmpresaPrestador : EntityBase
    {
        public int IdEmpresa { get; set; }
        public int IdPrestador { get; set; }
        public virtual Prestador Prestador { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
