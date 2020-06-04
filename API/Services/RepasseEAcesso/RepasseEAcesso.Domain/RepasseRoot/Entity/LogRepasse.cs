using RepasseEAcesso.Domain.DominioRoot.ItensDominio;
using RepasseEAcesso.Domain.SharedRoot.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RepasseEAcesso.Domain.RepasseRoot.Entity
{
    public class LogRepasse : EntityBase
    {
        public int IdStatusRepasse { get; set; }
        public int? IdMotivoRepasse { get; set; }
        public string Descricao { get; set; }
        public int IdRepasse { get; set; }

        [NotMapped]
        public string DescricaoStatus { get; set; }
        public virtual RepasseNivelUm Repasse { get; set; }        
        public virtual DomStatusRepasse StatusRepasse { get; set; }        

    }
}
