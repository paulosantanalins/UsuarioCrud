using Logger.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logger.Repository.Interfaces
{
    public interface IAuditoriaRepository : IDisposable
    {
        Task AddLog(Auditoria auditoria);
        Task AddLogs(List<Auditoria> auditorias);        
        Task<List<Auditoria>> GetLogsAsync(string tabela, string jsonIdAlterado);
        void SetLoginUsuario(string nome);
        string GetLoginUsuario();
    }
}
