using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class DocumentoPrestadorDto
    {
        public int Id { get; set; }
        public int IdPrestador { get; set; }
        public int IdTipoDocumentoPrestador { get; set; }
        public string DescricaoTipoOutros { get; set; }
        public string Tipo { get; set; }
        public string NomeAnexo { get; set; }
        public string CaminhoDocumento { get; set; }
        public string ArquivoBase64 { get; set; }
    }
}
