using System;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class ReajusteContratoGridDto
    {
        public int Id { get; set; }
        public int IdCelula { get; set; }
        public string Prestador { get; set; }
        public int Status { get; set; }
        public string DataSolicitacao { get; set; }
        public string DataReajuste { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Usuario { get; set; }
    }
}
