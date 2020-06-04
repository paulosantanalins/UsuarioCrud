using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.Base;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IDescontoPrestadorService
    {
        FiltroGenericoDtoBase<DescontoPrestadorDto> Filtrar(FiltroGenericoDtoBase<DescontoPrestadorDto> filtro);
        DescontoPrestador BuscarPorId(int id);
        void AtualizarDescontoPrestadorr(DescontoPrestador descontoPrestador);
        void SalvarDescontoPrestador(DescontoPrestador descontoPrestador);
        void Inativar(int id);
        List<Prestador> ObterPrestadoresPorCelula(int id, int idHorasMes);
    }
}
