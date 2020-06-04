using Forecast.Domain.ForecastRoot;
using Forecast.Domain.ForecastRoot.Dto;
using Forecast.Domain.ForecastRoot.Repository;
using Forecast.Domain.SharedRoot;
using Forecast.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.Base;

namespace Forecast.Infra.Data.SqlServer.Repository
{
    public class ValorForecastRepository : IValorForecastRepository
    {
        
        protected DbSet<ValorForecast> DbSet;
        private readonly ForecastContext _context;
        private readonly IVariablesToken _variables;

        public ValorForecastRepository(ForecastContext context, IVariablesToken variables) 
        {            
            _context = context;
            _variables = variables;
            DbSet = _context.Set<ValorForecast>();
        }
        

        public void Adicionar(ValorForecast entity)
        {
            entity.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;

            if (entity.DataAlteracao == null)
            {
                entity.DataAlteracao = DateTime.Now;
            }
            DbSet.Add(entity);
        }

        public void Update(ValorForecast valorForecast)
        {            
            _context.Entry(valorForecast);
            DbSet.Update(valorForecast);
            AtualizarDataAlteracaoEUsuario();          
        }

        private void AtualizarDataAlteracaoEUsuario()
        {
            var trackedEntities = _context.ChangeTracker.Entries<EntityBaseCompose>();
            foreach (var entity in trackedEntities)
            {
                entity.Entity.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
                entity.Entity.DataAlteracao = DateTime.Now;
            }
        }

        public ICollection<ValorForecast> BuscarTodos()
        {
            return DbSet.ToList();
        }
        
    }
}
