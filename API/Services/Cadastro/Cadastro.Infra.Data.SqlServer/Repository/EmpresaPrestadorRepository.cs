using Cadastro.Domain.FilialRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;
using Utils.Base;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class EmpresaPrestadorRepository : BaseRepository<EmpresaPrestador>, IEmpresaPrestadorRepository
    {
        private readonly IFilialService _filialService;

        public EmpresaPrestadorRepository(CadastroContexto context, IVariablesToken variables,
            IAuditoriaRepository auditoriaRepository,
            IFilialService filialService)
            : base(context, variables, auditoriaRepository)
        {
            _filialService = filialService;
        }

        public EmpresaPrestador BuscarPorId(int id)
        {
            var result = DbSet.Where(x => x.Id == id).FirstOrDefault();
            return result;
        }


        public List<RelatorioPrestadoresDto> BuscarRelatorioPrestadores(int empresaId, int filialId, FiltroGenericoDtoBase<RelatorioPrestadoresDto> filtro)
        {
            IQueryable<EmpresaPrestador> query = QueryBaseFiltrarPrestadores();

            if (empresaId != 0)
                query = query.Where(x => x.Prestador.IdEmpresaGrupo.Equals(empresaId));
            if (filialId != 0)
                query = query.Where(x => x.Prestador.IdFilial.Equals(filialId));

            query = filtro.Id == -1 ? query :
                filtro.Id == 1 ? query.Where(x => x.Prestador.DataDesligamento != null) :
                query.Where(x => x.Prestador.DataDesligamento == null && x.Empresa.Ativo);

            query = query.Where(x => _variables.CelulasComPermissao.Any(y => y == x.Prestador.IdCelula));
            return TratarRetornoQueryFiltrarRelatorioPrestadores(query);
        }



        public IQueryable<EmpresaPrestador> QueryBaseFiltrarPrestadores()
        {

            var result = DbSet.AsQueryable().AsNoTracking();

            result = result
                        .Include(x => x.Empresa)
                        .Include(x => x.Prestador)
                           .ThenInclude(x => x.Cargo)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.ValoresPrestador)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.Contratacao)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.TipoRemuneracao)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.DiaPagamento)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.Pessoa)
                                .ThenInclude(x => x.EstadoCivil)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.Pessoa)
                               .ThenInclude(x => x.Endereco)
                                   .ThenInclude(x => x.Cidade)
                                       .ThenInclude(x => x.Estado)
                       .Include(x => x.Empresa)
                               .ThenInclude(x => x.Endereco)
                                   .ThenInclude(x => x.Cidade)
                                       .ThenInclude(x => x.Estado)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.Pessoa)
                               .ThenInclude(x => x.Telefone)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.Pessoa)
                               .ThenInclude(x => x.Sexo)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.SituacaoPrestador)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.DiaPagamento)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.AreaFormacao)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.InativacoesPrestador)
                       .Include(x => x.Prestador)
                           .ThenInclude(x => x.Celula)
                               .ThenInclude(x => x.CelulaSuperior)
                       .Include(X => X.Prestador)
                           .ThenInclude(x => x.ClientesServicosPrestador)
                       .Include(X => X.Prestador)
                           .ThenInclude(x => x.ContratosPrestador)
                       .AsNoTracking();

            return result;
        }


        private List<RelatorioPrestadoresDto> TratarRetornoQueryFiltrarRelatorioPrestadores(IQueryable<EmpresaPrestador> query)
        {
            var result = query.Select(x => new RelatorioPrestadoresDto
            {
                Admissao = x.Prestador.DataInicio.ToShortDateString() ?? "",
                Cargo = x.Prestador.Cargo.DescricaoValor ?? "",
                Celula = x.Prestador.Celula.Id.ToString() ?? "",
                IdCelula = x.Prestador.Celula.Id,
                Cidade = x.Prestador.Pessoa.Endereco.Cidade != null ? x.Prestador.Pessoa.Endereco.Cidade.NmCidade : "",
                IdCliente = x.Prestador.ClientesServicosPrestador.Any(y => y.Ativo) ? x.Prestador.ClientesServicosPrestador.FirstOrDefault(y => y.Ativo).IdCliente : 0,
                CNPJ = x.Empresa.Cnpj ?? "",
                Contratacao = x.Prestador.Contratacao.DescricaoValor ?? "",
                CPF = x.Prestador.Pessoa.Cpf ?? "",
                //Custo = x.Prestador.SituacaoPrestador.DescricaoValor,
                DiaPagamento = x.Prestador.DiaPagamento != null ? x.Prestador.DiaPagamento.DescricaoValor : "",
                Diretoria = x.Prestador.Celula.CelulaSuperior != null ? x.Prestador.Celula.CelulaSuperior.Descricao : "",
                IdEmpresaGrupo = x.Prestador.IdEmpresaGrupo ?? 0,
                Estado = x.Empresa.Endereco.Cidade != null ? x.Empresa.Endereco.Cidade.Estado.NmEstado : "",
                Nome = x.Prestador.Pessoa.Nome ?? "",
                RazaoSocial = x.Empresa.RazaoSocial ?? "",
                IdServico = x.Prestador.ClientesServicosPrestador.Any(y => y.Ativo) ? x.Prestador.ClientesServicosPrestador.FirstOrDefault(y => y.Ativo).IdServico : 0,
                Sexo = x.Prestador.Pessoa.Sexo.DescricaoValor ?? "",
                TipoRemuneracao = x.Prestador.TipoRemuneracao != null ? x.Prestador.TipoRemuneracao.DescricaoValor : "",
                Valor = x.Prestador.ValoresPrestador.Count > 0 ? x.Prestador.TipoRemuneracao.DescricaoValor.Equals("HORISTA")
                    ? x.Prestador.ValoresPrestador.OrderByDescending(y => y.DataAlteracao).First().ValorHora.ToString()
                        : x.Prestador.ValoresPrestador.OrderByDescending(y => y.DataAlteracao).First().ValorMes.ToString()
                    : "",
                NomeFilial = x.Empresa.IdEmpresaRm != null ? BuscarFilialNoRm(x.Prestador.IdFilial, x.Prestador.IdEmpresaGrupo ?? 0) : "",
                DataDesligamento = ObterDataDesligamentoPrestador(x.Prestador) ?? "",
                DescricaoDoTrabalho = ObterDescricaoTrabalhoPrestador(x.Prestador.ClientesServicosPrestador) ?? "",
                DataVigenciaEmpresa = x.Empresa.DataVigencia
            }).OrderBy(x => x.Celula).ToList();

            result = result.GroupBy(x => x.CPF)
                .Select(x => x.First(y => y.DataVigenciaEmpresa == x.Max(z => z.DataVigenciaEmpresa))).ToList();

            return result;
        }

        public string BuscarFilialNoRm(int idFilial, int idEmpresa) => _filialService.BuscarFilialNoRm(idFilial, idEmpresa)?.Descricao ?? null;
        public string ObterDataDesligamentoPrestador(Prestador p) => p.DataDesligamento != null ? ((DateTime)p.DataDesligamento).ToString("dd/MM/yyyy", new CultureInfo("pt-BR")) : null;
        public string ObterDescricaoTrabalhoPrestador(IEnumerable<ClienteServicoPrestador> cs) => cs?.LastOrDefault(x => x.Ativo)?.DescricaoTrabalho ?? null;

        public Empresa BuscarEmpresaAtiva(int idPrestador)
        {
            var result = DbSet
                        .Include(x => x.Empresa)
                        .Where(x => x.IdPrestador == idPrestador)
                        .Select(x => x.Empresa)
                        .OrderByDescending(x => x.Id);

            return result.FirstOrDefault();
        }
    }
}
