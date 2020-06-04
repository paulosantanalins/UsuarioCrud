using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.Extensions;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class HorasMesPrestadorRepository : BaseRepository<HorasMesPrestador>, IHorasMesPrestadorRepository
    {
        public HorasMesPrestadorRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {

        }
        
        public int BuscarPorIdHoraMes(int idHoraMes)
        {
            var result = DbSet.Where(x => x.IdHorasMes == idHoraMes).ToList();
            return result.Count;
        }

        public HorasMesPrestador BuscarLancamentoParaPeriodoVigente(int idPrestador, int idHorasMes)
        {
            var result = DbSet.Include(x => x.LogsHorasMesPrestador).FirstOrDefault(x => x.IdHorasMes == idHorasMes && x.IdPrestador == idPrestador);
            return result;
        }

        public HorasMesPrestador BuscarLancamentoParaPeriodoVigenteIdHoraMesPrestador(int idHoraMesPrestador)
        {
            var result = DbSet.Include(x => x.LogsHorasMesPrestador).FirstOrDefault(x => x.Id == idHoraMesPrestador);
            return result;
        }

        public List<HorasMesPrestador> BuscarAprovacoesPendentes(int idHoraMes)
        {
            var result = DbSet
                            .Include(x => x.HorasMes.PeriodosDiaPagamento)
                            .Include(x => x.Prestador.Celula.CelulaSuperior)
                            .Include(x => x.Prestador.Celula.Pessoa)
                            .AsNoTracking()
                            .Where(x => 
                            x.IdHorasMes == idHoraMes && 
                            (
                                x.Situacao == SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_CADASTRADAS.GetDescription() ||
                                x.Situacao == SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_RECADASTRADAS.GetDescription()
                            )).ToList();
            return result;
        }

        public HorasMesPrestador BuscarPorIdComIncludes(int id)
        {      
            var result = DbSet
                        .Include(x => x.HorasMes)
                        .Include(x => x.DescontosPrestador)
                        .Include(x => x.Prestador.Celula)
                        .Include(x => x.Prestador.Pessoa.Telefone)
                        .Include(x => x.Prestador.ValoresPrestador)
                        .Include(x => x.Prestador.Pessoa.Nacionalidade)
                        .Include(x => x.Prestador.TipoRemuneracao)
                        .Include(x => x.Prestador.Cargo)
                        .Include(x => x.Prestador.Contratacao)
                        .Include(x => x.Prestador.Pessoa.Escolaridade)
                        .Include(x => x.Prestador.Pessoa.Extensao)
                        .Include(x => x.Prestador.Pessoa.Graduacao)
                        .Include(x => x.Prestador.Pessoa.EstadoCivil)
                        .Include(x => x.Prestador.Pessoa.Sexo)
                        .Include(x => x.Prestador.SituacaoPrestador)
                        .Include(x => x.Prestador.DiaPagamento)
                        .Include(x => x.Prestador.AreaFormacao)
                        .Include(x => x.Prestador.ClientesServicosPrestador)
                        .Include(x => x.PrestadoresEnvioNf)
                        .Include(x => x.Prestador.EmpresasPrestador)
                            .ThenInclude(x => x.Empresa)
                                .ThenInclude(x => x.Endereco)
                        .Include(x => x.Prestador.Pessoa.Endereco)
                                .ThenInclude(x => x.Cidade)
                                    .ThenInclude(x => x.Estado)
                                        .ThenInclude(x => x.Pais)
                        .Where(x => x.Id == id)
                        .FirstOrDefault();

            return result;
        }

        public List<HorasMesPrestador> BuscarLancamentosAprovados(int idHorasMes)
        {
            var result = DbSet
                        .Include(x => x.Prestador.DiaPagamento)
                        .Where(x => x.IdHorasMes == idHorasMes
                                        && x.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_APROVADAS.GetDescription())).ToList();
            return result;
        }

        public List<HorasMesPrestador> BuscarLancamentosComPagamentoPendente(int idHorasMes)
        {
            var result = DbSet
                        .Include(x => x.Prestador.DiaPagamento)
                        .Where(x => x.IdHorasMes == idHorasMes
                                        && x.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.NOTA_FISCAL_SOLICITADA.GetDescription())).ToList();
            return result;
        }

        public List<HorasMesPrestador> BuscarLancamentosComPagamentoSolicitado(int idHorasMes)
        {
            var result = DbSet
                        .Include(x => x.Prestador.DiaPagamento)
                        .Where(x => x.IdHorasMes == idHorasMes && x.IdChaveOrigemIntRm.HasValue && (
                                        x.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.AGUARDANDO_INTEGRACAO.GetDescription()) 
                                        || x.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.AGUARDANDO_ENTRADA_DA_NF.GetDescription())
                                        || x.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.AGUARDANDO_PAGAMENTO.GetDescription())
                                        || x.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.CANCELADO.GetDescription())
                                        || x.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.ERRO_SOLICITAR_PAGAMENTO.GetDescription()))
                                    ).ToList();
            return result;
        }
    }
}
