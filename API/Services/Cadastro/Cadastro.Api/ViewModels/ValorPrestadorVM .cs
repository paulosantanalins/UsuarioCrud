using Cadastro.Domain.PrestadorRoot.Entity;
using System;

namespace Cadastro.Api.ViewModels
{
    public class ValorPrestadorVM
    {
        public int Id { get; set; }
        public int IdPrestador { get; set; }
        public int IdMoeda { get; set; }
        public decimal ValorMes { get; set; }
        public decimal ValorHora { get; set; }
        public int IdTipoRemuneracao { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorClt { get; set; }
        public decimal ValorPericulosidade { get; set; }
        public decimal ValorPropriedadeIntelectual { get; set; }              
        public DateTime? DataAlteracao { get; set; }
        public DateTime? DataReferencia { get; set; }
        public decimal ValorBeneficio { get; set; }        
        public int IdProfissional { get; set; }
        public decimal? ValorLtda { get; set; }
        public bool PermiteExcluir { get; set; }


    }
}
