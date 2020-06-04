using Cadastro.Domain.DominioRoot.ItensDominio;
using Cadastro.Domain.SharedRoot;
using System;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class DocumentoPrestador : EntityBase
    {
        public string NomeAnexo { get; set; }
        public string CaminhoDocumento { get; set; }                
        public string DescricaoTipoOutros { get; set; }    
        public virtual Prestador Prestador { get; set; }
        public virtual DomTipoDocumentoPrestador TipoDocumentoPrestador { get; set; }
        public bool Inativo { get; set; }
        public int IdTipoDocumentoPrestador { get; set; }
        public int IdPrestador { get; set; }
    }
}
