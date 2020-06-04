using Microsoft.EntityFrameworkCore.ChangeTracking;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.SharedRoot.Repository;
using System.Collections.Generic;

namespace RepasseEAcesso.Domain.RepasseRoot.Repository
{
    public interface ILogRepasseRepository : IBaseRepository<LogRepasse>
    {
       
    }
}
