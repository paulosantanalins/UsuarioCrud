using System.Collections.Generic;

namespace Cadastro.Api.ViewModels
{
    public class CidadeVM
    {
        public int Id { get; set; }
        public string NmCidade { get; set; }
        public int IdEstado { get; set; }
        public string NmEstado { get; set; }
        public int IdPais { get; set; }
        public string NmPais { get; set; }
    }
}
