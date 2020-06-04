using Cadastro.Domain.DominioRoot.ItensDominio;
using Cadastro.Domain.SharedRoot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class ObservacaoPrestador : EntityBase
    {
        public int IdPrestador { get; set; }
        public int IdTipoOcorencia { get; set; }
        public bool Status { get; set; }
        public string Observacao { get; set; }

        [NotMapped]
        public int IdProfissional { get; set; }

        [NotMapped]
        public string DescricaoTipoOcorrencia { get; set; }

        public virtual DomTipoOcorrencia TipoOcorrencia { get; set; }

        public virtual Prestador Prestador { get; set; }
    }
}
