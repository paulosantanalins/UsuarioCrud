using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using System.Collections.Generic;

namespace Cadastro.Domain.DominioRoot.ItensDominio
{
    public class DomDiaPagamento : Dominio
    {
        public ICollection<Prestador> Prestadores { get; set; }
        public ICollection<PeriodoDiaPagamento> PeriodosDiaPagamento { get; set; }
    }
}