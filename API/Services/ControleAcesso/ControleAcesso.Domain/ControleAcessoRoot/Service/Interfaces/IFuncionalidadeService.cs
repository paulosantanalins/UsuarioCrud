using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces
{
    public interface IFuncionalidadeService
    {
        List<Funcionalidade> BuscarTodasFuncionalidades();
    }
}
