using Cadastro.Domain.NotaFiscalRoot.Entity;
using Cadastro.Domain.NotaFiscalRoot.Repository;
using Cadastro.Domain.NotaFiscalRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Connections;

namespace Cadastro.Domain.NotaFiscalRoot.Service
{
    public class NotaFiscalService : INotaFiscalService
    {
        private readonly IPrestadorEnvioNfRepository _prestadorEnvioNfRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotaFiscalService(IPrestadorEnvioNfRepository prestadorEnvioNfRepository,
            IOptions<ConnectionStrings> connectionStrings,
            IUnitOfWork unitOfWork
            )
        {
            _prestadorEnvioNfRepository = prestadorEnvioNfRepository;
            _unitOfWork = unitOfWork;
        }

        public PrestadorEnvioNf BuscarNfPrestadorInfoPorToken(string token)
        {
            return _prestadorEnvioNfRepository.BuscarInfoParaEnvioNfPrestadorPorToken(token);
        }

        public PrestadorEnvioNf BuscarNfPrestadorInfoPorIdHorasMesPrestador(int idHorasMesPrestador)
        {
            return _prestadorEnvioNfRepository.BuscarNfPrestadorInfoPorIdHorasMesPrestador(idHorasMesPrestador);
        }

        public void AtualizarNotaFiscalPrestador(PrestadorEnvioNf prestadorEnvioNf)
        {
            prestadorEnvioNf.CaminhoNf = prestadorEnvioNf.Token;

            _prestadorEnvioNfRepository.Update(prestadorEnvioNf);
        }

        public string BuscarTokenPorIdHorasMesPrestador(int idHorasMesPrestador)
        {            
            var result = _prestadorEnvioNfRepository.Buscar(x => x.IdHorasMesPrestador == idHorasMesPrestador).OrderByDescending(x => x.Id).FirstOrDefault();

            //Valida se existe o token, caso contrário cria novamente
            if (result == null)
            {
                var prestadorEnvioNf = new PrestadorEnvioNf
                {
                    IdHorasMesPrestador = idHorasMesPrestador,
                    Token = Guid.NewGuid().ToString(),
                };

                _prestadorEnvioNfRepository.Adicionar(prestadorEnvioNf);
                _unitOfWork.Commit();
            }

            return result.Token;
        }

        public string MontarNomeArquivoPdf(string nomeArquivo)        {

            var prestadorEnvioNf = BuscarNfPrestadorInfoPorToken(nomeArquivo.Split(".pdf")[0]);

             return prestadorEnvioNf.HorasMesPrestador.Prestador.Celula.Id 
                + "_" + prestadorEnvioNf.HorasMesPrestador.Prestador.Pessoa.Nome
                + "_" + prestadorEnvioNf.HorasMesPrestador.HorasMes.Mes 
                + "-" + prestadorEnvioNf.HorasMesPrestador.HorasMes.Ano;
        }

        public List<PrestadorEnvioNf> BuscarTodosPorIdHorasMesPrestador(int idHorasMesPrestador)
        {
            return _prestadorEnvioNfRepository.BuscarPorIdHorasMesPrestador(idHorasMesPrestador)
                .OrderByDescending(x => x.Id)
                .ToList();
        }
    }
}
