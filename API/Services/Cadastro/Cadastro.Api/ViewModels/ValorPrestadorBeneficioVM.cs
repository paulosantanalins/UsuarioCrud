
namespace Cadastro.Api.ViewModels
{
    public class ValorPrestadorBeneficioVM
    {        
        public int IdValorPrestador { get; set; }
        public int IdBeneficio { get; set; }
        public string descricaoBeneficio { get; set; }
        public decimal ValorBeneficio { get; set; }
    }
}
