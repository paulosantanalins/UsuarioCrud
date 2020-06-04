using System;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class PrestadorMigradoDto
    {
        public int Id { get; set; }
        public int? IdEacesso { get; set; }
        public int? IdTelefone { get; set; }
        public int? IdEndereco { get; set; }
        public int? IdEmpresaGrupo { get; set; }
        public int? IdCelula { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
    }
}
