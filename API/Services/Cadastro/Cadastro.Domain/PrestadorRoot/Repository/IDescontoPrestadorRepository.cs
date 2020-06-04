using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.Base;

namespace Cadastro.Domain.PrestadorRoot.Repository
{
    public interface IDescontoPrestadorRepository : IBaseRepository<DescontoPrestador>
    {
        FiltroGenericoDtoBase<DescontoPrestadorDto> Filtrar(FiltroGenericoDtoBase<DescontoPrestadorDto> filtro);
        decimal ObterValorDescontoPorIdHorasMesPrestador(int idHorasMesPrestador, string tipoDesconto);
    }
}
