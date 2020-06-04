using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Repository
{
    public interface IContratoRepository : IBaseRepository<Contrato>
    {
        bool VerificarExistenciaPorNumeroContrato(string nrContrato);
        Contrato ObterContratoPorNumeroContrato(string nrContrato);
        FiltroGenericoDto<ContratoDto> Filtrar(FiltroGenericoDto<ContratoDto> filtroDto);
        ICollection<Contrato> BuscarContratos(int id);
        ICollection<int> BuscarIdsClientePorIdCelula(int idCelula);
        Contrato BuscarContratoComServicosContratados(int idContrato);
        Contrato BuscarContratoComVinculos(int idContrato);
        int ObterCelulaComercialVigenteContrato(int idContrato);
        int VerificarContratoDefaultPorIdCliente(int idCliente);
    }
}
