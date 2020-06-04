using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class ContratoPrestadorDto 
    {
        public int? Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Tipo { get; set; }
        public string NomeAnexo { get; set; }        
        public string CaminhoContrato { get; set; }
        public string ArquivoBase64 { get; set; }
        public int IdPrestador { get; set; }
        public ICollection<ExtensaoContratoPrestadorDto> ExtensoesContratoPrestador { get; set; }
    }
}
