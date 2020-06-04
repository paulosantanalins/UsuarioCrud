using System.ComponentModel;

namespace Cadastro.Domain.SharedRoot
{
    public class SharedEnuns
    {
        public enum TipoHierarquia
        {
            [Description("TIPO_HIERARQUIA")]
            VL_TIPO_DOMINIO,
            [Description("TIPO_HIERARQUIA")]
            TIPO_HIERARQUIA,
            [Description("TIPO_CONTABIL")]
            TIPO_CONTABIL
        }

        public enum EstadoCivil
        {
            [Description("ESTADOCIVIL")]
            VL_TIPO_DOMINIO,
            [Description("CASADO(A)")]
            CASADO = 1,
            [Description("SOLTEIRO(A)")]
            SOLTEIRO = 2,
            [Description("DIVORCIADO(A)")]
            DIVORCIADO = 3,
            [Description("VIUVO(A)")]
            VIUVO = 5,
            [Description("AMASIADO(A)")]
            AMASIADO = 6,
            [Description("SEPARADO(A) JUDICIALMENTE")]
            SEPARADO_JUDICIALMENTE = 7,
            [Description("DESQUITADO(A)")]
            DESQUITADO = 8,
            [Description("MARITAL")]
            MARITAL = 9,
            [Description("SEPARADO(A)")]
            SEPARADO = 10,
            [Description("UNIAO ESTAVEL")]
            UNIAO_ESTAVEL = 11
        }

        public enum PrestadorInativacaoOrdemMotivo
        {
            [Description("E")]
            EMPRESA,
            [Description("P")]
            PROFISSIONAL
        }
        public enum Nacionalidade
        {
            [Description("NACIONALIDADE")]
            VL_TIPO_DOMINIO
        }

        public enum Sexo
        {
            [Description("SEXO")]
            VL_TIPO_DOMINIO = 0,
            [Description("MASCULINO")]
            MASCULINO = 1,
            [Description("FEMININO")]
            FEMININO = 2 
        }

        public enum TipoSituacaoHorasMesPrestador
        {
            [Description("HORAS PENDENTE")]
            HORAS_PENDENTE,
            [Description("HORAS CADASTRADAS")]
            HORAS_CADASTRADAS,
            [Description("HORAS RECADASTRADAS")]
            HORAS_RECADASTRADAS,
            [Description("CANCELADO")]
            CANCELADO,
            [Description("HORAS APROVADAS")]
            HORAS_APROVADAS,
            [Description("NEGADO")]
            NEGADO,
            [Description("NOTA FISCAL SOLICITADA")]
            NOTA_FISCAL_SOLICITADA,
            [Description("AGUARDANDO INTEGRAÇÃO")]
            AGUARDANDO_INTEGRACAO,
            [Description("ERRO AO SOLICITAR PAGAMENTO NO RM")]
            ERRO_SOLICITAR_PAGAMENTO,
            [Description("AGUARDANDO ENTRADA DA NF")]
            AGUARDANDO_ENTRADA_DA_NF,
            [Description("AGUARDANDO PAGAMENTO")]
            AGUARDANDO_PAGAMENTO,
            [Description("PAGAMENTO REALIZADO")]
            PAGAMENTO_REALIZADO
        }

        public enum TipoDesconto
        {
            [Description("EMPRESTIMO")]
            EMPRESTIMO,
            [Description("DESC AD VIAGEM")]
            DESC_AD_VIAGEM,
            [Description("MULTA TRÂNSITO")]
            MULTA_TRANSITO,
            [Description("COMPRA NOTEBOOK")]
            COMPRA_NOTEBOOK,
            [Description("PENSÃO ALIMENTÍCIA")]
            PENSAO_ALIMENTICIA,
            [Description("SINISTRO")]
            SINISTRO,
            [Description("TELEFONIA CELULAR")]
            TELEFONIA_CELULAR
        }

        public enum TipoDocumentoPrestador
        {
            [Description("TIPO_DOCUMENTO_PRESTADOR")]
            VL_TIPO_DOMINIO,
            [Description("OUTROS")]
            OUTROS = 0,
        }

        public enum SituacoesTransferenciaEnum
        {
            [Description("APROVADO")]
            Aprovado = 1,
            [Description("AGUARDANDO APROVAÇÃO")]
            AguardandoAprovacao = 2,
            [Description("NEGADO")]
            Negado = 3,
            [Description("EFETIVADO")]
            Efetivado = 4,
            [Description("NOVA SOLICITAÇÃO")]
            NovaSolicitacao =5,
            [Description("SOLICITAÇÃO EDITADA")]
            SolicitacaoEditada = 6,
        }

        public enum SituacoesFinalizarContrato
        {
            [Description("PENDENTE")]
            Pendente = 1,
            [Description("FINALIZADO")]
            Finalizado = 2,
            [Description("CANCELADO")]
            Cancelado = 3
        }

        public enum SituacoesReajusteContrato
        {
            [Description("AprovarNegarReajusteBP")]
            AguardandoAprovacaoBP = 1,
            [Description("AprovarNegarReajusteRem")]
            AguardandoAprovacaoRemuneracao = 2,
            [Description("AprovarNegarReajusteControladoria")]
            AguardandoAprovacaoContrtoladoria = 3,
            [Description("AprovarNegarReajusteDiretoriaCel")]
            AguardandoAprovacaoDiretoriaCel = 4,
            ReajusteAprovado = 5,
            ReajusteFinalizado = 6,
            ReajusteCancelado = 7
        }

        public enum AcoesLog
        {
            NovaSolicitacao = 1,
            Edicao = 2,
            Inativacao = 3,
            AprovacaoBP = 4,
            AprovacaoRem = 5,
            AprovacaoControladoria = 6,
            AprovacaoDirCel = 7,
            Negado = 8,
            Finalizado = 9
        }

        public enum StatusCelula
        {
            [Description("Ativo")]
            Ativada = 0,
            [Description("Inativado")]
            Inativada = 1,
            [Description("Célula Processo de Inativação")]
            PendenteParaInativacao = 2,
            [Description("Reativado")]
            Reativada = 3
        }
    }
}
