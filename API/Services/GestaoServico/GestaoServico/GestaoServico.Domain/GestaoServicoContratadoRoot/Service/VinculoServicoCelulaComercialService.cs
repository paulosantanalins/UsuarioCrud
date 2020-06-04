using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Repository;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using GestaoServico.Domain.Interfaces;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Service
{
    public class VinculoServicoCelulaComercialService : IVinculoServicoCelulaComercialService
    {
        private readonly IVinculoServicoCelulaComercialRepository _vinculoServicoCelulaComercialRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VinculoServicoCelulaComercialService(
            IVinculoServicoCelulaComercialRepository vinculoServicoCelulaComercialRepository,
            IUnitOfWork unitOfWork)
        {
            _vinculoServicoCelulaComercialRepository = vinculoServicoCelulaComercialRepository;
            _unitOfWork = unitOfWork;
        }

        public void Adicionar(VinculoServicoCelulaComercial vinculoServicoCelulaComercial)
        {
            _vinculoServicoCelulaComercialRepository.Adicionar(vinculoServicoCelulaComercial);
        }
    }
}
