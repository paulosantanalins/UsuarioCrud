using System;

namespace Cadastro.Api.ViewModels
{
    public class PrestadorEnvioNfVM
    {
        public int Id { get; set; }

        public int IdHorasMesPrestador { get; set; }
        public HorasMesPrestadorVM HorasMesPrestador { get; set; }

        public string Responsavel { get; set; }
        public DateTime DiaLimiteEnvioNF { get; set; }
        public string Token { get; set; }
        public string CaminhoNf { get; set; }
        public decimal? ValorNf { get; set; }
        public string PdfBase64 { get; set; }
    }
}
