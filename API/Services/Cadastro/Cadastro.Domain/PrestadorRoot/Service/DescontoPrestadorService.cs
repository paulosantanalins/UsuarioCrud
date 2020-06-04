using AutoMapper;
using Cadastro.Domain.DominioRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Base;
using Utils.Connections;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class DescontoPrestadorService : IDescontoPrestadorService
    {
        private readonly IDominioRepository _dominioRepository;
        private readonly IPrestadorRepository _prestadorRepository;
        private readonly IDescontoPrestadorRepository _descontoPrestadorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public DescontoPrestadorService(
            IDominioRepository dominioRepository,
            IPrestadorRepository prestadorRepository,
            IDescontoPrestadorRepository descontoPrestadorRepository,
            IOptions<ConnectionStrings> connectionStrings,
            IUnitOfWork unitOfWork)
        {
            _dominioRepository = dominioRepository;
            _prestadorRepository = prestadorRepository;
            _descontoPrestadorRepository = descontoPrestadorRepository;
            _unitOfWork = unitOfWork;
            _connectionStrings = connectionStrings;
        }

        public FiltroGenericoDtoBase<DescontoPrestadorDto> Filtrar(FiltroGenericoDtoBase<DescontoPrestadorDto> filtro)
        {
            var result = _descontoPrestadorRepository.Filtrar(filtro);

            return Mapper.Map<FiltroGenericoDtoBase<DescontoPrestadorDto>>(result);
        }

        public DescontoPrestador BuscarPorId(int id)
        {
            var result = _descontoPrestadorRepository.BuscarPorId(id);
            return result;
        }

        public void AtualizarDescontoPrestadorr(DescontoPrestador descontoPrestador)
        {
            _descontoPrestadorRepository.Update(descontoPrestador);
            _unitOfWork.Commit();
        }

        public void SalvarDescontoPrestador(DescontoPrestador descontoPrestador)
        {
            _descontoPrestadorRepository.Adicionar(descontoPrestador);
            _unitOfWork.Commit();
        }

        public List<Prestador> ObterPrestadoresPorCelula(int id, int idHorasMes)
        {
            var result = _prestadorRepository.BuscarPorIdCelula(id, idHorasMes).OrderBy(x => x.Pessoa.Nome).ToList();
            foreach(var prestador in result)
            {
                prestador.Id = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes).Id;


            }
            return result;
        }

        public void Inativar(int id)
        {
            var model = _descontoPrestadorRepository.BuscarPorId(id);

            if (model != null)
            {
                _descontoPrestadorRepository.Remove(model);
                _unitOfWork.Commit();
            }
        }
    }
}
