using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoPortfolioRepository
{
    public class DeliveryRepository : BaseRepository<Delivery>, IDeliveryRepository
    {
        public DeliveryRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables)
            : base(gestaoServicoContext, variables)
        {

        }
        public FiltroGenericoDto<Delivery> Filtrar(FiltroGenericoDto<Delivery> filtro)
        {
            throw new NotImplementedException();
        }

        public bool Validar(Delivery delivery)
        {
            throw new NotImplementedException();
        }
    }
}
