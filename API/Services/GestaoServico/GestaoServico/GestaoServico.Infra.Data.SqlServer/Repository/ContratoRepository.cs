using GestaoServico.Domain.GestaoContratoRoot.Entity;
using GestaoServico.Domain.GestaoContratoRoot.Repository;
using GestaoServico.Domain.GestaoVinculoClienteRoot.Entity;
using GestaoServico.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoServico.Infra.Data.SqlServer.Repository
{
    public class ContratoRepository : IContratoRepository
    {
        private readonly GestaoServicoContext _gestaoServicoContext;

        public ContratoRepository(GestaoServicoContext gestaoServicoContext)
        {
            _gestaoServicoContext = gestaoServicoContext;
        }

        public async Task<Contrato> ObterContratoPorCodigo(string Codigo)
        {
            return new Contrato();
        }

        public async Task PersistirContrato()
        {
            await _gestaoServicoContext.Contratos.AddAsync(new Contrato() { Id = 0});
            _gestaoServicoContext.SaveChanges();
        }

        public async Task<bool> ObterFilialPorCnpj(string cnpjFilial)
        {
            var result = _gestaoServicoContext.Filiais.Where(x => x.NrCnpj.ToUpper() == cnpjFilial.ToUpper()).Any();
            return result;
        }

        public async Task<Filial> ObterFilialComEmpresa(string cnpjFilial)
        {
            var result = await _gestaoServicoContext.Filiais
                                              .Include(x => x.Empresa)
                                              .Where(x => x.NrCnpj.ToUpper() == cnpjFilial.ToUpper()).FirstOrDefaultAsync();

            return result;
        }
    }
}
