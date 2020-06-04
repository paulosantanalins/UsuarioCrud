using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using Logger.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces
{
    public interface ICelulaService
    {
        void RealizarMigracaoCelulas();
        void AtualizarMigracaoCelulas();
        void Adicionar(Celula celula);
        List<Celula> ObterTodas();
        List<VinculoTipoCelulaTipoContabil> ObterTodosTiposCelulatipoContabil();
        FiltroGenericoDto<ServicoCelulaDTO> BuscarServicosPendente(FiltroGenericoDto<ServicoCelulaDTO> filtro);
        List<Celula> ObterTodasEacesso();
        List<TipoCelula> ObterTodosTiposCelula();
        FiltroGenericoDto<CelulaDto> FiltrarCelula(FiltroGenericoDto<CelulaDto> filtro);
        string ObterEmailGerenteServico(int idCelula);
        List<Celula> BuscarCelulasQuePessoaEhResponsavel(int idPessoa);
        Celula ObterUltimaCadastrada();
        void Inativar(int id);
        void Reativar(int id);
        bool ValidarExisteCelula(int id);
        void Persistir(Celula celula, string loginResponsavel);
        Celula BuscarPorId(int id);
        Pessoa ObterPessoaApiCadastro(int? id);
        UsuarioAdDto ObterUsuarioAd(string login);
        Task<List<LogCampoCelulaDto>> ObterLogsDeAlteracaoDeCampoDeCelulaPorCelula(int idCelula);
        (int, string) ObterIdCelulaResponsavel(string cpf);
        bool ExisteServicoPendente(int idCelula);
        void RealizarInativacoesJob();
    }
}
