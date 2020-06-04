namespace Cadastro.Api.ViewModels
{
    public class HorasMesPrestadorVM
    {  
        public int IdPrestador { get; set; }
        public int IdHorasMes { get; set; }
        public decimal? Horas { get; set; }
        public decimal? Extras { get; set; }
        public string Situacao { get; set; }
        public bool SemPrestacaoServico { get; set; }
        public string ObservacaoSemPrestacaoServico { get; set; }

        public virtual PrestadorVM Prestador { get; set; }        
        public virtual HorasMesVM HorasMes { get; set; }
    
    }
}
