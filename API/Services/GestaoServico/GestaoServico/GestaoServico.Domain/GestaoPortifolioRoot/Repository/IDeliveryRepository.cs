using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Repository
{
    public interface IDeliveryRepository : IBaseRepository<Delivery>
    {
        bool Validar(Delivery delivery);
        FiltroGenericoDto<Delivery> Filtrar(FiltroGenericoDto<Delivery> filtro);
    }
}
