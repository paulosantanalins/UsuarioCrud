using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class SocioEmpresaPrestadorEAcessoDto
    {
        public int Id { get; set; }
        public string NomeSocio { get; set; }
        public string CpfSocio { get; set; }
        public string RgSocio { get; set; }
        public string Profissao { get; set; }
        public string TipoPessoa { get; set; }
        public string CnpjEmpresa { get; set; }
        public int? Nacionalidade { get; set; }
        public int? EstadoCivil { get; set; }        
        public decimal? Participacao { get; set; }        
        public int IdEmpresaEacesso { get; set; }
    }
}
