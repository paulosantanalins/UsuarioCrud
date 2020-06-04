using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;
using Utils;
using Utils.Base;

namespace Cadastro.Domain.PrestadorRoot.Repository
{
    public interface IEmpresaPrestadorRepository : IBaseRepository<EmpresaPrestador>
    {
        EmpresaPrestador BuscarPorId(int id);
        List<RelatorioPrestadoresDto>BuscarRelatorioPrestadores(int empresaId, int filialId, FiltroGenericoDtoBase<RelatorioPrestadoresDto> filtro);
        Empresa BuscarEmpresaAtiva(int idPrestador);
    }
}
