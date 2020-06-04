using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Dto;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Repository;
using RepasseEAcesso.Domain.SharedRoot;
using RepasseEAcesso.Infra.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Utils;
using Utils.Base;
using Utils.Extensions;

namespace RepasseEAcesso.Infra.Data.SqlServer.Repository.RepasseRepository
{
    public class RepasseNivelUmRepository : BaseRepository<RepasseNivelUm>, IRepasseNivelUmRepository
    {
        private readonly RepasseEAcessoContext _context;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IVariablesToken _variables;


        public RepasseNivelUmRepository(
            RepasseEAcessoContext context,
            MicroServicosUrls microServicosUrls,
            IVariablesToken variables) : base(context, variables)
        {
            _context = context;
            _microServicosUrls = microServicosUrls;
            _variables = variables;

        }

        public List<RepasseNivelUm> BuscarComInclude()
        {
            var query = DbSet
                        .Include(x => x.LogsRepasse)
                        .AsNoTracking();

            return query.ToList();
        }

        public void DetachEntity(RepasseNivelUm repasse)
        {
            _context.Entry(repasse).State = EntityState.Detached;
        }

        public List<RepasseNivelUm> BuscarTodosFilhos(int idRepasse) => DbSet.Where(x => x.IdRepasseMae == idRepasse).AsNoTracking().ToList();

        public List<RepasseNivelUm> BuscarTodosFilhosLegado(int idRepasseEacesso)
        {
            var query = DbSet
                        .Where(x => x.IdRepasseEacesso == idRepasseEacesso)
                        .AsNoTracking().ToList();

            return query.ToList();
        }

        public RepasseNivelUm BuscarComIncludeId(int id)
        {
            var query = DbSet
                        .Include(x => x.LogsRepasse)
                        .FirstOrDefault(x => x.Id == id);

            return query;
        }


        private PessoaDto BuscarEntidadePessoaUsuarioLogado(int idEAcesso)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Bearer", _variables.Token);          
                
                client.BaseAddress = new Uri(_microServicosUrls.UrlApiCadastro);               
                var result = client.GetAsync($"api/pessoa/obter-por-idEAcesso/{idEAcesso}").Result;

