namespace GestaoServico.Api.ViewModels.GestaoPortifolio
{
    public class CategoriaContabilVM : ControleEdicaoVM
    {
        public int Id { get; set; }
        public string DescCategoria { get; set; }
        public string SgCategoriaContabil { get; set; }
        public bool FlStatus { get; set; }
    }
}
