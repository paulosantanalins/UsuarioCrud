using Cadastro.Domain.PessoaRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain.TelefoneRoot.Entity
{
    public class Telefone : EntityBase
    {
        public string IdNextel { get; set; }
        public string NumeroNextel { get; set; }
        public string DDD { get; set; }
        public string Celular { get; set; }
        public string NumeroResidencial { get; set; }
        public string NumeroComercial { get; set; }
        public string NumeroComercialRamal { get; set; }

        [NotMapped]
        public int IdProfissional { get; set; }

        public virtual ICollection<Pessoa> Pessoas { get; set; }
    }
}
