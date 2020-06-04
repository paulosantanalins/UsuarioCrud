using System;

namespace GestaoServico.Domain.OperacaoMigradaRoot.DTO
{
    public class OperacaoMigradaDTO
    {
        public int Id { get; set; }
        public int IdCombinadaCelula { get; set; }
        public int IdGrupoDelivery { get; set; }
        public string DescricaoOperacao { get; set; }
        public int IdCelula { get; set; }
        public int IdCliente { get; set; }
        public int IdServico { get; set; }
        public int IdTipoCelula { get; set; }
        public string NomeCliente { get; set; }
        public string DescricaoServico { get; set; }
        public string Status { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Usuario { get; set; }
    }
}
