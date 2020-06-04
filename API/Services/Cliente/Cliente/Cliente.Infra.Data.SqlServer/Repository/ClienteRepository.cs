using Cliente.Domain.ClienteRoot.Entity;
using Cliente.Domain.ClienteRoot.Repository;
using Cliente.Infra.Data.SqlServer.Context;
using Cliente.Infra.Data.SqlServer.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Cliente.Domain.SharedRoot;
using Utils;
using Utils.Base;

namespace Cliente.Infra.Data.SqlServer.Repository
{
    public class ClienteRepository : BaseRepository<ClienteET>, IClienteRepository
    {
        public ClienteRepository(ClienteContext clienteContext, IVariablesToken variables) : base(clienteContext, variables)
        {
        }

        public int? ObterClientePorIdSalesForce(string idSalesForce)
        {
            var result = DbSet.Where(x => x.IdSalesforce == idSalesForce).Select(x => x.Id).FirstOrDefault();
            return result;
        }

        public async Task<MultiselectDto> PopularComboClienteRepasse(int id)
        {
            var result = await DbSet.Where(x => x.Id == id).Select(x => new MultiselectDto {
                Id = x.Id,
                Nome = x.NmFantasia != null && x.NmFantasia.Count() > 1 ? x.NmFantasia : 
                x.NmRazaoSocial != null && x.NmRazaoSocial.Count() > 1 ? x.NmRazaoSocial :
                x.NrCnpj,
                IdSecundario = x.FlStatus == "A" ? 0 : 1 }).FirstOrDefaultAsync();
            return result;
        }

        public int VerificarIdCliente()
        {
            var result = DbSet.Where(x => x.Id >= 20000).Count();
            return result;
        }

        public FiltroGenericoDto<ClienteET> Filtrar(FiltroGenericoDto<ClienteET> filtro)
        {
            var query = DbSet.AsNoTracking().AsQueryable();

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                var stringAtivo = "ATIVO";
                var stringInativo = "INATIVO";
                var stringPendente = "PENDENTE";


                query = query.Where(x =>
                    x.Id.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    (x.NmFantasia != null ? x.NmFantasia.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : false) ||
                    (x.NmRazaoSocial != null ? x.NmRazaoSocial.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : false) ||
                    (x.NrCnpj != null ? x.NrCnpj.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : false) ||
                    x.DataAlteracao.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    x.Usuario.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    (x.FlStatus.ToUpper().Equals("A") && stringAtivo.Equals(filtro.ValorParaFiltrar.ToUpper())) ||
                    (x.FlStatus.ToUpper().Equals("I") && stringInativo.Equals(filtro.ValorParaFiltrar.ToUpper())) ||
                    (x.FlStatus.ToUpper().Equals("P") && stringPendente.Equals(filtro.ValorParaFiltrar.ToUpper())));
            }

            var dados = query.Select(p => new ClienteET
            {
                Id = p.Id,
                NmFantasia = p.NmFantasia,
                NmRazaoSocial = p.NmRazaoSocial,
                IdGrupoCliente = p.IdGrupoCliente,
                NrCnpj = p.NrCnpj,
                FlTipoHierarquia = p.FlTipoHierarquia,
                FlStatus = p.FlStatus,
                NrTelefone = p.NrTelefone,
                NrTelefone2 = p.NrTelefone2,
                NrFax = p.NrFax,
                NmSite = p.NmSite,
                NmEmail = p.NmEmail,
                NrInscricaoEstadual = p.NrInscricaoEstadual,
                NrInscricaoMunicipal = p.NrInscricaoMunicipal,
                Usuario = p.Usuario,
                DataAlteracao = p.DataAlteracao
            });
            filtro.Total = dados.Count();

            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                dados = dados.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                dados = dados.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else
            {
                dados = dados.OrderByDescending(x => x.FlStatus).ThenBy(x => x.Id);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        //public bool Validar(ClienteET clienteET)
        //{
        //    var result = DbSet.Where(x => x.SgCategoriaContabil.ToUpper() == clienteET.SgCategoriaContabil.ToUpper() && x.FlStatus && clienteET.Id != x.Id).Any();
        //    return result;
        //}
    }
}
