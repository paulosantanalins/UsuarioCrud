using RepasseEAcesso.Domain.SharedRoot.Entity;

namespace RepasseEAcesso.Domain.DominioRoot.Entity
{
    public class Dominio : EntityBase
    {
        public string ValorTipoDominio { get; set; }
        public int IdValor { get; set; }
        public string DescricaoValor { get; set; }
        public bool Ativo { get; set; }
    }
}
