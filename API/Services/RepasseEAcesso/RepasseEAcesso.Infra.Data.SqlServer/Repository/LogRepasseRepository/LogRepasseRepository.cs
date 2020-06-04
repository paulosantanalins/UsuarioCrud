using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Repository;
using RepasseEAcesso.Infra.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace RepasseEAcesso.Infra.Data.SqlServer.Repository.LogRepasseRepository
{
    public class LogRepasseRepository : BaseRepository<LogRepasse>, ILogRepasseRepository
    {
        public LogRepasseRepository(RepasseEAcessoContext context, IVariablesToken variables) : base(context, variables)
        {
        }       
    }
}
