using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RepasseEAcesso.Domain.RepasseRoot.Dto
{
    public class LogRepasseDto
    {
        public int Id { get; set; }
        public int IdStatusRepasse { get; set; }
        public int IdMotivoRepasse { get; set; }
        public string Descricao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Usuario { get; set; }
        public RepasseNivelUmDto Repasse { get; set; }
        public int IdRepasse { get; set; }
        public string DescricaoStatus { get; set; }
    }
}