                var content = result.Content.ReadAsStringAsync().Result;
                var pessoa = JsonConvert.DeserializeObject<PessoaDto>(content);
                return pessoa;
            }
        }

        public Boolean ValidarUsuarioResponsavelPelaCelula(int idCelulaDestino)
        {
            var pessoaUsuarioLogado = BuscarEntidadePessoaUsuarioLogado(_variables.IdEacesso);
            var celulasQueUsuarioEhDono = BuscarCelulasPertencentesAoUsuario(pessoaUsuarioLogado.Id);

            return celulasQueUsuarioEhDono.Any(x=>x.Id ==idCelulaDestino)? true :false;
        }

        private List<CelulaDto> BuscarCelulasPertencentesAoUsuario(int idPessoaResponsavel)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Bearer", _variables.Token);            
                client.BaseAddress = new Uri(_microServicosUrls.UrlApiControle);                
                var result = client.GetAsync($"api/celula/celulas-que-pessoa-e-responsavel/{idPessoaResponsavel}").Result;
                var content = result.Content.ReadAsStringAsync().Result;
                var celulas = JsonConvert.DeserializeObject<IEnumerable<CelulaDto>>(content).ToList();
                return celulas;
            }
        }


        public FiltroGenericoDtoBase<AprovarRepasseDto> Filtrar(FiltroGenericoDtoBase<AprovarRepasseDto> filtro)
        {
            var query = DbSet.AsQueryable().AsNoTracking();
            var periodoVigente = _context.PeriodoRepasses.LastOrDefault();

            var pessoaUsuarioLogado = BuscarEntidadePessoaUsuarioLogado(_variables.IdEacesso);            
            var celulasQueUsuarioEhDono = BuscarCelulasPertencentesAoUsuario(pessoaUsuarioLogado.Id);

            query = query.Where(x => _variables.CelulasComPermissao.Any(y => y == x.IdCelulaDestino));
            query = query.Where(x => x.DataRepasse <= periodoVigente.DtLancamentoFim
                && x.Status == SharedEnuns.StatusRepasseEacesso.NAO_ANALISADO.GetDescription());

            if (!string.IsNullOrEmpty(filtro.ValorParaFiltrar) && filtro.ValorParaFiltrar.Any())
            {
                var valorParaFiltrarSplit = filtro.ValorParaFiltrar.Split(',');

                if (valorParaFiltrarSplit != null && valorParaFiltrarSplit.Length > 0)
                {
                    var idCelula = int.Parse(valorParaFiltrarSplit[0]);
                    if (idCelula > 0)
                    {
                        query = query.Where(x => x.IdCelulaOrigem.ToString().ToUpper().Equals(valorParaFiltrarSplit[0])
                                               || x.IdCelulaDestino.ToString().ToUpper().Equals(valorParaFiltrarSplit[0])
                                           );
                    }

                    var idCliente = int.Parse(valorParaFiltrarSplit[1]);
                    if (idCliente > 0)
                    {
                        query = query.Where(x => x.IdClienteDestino.ToString().ToUpper().Equals(valorParaFiltrarSplit[1]));
                    }

                    var idServico = int.Parse(valorParaFiltrarSplit[2]);
                    if (idServico > 0)
                    {
                        query = query.Where(x => x.IdServicoDestino.ToString().ToUpper().Equals(valorParaFiltrarSplit[2]));
                    }

                    //var idPeriodo = int.Parse(valorParaFiltrarSplit[3]);
                    //if (idPeriodo > 0)
                    //{
                    //    query = query.Where(x => x.IdPeriodoRepasse.ToString().ToUpper().Equals(valorParaFiltrarSplit[3]));
                    //}
                }
            }

            var dados = query.Select(p => new AprovarRepasseDto
            {
                Id = p.Id,
                Aprovar = ObterStatusAprovacao(p.Status),
                Status = p.Status,
                StatusDesc = ObterNomeStatus(p.Status),
                IdCelulaDestino = p.IdCelulaDestino,
                IdCelulaOrigem = p.IdCelulaOrigem,
                ClienteDestino = p.NomeClienteDestino,
                DataAlteracao = p.DataAlteracao,
                DataRepasse = p.DataRepasse,
                QuantidadeHoras = p.QuantidadeItens,
                ServicoDestino = p.NomeServicoDestino,
                Usuario = p.Usuario,
                ValorTotal = p.ValorTotal,
                ValorUnitario = p.ValorUnitario,
                ValorCustoProfissional = p.ValorCustoProfissional,
                Desabilita = !DataRepasseEstaDentroDoUltimoPeriodoCadastrado(p.DataRepasse, periodoVigente),
                UsuarioLogadoEhDonoDaCelulaPagadora = celulasQueUsuarioEhDono.Any(x => x.Id == p.IdCelulaDestino),
                Descricao = p.DescricaoProjeto
            });

            if (!string.IsNullOrEmpty(filtro.FiltroGenerico) && filtro.FiltroGenerico.Any())
            {
                dados = dados.Where(x =>
                                       x.IdCelulaOrigem.ToString().ToUpper().Equals(filtro.FiltroGenerico.ToUpper())
                                       || x.IdCelulaDestino.ToString().ToUpper().Equals(filtro.FiltroGenerico.ToUpper())
                                       || (x.ClienteDestino != null ? x.ClienteDestino.ToUpper().Contains(filtro.FiltroGenerico.ToUpper()) : false)
                                       || (x.ServicoDestino != null ? x.ServicoDestino.ToUpper().Contains(filtro.FiltroGenerico.ToUpper()) : false)
                                       || (x.ValorCustoProfissional.HasValue ? x.ValorCustoProfissional.Value.Equals(filtro.FiltroGenerico.ToUpper()) : false)
                                       || (x.ValorUnitario.HasValue ? x.ValorUnitario.Value.Equals(filtro.FiltroGenerico.ToUpper()) : false)
                                       || (x.QuantidadeHoras.HasValue ? x.QuantidadeHoras.Value.Equals(filtro.FiltroGenerico.ToUpper()) : false)
                                       || (x.ValorTotal.HasValue ? x.ValorTotal.Value.Equals(filtro.FiltroGenerico.ToUpper()) : false));
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
                dados = dados.OrderBy(x => x.Status).ThenBy(x => x.IdCelulaOrigem).ThenBy(y => y.ClienteDestino);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
           
                return filtro;
        }

        private bool DataRepasseEstaDentroDoUltimoPeriodoCadastrado(DateTime dataRepasse, PeriodoRepasse periodoVigente) =>
            dataRepasse >= periodoVigente.DtAnaliseInicio && dataRepasse <= periodoVigente.DtAnaliseFim;
               
        public FiltroRepasseNivelUmDto<AprovarRepasseDto> Filtrar(FiltroRepasseNivelUmDto<AprovarRepasseDto> filtro)
        {
            var periodoDoFiltro = _context.PeriodoRepasses.AsNoTracking().LastOrDefault(x => x.Id == filtro.IdPeriodoRepasse);

            var query = DbSet.AsQueryable().AsNoTracking();
            query = query.Where(x => x.DataLancamento.Month == periodoDoFiltro.DtLancamento.Month
                                && x.DataLancamento.Year == periodoDoFiltro.DtLancamento.Year);

            if (_variables.CelulasComPermissao.Any() && filtro.CelulasEscolhidas.Length == 0)
            {
                filtro.CelulasEscolhidas = _variables.CelulasComPermissao.ToArray();
            }

            if (filtro.Id == 1)
            {
                //query = query.Where(x => Variables.CelulasComPermissao.Any(y => y == x.IdCelulaDestino));
                query = query.Where(x => filtro.CelulasEscolhidas.Contains(x.IdCelulaDestino));
            }
            else
            {
                //query = query.Where(x => Variables.CelulasComPermissao.Any(y => y == x.IdCelulaOrigem));
                query = query.Where(x => filtro.CelulasEscolhidas.Contains(x.IdCelulaOrigem));
            }

            if (!string.IsNullOrEmpty(filtro.ValorParaFiltrar))
            {
                filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

                query = query.Where(x => x.NomeClienteDestino.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                              || (x.Id.ToString().Contains(filtro.ValorParaFiltrar.ToUpper())
                              || (x.NomeServicoDestino.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()))));
            }

            if (filtro.Status != null)
            {
                query = query.Where(x => x.Status == filtro.Status.GetDescription());
            }


            var dados = query.Select(p => new AprovarRepasseDto
            {
                Id = p.Id,
                Aprovar = ObterStatusAprovacao(p.Status),
                Status = p.Status,
                StatusDesc = ObterNomeStatus(p.Status),
                IdCelulaDestino = p.IdCelulaDestino,
                IdCelulaOrigem = p.IdCelulaOrigem,
                ClienteDestino = p.NomeClienteDestino,
                DataAlteracao = p.DataAlteracao,
                QuantidadeHoras = p.QuantidadeItens,
                ServicoDestino = p.NomeServicoDestino,
                Usuario = p.Usuario,
                ValorTotal = p.ValorTotal,
                ValorUnitario = p.ValorUnitario,
                ValorCustoProfissional = p.ValorCustoProfissional,
                Desabilita = p.Status == SharedEnuns.StatusRepasseEacesso.NAO_ANALISADO.GetDescription() ? false : true,
                Descricao = p.DescricaoProjeto
            });

            var lista = dados.ToList();


            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                lista = lista.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x)).ToList();
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                lista = lista.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x)).ToList();
            }
            else
            {
                lista = lista.OrderBy(x => x.Status).ThenBy(x => x.IdCelulaOrigem).ThenBy(y => y.ClienteDestino).ToList();
            }

            filtro.Total = dados.Count();
            filtro.Valores = lista.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        public FiltroRepasseNivelDoisDto<AprovarRepasseNivelDoisDto> Filtrar(FiltroRepasseNivelDoisDto<AprovarRepasseNivelDoisDto> filtro)
        {
            var query = DbSet.AsQueryable().AsNoTracking();

            var periodoVigente = _context.PeriodoRepasses.LastOrDefault();

            //query = query.Where(x => x.PeriodoRepasse.Id == filtro.IdPeriodoRepasse);
            query = query.Where(x => x.DataRepasse <= periodoVigente.DtLancamentoFim && x.Status == SharedEnuns.StatusRepasseEacesso.APROVADO_NIVEL_UM.GetDescription());

            if (filtro.IdsCelulasOrigem.Length > 0 && filtro.IdsCelulasDestino.Length > 0)
            {
                query = query.Where(x => filtro.IdsCelulasOrigem.Contains(x.IdCelulaOrigem)
                                    || filtro.IdsCelulasDestino.Contains(x.IdCelulaDestino));
            }
            else if (filtro.IdsCelulasOrigem.Length > 0 && filtro.IdsCelulasDestino.Length == 0)
            {
                query = query.Where(x => filtro.IdsCelulasOrigem.Contains(x.IdCelulaOrigem));
            }
            else if (filtro.IdsCelulasOrigem.Length == 0 && filtro.IdsCelulasDestino.Length > 0)
            {
                query = query.Where(x => filtro.IdsCelulasDestino.Contains(x.IdCelulaDestino));
            }

            if (!string.IsNullOrEmpty(filtro.FiltroGenerico))
            {
                query = query.Where(x => x.NomeClienteOrigem.ToUpper().Contains(filtro.FiltroGenerico.ToUpper())
                                    || x.NomeClienteDestino.ToString().Contains(filtro.FiltroGenerico.ToUpper())
                                    || x.NomeServicoOrigem.ToString().Contains(filtro.FiltroGenerico.ToUpper())
                                    || x.NomeServicoDestino.ToUpper().Contains(filtro.FiltroGenerico.ToUpper()));
            }

            var dados = query.Select(p => new AprovarRepasseNivelDoisDto
            {
                Id = p.Id,
                Aprovado = ObterStatusAprovacaoNivelDois(p.Status),
                Status = p.Status,
                StatusDesc = ObterNomeStatus(p.Status),
                IdCelulaDestino = p.IdCelulaDestino,
                IdCelulaOrigem = p.IdCelulaOrigem,
                ClienteOrigem = p.NomeClienteOrigem,
                ClienteDestino = p.NomeClienteDestino,
                DataAlteracao = p.DataAlteracao,
                QuantidadeHoras = p.QuantidadeItens,
                ServicoOrigem = p.NomeServicoOrigem,
                ServicoDestino = p.NomeServicoDestino,
                Usuario = p.Usuario,
                DataRepasse = p.DataRepasse,
                ValorTotal = p.ValorTotal,
                ValorUnitario = p.ValorUnitario,
                EstaNoPeriodoVigente = DataRepasseEstaDentroDoUltimoPeriodoCadastrado(p.DataRepasse, periodoVigente),
                ValorCustoProfissional = p.ValorCustoProfissional,
                Descricao = p.DescricaoProjeto
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
                dados = dados.OrderBy(x => x.Status).ThenBy(x => x.IdCelulaOrigem).ThenBy(y => y.ClienteDestino);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        public IEnumerable<string> ObterLoginsComFuncionalidadeAprovarNivelDois()
        {
            var query = @"SELECT DISTINCT usuarioPerfil.LGUSUARIOLOGADO
                            FROM dbo.TBLFUNCIONALIDADE func
                            JOIN dbo.TBLVINCULOPERFILFUNCIONALIDADE perfilfunc ON perfilfunc.IDFUNCIONALIDADE = func.IDFUNCIONALIDADE
                            JOIN dbo.tblUsuarioPerfil usuarioPerfil ON usuarioPerfil.IDPERFIL = perfilfunc.IDPERFIL
                          WHERE func.NMFUNCIONALIDADE = 'AprovarRepasseNivelDois'";

            var result = _context.Database.GetDbConnection().Query<string>(query);
            _context.Database.GetDbConnection().Close();

            return result;
        }

        private bool? ObterStatusAprovacao(string status)
        {
            switch (status)
            {
                case "A1":
                    return true;
                case "AP":
                    return true;
                case "NG":
                    return false;
                default:
                    return null;
            }
        }

        private bool? ObterStatusAprovacaoNivelDois(string status)
            => status.Trim().ToUpper() == "AP" ? true : status.Trim().ToUpper() == "NG" ? false : (bool?)null;


        private string ObterNomeStatus(string status)
        {
            switch (status)
            {
                case "A1":
                    return "APROVADO NIVEL 1";
                case "AP":
                    return "APROVADO";
                case "NG":
                    return "NEGADO";
                case "CC":
                    return "CANCELADO";
                default:
                    return null;
            }
        }


    }
}
