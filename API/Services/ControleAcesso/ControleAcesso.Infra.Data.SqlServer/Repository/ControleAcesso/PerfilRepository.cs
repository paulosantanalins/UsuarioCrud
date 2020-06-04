using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Infra.Data.SqlServer.Context;
using ControleAcesso.Infra.Data.SqlServer.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControleAcesso.Domain.SharedRoot;
using Utils;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class PerfilRepository : BaseRepository<Perfil>, IPerfilRepository
    {
        public PerfilRepository(ControleAcessoContext controleAcessoContext, IVariablesToken variables) : base(controleAcessoContext, variables) { }

        public Perfil ObterPerfilComUsuarios(int id)
        {
            var result = DbSet.AsQueryable().Include(x => x.UsuarioPerfis);

            return result.FirstOrDefault(x => x.Id == id && x.UsuarioPerfis.Any());
        }

        public Perfil ObterPerfilComVinculo(int id)
        {
            var result = DbSet.AsQueryable().Include(x => x.VinculoPerfilFuncionalidades);

            return result.FirstOrDefault(x => x.Id == id);
        }

        public FiltroGenericoDto<PerfilDto> FiltrarPerfil(FiltroGenericoDto<PerfilDto> filtro)
        {
            filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

            var query = DbSet.AsQueryable().AsNoTracking();
            query = query.Include(x => x.VinculoPerfilFuncionalidades).ThenInclude(x => x.Funcionalidade);
            var stringAtivo = "ATIVO";
            var stringInativo = "INATIVO";

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                query = query.Where(x => x.NmPerfil.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.FlAtivo == true && stringAtivo.Equals(filtro.ValorParaFiltrar.ToUpper()))
                                    || (x.FlAtivo == false && stringInativo.Equals(filtro.ValorParaFiltrar.ToUpper()))
                                    || (x.VinculoPerfilFuncionalidades.Any(y => y.Funcionalidade.DescFuncionalidade.Contains(filtro.ValorParaFiltrar.ToUpper()))));
            }

            var dados = query.Select(x => new PerfilDto
            {
                Id = x.Id,
                DataAlteracao = x.DataAlteracao,
                Usuario = x.Usuario,
                FlAtivo = x.FlAtivo,
                NmPerfil = x.NmPerfil,
                NmModulo = x.NmModulo,
                Funcionalidades = x.VinculoPerfilFuncionalidades.Select(y => y.Funcionalidade.DescFuncionalidade).ToList(),
                FuncionalidadesKeys = x.VinculoPerfilFuncionalidades.Select(y => y.Funcionalidade.NmFuncionalidade).ToList()
            });
            filtro.Total = dados.Count();

            switch (filtro.CampoOrdenacao)
            {
                case "nmPerfil":
                    dados = filtro.OrdemOrdenacao.Equals("asc") ? dados.OrderBy(x => x.NmPerfil) : dados.OrderByDescending(x => x.NmPerfil);
                    break;
                case "flStatus":
                    dados = filtro.OrdemOrdenacao.Equals("asc") ? dados.OrderByDescending(x => x.FlAtivo) : dados.OrderBy(x => x.FlAtivo);
                    break;
                default:
                    dados = dados.OrderByDescending(x => x.FlAtivo).ThenBy(x => x.NmPerfil);
                    break;
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }


        public bool Validar(Perfil perfil)
        {
            var result = DbSet.Where(x => x.NmPerfil.ToUpper() == perfil.NmPerfil.ToUpper() && x.FlAtivo && perfil.Id != x.Id).Any();
            return result;
        }
     
        public List<Perfil> BuscarPerfisPorFuncionalidades(int[] idsFuncionalidades)
        {
            var result = DbSet.Where(x => x.VinculoPerfilFuncionalidades.Any(y => idsFuncionalidades.Contains(y.IdFuncionalidade))).ToList();            
            return result;
        }      
    }
}
