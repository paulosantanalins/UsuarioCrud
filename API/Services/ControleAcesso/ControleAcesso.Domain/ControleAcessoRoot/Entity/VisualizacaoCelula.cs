namespace ControleAcesso.Domain.ControleAcessoRoot.Entity
{
    public class VisualizacaoCelula : EntityBase
    {
        public int? IdCelula { get; set; }
        public string LgUsuario { get; set; }
        public int IdCelulaUsuarioVinculado { get; set; }
        public bool TodasAsCelulasSempre { get; set; }
        public bool TodasAsCelulasSempreMenosAPropria { get; set; }
        public virtual Celula Celula { get; set; }
    }
}
