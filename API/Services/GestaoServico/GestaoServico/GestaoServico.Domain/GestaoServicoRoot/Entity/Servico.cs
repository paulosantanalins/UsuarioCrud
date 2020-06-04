using GestaoServico.Domain.GestaoCelulaRoot.Entity;
using GestaoServico.Domain.GestaoContratoRoot.Entity;
using GestaoServico.Domain.GestaoVinculoClienteRoot.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GestaoServico.Domain.GestaoServicoRoot.Entity
{
    public class Servico
    {
        public Servico()
        {
            VinculoServicoTipoServicos = new HashSet<VinculoServicoTipoServico>();
            VinculoCombinadaServicos = new HashSet<VinculoCombinadaServico>();
            VinculoContratoServicos = new HashSet<VinculoContratoServico>();
            VinculoCelulaServicos = new HashSet<VinculoCelulaServico>();
            VinculoClienteServicos = new HashSet<VinculoClienteServico>();
        }
        public int Id { get; set; }
        public string FlStatus { get; set; }
        public string FlMigrado { get; set; }
        public int IdServicoPai { get; set; }
        public virtual ServicoPai ServicoPai { get; set; }

        public virtual ICollection<VinculoServicoTipoServico> VinculoServicoTipoServicos { get; set; }
        public virtual ICollection<VinculoCombinadaServico> VinculoCombinadaServicos { get; set; }
        public virtual ICollection<VinculoContratoServico> VinculoContratoServicos { get; set; }
        public virtual ICollection<VinculoCelulaServico> VinculoCelulaServicos { get; set; }
        public virtual ICollection<VinculoClienteServico> VinculoClienteServicos { get; set; }

    }
}
