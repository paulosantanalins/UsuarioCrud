using System;

namespace ControleAcesso.Api.ViewModels
{
    public class CelulaVM
    {
        public int Id { get; set; }
        public string DescCelula { get; set; }
        public int? IdCelulaSuperior { get; set; }
        public int? IdGrupo { get; set; }
        public int? IdTipoCelula { get; set; }
        public bool Inativa { get; set; }
        public string NomeResponsavel { get; set; }
        public string EmailResponsavel { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
    }
}
