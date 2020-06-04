using Cadastro.Domain.CelulaRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class ClienteServicoPrestador : EntityBase
    {
        public int IdPrestador { get; set; }
        public int IdCelula { get; set; }
        public int IdCliente { get; set; }
        public int IdServico { get; set; }
        public int IdLocalTrabalho { get; set; }
        public string DescricaoTrabalho { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataPrevisaoTermino { get; set; }
        public decimal ValorCusto { get; set; }
        public decimal ValorVenda { get; set; }
        public decimal ValorRepasse { get; set; }
        public bool Ativo { get; set; }

        public virtual Prestador Prestador { get; set; }
        public virtual Celula Celula { get; set; }

        public ClienteServicoPrestador Clone()
        {
            return this.MemberwiseClone() as ClienteServicoPrestador;
        }
    }
}
