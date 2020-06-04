using System;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class PrestadorParaTransferenciaDto
    {
        public int Id { get; set; }
        public string EmpresaGrupo { get; set; }
        public int IdEmpresaGrupo { get; set; }
        public string Filial { get; set; }
        public int IdFilial { get; set; }
        public string Celula { get; set; }
        public int IdCelula { get; set; }
        public string Cliente { get; set; }
        public int IdCliente { get; set; }
        public string Servico { get; set; }
        public int IdServico { get; set; }
        public string LocalDeTrabalho { get; set; }
        public int IdLocalTrabalho { get; set; }
        public DateTime DataTransferencia { get; set; }
        public int IdPrestador { get; set; }
    }
}
