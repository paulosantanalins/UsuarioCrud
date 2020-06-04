using Cadastro.Domain.SharedRoot;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class InativacaoPrestador : EntityBase
    {        
        public int IdPrestador { get; set; }
        public int? CodEacessoLegado { get; set; }
        public int? FlagIniciativaDesligamento { get; set; }        
        public int FlagRetorno { get; set; }        
        public DateTime? DataDesligamento { get; set; }
        public string Motivo { get; set; }
        public string Responsavel { get; set; }
        [NotMapped]
        public string DescIniciativaDesligamento { get; set; }
        [NotMapped]
        public string DescRetorno { get; set; }
        [NotMapped]
        public int IdCelula { get; set; }

        public virtual Prestador Prestador { get; set; }
    }
}
