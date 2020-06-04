using ControleAcesso.Domain.DominioRoot.ItensDominio;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleAcesso.Domain.ControleAcessoRoot.Entity
{
    public class Celula : EntityBase
    {
        public string DescCelula { get; set; }
        public string NomeResponsavel { get; set; }
        public string EmailResponsavel { get; set; }
        public int Status { get; set; }
        public int? IdPais { get; set; }
        public int? IdMoeda { get; set; }
        public int? IdEmpresaGrupo { get; set; }
        public int? IdCelulaSuperior { get; set; }
        public bool FlHabilitarRepasseMesmaCelula { get; set; }
        public bool FlHabilitarRepasseEpm { get; set; }
        public virtual Celula CelulaSuperior { get; set; }
        public int? IdGrupo { get; set; }
        public virtual Grupo Grupo { get; set; }
        public int? IdPessoaResponsavel { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public int? IdTipoCelula { get; set; }
        public virtual TipoCelula TipoCelula { get; set; }     
        public int? IdTipoContabil { get; set; }        
        public virtual DomTipoContabil TipoContabil { get; set; }
        public int? IdTipoServicoDelivery { get; set; }
        public virtual DomTipoServicoDelivery TipoServicoDelivery { get; set; }
        public int? IdTipoHierarquia { get; set; }
        public virtual DomTipoHierarquia TipoHierarquia { get; set; }
  

        public virtual ICollection<Celula> CelulasSubordinadas { get; set; }
        public virtual ICollection<VisualizacaoCelula> VisualizacoesCelula { get; set; }

        [NotMapped]
        public int? IdEacesso { get; set; }
        [NotMapped]
        public bool Inativa { get; set; }
    }
}
