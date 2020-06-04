using System;

namespace Cadastro.Api.ViewModels
{
    public class ExtensaoContratoPrestadorVM
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Status { get; set; }
        public string NomeAnexo { get; set; }
        public string CaminhoContrato { get; set; }
        public string ArquivoBase64 { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime DataAlteracao { get; set; }
        public ContratoPrestadorVM ContratoPrestador { get; set; }
        public int IdContratoPrestador { get; set; }


    }
}
