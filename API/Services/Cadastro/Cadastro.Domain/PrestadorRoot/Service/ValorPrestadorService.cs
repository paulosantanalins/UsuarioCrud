using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Dapper;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utils.Connections;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class ValorPrestadorService : IValorPrestadorService
    {
        private readonly IValorPrestadorRepository _valorPrestadorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHorasMesPrestadorService _horasMesPrestadorService;
        private readonly IPrestadorRepository _prestadorRepository;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public ValorPrestadorService(
            IValorPrestadorRepository valorPrestadorRepository,
            IHorasMesPrestadorService horasMesPrestadorService,
            IPrestadorRepository prestadorRepository,
            IOptions<ConnectionStrings> connectionStrings,
            IUnitOfWork unitOfWork)
        {
            _valorPrestadorRepository = valorPrestadorRepository;
            _horasMesPrestadorService = horasMesPrestadorService;
            _prestadorRepository = prestadorRepository;
            _unitOfWork = unitOfWork;
            _connectionStrings = connectionStrings;
        }

        public ValorPrestador BuscarPorId(int id)
        {
            var result = _valorPrestadorRepository.BuscarPorId(id);
            return result;
        }

        public void RemoverValorPrestador(ValorPrestador valorPrestador)
        {
            _valorPrestadorRepository.Remove(valorPrestador);
            _unitOfWork.Commit();

            var prestador = _prestadorRepository.BuscarPorId(valorPrestador.IdPrestador);
            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                foreach (var valor in prestador.ValoresPrestador)
                {
                    InativarRemuneracaoPrestador(valor);
                }
            }
        }

        public void InativarRemuneracaoPrestador(ValorPrestador valorPrestador)
        {
            var prestador = _prestadorRepository.BuscarPorId(valorPrestador.IdPrestador);

            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                foreach (var valor in prestador.ValoresPrestador)
                {
                    var query = "UPDATE stfcorp.tblProfissionaisRemuneracoes SET" +
                        " Inativo = 1" +                        
                        //" WHERE IdProfissional = " + prestador.CodEacessoLegado + " and (vrmes = " + valor.ValorMes + " or vrhora = " + valor.ValorHora + ")";
                        " WHERE IdProfissional = " + prestador.CodEacessoLegado + " and (vrmes = @ValorMes or vrhora = @ValorHora)";

                    dbConnection.Query(query, new { valor.ValorMes, valor.ValorHora});
                }
            }
        }

        public bool ValidaExcluir(ValorPrestador valorPrestador)
        {
            var horasMesPrestador = _horasMesPrestadorService.BuscarHorasMesPrestadorPorPrestador(valorPrestador.IdPrestador);
            return horasMesPrestador.Count == 0 || !horasMesPrestador.Any(x => x.DataAlteracao >= valorPrestador.DataAlteracao);
        }
    }
}
