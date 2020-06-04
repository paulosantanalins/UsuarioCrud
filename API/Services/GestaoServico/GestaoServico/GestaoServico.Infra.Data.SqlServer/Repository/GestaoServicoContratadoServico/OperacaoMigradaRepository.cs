using GestaoServico.Domain.OperacaoMigradaRoot.DTO;
using GestaoServico.Domain.OperacaoMigradaRoot.Entity;
using GestaoServico.Domain.OperacaoMigradaRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.Base;
using Utils.Connections;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoServicoContratadoServico
{
    public class OperacaoMigradaRepository : BaseRepository<OperacaoMigrada>, IOperacaoMigradaRepository
    {
        private readonly IVariablesToken _variables;

        public OperacaoMigradaRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables, IOptions<ConnectionStrings> connectionStrings)
            : base(gestaoServicoContext, variables)
        {
            _variables = variables;
        }

        public List<OperacaoMigradaDTO> BuscarServicosPorGrupoDelivery(int idGrupoDelivery)
        {
            const string SERVICO_CONTRATADO_JA_ATRIBUIDO = "T";
            var query = DbSet.Where(x => x.Status != SERVICO_CONTRATADO_JA_ATRIBUIDO).AsQueryable().AsNoTracking();

            var dados = query.Where(x => x.IdGrupoDelivery == idGrupoDelivery).Select(p => new OperacaoMigradaDTO
            {
                Id = p.Id,
                IdCombinadaCelula = p.IdCombinadaCelula,
                IdGrupoDelivery = p.IdGrupoDelivery,
                DescricaoOperacao = p.DescricaoOperacao + " (" + p.IdGrupoDelivery + ")",
                IdCelula = p.IdCelula,
                IdCliente = p.IdCliente,
                IdServico = p.IdServico,
                IdTipoCelula = p.IdTipoCelula,
                NomeCliente = p.NomeCliente,
                DescricaoServico = "CÉL: " + CompletarZerosEsquerda(p.IdCelula, 3) + " | SERV: " + CompletarZerosEsquerda(p.IdServico, 6) + " - " + p.DescricaoServico.ToUpper(),
                Status = p.Status,
                Ativo = p.Ativo,
                DataAlteracao = p.DataAlteracao,
                Usuario = p.Usuario
            })
            .ToList();

            return dados;
        }

        private string CompletarZerosEsquerda(int numero, int tamMaxNum)
        {
            int tamNum = numero.ToString().Length;

            string zeros = "";
            for (int i = 0; i < tamMaxNum - tamNum; i++)
            {
                zeros += "0";
            }

            return zeros + numero;
        }

        public FiltroGenericoDtoBase<OperacaoMigradaDTO> Filtrar(FiltroGenericoDtoBase<OperacaoMigradaDTO> filtro)
        {
            const int TODOS = 1;
            const string TOTALMENTE_AGRUPADO = "T";
            const string SEM_AGRUPAMENTO = "S";
            const string PARCIALMENTE_AGRUPADO = "P";

            var query = DbSet.AsQueryable().AsNoTracking();

            var dados = query.GroupBy(x => x.IdGrupoDelivery).Select(p => new OperacaoMigradaDTO
            {
                Id = 0,
                IdCombinadaCelula = p.FirstOrDefault().IdCombinadaCelula,
                IdGrupoDelivery = p.Key,
                DescricaoOperacao = p.FirstOrDefault().DescricaoOperacao + " (" + p.Key + ")",
                IdCelula = p.FirstOrDefault().IdCombinadaCelula,
                IdCliente = 0,
                IdServico = 0,
                DescricaoServico = "",
                NomeCliente = string.Join(", ", p.GroupBy(x => x.NomeCliente).Select(x => x.Key)),
                Status = p.Any(y => !y.Status.Equals(TOTALMENTE_AGRUPADO)) ? 
                                        (p.Any(y => y.Status.Equals(TOTALMENTE_AGRUPADO)) ? PARCIALMENTE_AGRUPADO : SEM_AGRUPAMENTO)
                                        :
                                        TOTALMENTE_AGRUPADO,
                Ativo = true,
                DataAlteracao = p.OrderByDescending(x => x.DataAlteracao).FirstOrDefault().DataAlteracao,
                Usuario = p.OrderByDescending(x => x.DataAlteracao).FirstOrDefault().Usuario
            });

            dados = dados.Where(x => _variables.CelulasComPermissao.Any(y => y == x.IdCelula));

            if (filtro.Id != TODOS)
            {
                dados = dados.Where(x => !x.Status.Equals(TOTALMENTE_AGRUPADO));
            }

            if (!string.IsNullOrEmpty(filtro.FiltroGenerico) && filtro.FiltroGenerico.Any())
            {
                var idCelulas = filtro.FiltroGenerico.Split(',').Select(x => int.Parse(x)).ToList();

                dados = dados.Where(x => idCelulas.Any(y => y == x.IdCelula));
            }

            if (!string.IsNullOrEmpty(filtro.ValorParaFiltrar) && filtro.ValorParaFiltrar.Any())
            {
                filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

                dados = dados.Where(x =>
                                       x.IdCombinadaCelula.ToString().ToUpper().Equals(filtro.ValorParaFiltrar.ToUpper())
                                    || x.DescricaoOperacao.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || x.NomeCliente.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    );
            }

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
                dados = dados.OrderBy(x => x.IdCombinadaCelula).ThenBy(x => x.DescricaoOperacao).ThenBy(y => y.IdCombinadaCelula);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }
    }
}
