using System;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class FinalizacaoContratoDto
    {
        public int Id { get; set; }
        public bool FinalizarImediatamente { get; set; }
        public bool DesabilitarContratosFuturos { get; set; }
        public DateTime UltimoDiaTrabalho { get; set; }
        public string Motivo { get; set; }
        public int IdPrestador { get; set; }
        public int IdCelula { get; set; }
    }
}
