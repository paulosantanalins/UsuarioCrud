using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Domain.DominioRoot.Repository;
using ControleAcesso.Infra.Data.SqlServer.Context;
using ControleAcesso.Infra.Data.SqlServer.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;
using ControleAcesso.Domain.SharedRoot;
using Utils;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class TipoCelulaRepository : BaseRepository<TipoCelula>, ITipoCelulaRepository
    {
        private readonly IDominioRepository _dominioRepository;

        public TipoCelulaRepository(ControleAcessoContext controleAcessoContext,
            IDominioRepository dominioRepository,
            IVariablesToken variables) : base(controleAcessoContext, variables)
        {
            _dominioRepository = dominioRepository;
        }
    }
}
