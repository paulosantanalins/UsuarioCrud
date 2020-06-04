using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoServicoRoot.Entity
{
    public class ServicoPai
    {
        public ServicoPai()
        {
            Servicos = new HashSet<Servico>();
        }
        public int Id { get; set; }
        public string NmServicoPai { get; set; }
        public string DescServicoPai { get; set; }

        public virtual ICollection<Servico> Servicos { get; set; }
    }
}
