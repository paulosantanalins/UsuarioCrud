using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.RmRoot.Service.Interfaces;
using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utils.Connections;

namespace Cadastro.Domain.RmRoot.Service
{
    public class RmService : IRmService
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public RmService(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public int? ObterIdMovOrigem(int? idChaveOrigemIntRm, int idColigada)
        {
            int? idMovOrigem = null;
            if (idChaveOrigemIntRm.HasValue)
            {
                var connectionStringIntegracaoRm = _connectionStrings.Value.RMIntegracaoConnection;
                using (IDbConnection dbConnection = new SqlConnection(connectionStringIntegracaoRm))
                {
                    var query = "select idmov from INT_TMOV_IMP where chaveorigem_int = " + idChaveOrigemIntRm.Value;
                    var result = dbConnection.Query<int?>(query).AsList();
                    
                    if (result != null)
                    {
                        idMovOrigem = result.FirstOrDefault();
                    }
                    dbConnection.Close();
                }
            }

            return idMovOrigem;
        }

        public int? ObterIdMovDestino(int idMovOrigem, int idColigada)
        {
            int? idMovDestino;
            var connectionStringRm = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRm))
            {
                var query = "select idmovdestino from tmovrelac where idmovorigem = " + idMovOrigem + " and codcolorigem = " + idColigada;
                idMovDestino = dbConnection.Query<int>(query).FirstOrDefault();
                dbConnection.Close();
            }

            return idMovDestino;
        }

        public decimal ObterValorPagamento(int idMovDestino, int idColigada, bool bruto)
        {
            decimal? valor;
            var connectionStringRm = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRm))
            {
                var query = "select " + ( bruto ? "valorBruto" : "valorOutros as valorLiquido") + " from tmov where idmov = " + idMovDestino + " and codcoligada = " + idColigada;
                valor = dbConnection.Query<decimal>(query).FirstOrDefault();
                dbConnection.Close();
            }

            return valor ?? 0;
        }

        private TMovRmDto ObterValoresPagamento(int idMovDestino, int idColigada)
        {
            var listaTMovRmDto = new TMovRmDto();
            var connectionStringRm = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRm))
            {
                var query = "select valorBruto, valorOutros as valorLiquido, status from tmov where idmov = " + idMovDestino + " and codcoligada = " + idColigada;
                listaTMovRmDto = dbConnection.Query<TMovRmDto>(query).AsList().FirstOrDefault();
                dbConnection.Close();
            }

            return listaTMovRmDto;
        }

        public string ObterStatusPagamento(int idMovDestino, int idColigada)
        {
            string status;
            var connectionStringRm = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRm))
            {
                var query = "select status from tmov where idmov = " + idMovDestino + " and codcoligada = " + idColigada;
                status = dbConnection.Query<string>(query).FirstOrDefault();
                dbConnection.Close();
            }

            return status ?? "A";
        }

        public decimal ObterValorRm(int? idColigada, int? idChaveOrigemIntRm, bool bruto)
        {
            if (idColigada.HasValue)
            {
                int? idMovOrigem;
                int? idMovDestino;
                idMovOrigem = ObterIdMovOrigem(idChaveOrigemIntRm, idColigada.Value);
                if (idMovOrigem.HasValue)
                {
                    idMovDestino = ObterIdMovDestino(idMovOrigem.Value, idColigada.Value);
                    if (idMovDestino.HasValue)
                    {
                        var valor = ObterValorPagamento(idMovDestino.Value, idColigada.Value, bruto);
                        return valor;
                    }
                }
            }
            return 0;
        }

        public TMovRmDto ObterValoresRm(int? idColigada, int? idChaveOrigemIntRm)
        {
            if (idColigada.HasValue)
            {
                int? idMovOrigem;
                int? idMovDestino;
                idMovOrigem = ObterIdMovOrigem(idChaveOrigemIntRm, idColigada.Value);
                if (idMovOrigem.HasValue)
                {
                    idMovDestino = ObterIdMovDestino(idMovOrigem.Value, idColigada.Value);
                    if (idMovDestino.HasValue)
                    {
                        var valor = ObterValoresPagamento(idMovDestino.Value, idColigada.Value);
                        return valor;
                    }
                }
            }
            return null;
        }

        public string ObterStatusRm(int? idColigada, int? idChaveOrigemIntRm)
        {
            if (idColigada.HasValue)
            {
                int? idMovOrigem;
                int? idMovDestino;
                idMovOrigem = ObterIdMovOrigem(idChaveOrigemIntRm, idColigada.Value);
                if (idMovOrigem.HasValue)
                {
                    idMovDestino = ObterIdMovDestino(idMovOrigem.Value, idColigada.Value);
                    if (idMovDestino.HasValue)
                    {
                        var valor = ObterStatusPagamento(idMovDestino.Value, idColigada.Value);
                        return valor;
                    }
                }
            }
            return "A";
        }
    }
}
