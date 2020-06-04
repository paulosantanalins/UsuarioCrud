using System;

namespace GestaoServico.Domain.Dto
{
    public class ContratoDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string NrAssetSalesForce { get; set; }
        public string DescContrato { get; set; }
        public int IdCliente { get; set; }
        public string NmFantasia { get; set; }
        public string NmRazaoSocial { get; set; }
        public string NrCnpj { get; set; }
        public DateTime DtInicial { get; set; }
        public DateTime? DtFinalizacao { get; set; }
        public string DescStatusSalesForce { get; set; }
        public int? IdCelulaComercial { get; set; }
    }
}
