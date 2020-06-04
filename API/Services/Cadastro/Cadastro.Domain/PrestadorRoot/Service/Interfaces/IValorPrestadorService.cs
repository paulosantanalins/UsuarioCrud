using Cadastro.Domain.PrestadorRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IValorPrestadorService
    {
        ValorPrestador BuscarPorId(int id);
        void RemoverValorPrestador(ValorPrestador valorPrestador);
        void InativarRemuneracaoPrestador(ValorPrestador valorPrestador);
        bool ValidaExcluir(ValorPrestador valorPrestador);
    }
}
