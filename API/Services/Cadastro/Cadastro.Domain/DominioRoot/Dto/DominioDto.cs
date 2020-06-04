using System;

namespace Cadastro.Domain.DominioRoot.Dto
{
    public class DominioDto
    {
        public int Id { get; set; }
        public int IdValor { get; set; }
        public string GrupoDominio { get; set; }
        public string ValorDominio { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool Ativo { get; set; }
        public string Usuario { get; set; }
    }
}
