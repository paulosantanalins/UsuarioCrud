namespace GestaoServico.Api.ViewModels.GestaoPortifolio
{
    public class ClassificacaoContabilVM : ControleEdicaoVM
    {
        public int Id { get; set; }
        public string DescClassificacaoContabil { get; set; }
        public string SgClassificacaoContabil { get; set; }
        public bool FlStatus { get; set; }

        public int? IdCategoriaContabil { get; set; }
        public string DescCategoria { get; set; }

        public string SgCategoria { get; set; }
    }
}
