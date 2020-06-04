namespace GestaoServico.Domain.PessoaRoot.Entity
{
    public class Pessoa : EntityBase
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string EmailInterno { get; set; }
        public int? IdEacesso { get; set; }
    }
}
