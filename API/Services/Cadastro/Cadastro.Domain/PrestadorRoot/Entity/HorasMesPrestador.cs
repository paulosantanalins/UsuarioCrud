using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.NotaFiscalRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class HorasMesPrestador : EntityBase
    {
        public HorasMesPrestador()
        {
            LogsHorasMesPrestador = new HashSet<LogHorasMesPrestador>();
        }

        public int IdPrestador { get; set; }
        public int IdHorasMes { get; set; }
        public decimal? Horas { get; set; }
        public decimal? Extras { get; set; }
        public string Situacao { get; set; }
        public bool SemPrestacaoServico { get; set; }
        public string ObservacaoSemPrestacaoServico { get; set; }
        public int? IdChaveOrigemIntRm { get; set; }

        public virtual Prestador Prestador { get; set; }
        public virtual ICollection<PrestadorEnvioNf> PrestadoresEnvioNf{ get; set; }
        public virtual HorasMes HorasMes { get; set; }
        public ICollection<DescontoPrestador> DescontosPrestador { get; set; }
        public ICollection<LogHorasMesPrestador> LogsHorasMesPrestador { get; set; }
    }
}
