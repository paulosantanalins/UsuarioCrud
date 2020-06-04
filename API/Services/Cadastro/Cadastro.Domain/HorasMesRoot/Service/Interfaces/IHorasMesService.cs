using Cadastro.Domain.HorasMesRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.HorasMesRoot.Service.Interfaces
{
    public interface IHorasMesService
    {
        IEnumerable<HorasMes> BuscarPeriodos();
        HorasMes BuscarPorId(int id);
        void AtualizarHorasMes(HorasMes horasMes);
        void SalvarHorasMes(HorasMes horasMes);
        void Inativar(int id);
    }
}
