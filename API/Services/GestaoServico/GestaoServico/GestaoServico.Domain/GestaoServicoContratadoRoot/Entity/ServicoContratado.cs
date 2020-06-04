using GestaoServico.Domain.GestaoFilialRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Entity
{
    public class ServicoContratado : EntityBase
    {
        public ServicoContratado()
        {
            VinculoMarkupServicosContratados = new HashSet<VinculoMarkupServicoContratado>();
            DeParaServicos = new HashSet<DeParaServico>();
            VinculoServicoCelulaComercial = new HashSet<VinculoServicoCelulaComercial>();
        }

        public DateTime DtInicial { get; set; }
        public DateTime? DtFinal { get; set; }
        public string FormaFaturamento { get; set; }
        public decimal? VlRentabilidade { get; set; }
        public bool FlHorasExtrasReembosaveis { get; set; }
        public bool FlFaturaRecorrente { get; set; }
        public bool FlReoneracao { get; set; }
        public decimal? VlKM { get; set; }
        public bool FlReembolso { get; set; }
        public int? QtdExtraReembolso { get; set; }
        public string NmTipoReembolso { get; set; }
        public int IdContrato { get; set; }   
        public int? IdEmpresa { get; set; }
        public int? IdFilial { get; set; }
        public string IdProdutoRM { get; set; }
        public int? IdEscopoServico { get; set; }
        public string DescricaoServicoContratado { get; set; }
        public int? IdGrupoDelivery { get; set; }
        public int? IdFrenteNegocio { get; set; }
        public int? IdServicoContratadoOrigem { get; set; }


        //novosCampos
        public string DescTipoCelula { get; set; }
        public int IdCelula { get; set; }

        [NotMapped]
        public int IdCliente { get; set; }

        public virtual Contrato Contrato { get; set; }
        public virtual EscopoServico EscopoServico { get; set; }
        public virtual ICollection<ServicoContratado> ServicosContratadosMigracao { get; set; }
        public virtual ServicoContratado ServicoContratadoOrigem { get; set; }

        public virtual ICollection<Repasse> RepasseOrigens { get; set; }
        public virtual ICollection<Repasse> RepasseDestinos { get; set; }
        public virtual ICollection<DeParaServico> DeParaServicos { get; set; }

        public virtual ICollection<VinculoMarkupServicoContratado> VinculoMarkupServicosContratados { get; set; }

        public virtual ICollection<VinculoServicoCelulaComercial> VinculoServicoCelulaComercial { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
