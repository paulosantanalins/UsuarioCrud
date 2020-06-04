using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces
{
    public interface IPerfilService
    {
        void PersistirPerfil(Perfil perfil);
        bool VerificarExistenciaUsuariosComPerfil(int id);
        void InativarPerfil(int id);
        FiltroGenericoDto<PerfilDto> FiltrarPerfil(FiltroGenericoDto<PerfilDto> filtro);
        Perfil BuscarPorId(int id);
        List<Perfil> BuscarTodosPerfis();
        List<Perfil> BuscarTodosPerfisAtivos();
        List<Perfil> BuscarPerfisPorFuncionalidades(int[] funcionalidadesIds);
    }
}
