using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoContratoRoot.Entity
{
    public class Contrato
    {
        public int Id { get; set; }
        public int NrContrato { get; set; }
        public virtual ICollection<VinculoContratoServico> VinculoContratoServicos { get; set; }
    }
}
