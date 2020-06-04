using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.ViewModels
{
    public class ServicoSalesForceVM
    {
        public int IdCelula { get; set; }
        public string CNPJ { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Escopo { get; set; }
        public string Contrato { get; set; }
        public string Categoria { get; set; }
        public string SiglaTipoServico { get; set; }
        public string DtFinalizacao { get; set; }
        public decimal Markup { get; set; }
        public decimal ValorKM { get; set; }
        public string FlagReembolso { get; set; }
        public string FlagQuarter { get; set; }
        public string Delivery { get; set; }
        public int? IdCelulaDelivery { get; set; }
        public int? IdDelivery { get; set; }
        public string ExtrasReemb { get; set; }
        public decimal Rentabilidade { get; set; }
        public string IdSalesForce { get; set; }
        public string Mensal { get; set; }
        public string Codificacao { get; set; }
        public string LocalServico { get; set; }
        public string CnpjFilial { get; set; }
        public decimal? VHoraExtra { get; set; }
        public string HorasExtrasPgMensal { get; set; }
        public string Usuario { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
    }
}
