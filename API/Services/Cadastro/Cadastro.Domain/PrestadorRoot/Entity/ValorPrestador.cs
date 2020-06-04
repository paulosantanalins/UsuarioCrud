using Cadastro.Domain.DominioRoot.ItensDominio;
using Cadastro.Domain.SharedRoot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class ValorPrestador : EntityBase
    {
        public int IdPrestador { get; set; }
        public int IdMoeda { get; set; }
        public decimal ValorMes { get; set; }
        public decimal ValorHora { get; set; }
        public int IdTipoRemuneracao { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorClt { get; set; }
        public decimal ValorPericulosidade { get; set; }
        public decimal ValorPropriedadeIntelectual { get; set; }
        public long IdRemuneracaoEacesso { get; set; }
        public DateTime DataReferencia { get; set; }

        [NotMapped]
        public bool PermiteExcluir { get; set; }
        [NotMapped]
        public int IdProfissional { get; set; }

        public virtual DomTipoRemuneracao TipoRemuneracao { get; set; }
        public virtual Prestador Prestador { get; set; }
        public virtual ICollection<ValorPrestadorBeneficio> ValoresPrestadorBeneficio { get; set; }
    }
}
