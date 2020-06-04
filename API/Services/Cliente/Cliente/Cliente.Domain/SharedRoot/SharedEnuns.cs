using System.ComponentModel;

namespace Cliente.Domain.SharedRoot
{
    public class SharedEnuns
    {
        public enum TipoClienteRM
        {
            [Description("Cliente")]
            CLIENTE = 21,
            [Description("Cliente - Empresa Pública")]
            CLIENTE_EMPRESA_PUBLICA = 24
        }
    }
}
