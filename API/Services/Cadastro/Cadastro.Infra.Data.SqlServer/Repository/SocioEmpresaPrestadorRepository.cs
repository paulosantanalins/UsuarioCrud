using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Service;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class SocioEmpresaPrestadorRepository : BaseRepository<SocioEmpresaPrestador>, ISocioEmpresaPrestadorRepository
    {
        public SocioEmpresaPrestadorRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository) : base(context, variables, auditoriaRepository)
        {
        }

        public SocioEmpresaPrestador BuscarPorIdEAcesso(int idEacesso) => DbSet.FirstOrDefault(x => x.IdEAcesso == idEacesso);
    }

}
