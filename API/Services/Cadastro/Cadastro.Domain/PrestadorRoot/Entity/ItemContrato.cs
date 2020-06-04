using Cadastro.Domain.SharedRoot;
using System;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class ContratoBase : EntityBase
    {
        public DateTime? DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Tipo { get; set; }
        public string NomeAnexo { get; set; }
        public bool Inativo { get; set; }
        public string CaminhoContrato { get; set; }  
    }
}
