using Cadastro.Domain.PessoaRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System;
using System.Collections.Generic;

namespace Cadastro.Domain.CelulaRoot.Entity
{
    public class Celula : EntityBase
    {
        public string Descricao { get; set; }
        public int? IdCelulaSuperior { get; set; }
        public int? IdGrupo { get; set; }
        public int? IdTipoCelula { get; set; }
        public int Status { get; set; }
        public string NomeResponsavel { get; set; }
        public string EmailResponsavel { get; set; }
        public int? IdPessoaResponsavel { get; set; }
        public bool FlHabilitarRepasseMesmaCelula { get; set; }
        public bool FlHabilitarRepasseEpm { get; set; }

        public virtual Pessoa Pessoa { get; set; }
        public virtual TipoCelula TipoCelula { get; set; }
        public virtual Grupo Grupo { get; set; }
        public virtual Celula CelulaSuperior { get; set; }
        public virtual ICollection<Celula> CelulasSuperiores { get; set; }
        public virtual ICollection<Prestador> Prestadores { get; set; }
        public virtual ICollection<ClienteServicoPrestador> ClientesServicosPrestador { get; set; }
    }
}
