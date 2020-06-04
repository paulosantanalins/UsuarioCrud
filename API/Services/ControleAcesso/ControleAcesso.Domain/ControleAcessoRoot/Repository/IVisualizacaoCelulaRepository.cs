using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace ControleAcesso.Domain.ControleAcessoRoot.Repository
{
    public interface IVisualizacaoCelulaRepository : IBaseRepository<VisualizacaoCelula>
    {
        List<VisualizacaoCelula> ObterVinculoPorLogin(string login);
        ICollection<VisualizacaoCelulaDto> ObterVisualizacaoCelularPorLogin(string login);
        FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> ObterUsuariosComVisualizacao(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro);
        FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> FiltrarUsuariosCelulaDropdown(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro);
        FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> VisualizarUsuariosCelula(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro);
        FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> FiltrarGrid(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro);
        List<VisualizacaoCelula> RemoveRepetidas(List<VisualizacaoCelula> visualizacaoCelulas);     
        List<VisualizacaoCelula> ObterVisualizacoesPorIdsCelula(IEnumerable<string> lista);
        List<VisualizacaoCelula> BuscarTodosPorLoginDistinct();
    }
}
