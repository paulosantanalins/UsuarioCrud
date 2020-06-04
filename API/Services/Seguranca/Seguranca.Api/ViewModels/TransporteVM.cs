using System.ComponentModel.DataAnnotations;

namespace Seguranca.Api.ViewModels
{
    public class TransporteVM
    {
        [Required]
        public string Usuario { get; set; }
        [Required]
        public string Chave { get; set; }
        [Required]
        public string Dominio { get; set; }

    }
}

