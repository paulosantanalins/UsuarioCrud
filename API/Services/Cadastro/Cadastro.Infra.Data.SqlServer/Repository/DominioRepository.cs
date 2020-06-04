using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.DominioRoot.Repository;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cadastro.Domain.DominioRoot.Dto;
using Cadastro.Domain.SharedRoot;
using Dapper;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class DominioRepository : BaseRepository<Dominio>, IDominioRepository
    {
        private readonly IVariablesToken _variablesToken;

        public DominioRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {
            _variablesToken = variables;
        }

        public int? ObterIdPeloCodValor(int idValor, string tipoDominio)
        {
            if (idValor != 0)
            {
                var result = DbSet.FirstOrDefault(x => x.IdValor == idValor && x.ValorTipoDominio.Equals(tipoDominio));
                return result.Id;
            }
            else
            {
                return null;
            }
        }

        public List<Dominio> ObterPeloVlTipoDominio(string tipoDominio)
        {
            var result = DbSet.Where(x => x.ValorTipoDominio.Equals(tipoDominio)).OrderBy(x => x.IdValor).ToList();
            return result;
        }

        public FiltroGenericoDto<DominioDto> FiltrarDominios(FiltroGenericoDto<DominioDto> filtro)
        {
            _variablesToken.UserName = "";

            var query = new StringBuilder();
            query.Append(@"SELECT IDDOMINIO as Id
                                  ,VLTIPODOMINIO as GrupoDominio
                                  ,IDVALOR as IdValor
                                  ,VLDOMINIO as ValorDominio
                                  ,DTALTERACAO as DataAlteracao
                                  ,LGUSUARIO as Usuario
                                  ,FLATIVO as Ativo
                              FROM dbo.TBLDOMINIO ");

            var colocarParenteses = false;

            if (!string.IsNullOrEmpty(filtro.FiltroGenerico))
            {
                query.AppendLine($" WHERE VLTIPODOMINIO = '{filtro.FiltroGenerico}'");
                colocarParenteses = true;
            }

            var filtroIdQuery = filtro.Id == 1 ? "AND FLATIVO = 1" :
            filtro.Id == 0 ? " AND FLATIVO = 0" : "";
            query.AppendLine(filtroIdQuery);
           

            if (!string.IsNullOrEmpty(filtro.ValorParaFiltrar))
            {
                query.AppendLine($@" AND{(colocarParenteses ? " (" : string.Empty)} 
                            ((CHARINDEX(N'{filtro.ValorParaFiltrar}', UPPER(VLDOMINIO)) > 0)
		                    OR (CHARINDEX(N'{filtro.ValorParaFiltrar}', UPPER(VLTIPODOMINIO)) > 0))
                            {(int.TryParse(filtro.ValorParaFiltrar, out var valor) ? $" OR IDVALOR = {valor}" : string.Empty)}
                            {(colocarParenteses ? ")" : string.Empty)}");
            }


            var dados = _context.Database.GetDbConnection().Query<DominioDto>(query.ToString());

            filtro.Total = dados.Count();

            switch (filtro?.CampoOrdenacao)
            {
                case "grupoDominio":
                    dados = filtro.OrdemOrdenacao.Equals("asc") ? dados.OrderByDescending(x => x.GrupoDominio) : dados.OrderBy(x => x.GrupoDominio);
                    break;
                case "valorDominio":
                    dados = filtro.OrdemOrdenacao.Equals("asc") ? dados.OrderByDescending(x => x.ValorDominio) : dados.OrderBy(x => x.ValorDominio);
                    break;
                case "idValor":
                    dados = filtro.OrdemOrdenacao.Equals("asc") ? dados.OrderByDescending(x => x.IdValor) : dados.OrderBy(x => x.IdValor);
                    break;
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

            return filtro;
        }

        public Dominio BuscarDominioPorId(int id)
        {
            var dominio = DbSet.AsNoTracking().FirstOrDefault(x => x.Id == id);

            return dominio;
        }

        public void MudarStatusDominio(Dominio dominio)
        {
            _context.Entry(dominio).Property(x => x.Ativo).IsModified = true;
        }

        public List<string> BuscarCombosGruposDominios()
        {
            var query = @"SELECT distinct
                            VLTIPODOMINIO
                           FROM dbo.TBLDOMINIO 
                           ORDER BY VLTIPODOMINIO";

            var result = _context.Database.GetDbConnection().Query<string>(query);
            _context.Database.GetDbConnection().Close();

            return result.ToList();
        }

        public bool ValidarDominio(DominioDto dominioDto)
        {
            var valido = DbSet.Any(x =>
                x.ValorTipoDominio == dominioDto.GrupoDominio && 
                x.DescricaoValor.Replace(" ", "") == dominioDto.ValorDominio.Replace(" ", "") && 
                x.Ativo &&
                x.Id != dominioDto.Id);

            return !valido;
        }

        public IEnumerable<ComboDefaultDto> BuscarDominiosPorTipo(string tipo, bool ativo)
        {
            var query = $@"SELECT
	                       IDDOMINIO as Id
                          ,VLDOMINIO as Descricao
                          ,FLATIVO as Ativo
                          ,IDVALOR as IdValor
                          FROM dbo.TBLDOMINIO
                          where VLTIPODOMINIO = '{tipo}'";

            if (ativo)
            {
                query += " and FLATIVO = 1";
            }

            var result = _context.Database.GetDbConnection().Query<ComboDefaultDto>(query);
            _context.Database.GetDbConnection().Close();

            return result.ToList();
        }

        public IEnumerable<ComboDefaultDto> BuscarDominiosPropriedade(string tipo, bool ativo)
        {
            var query = "";
            switch (tipo)
            {
                case "ALIMENTAÇÃO":
                    query = $@" SELECT	
                                    	IDDOMINIO AS ID,
                                        VLDOMINIO AS DESCRICAO,
                                        FLATIVO AS ATIVO,
                                        IDVALOR AS IDVALOR 
                                    FROM TBLDOMINIO 
                                    	WHERE VLDOMINIO 
                                    		IN('OBRIGATÓRIO','COMPLEMENTAR')
                                            AND VLTIPODOMINIO = 'PROPRIEDADEBENEFICIO'";
                    break;
                case "ACADEMIA":
                    query = $@" SELECT	
                                	IDDOMINIO AS ID,
                                    VLDOMINIO AS DESCRICAO,
                                    FLATIVO AS ATIVO,
                                    IDVALOR AS IDVALOR 
                                FROM TBLDOMINIO 
                                	WHERE VLDOMINIO 
                                		IN('PARCERIA')
                                        AND VLTIPODOMINIO = 'PROPRIEDADEBENEFICIO'";
                    break;
                case "COMBUSTÍVEL":
                    query = $@" SELECT	
                                	IDDOMINIO AS ID,
                                    VLDOMINIO AS DESCRICAO,
                                    FLATIVO AS ATIVO,
                                    IDVALOR AS IDVALOR 
                                FROM TBLDOMINIO 
                                	WHERE VLDOMINIO 
                                		IN('COMPLEMENTAR')
                                        AND VLTIPODOMINIO = 'PROPRIEDADEBENEFICIO'";
                    break;
                case "EDUCAÇÃO":
                    query = $@" SELECT	
                                	IDDOMINIO AS ID,
                                    VLDOMINIO AS DESCRICAO,
                                    FLATIVO AS ATIVO,
                                    IDVALOR AS IDVALOR 
                                FROM TBLDOMINIO 
                                	WHERE VLDOMINIO 
                                		IN('PARCERIA')
                                        AND VLTIPODOMINIO = 'PROPRIEDADEBENEFICIO'";
                    break;
                case "PREVIDÊNCIA":
                    query = $@" SELECT	
                                	IDDOMINIO AS ID,
                                    VLDOMINIO AS DESCRICAO,
                                    FLATIVO AS ATIVO,
                                    IDVALOR AS IDVALOR 
                                FROM TBLDOMINIO 
                                	WHERE VLDOMINIO 
                                		IN('PGBL','VGBL')
                                        AND VLTIPODOMINIO = 'PROPRIEDADEBENEFICIO'";
                    break;
                case "REFEIÇÃO":
                    query = $@" SELECT	
                                    	IDDOMINIO AS ID,
                                        VLDOMINIO AS DESCRICAO,
                                        FLATIVO AS ATIVO,
                                        IDVALOR AS IDVALOR 
                                    FROM TBLDOMINIO 
                                    	WHERE VLDOMINIO 
                                    		IN('OBRIGATÓRIO','COMPLEMENTAR')
                                            AND VLTIPODOMINIO = 'PROPRIEDADEBENEFICIO'";
                    break;
                case "SAÚDE":
                    query = $@" SELECT	
                                    	IDDOMINIO AS ID,
                                        VLDOMINIO AS DESCRICAO,
                                        FLATIVO AS ATIVO,
                                        IDVALOR AS IDVALOR 
                                    FROM TBLDOMINIO 
                                    	WHERE VLDOMINIO 
                                    		IN('MENSAL','ODONTO','COPART')
                                            AND VLTIPODOMINIO = 'PROPRIEDADEBENEFICIO'";
                    break;
                case "SEGURO":
                    query = $@" SELECT	
                                    	IDDOMINIO AS ID,
                                        VLDOMINIO AS DESCRICAO,
                                        FLATIVO AS ATIVO,
                                        IDVALOR AS IDVALOR 
                                    FROM TBLDOMINIO 
                                    	WHERE VLDOMINIO 
                                    		IN('VIDA')
                                            AND VLTIPODOMINIO = 'PROPRIEDADEBENEFICIO'";
                    break;
                case "VALE TRANSPORTE":
                    query = $@" SELECT	
                                    	IDDOMINIO AS ID,
                                        VLDOMINIO AS DESCRICAO,
                                        FLATIVO AS ATIVO,
                                        IDVALOR AS IDVALOR 
                                    FROM TBLDOMINIO 
                                    	WHERE VLDOMINIO 
                                    		IN('TRANSPORTE','TAXA')
                                            AND VLTIPODOMINIO = 'PROPRIEDADEBENEFICIO'";
                    break;           
            }

            if (ativo)
            {
                query += " and FLATIVO = 1";
            }

            var result = _context.Database.GetDbConnection().Query<ComboDefaultDto>(query);
            _context.Database.GetDbConnection().Close();

            return result.ToList();
        }

    }
}
