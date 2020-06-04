using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.NotaFiscalRoot.Entity
{
    public class PrestadorEnvioNf : EntityBase
    {        
        public int IdHorasMesPrestador { get; set; }
        public HorasMesPrestador HorasMesPrestador { get; set; }        
        public string Token { get; set; }
        public string CaminhoNf { get; set; }        

        public PrestadorEnvioNf Clone()
        {
            return MemberwiseClone() as PrestadorEnvioNf;
        }
    }
}
