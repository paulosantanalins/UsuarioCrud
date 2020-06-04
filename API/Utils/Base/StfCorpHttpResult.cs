using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Base
{
    public class StfCorpHttpSingleResult<T>
    {
        public T Dados { get; set; }
        public string Notifications { get; set; }
        public bool Success { get; set; }

    }
}
