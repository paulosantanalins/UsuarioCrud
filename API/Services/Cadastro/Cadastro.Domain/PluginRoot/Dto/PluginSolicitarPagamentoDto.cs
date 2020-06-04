using Cadastro.Domain.CelulaRoot.Entity;
using System;

namespace Cadastro.Domain.PluginRoot.Dto
{
    public class PluginSolicitarPagamentoDto
    {
        public PluginSolicitarPagamentoDto()
        {
            StatusInt = "I";
            OperacaoInt = "I";
            DataInsercaoInt = DateTime.Now;
            DataCriacao = DateTime.Now;
            DataEmissao = DateTime.Now;
            Serie = "OC";
            CodTmv = "1.1.28";
            Status = "A";
            CodColCfo = 0;
            Quantidade = 1;
            Codund = "UN";
            Contador = 1;
        }

        public string StatusInt { get; set; }
        public string OperacaoInt { get; set; }
        public DateTime DataInsercaoInt { get; set; }
        public int? SeqlEACesso { get; set; }
        public string Serie { get; set; }
        public string NumeroMov { get; set; }
        public int? CodFilial { get; set; }
        public int? CodColigada { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataEntrega { get; set; }
        public decimal ValorBruto { get; set; }
        public string CodCfo { get; set; }
        public string CodTmv { get; set; }
        public int? ChaveOrigemInt { get; set; }
        public string Status { get; set; }
        public int CodColCfo { get; set; }
        public string CodLoc { get; set; }
        public string CodCpg { get; set; }
        public string CodContabil { get; set; }

        public string HistoricoCurto { get; set; }

        public Celula CelulaPrestador { get; set; }

        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
        public string Codund { get; set; }
        public int Contador { get; set; }
        public string CodProf { get; set; }

        public int IdDetalhe { get; set; }
        public decimal MadEmprestimo { get; set; }
        public decimal MDescAdViagem { get; set; }
        public decimal MMultaTransito { get; set; }
        public decimal MNotebookCompra { get; set; }
        public decimal MPensaoAlimenticia { get; set; }
        public decimal MDescSinistro { get; set; }
        public decimal MDescTelMov { get; set; }

        public string CodCCusto { get; set; }

        public string Historico { get; set; }
    }
}
