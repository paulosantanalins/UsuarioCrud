using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace ControleAcesso.Domain.ControleAcessoRoot.Repository
{
    public interface IPerfilRepository : IBaseRepository<Perfil>
    {
        Perfil ObterPerfilComUsuarios(int id);
        Perfil ObterPerfilComVinculo(int id);
        FiltroGenericoDto<PerfilDto> FiltrarPerfil(FiltroGenericoDto<PerfilDto> filtro);
        bool Validar(Perfil perfil);
        List<Perfil> BuscarPerfisPorFuncionalidades(int[] idsFuncionalidades);
    }
}
