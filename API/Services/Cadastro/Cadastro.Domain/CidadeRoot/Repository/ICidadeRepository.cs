using Cadastro.Domain.CidadeRoot.Dto;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.Base;

namespace Cadastro.Domain.CidadeRoot.Repository
{
    public interface ICidadeRepository : IBaseRepository<Cidade>
    {
        int? ObterIdPeloNome(string nome);

        Cidade BuscarPorIdComIncludes(int id);
        Cidade BuscarPorNomeComIncludes(string nmCidade);

        FiltroGenericoDtoBase<CidadeDto> FiltrarCidades(FiltroGenericoDtoBase<CidadeDto> filtro);
    }
}
