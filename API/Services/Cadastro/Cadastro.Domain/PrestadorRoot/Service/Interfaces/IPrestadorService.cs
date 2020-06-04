using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using Utils.Base;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IPrestadorService 
    {
        int Adicionar(Prestador prestador);
        void AtualizarPrestador(Prestador prestador);
        Prestador BuscarPorId(int id);
        FiltroGenericoDtoBase<PrestadorDto> Filtrar(FiltroGenericoDtoBase<PrestadorDto> filtro);
        FiltroGenericoDtoBase<PrestadorDto> FiltrarHoras(FiltroGenericoDtoBase<PrestadorDto> filtro);
        void AtualizarMigracao(int? idProfissionalEacesso);
        void RealizarMigracao(int? idProfissionalEacesso);       
        FiltroGenericoDtoBase<AprovarPagamentoDto> FiltrarAprovarPagamento(FiltroGenericoDtoBase<AprovarPagamentoDto> filtro);
        DadosPagamentoPrestadorDto ObterDadosPagementoPorId(int id, int IdHorasMes);
        List<Prestador> ObterPrestadores(IDbConnection dbConnection, int? idProfissionalEacesso);
        List<PrestadorMigradoDto> ObterPrestadoresMigrados();
        void AdicionarPrestadoresNovos(List<Prestador> prestadoresNovos);
        void AtualizarPrestadorVindosEacesso(List<Prestador> prestadoresAtualizados, List<PrestadorMigradoDto> prestadoresAntigos);
        DadosFinanceiroDto ObterDadosFinanceiroRMPrestador(string cpf, int? idEmpresaGrupo);
        DadosFinanceiroDto ObterDadosFinanceiroColigada(string idBanco, int idColigada);
        string ObterContaCaixaNoRm(int idColigada, string banco);
        void SolicitarNF(int idHorasMesPrestador);
        decimal ObterValor(HorasMesPrestador horasMesPrestador, bool considerarDescontos);
        bool VerificarCpfProfissinalExiste(string cpf);
        bool VerificarCpfProfissinalInativoExiste(string cpf);
        bool VerificarCpfProfissinalCandidatoExiste(string cpf);
        void InativarPrestador(InativacaoPrestador inativacaoPrestador);
        void ReativarPrestador(InativacaoPrestador inativacaoPrestador);
        void AtualizarMigracaoInativacaoPrestador(int? idProfissionalEacesso);
        void AtualizarMigracaoClienteServico(int? idProfissionalEacesso);
        void AtualizarMigracaoBeneficio(int? idProfissionalEacesso);
        void AdicionarEAcesso(int idPrestador, IDbConnection eacessoConnection, IDbTransaction dbTransaction);
        void AtualizarEAcesso(int idPrestador, IDbConnection eacessoConnection, IDbTransaction dbTransaction);
        List<RelatorioPrestadoresDto>BuscarRelatorioPrestadores(int empresaId, int filialId , FiltroGenericoDtoBase<RelatorioPrestadoresDto> filtro);
        void PersistirPrestadorRemuneracao(ValorPrestador valorPrestador);
        void AtualizarMigracaoRemuneracaoPrestador(int? idProfissionalEacesso);
        void AtualizarMigracaoPrestadorPessoa();
        void AtualizarMigracaoPrestadorInicioCLT();
        List<ValorPrestadorBeneficio> ObterValoresPrestadorBeneficios(int idValorPrestador);
        void ReabrirLancamento(int idHorasMesPrestador);
        void AtualizarSituacao(int idHorasMesPrestador, string situacaoNova);
        string ObterBancoNoRm(int? idBanco);
        int? ObterCidadeEmpresaEAcesso(string cidade, IDbConnection dbConnection, IDbTransaction dbTransaction);
        FiltroGenericoDtoBase<ConciliacaoPagamentoDto> FiltrarConciliacaoPagamentos(FiltroGenericoDtoBase<ConciliacaoPagamentoDto> filtro, bool resumo);
        void InserirRemuneracaoEAcesso(ValorPrestador valor);
        bool VerificarDataInaticacaoPrestador(int idProfissional, DateTime dtDesligamento);
        void InserirEmpresaEAcesso(Prestador prestador, IDbConnection dbConnection, int idPrestadorEacesso, IDbTransaction dbTransaction);
        void PersistirPrestadorObservacao(ObservacaoPrestador observacaoPrestador);
        void AtualizarMigracaoObservacaoPrestador(int? idProfissionalEacesso);
        void FazerUploadContratoPrestadorParaOMinIO(string nomeAnexo, string caminhoContrato, string arquivoBase64);
        void FazerUploadDocumentoPrestadorParaOMinIO(string nomeAnexo, string caminhoContrato, string arquivoBase64);
        void FazerUploadExtensaoContratoPrestadorParaOMinIO(string nomeAnexo, string caminhoDocumento, string arquivoBase64);
        void RealizarMigracaoCltPj(int idProfissionalEacesso);
        void AtualizarMigracaoSociosEmpresaPrestador();
        void AtualizarSituacao(int idHorasMesPrestador, string situacaoNova, string usuarioAlteracao);
    }
}
