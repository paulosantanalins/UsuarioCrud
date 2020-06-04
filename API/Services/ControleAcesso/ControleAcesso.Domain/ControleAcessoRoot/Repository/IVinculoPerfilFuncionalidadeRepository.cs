using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Repository
{
    public interface IVinculoPerfilFuncionalidadeRepository : IBaseRepository<VinculoPerfilFuncionalidade>
    {
        void RemoverVinculos(List<VinculoPerfilFuncionalidade> vinculoPerfils);
    }
}
