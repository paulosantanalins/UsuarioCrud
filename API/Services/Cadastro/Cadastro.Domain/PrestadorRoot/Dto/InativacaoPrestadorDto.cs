using System;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class InativacaoPrestadorDto
    {
        public int Id { get; set; }
        public int IdPrestador { get; set; }
        public int? CodEacessoLegado { get; set; }
        public int? FlagIniciativaDesligamento { get; set; }
        public string DescIniciativaDesligamento { get; set; }
        public int FlagRetorno { get; set; }
        public DateTime? DataDesligamento { get; set; }
        public string Motivo { get; set; }
        public string Responsavel { get; set; }
    }
}
