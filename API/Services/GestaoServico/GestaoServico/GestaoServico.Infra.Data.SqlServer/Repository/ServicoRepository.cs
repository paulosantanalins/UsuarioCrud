using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoServicoRoot.Entity;
using GestaoServico.Domain.GestaoServicoRoot.Repository;
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
    public class ServicoRepository : IServiceRepository
    {
        private readonly GestaoServicoContext _gestaoServicoContext;

        public ServicoRepository(GestaoServicoContext gestaoServicoContext)
        {
            _gestaoServicoContext = gestaoServicoContext;
        }

        public async Task<TipoServico> ObterTipoServicoPorSigla(string sigla)
        {
            var result = await _gestaoServicoContext.TipoServicos.Where(x => x.SgTipoServico.ToUpper() == sigla.ToUpper()).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> ValidarTipoServico(string sigla)
        {
            var result = await _gestaoServicoContext.TipoServicos.Where(x => x.SgTipoServico.ToUpper() == sigla.ToUpper()).AnyAsync();

            return result;
        }
    }
}
