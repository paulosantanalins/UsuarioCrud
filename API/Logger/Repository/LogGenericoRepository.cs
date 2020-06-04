using Logger.Context;
using Logger.Model;
using Logger.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logger.Repository
{
    public class LogGenericoRepository : ILogGenericoRepository
    {
        private readonly LogGenericoContext _logGenericoContext;
        public LogGenericoRepository(LogGenericoContext logGenericoContext)
        {
            _logGenericoContext = logGenericoContext;
        }

        public async Task AddLog(LogGenerico logGenerico)
        {
            await _logGenericoContext.LogGenericos.AddAsync(logGenerico);
            await _logGenericoContext.SaveChangesAsync();
        }

        public void AddLogJob(string descJob)
        {
            var dbContext = new LogGenericoContext();
            dbContext.LogGenericos.Add(new LogGenerico
            {
                DescLogGenerico = descJob,
                DtHoraLogGenerico = DateTime.Now,
                NmOrigem = "JOB",
                NmTipoLog = "",
                DescExcecao = ""
            });
            dbContext.SaveChanges();
        }

        public async Task AddLogs(List<LogGenerico> logsGenericos)
        {
            await _logGenericoContext.LogGenericos.AddRangeAsync(logsGenericos);
            await _logGenericoContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _logGenericoContext.Dispose();
        }

        public bool LogAdicionadoNoDiaAtual(string descLogGenerico)
        {
            var existeLog = _logGenericoContext.LogGenericos.Any(x => x.DescLogGenerico.Equals(descLogGenerico) && x.DtHoraLogGenerico.Date == DateTime.Now.Date);
            return existeLog;
        }
    }
}
