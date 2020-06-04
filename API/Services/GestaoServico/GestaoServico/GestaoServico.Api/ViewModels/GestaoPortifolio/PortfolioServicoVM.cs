namespace GestaoServico.Api.ViewModels.GestaoPortifolio
{
    public class PortfolioServicoVM : ControleEdicaoVM
    {
        public int Id { get; set; }
        public string NmServico { get; set; }
        public string DescServico { get; set; }
        public bool FlStatus { get; set; }
        public int? IdDelivery { get; set; }

    }
}
