using System;
using System.ComponentModel;

namespace Logger.Model
{
    public class LogGenerico
    {
        public int Id { get; set; }
        public string NmTipoLog { get; set; }
        public string NmOrigem { get; set; }
        public string DescLogGenerico { get; set; }
        public DateTime DtHoraLogGenerico { get; set; }
        public string DescExcecao { get; set; }
    }

    public enum Severidade
    {
        [Description("INFO")]
        INFO,
        [Description("WARNING")]
        WARNING,
        [Description("ERROR")]
        ERROR,
        [Description("FATAL")]
        FATAL,
    }

    public enum Tipo
    {
        [Description("INTEGRACAO")]
        INTEGRACAO,
        [Description("TRANSACAO")]
        TRANSACAO,
    }

    public enum Origem
    {
        [Description("SALESFORCE")]
        SALESFORCE,
        [Description("EPM")]
        EPM,
        [Description("EAcesso")]
        EAcesso,
    }

}
