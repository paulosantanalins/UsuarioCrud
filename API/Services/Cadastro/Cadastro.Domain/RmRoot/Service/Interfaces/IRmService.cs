using Cadastro.Domain.PrestadorRoot.Dto;
using System.Collections.Generic;

namespace Cadastro.Domain.RmRoot.Service.Interfaces
{
    public interface IRmService
    {
        int? ObterIdMovDestino(int idMovOrigem, int idColigada);
        int? ObterIdMovOrigem(int? idChaveOrigemIntRm, int idColigada);
        decimal ObterValorPagamento(int idMovDestino, int idColigada, bool bruto);
        decimal ObterValorRm(int? idColigada, int? idChaveOrigemIntRm, bool bruto);
        string ObterStatusRm(int? idColigada, int? idChaveOrigemIntRm);
        TMovRmDto ObterValoresRm(int? idColigada, int? idChaveOrigemIntRm);
    }
}
