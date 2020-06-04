using GestaoServico.Domain.GestaoCelulaRoot.Entity;
using GestaoServico.Domain.GestaoCelulaRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GestaoServico.Infra.Data.SqlServer.Repository
{
    public class CelulaRepository : ICelulaRepository
    {
        private readonly GestaoServicoContext _gestaoServicoContext;

        public CelulaRepository(GestaoServicoContext gestaoServicoContext)
        {
            _gestaoServicoContext = gestaoServicoContext;
        }

        public async Task PersistirCelula()
        {
            await _gestaoServicoContext.Celulas.AddAsync(new Celula() { Id = 0, DescCelula = "teste" });
        } 
    }
}
