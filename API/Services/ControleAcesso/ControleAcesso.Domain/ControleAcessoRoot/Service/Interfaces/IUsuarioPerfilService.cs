using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces
{
    public interface IUsuarioPerfilService
    {
        UsuarioPerfilDto ObterUsuarioPerfilComFuncionalidades(string login);
        bool VerificaPrestadorMaster(string login);
        FiltroGenericoDto<UsuarioPerfilDto> FiltrarUsuario(FiltroGenericoDto<UsuarioPerfilDto> filtro);
        void BuscarUsuarioAdPorLogin(FiltroAdDto filtro);
        void PersistirVinculos(List<VinculoUsuarioPerfilDto> vinculos);
        UsuarioPerfilDto ObterUsuarioComPerfisPorLogin(string login);
        FiltroGenericoDto<UsuarioPerfilDto> ObterUsuariosComPerfil(FiltroGenericoDto<UsuarioPerfilDto> filtro);
        void RemoverVinculos(string login);
        void AtualizarVinculos(List<VinculoUsuarioPerfilDto> vinculos);

        List<String> ObterEmailPorFuncionalidade(string[] funcionalidade);
        List<string> BuscarUsuariosPorPerfis(int[] idPerfis);
    }
}
