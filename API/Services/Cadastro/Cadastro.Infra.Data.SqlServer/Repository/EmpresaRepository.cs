using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class EmpresaRepository : BaseRepository<Empresa>, IEmpresaRepository
    {
        public EmpresaRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {

        }

        public Empresa BuscarPorCnpj(string cnpj)
        {
            var result = DbSet.Include(x => x.Endereco)
                                .ThenInclude(x => x.Cidade)
                                    .ThenInclude(x => x.Estado)
                                        .ThenInclude(x => x.Pais).Where(x => x.Cnpj == cnpj).FirstOrDefault();
            return result;
        }

        public Empresa BuscarPorId(int id)
        {
            var result = DbSet.Include(x => x.Endereco)
                                .ThenInclude(x => x.Cidade)
                                    .ThenInclude(x => x.Estado)
                                        .ThenInclude(x => x.Pais).Where(x => x.Id == id).FirstOrDefault();
            return result;
        }

        public string ObterCodEmpresaRm(int idPrestador)
        {
            var empresa = DbSet.Include(x => x.EmpresasPrestador)
                              .FirstOrDefault(x => x.Ativo && x.EmpresasPrestador.Any(y => y.IdPrestador == idPrestador));
            if (empresa != null)
            {
                var valorString = ("000000" + empresa.IdEmpresaRm);
                var tam = valorString.Length;
                return valorString.Substring(tam - 6);
            }
            else
            {
                return "-1";
            }
        }
    }
}
