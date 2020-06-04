using Account.Domain.ProjetoRoot.Entity;
using Account.Domain.ProjetoRoot.Repository;
using Account.Infra.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Infra.Data.SqlServer.Repository
{
    public class SistemaRepository : ISistemaRepository
    {
        public readonly AccountContext _accountContext;
        public SistemaRepository(AccountContext accountContext)
        {
            _accountContext = accountContext;
        }

        public async Task AddSistema(Sistema sistema)
        {
            await _accountContext.AddAsync(sistema);
            await _accountContext.SaveChangesAsync();
        }

        public async Task UpdateSistema(Sistema sistema)
        {
            _accountContext.Update(sistema);
            await _accountContext.SaveChangesAsync();
        }

        public async Task<List<Sistema>> ObterTodosSistemas()
        {
            var result = _accountContext.Sistemas.OrderBy(x => x.Ordem).ToList();

            return result;
        }


        public async Task DeleteSistema(int id)
        {
            var sistema = _accountContext.Sistemas.Where(x => x.Id == id).SingleOrDefault();
            _accountContext.Sistemas.Remove(sistema);
            await _accountContext.SaveChangesAsync();
        }


    }
}
