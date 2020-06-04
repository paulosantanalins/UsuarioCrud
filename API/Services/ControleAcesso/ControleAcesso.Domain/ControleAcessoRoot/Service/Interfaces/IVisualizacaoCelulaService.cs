using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces
{
    public interface IVisualizacaoCelulaService
    {
        void PersistirVisualizacaoCelula(List<VisualizacaoCelula> visualizacaoCelula);
        void AtualizarVisualizacaoCelula(List<VisualizacaoCelula> visualizacaoCelula);
        List<VisualizacaoCelula> BuscarPorLogin(string login);
        ICollection<VisualizacaoCelulaDto> ObterVisualizacaoCelularPorLogin(string login, int celulaUsuario = -1);
        FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> FiltrarUsuariosCelulaDropdown(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro);
        FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> VisualizarUsuariosCelula(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro);
        FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> ObterUsuariosComVisualizacao(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro);
        List<UsuarioSegurancaDto> BuscarUsuariosAdPorCelulas(string celulas);        
        IEnumerable<UsuarioSegurancaDto> ObterUsuariosAdComFiltroCelula(FiltroAdDtoSeguranca filtro);
        FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> FiltrarGrid(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro);
        void RealizarMigracaoVisualizacaoCelula();
        void RemoverVisualizacaoTodasCelulas(string login);
    }
}
