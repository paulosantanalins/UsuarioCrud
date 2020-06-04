using Cadastro.Domain.CidadeRoot.Dto;
using Cadastro.Domain.EnderecoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.Base;

namespace Cadastro.Domain.CidadeRoot.Service.Interfaces
{
    public interface ICidadeService
    {
        Cidade BuscarPorId(int id);
        IEnumerable<Pais> BuscarPaises();
        void PersistirCidade(Cidade cidade);
        FiltroGenericoDtoBase<CidadeDto> Filtrar(FiltroGenericoDtoBase<CidadeDto> filtro);  
    }
}
