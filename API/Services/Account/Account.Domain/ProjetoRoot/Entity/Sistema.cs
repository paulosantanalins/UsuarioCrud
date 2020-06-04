using System;
using System.Collections.Generic;
using System.Text;

namespace Account.Domain.ProjetoRoot.Entity
{
    public class Sistema
    {
        public string LgUsuario { get; set; }
        public DateTime? DtAlteracao { get; set; }
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Link { get; set; }
        public int Ordem { get; set; }
        //public string Imagem { get; set; }
    }
}
