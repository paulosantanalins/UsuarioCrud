using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.CidadeRoot.Dto
{
    public class CidadeDto
    {
        public int Id { get; set; }
        public string NmCidade { get; set; }
        public int IdEstado { get; set; }
        public string NmEstado { get; set; }
        public int IdPais { get; set; }
        public string NmPais { get; set; }
    }
}
