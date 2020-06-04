using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.Jobs
{
    public interface IProvider<T>
    {
        T Get();
    }

}
