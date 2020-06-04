using System.ComponentModel;

namespace RepasseEAcesso.Domain.SharedRoot
{
    public class SharedEnuns
    {
        public enum StatusRepasseEacesso
        {
            [Description("STATUS_REPASSE")]
            VL_TIPO_DOMINIO,
            [Description("AP")]
            APROVADO_NIVEL_DOIS = 1,
            [Description("A1")]
            APROVADO_NIVEL_UM = 2, 
            [Description("CC")]
            CANCELADO = 3 ,
            [Description("NG")]
            NEGADO = 4,
            [Description("NA")]
            NAO_ANALISADO = 5,
            [Description("REPASSE CADASTRADO")]
            REPASSE_CADASTRADO = 6,
            [Description("REPASSE EDITADO")]
            REPASSE_EDITADO = 7,
        }

        public enum StatusRepasseEacessoDescricao
        {
            [Description("APROVADO 2º NÍVEL")]
            AP = 1,
            [Description("APROVADO 1º NÍVEL")]
            A1 = 2,
            [Description("CANCELADO")]
            CC = 3,
            [Description("NEGADO")]
            NG = 4,
            [Description("NÃO ANALISADO")]
            NA = 5,
            [Description("REPASSE CADASTRADO")]
            REPASSE_CADASTRADO = 6,
            [Description("REPASSE EDITADO")]
            REPASSE_EDITADO = 7,
        }
    }
}
