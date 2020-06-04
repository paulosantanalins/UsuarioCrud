using Cadastro.Domain.EnderecoRoot.Entity;
using System.Collections.Generic;

namespace Cadastro.Domain.EmpresaGrupoRoot.Service.Interfaces
{
    public interface IEnderecoService
    {
        IEnumerable<Estado> BuscarEstados();
        IEnumerable<Estado> BuscarEstadosDeUmPais(int id);
        IEnumerable<Cidade> BuscarCidadesPeloEstado(int idEstado);
        Pais BuscarPaisPorId(int idPais);
        IEnumerable<AbreviaturaLogradouro> BuscarAbreviaturaLogradouro();
    }
}
