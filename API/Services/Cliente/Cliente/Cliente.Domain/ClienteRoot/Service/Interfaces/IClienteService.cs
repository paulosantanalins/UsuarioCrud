using Cliente.Domain.ClienteRoot.Dto;
using Cliente.Domain.ClienteRoot.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;
using Utils.Base;

namespace Cliente.Domain.ClienteRoot.Service.Interfaces
{
    public interface IClienteService
    {
        ClienteET ObterPorId(int idCliente);
        int? ObterClientePorIdSalesForce(string idSalesForce);
        List<ComboDefaultDto> ObterClienteAtivoPorIdCelulaEAcesso(int idCelula);
        List<ComboDefaultDto> ObterTodosClientePorIdCelulaEAcesso(int idCelula);
        List<ComboLocalDto> ObterLocalTrabalhoIdClienteEAcesso(int idCliente);
        int PersistirCliente(ClienteET cliente);
        FiltroGenericoDto<ClienteET> Filtrar(FiltroGenericoDto<ClienteET> filtro);
        int VerificarIdCliente();
        string ApenasNumeros(string str);
        Task<bool> ValidarCNPJ(string cnpj, string pais);
        IEnumerable<ClienteET> ObterTodos();
        FiltroGenericoDto<ClienteEacessoDto> ObterClientesEacesso(FiltroGenericoDto<ClienteEacessoDto> filtro);
        ClienteEacessoDto ObterClienteEacesso(int idCliente);
        ClienteLocalTrabalhoEacessoDto ObterLocalTrabalhoEacesso(int idLocalTrabalho);
        ContatoClienteEacessoDto ObterContatoClienteEacesso(int idContato);
        Task<IEnumerable<MultiselectDto>> ObterClientesPorIds(List<int> ids);
        IEnumerable<ClienteET> ObterSomenteAtivos();
        bool VerificarExistenciaCliente(int idCliente);
        IEnumerable<ClienteET> ObterSomenteInativos();
        List<ClienteET> ObterClientesPorNome(string nome);
        ClienteET ObterClientePorIdEacesso(int idEAcesso);
        List<ClienteEacessoDto> ObterClientesEacessoPorIdCelula(int? id, bool isInativo);
        List<ClienteEacessoDto> ObterClientesEacessoPorIdCelulaEvisualizacaoServico(int? id, bool isInativo, string login);
    }
}
