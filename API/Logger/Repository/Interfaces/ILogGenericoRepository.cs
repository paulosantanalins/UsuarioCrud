using Logger.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logger.Repository.Interfaces
{
    public interface ILogGenericoRepository : IDisposable
    {
        Task AddLog(LogGenerico logGenerico);
        void AddLogJob(string descJob);
        bool LogAdicionadoNoDiaAtual(string descLogGenerico);
        Task AddLogs(List<LogGenerico> logsGenericos);
    }
}
