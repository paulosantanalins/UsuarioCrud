using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Linq;
using System.Collections.Generic;
using Utils.Base;

namespace Cadastro.Domain.PrestadorRoot.Repository
{
    public interface IPrestadorRepository : IBaseRepository<Prestador>
    {
        Prestador BuscarPorIdComIncludes(int id);
        FiltroGenericoDtoBase<PrestadorDto> Filtrar(FiltroGenericoDtoBase<PrestadorDto> filtro);
        FiltroGenericoDtoBase<PrestadorDto> FiltrarHoras(FiltroGenericoDtoBase<PrestadorDto> filtro);
        FiltroGenericoDtoBase<AprovarPagamentoDto> FiltrarAprovarPagamento(FiltroGenericoDtoBase<AprovarPagamentoDto> filtro);
        DadosPagamentoPrestadorDto ObterDadosPagementoPorId(int id, int IdHorasMes);
        Prestador BuscarPorIdComIncludeCelula(int id);
        string ObterAprovador(Prestador prestador, int idHorasMes);
        IQueryable<Prestador> BuscarPorIdCelula(int id, int idHorasMes);
        List<Prestador> BuscarTodosSemTracking();
        decimal ObterValorMensalista(ValorPrestador valorPrestador, HorasMesPrestador horasMesPrestador);
        FiltroGenericoDtoBase<ConciliacaoPagamentoDto> FiltrarConciliacaoPagamentos(FiltroGenericoDtoBase<ConciliacaoPagamentoDto> filtro, bool resumo);
        IEnumerable<KeyValueDto> ObterPrestadoresPorCelula(int idCelula);
        Prestador ObterPrestadorParaTransferencia(int idPrestador);
        ClienteET ObterClientePrestador(int idCliente);
        ComboLocalDto ObterLocalTrabalhoPorId(int idLocal, int idCliente);
        ComboDefaultDto ObterClienteAtivoPorIdCelulaEAcesso(int idCelula, int idCliente);
        IEnumerable<Prestador> ObterPrestadoresParaTransferencia(int[] idsPrestadores);
        Prestador ObterPorIdComInativacoes(int idPrestador);
    }
}
