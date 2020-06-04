using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.ViewModels
{
    public class VinculoServicoClienteVM
    {
        public VinculoServicoClienteVM()
        {

        }
        public int IdCliente { get; set; }
        public int IdServico { get; set; }
        public string NmClienteServico { get; set; }
        public string DescClienteServico { get; set; }
        public string NmEscopo { get; set; }
        public DateTime? DtFinalizacao { get; set; }
        public decimal VlMarkup { get; set; }
        public decimal VlReembolso { get; set; }
        public bool FlReembolso { get; set; }
        public string NmDelivery { get; set; }
        public int? IdDelivery { get; set; }
        public string IdSalesForce { get; set; }
        public bool FlMensal { get; set; }
        public string NmLocalServico { get; set; }
        public decimal? VlHoraExtra { get; set; }
        public bool? FlHoraExtraPgNensal { get; set; }
        public string NrCodificacao { get; set; }
        public int? HrExtraReembolso { get; set; }
        public bool? FlQuater { get; set; }
        public decimal? VlRentabilidade { get; set; }

        public int? IdEmpresa { get; set; }
        public int? IdFilial { get; set; }

        //public virtual Filial Filial { get; set; }
        //public virtual Empresa Empresa { get; set; }

        //public virtual ServicoVM Servico { get; set; }
    }
}
