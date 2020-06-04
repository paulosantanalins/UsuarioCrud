using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;
using Utils.Base;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces
{
    public interface IContratoService
    {
        bool VerificarExistenciaPorNumeroContrato(string nrContrato);
        Contrato ObterContratoPorNumeroContrato(string nrContrato);
        FiltroGenericoDto<ContratoDto> Filtrar(FiltroGenericoDto<ContratoDto> filtroDto);
        IEnumerable<Contrato> ObterTodos();
        ICollection<Contrato> ObterContratosPorCliente(int idCliente);
        ICollection<MultiselectDto> PopularComboClientePorCelula(int idCelula);
        Contrato ObterContratoComServicosContratados(int idContrato);
        void PersistirInformacoesContrato(int idContrato, int idMoeda, int idCelula, DateTime dtInicial, DateTime? dtFinalizacao);
        int VerificarExistenciaContratoEacesso(string contrato, int idCliente);

    }
}
