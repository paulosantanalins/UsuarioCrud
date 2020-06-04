using Cadastro.Domain.EmpresaGrupoRoot.Dto;
using System.Collections.Generic;

namespace Cadastro.Domain.EmpresaGrupoRoot.Service.Interfaces
{
    public interface IEmpresaGrupoService
    {
        EmpresaGrupoRmDto BuscarNoRMPorId(int id, bool pagarPelaMatriz, int idFilial);
        EmpresaGrupoRmDto BuscarNoRMPorId(int id);
        DadosBancariosDaEmpresaGrupoDto BuscarDadosBancariosDaEmpresaDoGrupoPorIdEmpresaGrupoEBanco(int idEmpresaGrupo, string banco);
        IEnumerable<EmpresaGrupoRmDto> BuscarTodasNoRM();
        IEnumerable<EmpresaGrupoRmDto> BuscarNoRM();
    }
}
