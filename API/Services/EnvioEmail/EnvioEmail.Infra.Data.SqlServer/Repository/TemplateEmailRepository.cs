using EnvioEmail.Domain.EmailRoot.Dto;
using EnvioEmail.Domain.EmailRoot.Entity;
using EnvioEmail.Domain.EmailRoot.Repository;
using EnvioEmail.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.Base;

namespace EnvioEmail.Infra.Data.SqlServer.Repository
{
    public class TemplateEmailRepository : BaseRepository<TemplateEmail>, ITemplateEmailRepository
    {
        private readonly IVariablesToken _variables;

        public TemplateEmailRepository(ServiceBContext context, IVariablesToken variables) : base(context)
        {
            _variables = variables;
        }

        public void AdicionarTemplateEmail(TemplateEmail entity)
        {
            entity.Usuario = _variables.UserName;
            entity.DataAlteracao = DateTime.Now;
            DbSet.Add(entity);
        }

        public TemplateEmail BuscarPorNome(string nome)
        {
            return DbSet.Where(x => x.Nome == nome).FirstOrDefault();
        }

        public IEnumerable<TemplateEmail> BuscarTodosComIncludes()
        {
            return DbSet.Include(x => x.Emails)
                .Include(x => x.Parametros)
                .ToList();
        }
                
        public FiltroGenericoDtoBase<TemplateEmailDto> FiltrarTemplatesEmail(FiltroGenericoDtoBase<TemplateEmailDto> filtro)
        {
            filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

            var query = DbSet.AsQueryable().AsNoTracking();

            query = query.Include(x => x.Emails)
                .Include(x => x.Parametros);

            query = query.Where(x => x.Id.ToString().ToUpper().Equals(filtro.ValorParaFiltrar.ToUpper())
                                  || x.Assunto.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                  || x.Corpo.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                  || x.Nome.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));

            var dados = query.Select(x => new TemplateEmailDto
            {
                Id = x.Id,
                Assunto = x.Assunto,
                Corpo = x.Corpo,
                Nome = x.Nome,
                FlagFixo = x.FlagFixo,
                Parametros = x.Parametros.Select(y => new ParametroTemplateDto
                {
                    Id = y.Id,
                    IdTemplate = x.Id,
                    NomeParametro = y.NomeParametro                   
                }),
                Usuario = x.Usuario,
                DataAlteracao = x.DataAlteracao
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
                dados = dados.OrderBy(x => x.Assunto);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

            return filtro;
        }
        
        public TemplateEmail BuscarPorIdComParametros(int id)
        {
            var template = DbSet.Include(x => x.Parametros).AsNoTracking().FirstOrDefault(x => x.Id == id);
            return template;
        }

        public void UpdateComParametro(TemplateEmail entity)
        {
            var entityDB = DbSet.Where(x => x.Id == entity.Id).Include(p => p.Parametros).FirstOrDefault();
            entity.Usuario = _variables.UserName;
            entity.DataAlteracao = DateTime.Now;
            _context.Entry(entityDB).CurrentValues.SetValues(entity);
            entityDB.Parametros = entity.Parametros;
            DbSet.Update(entityDB);
        }
    }

}
