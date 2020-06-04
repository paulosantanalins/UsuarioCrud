using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Repository;
using RepasseEAcesso.Domain.RepasseRoot.Service.Interfaces;
using RepasseEAcesso.Domain.SharedRoot.UoW.Interfaces;
using static RepasseEAcesso.Domain.SharedRoot.SharedEnuns;

namespace RepasseEAcesso.Domain.RepasseRoot.Service {
    public class LogRepasseService : ILogRepasseService
    {
        private readonly ILogRepasseRepository _logRepasseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogRepasseService(ILogRepasseRepository logRepasseRepository, IUnitOfWork unitOfWork)
        {
            _logRepasseRepository = logRepasseRepository;
            _unitOfWork = unitOfWork;
        }

        


    }
}
