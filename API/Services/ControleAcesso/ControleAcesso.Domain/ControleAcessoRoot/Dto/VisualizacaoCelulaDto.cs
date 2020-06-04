namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class VisualizacaoCelulaDto
    {
        public int Id { get; set; }
        public string DescCelula { get; set; }
        public string DescGrupo { get; set; }
        public string DescCelulaSuperior { get; set; }
        public bool Inativa { get; set; }
        public bool TodasAsCelulasSempre { get; set; }
        public bool TodasAsCelulasSempreMenosAPropria { get; set; }
        public bool RepasseParaMesmaCelula { get; set; }

    }
}
