using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Api.ViewModels
{
    public class DocumentoPrestadorVM
    {
        public int Id { get; set; }
        public int IdPrestador { get; set; }
        public int IdTipoDocumentoPrestador { get; set; }
        public string DescricaoTipoOutros { get; set; }
        public string Tipo { get; set; }
        public string NomeAnexo { get; set; }
        public string CaminhoDocumento { get; set; }
        public string ArquivoBase64 { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }

    }
}
