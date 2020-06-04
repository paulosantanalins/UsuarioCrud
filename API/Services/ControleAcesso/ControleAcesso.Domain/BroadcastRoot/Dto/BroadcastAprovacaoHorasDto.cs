namespace ControleAcesso.Domain.BroadcastRoot.Dto
{
    public class BroadcastAprovacaoHorasDto
    {
        public string NomeAprovador { get; set; }
        public string LoginAprovador { get; set; }
        public string PeriodoCompetencia { get; set; }
        public string DiaLimite { get; set; }
        public string DiaPagamento { get; set; }
        public string Link { get; set; }
    }
}
