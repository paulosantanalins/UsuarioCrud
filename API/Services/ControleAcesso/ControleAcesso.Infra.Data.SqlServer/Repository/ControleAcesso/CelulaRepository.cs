using System;
using System.Collections.Generic;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Infra.Data.SqlServer.Context;
using ControleAcesso.Infra.Data.SqlServer.Repository.Base;
using Microsoft.EntityFrameworkCore;
using Utils;
using System.Linq;
using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.DominioRoot.Repository;
using Logger.Repository.Interfaces;
using System.Threading.Tasks;
using ControleAcesso.Domain.SharedRoot;
using Newtonsoft.Json;
using Logger.Model;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class CelulaRepository : BaseRepository<Celula>, ICelulaRepository
    {
        private readonly IDominioRepository _dominioRepository;
        private readonly IAuditoriaRepository _auditoriaRepository;
        private readonly IVariablesToken _variables;

        public CelulaRepository(ControleAcessoContext controleAcessoContext,
            IDominioRepository dominioRepository,
            IAuditoriaRepository  auditoriaRepository,
            IVariablesToken variables) : base(controleAcessoContext, variables)
        {
            _dominioRepository = dominioRepository;
            _auditoriaRepository = auditoriaRepository;
            _variables = variables;
        }

        public List<Celula> BuscarTodosAtivasComInclude()
        {
            var result = DbSet.AsQueryable()
                .Include(x => x.CelulaSuperior)
                .Include(x => x.Grupo)
                .Where(x => x.Status != SharedEnuns.StatusCelula.Inativada.GetHashCode())
                .AsNoTracking();

            return result.ToList();
        }

        public List<Celula> BuscarTodosComInclude()
        {
            var result = DbSet.AsQueryable()
                .Include(x => x.CelulaSuperior)
                .Include(x => x.Grupo)
                .Include(x => x.TipoHierarquia)
                .Include(x => x.TipoContabil)
                .Include(x => x.TipoServicoDelivery)
                .Include(x => x.Pessoa)
                .AsNoTracking();

            return result.ToList();
        }

        public Celula BuscarComIncludes(int idCelula)
        {
            var result = DbSet.AsNoTracking()
                .Include(x => x.CelulaSuperior)
                .Include(x => x.Grupo)
                .Include(x => x.TipoHierarquia)
                .Include(x => x.TipoContabil)
                .Include(x => x.TipoServicoDelivery)
                .Include(x => x.Pessoa)
                .FirstOrDefault(x => x.Id == idCelula);
            return result;
        }

        public FiltroGenericoDto<CelulaDto> FiltrarCelula(FiltroGenericoDto<CelulaDto> filtro)
        {

            var query = DbSet.AsQueryable().AsNoTracking();
            query = query.Include(x => x.CelulaSuperior)
                .Include(x => x.Grupo)
                .Include(x => x.TipoCelula)
                .Include(x => x.TipoHierarquia);
                  
            query = filtro.Id == -1 ? query :
                filtro.Id == 1 ? query.Where(x => x.Status != SharedEnuns.StatusCelula.Inativada.GetHashCode()) :
                query.Where(x => x.Status == SharedEnuns.StatusCelula.Inativada.GetHashCode());

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

                query = query.Where(x => x.DescCelula.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.Id.ToString().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.Grupo.DescGrupo.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.TipoCelula.Descricao.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.TipoHierarquia.DescricaoValor.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.CelulaSuperior.DescCelula.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())))))));
            }

            var dados = query.Select(x => new CelulaDto
            {
                Id = x.Id,
                DescCelula = x.DescCelula,
                Grupo = x.Grupo.DescGrupo != null ? x.Grupo.DescGrupo : "",
                CelulaSuperior = x.CelulaSuperior.DescCelula != null ? x.CelulaSuperior.DescCelula : "",
                TipoCelula = x.TipoCelula.Descricao != null ? x.TipoCelula.Descricao : "",
                TipoHierarquia = x.TipoHierarquia.DescricaoValor != null ? x.TipoHierarquia.DescricaoValor : "",
                Inativa = x.Status == SharedEnuns.StatusCelula.Inativada.GetHashCode(),
                Status = x.Status,
                DataAlteracao = x.DataAlteracao,
                Usuario = x.Usuario
            });
            filtro.Total = dados.Count();

            switch (filtro?.CampoOrdenacao)
            {
                case "celula":
                    dados = filtro.OrdemOrdenacao.Equals("asc") ? dados.OrderByDescending(x => x.Id) : dados.OrderBy(x => x.Id);
                    break;
                default:
                    dados = dados.OrderByDescending(x => x.Inativa).ThenBy(x => x.DescCelula);
                    break;
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        public string ObterEmailGerenteServico(int idCelula)
        {
            var result = DbSet.AsQueryable()
                .Include(x => x.Pessoa)
                .Where(x => x.Id == idCelula)
                .FirstOrDefault();

            return result != null ? result.Pessoa.Email : "";
        }

        public bool ValidarExisteCelula(int id)
        {
            var result = DbSet.AsQueryable().Where(x => x.Id.Equals(id));
            return result.Count() > 0;
        }
        
        public async Task<List<Auditoria>> BuscarListaDeAuditoriaDeCelula(int idCelula)
        {
            var jsonObjectId = JsonConvert.SerializeObject(new { Id = idCelula });
            var listaLogs = await _auditoriaRepository.GetLogsAsync("TBLCELULA", jsonObjectId);
            return listaLogs.OrderBy(x => x.DataAlteracao).ToList();
        }
    }
}
