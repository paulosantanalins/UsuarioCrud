using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class LogHorasMesPrestador : EntityBase
    {
        public int IdHorasMesPrestador { get; set; }
        public string SituacaoAnterior { get; set; }
        public string SituacaoNova { get; set; }
        public string DescMotivo { get; set; }

        public virtual HorasMesPrestador HorasMesPrestador { get; set; }
    }
}
