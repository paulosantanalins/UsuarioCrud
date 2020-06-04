namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class ValorPrestadorBeneficioEacessoDto
    {
        public long IdRemuneracao { get; set; }
        public int IdProfissional { get; set; }
        public string NomeBeneficio { get; set; }
        public decimal ValorBeneficio { get; set; }
    }
}
