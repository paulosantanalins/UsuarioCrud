namespace Cadastro.Domain.PessoaRoot.Dto
{
    public class ViewAdimitidosFuturos
    {
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Requisitante { get; set; }
        public string NomeSocial { get; set; }
        public int? CodSindicato { get; set; }
        public string Estado { get; set; }
        public int CodVaga { get; set; }
        public int CodColigada { get; set; }
        public string NomeColigada { get; set; }
        public string Celula { get; set; }
        public string GestorCelula { get; set; }
        public string CelulaSuperior { get; set; }
        public string GestorSuperior { get; set; }
    }
}
