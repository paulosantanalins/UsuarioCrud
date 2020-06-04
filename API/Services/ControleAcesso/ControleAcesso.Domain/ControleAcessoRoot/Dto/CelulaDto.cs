using System;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class CelulaDto
    {
        public int Id { get; set; }
        public string DescCelula { get; set; }
        public string TipoCelula { get; set; }
        public string Grupo { get; set; }
        public string TipoHierarquia { get; set; }
        public string CelulaSuperior { get; set; }
        public bool FlHabilitarRepasseMesmaCelula { get; set; }
        public bool FlHabilitarRepasseEpm { get; set; }
        public bool Inativa { get; set; }
        public int Status { get; set; }
        public int IdPais { get; set; }
        public int IdMoeda { get; set; }
        public int IdCelulaResponsavel { get; set; }
        public string IdResponsavel { get; set; }
        public string LoginResponsavel { get; set; }
        public int IdTipoHierarquia { get; set; }
        public int IdTipoCelula { get; set; }
        public int IdTipoContabil { get; set; }
        public int IdGrupo { get; set; }
        public int? IdCelulaSuperior { get; set; }
        public int IdTipoServicoDelivery { get; set; }
        public int IdEmpresaGrupo { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Usuario { get; set; }
    }
}
