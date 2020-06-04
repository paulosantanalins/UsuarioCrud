using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using ControleAcesso.Infra.Data.SqlServer.Context;
using ControleAcesso.Infra.Data.SqlServer.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControleAcesso.Domain.SharedRoot;
using Utils;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class UsuarioPerfilRepository : BaseRepository<UsuarioPerfil>, IUsuarioPerfilRepository
    {
        private readonly IVisualizacaoCelulaService _visualizacaoCelulaService;

        public UsuarioPerfilRepository(
            ControleAcessoContext controleAcessoContext,
            IVariablesToken variables,
            IVisualizacaoCelulaService visualizacaoCelulaService) : base(controleAcessoContext, variables)
        {
            _visualizacaoCelulaService = visualizacaoCelulaService;
        }


        public UsuarioPerfilDto ObterUsuarioPerfilDto(string login)
        {
            var dadosFinal = new UsuarioPerfilDto();
            var query = DbSet.AsQueryable()
                             .Include(x => x.Perfil)
                                .ThenInclude(x => x.VinculoPerfilFuncionalidades)
                                    .ThenInclude(x => x.Funcionalidade);

            var dados = query.Where(x => x.LgUsuario.ToUpper() == login.ToUpper());

            var result = dados.Select(x => new UsuarioPerfilDto
            {
                LgUsuario = x.LgUsuario,
                NmPerfil = x.Perfil.NmPerfil,
                IdPerfil = x.IdPerfil,
                Funcionalidades = x.Perfil.VinculoPerfilFuncionalidades.Select(y => y.Funcionalidade.NmFuncionalidade).ToList()
            }).ToList();

            if (result != null && result.Any())
            {
                dadosFinal = new UsuarioPerfilDto
                {
                    LgUsuario = result.FirstOrDefault().LgUsuario,
                    NmPerfil = string.Join(",",result.Select(x => x.NmPerfil).ToList()),
                    Funcionalidades = result.SelectMany(x => x.Funcionalidades).Distinct().ToList()
                };
            }

            return dadosFinal;
        }

        public bool VerificaPrestadorMaster(string login)
        {
            var dadosFinal = new UsuarioPerfilDto();
            var query = DbSet.AsQueryable()
                             .Include(x => x.Perfil)
                                .ThenInclude(x => x.VinculoPerfilFuncionalidades)
                                    .ThenInclude(x => x.Funcionalidade);

            var dados = query.Where(x => x.LgUsuario.ToUpper() == login.ToUpper());

            var result = dados.Select(x => new UsuarioPerfilDto
            {
                LgUsuario = x.LgUsuario,
                NmPerfil = x.Perfil.NmPerfil,
                IdPerfil = x.IdPerfil,
                Funcionalidades = x.Perfil.VinculoPerfilFuncionalidades.Select(y => y.Funcionalidade.NmFuncionalidade).ToList()
            }).ToList();

            if (result != null && result.Any())
            {
                dadosFinal = new UsuarioPerfilDto
                {
                    LgUsuario = result.FirstOrDefault().LgUsuario,
                    NmPerfil = string.Join(",", result.Select(x => x.NmPerfil).ToList()),
                    Funcionalidades = result.SelectMany(x => x.Funcionalidades).Distinct().ToList()
                };
            }

            return dadosFinal.Funcionalidades.Any(x => x.Equals("PrestadorMaster", StringComparison.InvariantCultureIgnoreCase));
        }

        public FiltroGenericoDto<UsuarioPerfilDto> FiltrarUsuario(FiltroGenericoDto<UsuarioPerfilDto> filtro)
        {
            var usuariosADPorCelula = _visualizacaoCelulaService.BuscarUsuariosAdPorCelulas(filtro.ValorParaFiltrar);

            var query = _context.UsuarioPerfils.Where(x => usuariosADPorCelula.Any(y => y.Login.Equals(x.LgUsuario))).Include(x => x.Perfil).AsQueryable();

            if (filtro.FiltroGenerico != null && filtro.FiltroGenerico.Any())
            {
                query = query.Where(x => x.LgUsuario.ToUpper().Contains(filtro.FiltroGenerico.ToUpper())
                                     || x.Perfil.NmPerfil.ToUpper().Contains(filtro.FiltroGenerico.ToUpper()));
            }

            var dados = query.GroupBy(x => x.LgUsuario);

            filtro.Total = dados.Count();
            var result = dados.Select(x => new UsuarioPerfilDto
            {
                Id = 0,
                DataAlteracao = x.OrderByDescending(z=>z.DataAlteracao).FirstOrDefault().DataAlteracao,
                NmUsuario = usuariosADPorCelula.FirstOrDefault(y => y.Login == x.Key).NomeCompleto,
                Usuario = x.OrderByDescending(z => z.DataAlteracao).FirstOrDefault().Usuario,
                LgUsuario = x.Key
            });

            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                result = result.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                result = result.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else
            {
                result = result.OrderBy(x => x.LgUsuario);
            }

            filtro.Valores = result.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            PreencherPerfis(filtro);
            return filtro;
        }
        
        private void PreencherPerfis(FiltroGenericoDto<UsuarioPerfilDto> filtro)
        {
            foreach (var item in filtro.Valores)
            {
                item.Perfis = string.Join(", ",ObterTodosPerfisPorUsuario(item.LgUsuario));
            }
        }

        public List<string> ObterTodosPerfisPorUsuario(string login)
        {
            var query = DbSet.Include(x => x.Perfil)
                             .AsQueryable();
            var result = query.Where(x => x.LgUsuario == login).Select(x => x.Perfil.NmPerfil).ToList();

            return result;

        }

        public UsuarioPerfilDto ObterUsuarioComTodosPerfis(string login)
        {
            var query = DbSet.AsQueryable()
                             .Include(x => x.Perfil);

            var dados = query.Where(x => x.LgUsuario.ToUpper() == login.ToUpper()).ToList();

            var result = dados.Select(x => new UsuarioPerfilDto
            {
                LgUsuario = x.LgUsuario,
                NmPerfil = x.Perfil.NmPerfil,
                IdPerfil = x.IdPerfil,
                IdsPerfis = dados.Select(y => y.IdPerfil).ToList()
            }).FirstOrDefault();
            return result;
        }

        public FiltroGenericoDto<UsuarioPerfilDto> ObterUsuariosComPerfil(FiltroGenericoDto<UsuarioPerfilDto> filtro)
        {
            var query = DbSet.Include(x => x.Perfil).GroupBy(x => x.LgUsuario)
                             .AsQueryable();

            if (filtro.FiltroGenerico != null && filtro.FiltroGenerico.Any())
            {
                query = query.Where(x =>
                        x.Key.ToUpper().Contains(filtro.FiltroGenerico.ToUpper())
                        || String.Join(", ", x.Select(y => y.Perfil.NmPerfil)).ToUpper().Contains(filtro.FiltroGenerico.ToUpper()));
            }

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                query = query.Where(x => filtro.ValorParaFiltrar.ToUpper().Contains(x.Key.ToUpper().Trim()));
            }

            var dados = query.Select(x => new UsuarioPerfilDto {
                LgUsuario = x.Key,
                Perfis = String.Join(", ",x.Select(y => y.Perfil.NmPerfil))
            });

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
                dados = dados.OrderBy(x => x.LgUsuario);
            }

            filtro.Total = dados.Count();
            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

            return filtro;
        }
              
        public List<string> BuscarUsuariosPorPerfis(int[] idPerfis)
        {
            var result = DbSet.Where(x => idPerfis.Contains(x.IdPerfil)).Select(y => y.LgUsuario).Distinct().ToList();
            return result;
        }

        public List<string> BuscarUsuariosPorFuncionalidades(int[] idFuncionalidades)
        {
            var result = DbSet.Where(x => x.Perfil.VinculoPerfilFuncionalidades.Any(y => idFuncionalidades.Contains(y.IdFuncionalidade)))
                .Select(x => x.LgUsuario)
                .ToList();
            return result;
        }
    }
}

