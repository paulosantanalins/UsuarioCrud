using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Infra.Data.SqlServer.Context;
using ControleAcesso.Infra.Data.SqlServer.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using ControleAcesso.Domain.SharedRoot;
using Utils;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class VisualizacaoCelulaRepository : BaseRepository<VisualizacaoCelula>, IVisualizacaoCelulaRepository
    {
        private readonly ICelulaRepository _celulaRepository;

        public VisualizacaoCelulaRepository(
            ICelulaRepository celulaRepository,
            ControleAcessoContext controleAcessoContext,
            IVariablesToken variables) : base(controleAcessoContext, variables)
        {
            _celulaRepository = celulaRepository;
        }

        public List<VisualizacaoCelula> ObterVisualizacoesPorIdsCelula(IEnumerable<string> lista)
        {
            var nullableIntList = lista.Select(x => int.TryParse(x, out var f) ? f : default(int?));
            return DbSet.Where(x => nullableIntList.Contains(x.IdCelula) || x.IdCelula == null).ToList();
        }

        public List<VisualizacaoCelula> BuscarTodosPorLoginDistinct()
        {           
            var lista = DbSet.GroupBy(x => new { x.LgUsuario }).Select(x => x.FirstOrDefault()).ToList();
            return lista;
        }

        public List<VisualizacaoCelula> ObterVinculoPorLogin(string login)
        {
            var result = DbSet.Include(x => x.Celula).
                Where(x => x.LgUsuario.Trim().ToLower().Equals(login.Trim().ToLower())).AsNoTracking().ToList();
            return result;
        }

        public ICollection<VisualizacaoCelulaDto> ObterVisualizacaoCelularPorLogin(string login)
        {
            var query = DbSet.AsQueryable()
                .Include(x => x.Celula).ThenInclude(x => x.CelulaSuperior)
                .Include(x => x.Celula).ThenInclude(x => x.Grupo);

            List<VisualizacaoCelula> dados = query.Where(x => x.LgUsuario.Trim().ToUpper().Equals(login.Trim().ToUpper())).OrderBy(x => x.IdCelula).ToList();

            var result = new List<VisualizacaoCelulaDto>();
            if (dados.Any(x => !x.IdCelula.HasValue))
            {
                var celulas = ObterCelulas();
                result = celulas.Select(x => new VisualizacaoCelulaDto
                {
                    Id = x.Id,
                    Inativa = x.Status == SharedEnuns.StatusCelula.Inativada.GetHashCode(),
                    DescCelula = x.Id.ToString() + " | " + x.DescCelula,
                    DescCelulaSuperior = x.CelulaSuperior == null ? string.Empty : x.CelulaSuperior.Id.ToString() + " | " + x.CelulaSuperior.DescCelula,
                    DescGrupo = x.Grupo == null ? string.Empty : x.Grupo.DescGrupo,
                    TodasAsCelulasSempreMenosAPropria = dados.FirstOrDefault().TodasAsCelulasSempreMenosAPropria,
                    RepasseParaMesmaCelula = x.FlHabilitarRepasseMesmaCelula
                }).ToList();
            }
            else
            {
                result = dados.Select(x => new VisualizacaoCelulaDto
                {
                    Id = x.Id,                    
                    DescCelula = x.IdCelula.ToString() + " | " + x.Celula.DescCelula,
                    DescCelulaSuperior = x.Celula.CelulaSuperior == null ? string.Empty : x.Celula.CelulaSuperior.Id.ToString() + " | " + x.Celula.CelulaSuperior.DescCelula,
                    DescGrupo = x.Celula.Grupo == null ? string.Empty : x.Celula.Grupo.DescGrupo,
                    RepasseParaMesmaCelula = x.Celula.FlHabilitarRepasseMesmaCelula

                }).ToList();
            }

            return result;
        }

        private List<Celula> ObterCelulas()
        {
            var celulas = _celulaRepository.BuscarTodosComInclude();
            return celulas.ToList();
        }

        public FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> ObterUsuariosComVisualizacao(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro)
        {
            var query = DbSet.GroupBy(x => x.LgUsuario.Trim().ToUpper()).Select(x => new UsuarioVisualizacaoCelulaDto
            {
                Id = 0,
                LgUsuario = x.Key.Trim().ToLower(),
                DataAlteracao = x.FirstOrDefault().DataAlteracao,
                Usuario = x.FirstOrDefault().Usuario.Trim().ToLower()
            }).AsQueryable();

            filtro.Valores = query.ToList();
            return filtro;
        }

        public FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> FiltrarUsuariosCelulaDropdown(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro)
        {

            var query = DbSet.AsQueryable();

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                query = query.Where(x => filtro.ValorParaFiltrar.ToUpper().Contains(x.IdCelula.ToString()));
            }

            var dados = query.GroupBy(x => x.LgUsuario);
            var result = dados.Select(x => new UsuarioVisualizacaoCelulaDto
            {
                Id = 0,
                NomeCompleto = x.FirstOrDefault().Usuario,
                LgUsuario = x.Key
            });

            filtro.Valores = result.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        public FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> VisualizarUsuariosCelula(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro)
        {
            var query = DbSet.AsQueryable();

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                query = query.Where(x => filtro.ValorParaFiltrar.ToUpper().Contains(x.LgUsuario.ToUpper()));

            }

            if (filtro.FiltroGenerico != null && filtro.FiltroGenerico.Any())
            {
                query = query.Where(x => x.LgUsuario.ToUpper().Contains(filtro.FiltroGenerico.ToUpper())
                                     || x.Usuario.ToString().ToUpper().Contains(filtro.FiltroGenerico.ToUpper())
                                     || x.DataAlteracao.ToString().ToUpper().Contains(filtro.FiltroGenerico.ToUpper()));
            }

            var dados = query.GroupBy(x => x.LgUsuario);

            filtro.Total = dados.Count();
            var result = dados.Select(x => new UsuarioVisualizacaoCelulaDto
            {
                Id = 0,
                NomeCompleto = x.FirstOrDefault().Usuario,
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

            return filtro;
        }

        public FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> FiltrarGrid(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro)
        {
            var dados = filtro.Valores;

            if (filtro.FiltroGenerico != null && filtro.FiltroGenerico.Any())
            {
                dados = dados.Where(x =>
                                    x.LgUsuario.ToUpper().Contains(filtro.FiltroGenerico.ToUpper()) ||
                                    x.NomeCompleto.ToUpper().Contains(filtro.FiltroGenerico.ToUpper()))
                                    .ToList();
            }

            filtro.Total = dados.Count();

            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                dados = dados.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x)).ToList();
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                dados = dados.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x)).ToList();
            }
            else
            {
                dados = dados.OrderBy(x => x.LgUsuario).ToList();
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

            return filtro;
        }

        public List<VisualizacaoCelula> RemoveRepetidas(List<VisualizacaoCelula> visualizacaoCelulas)
        {
            var usuarios = visualizacaoCelulas.GroupBy(x => x.LgUsuario).Select(x => x.Key);
            var query = DbSet.Where(x => usuarios.Any(y => y.Equals(x.LgUsuario)));
            var repetidas = query.Where(x => visualizacaoCelulas.Any(y => x.IdCelula == y.IdCelula)).ToList();

            var retorno = visualizacaoCelulas.Where(x => !repetidas.Any(y => y.IdCelula == x.IdCelula && y.LgUsuario.Equals(x.LgUsuario)));

            return retorno.ToList();
        }
    }
}
