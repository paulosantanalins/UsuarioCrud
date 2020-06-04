using System;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class TransferenciaCltPjDto
    {
        public int Id { get; set; }
        public int IdEacessoLegado { get; set; }
        public int IdPrestadorTransferido { get; set; }
        public string NomePrestador { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
