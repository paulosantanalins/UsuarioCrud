using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleAcesso.Domain.ControleAcessoRoot.Entity
{
    public class Pessoa : EntityBase
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Matricula { get; set; }
        public int? IdEacesso { get; set; }
        [NotMapped]
        public string Cpf { get; set; }

        public virtual ICollection<Celula> Celulas { get; set; }
    }
}
