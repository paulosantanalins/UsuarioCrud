namespace RepasseEAcesso.Domain.RepasseRoot.Dto
{
    public class ProfissionalEAcessoDto
    {
        public int Id { get; set; }
        public int IdSecundario { get; set; }
        public string Nome { get; set; }
        public int Situacao { get; set; }
        public bool Inativo { get; set; }
    }
}
