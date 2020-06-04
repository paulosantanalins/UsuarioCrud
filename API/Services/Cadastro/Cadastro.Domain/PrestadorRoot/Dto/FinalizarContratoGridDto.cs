using System;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class FinalizarContratoGridDto
    {
        public int Id { get; set; }
        public int IdCelula { get; set; }
        public string Prestador { get; set; }
        public string DiaFimContrato { get; set; }
        public int Status { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Usuario { get; set; }
    }
}
