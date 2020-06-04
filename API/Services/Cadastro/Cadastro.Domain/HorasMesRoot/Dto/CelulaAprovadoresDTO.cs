using System.Collections.Generic;

namespace Cadastro.Domain.HorasMesRoot.Dto
{
    public class CelulaAprovadoresDTO
    {
        public List<int> CelulasGerenteAprova { get; set; }
        public List<int> CelulasDiretorAprova { get; set; }
    }
}
