using System;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class TransferenciaPrestadorDto
    {
        public int Id { get; set; }
        public int Celula { get; set; }
        public string Prestador { get; set; }
        public string Status { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
