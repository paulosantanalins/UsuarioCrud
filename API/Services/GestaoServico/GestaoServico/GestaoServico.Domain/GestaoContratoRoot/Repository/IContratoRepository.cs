using GestaoServico.Domain.GestaoContratoRoot.Entity;
using GestaoServico.Domain.GestaoVinculoClienteRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GestaoServico.Domain.GestaoContratoRoot.Repository
{
    public interface IContratoRepository
    {
        Task<Contrato> ObterContratoPorCodigo(string Codigo);
        Task PersistirContrato();
        Task<bool> ObterFilialPorCnpj(string cnpjFilial);
        Task<Filial> ObterFilialComEmpresa(string cnpjFilial);
    }
}
