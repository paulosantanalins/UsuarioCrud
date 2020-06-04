namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class UsuarioSegurancaDto
    {
        public string Login { get; set; }
        public string NomeCompleto { get; set; }
        public string Celula { get; set; }
        public int IdCelula { get; set; }        
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Cargo { get; set; }
        public bool PossuiAlgumaVisualizacaoCelula { get; set; }
    }
}
