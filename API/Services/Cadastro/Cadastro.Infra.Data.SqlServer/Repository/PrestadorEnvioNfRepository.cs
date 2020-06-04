using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.NotaFiscalRoot.Entity;
using Cadastro.Domain.NotaFiscalRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{

    public class PrestadorEnvioNfRepository : BaseRepository<PrestadorEnvioNf>, IPrestadorEnvioNfRepository
    {
        private readonly IVariablesToken _variables;
        private readonly IAuditoriaRepository _auditoriaRepository;

        public PrestadorEnvioNfRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository) : base(context, variables, auditoriaRepository)
        {
            _variables = variables;
            _auditoriaRepository = auditoriaRepository;
        }

        public PrestadorEnvioNf BuscarNfPrestadorInfoPorIdHorasMesPrestador(int id)
        {
            return DbSet
                .AsNoTracking()
                .Where(x => x.IdHorasMesPrestador == id && !string.IsNullOrEmpty(x.CaminhoNf))
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
        }

        public IQueryable<PrestadorEnvioNf> BuscarPorIdHorasMesPrestador(int idHorasMesPrestador)
        {
            return  DbSet
               .AsNoTracking()               
               .Where(x => x.IdHorasMesPrestador == idHorasMesPrestador);
        }

        public PrestadorEnvioNf BuscarInfoParaEnvioNfPrestadorPorToken(string token)
        {
            var result = DbSet.AsNoTracking()
                 .Include(x => x.HorasMesPrestador)
                     .ThenInclude(x => x.Prestador)
                         .ThenInclude(x => x.ValoresPrestador)
                 .Include(x => x.HorasMesPrestador)
                     .ThenInclude(x => x.Prestador)
                         .ThenInclude(x => x.Celula)
                 .Include(x => x.HorasMesPrestador)
                     .ThenInclude(x => x.DescontosPrestador)
                 .Include(x => x.HorasMesPrestador)
                     .ThenInclude(x => x.Prestador)
                         .ThenInclude(x => x.TipoRemuneracao)
                 .Include(x => x.HorasMesPrestador)
                     .ThenInclude(x => x.Prestador)
                         .ThenInclude(x => x.EmpresasPrestador)
                             .ThenInclude(x => x.Empresa)
                 .Include(x => x.HorasMesPrestador)
                     .ThenInclude(x => x.Prestador)
                         .ThenInclude(x => x.DiaPagamento)
                             .ThenInclude(x => x.PeriodosDiaPagamento)
                 .Include(x => x.HorasMesPrestador)
                     .ThenInclude(x => x.HorasMes)
                 .FirstOrDefault(x => x.Token == token);

            if (result != null)
            {
                result.HorasMesPrestador.Prestador.EmpresasPrestador = result.HorasMesPrestador.Prestador.EmpresasPrestador.Where(x => x.Empresa.Ativo).ToList();
            }

            return result;
        }
    }
}
