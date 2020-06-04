using System;

namespace Cadastro.Api.ViewModels
{
    public class ObservacaoPrestadorVM
    {
        public int Id { get; set; }
        public int IdPrestador { get; set; }
        public int IdTipoOcorencia { get; set; }        
        public string DescricaoTipoOcorrencia { get; set; }
        public bool Status { get; set; }
        public string  Observacao { get; set; }
        public string Usuario;
        public DateTime? DataAlteracao { get; set; }
    }
}
