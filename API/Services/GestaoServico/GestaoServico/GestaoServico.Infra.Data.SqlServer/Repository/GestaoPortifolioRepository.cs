using GestaoServico.Infra.Data.SqlServer.Context;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Repository
{
    public class GestaoPortifolioRepository
    {

        private readonly GestaoServicoContext _gestaoServicoContext;
        private readonly Variables _variables;

        public GestaoPortifolioRepository(GestaoServicoContext gestaoServicoContext, Variables variables)
        {
            _gestaoServicoContext = gestaoServicoContext;
            _variables = variables;
        }

        //public async Task<bool> ValidarCategoriaContabil(CategoriaContabil categoria)
        //{
        //    var result = await _gestaoServicoContext.Categorias.Where(x => x.SgCategoriaContabil.ToUpper() == categoria.SgCategoriaContabil.ToUpper() && x.FlStatus && categoria.Id != x.Id).AnyAsync();
        //    return result;
        //}

        //public async Task<FiltroGenericoDto<CategoriaContabil>> FiltrarCategoriaContabil(FiltroGenericoDto<CategoriaContabil> filtro)
        //{
        //    var query = _gestaoServicoContext.Categorias.AsNoTracking();
        //    var stringAtivo = "ATIVO";
        //    var stringInativo = "INATIVO";

        //    if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
        //    {
        //        query = query.WhereAtLeastOneProperty((string s) => s.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));
        //    }

        //    filtro.Total = await query.CountAsync();
        //    filtro.Valores = query.OrderBy(x => x.Id).Skip((filtro.Pagina - 1) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

        //    return filtro;
        //}

        //public async Task InserirCategoria(CategoriaContabil categoria)
        //{
        //    _gestaoServicoContext.Entry(categoria).State = EntityState.Modified;
        //    categoria.Usuario = _variables.GetUserName();
        //    categoria.DataAlteracao = DateTime.Now;
        //    await _gestaoServicoContext.Categorias.AddAsync(categoria);
        //}

        //public async Task UpdateCategoria(CategoriaContabil categoria)
        //{
        //    _gestaoServicoContext.Entry(categoria).State = EntityState.Modified;
        //    categoria.Usuario = _variables.GetUserName();
        //    categoria.DataAlteracao = DateTime.Now;
        //    await Task.Run(() => _gestaoServicoContext.Categorias.Update(categoria));
        //}

        //public async Task<CategoriaContabil> ObterCategoriaPorId(int id)
        //{
        //    var result = await _gestaoServicoContext.Categorias.FirstOrDefaultAsync(x => x.Id == id);

        //    return result;
        //}

        //public async Task<FiltroGenericoDto<TipoServico>> FiltrarTipoServico(FiltroGenericoDto<TipoServico> filtro)
        //{
        //    var query = _gestaoServicoContext.TipoServicos.AsNoTracking();
        //    var stringAtivo = "ATIVO";
        //    var stringInativo = "INATIVO";

        //    if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
        //    {
        //        query = query.Where(x => x.DescTipoServico.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
        //                            || x.Id.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
        //                            || (x.FlStatus == true && stringAtivo.Contains(filtro.ValorParaFiltrar.ToUpper()))
        //                            || (x.FlStatus == false && stringInativo.Contains(filtro.ValorParaFiltrar.ToUpper())));
        //    }

        //    filtro.Total = await query.CountAsync();
        //    filtro.Valores = query.OrderBy(x => x.Id).Skip((filtro.Pagina - 1) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

        //    return filtro;
        //}

        //public async Task InserirTipoServico(TipoServico tipoServico)
        //{
        //    _gestaoServicoContext.Entry(tipoServico).State = EntityState.Modified;
        //    tipoServico.Usuario = _variables.GetUserName();
        //    tipoServico.DataAlteracao = DateTime.Now;
        //    await _gestaoServicoContext.TipoServicos.AddAsync(tipoServico);
        //}

        //public async Task UpdateTipoServico(TipoServico tipoServico)
        //{
        //    _gestaoServicoContext.Entry(tipoServico).State = EntityState.Modified;
        //    tipoServico.Usuario = _variables.GetUserName();
        //    tipoServico.DataAlteracao = DateTime.Now;
        //    await Task.Run(() => _gestaoServicoContext.TipoServicos.Update(tipoServico));
        //}

        //public async Task<TipoServico> ObterTipoServicoPorId(int id)
        //{
        //    var result = await _gestaoServicoContext.TipoServicos.FirstOrDefaultAsync(x => x.Id == id);

        //    return result;
        //}

        //public async Task<bool> ValidarTipoServico(string descricao)
        //{
        //    var result = await _gestaoServicoContext.TipoServicos.Where(x => x.DescTipoServico.ToUpper() == descricao.ToUpper() && x.FlStatus).AnyAsync();
        //    return result;
        //}

        //public async Task<ICollection<CategoriaContabil>> ObterTodasCategorias()
        //{
        //    var result = await _gestaoServicoContext.Categorias.Where(x => x.FlStatus).OrderBy(x => x.DescCategoria).ToListAsync();
        //    return result;
        //}
    }
}
