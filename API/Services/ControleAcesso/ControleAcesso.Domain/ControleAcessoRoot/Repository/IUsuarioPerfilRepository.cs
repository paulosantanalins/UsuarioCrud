using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace ControleAcesso.Domain.ControleAcessoRoot.Repository
{
    public interface IUsuarioPerfilRepository : IBaseRepository<UsuarioPerfil>
    {
        UsuarioPerfilDto ObterUsuarioPerfilDto(string login);
        bool VerificaPrestadorMaster(string login);
        FiltroGenericoDto<UsuarioPerfilDto> FiltrarUsuario(FiltroGenericoDto<UsuarioPerfilDto> filtro);
        UsuarioPerfilDto ObterUsuarioComTodosPerfis(string login);
        FiltroGenericoDto<UsuarioPerfilDto> ObterUsuariosComPerfil(FiltroGenericoDto<UsuarioPerfilDto> filtro);
        List<string> BuscarUsuariosPorPerfis(int[] idPerfis);
        List<string> BuscarUsuariosPorFuncionalidades(int[] idFuncionalidades);
    }
}
