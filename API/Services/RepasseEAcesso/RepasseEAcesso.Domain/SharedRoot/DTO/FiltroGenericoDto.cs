using System.Collections.Generic;

namespace RepasseEAcesso.Domain.SharedRoot.DTO
{
    public class FiltroGenericoDto<T>
    {
        public int Pagina { get; set; }
        public int QuantidadePorPagina { get; set; }
        public int Total { get; set; }
        public List<T> Valores { get; set; }
    }
}
