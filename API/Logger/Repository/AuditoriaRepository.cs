using Logger.Context;
using Logger.Model;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logger.Repository
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly LogGenericoContext _logGenericoContext;
        static private string Usuario { get; set; }
        public AuditoriaRepository(LogGenericoContext logGenericoContext)
        {
            _logGenericoContext = logGenericoContext;
        }

        public async Task AddLog(Auditoria auditoria)
        {
            await _logGenericoContext.Auditorias.AddAsync(auditoria);
            await _logGenericoContext.SaveChangesAsync();
        }

        public async Task AddLogs(List<Auditoria> auditorias)
        {
            await _logGenericoContext.Auditorias.AddRangeAsync(auditorias);
            await _logGenericoContext.SaveChangesAsync();
        }
        
        public async Task<List<Auditoria>> GetLogsAsync(string tabela, string jsonIdAlterado)
        {
            var listaAuditorias = await _logGenericoContext.Auditorias.Where(x => x.Tabela == tabela && x.IdsAlterados == jsonIdAlterado)
                .ToListAsync();
            return listaAuditorias;
        }

        public void Dispose()
        {
            _logGenericoContext.Dispose();
        }

        public void SetLoginUsuario(string nome)
        {
            Usuario = nome;
        }
        public string GetLoginUsuario()
        {
            return Usuario;
        }
    }
}
