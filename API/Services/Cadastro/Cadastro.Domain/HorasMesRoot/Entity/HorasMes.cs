using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.HorasMesRoot.Entity
{
    public class HorasMes : EntityBase
    {
        public int Mes { get; set; }
        public int Ano { get; set; }
        public int Horas { get; set; }
        public bool Ativo { get; set; }

        public virtual ICollection<HorasMesPrestador> HorasMesPrestador { get; set; }
        public virtual ICollection<PeriodoDiaPagamento> PeriodosDiaPagamento { get; set; }
    }
}
