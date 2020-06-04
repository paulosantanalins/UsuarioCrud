using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoPacoteServico
{
    public class ContratoRepository : BaseRepository<Contrato>, IContratoRepository
    {
        public ContratoRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables)
            : base(gestaoServicoContext, variables)
        {

        }

        public bool VerificarExistenciaPorNumeroContrato(string nrContrato)
        {
            var numero = Int32.Parse(nrContrato);
            var queryContrato = DbSet;
            var result = queryContrato.Any(x => x.NrAssetSalesForce == numero.ToString());
            return result;
        }

        public Contrato ObterContratoPorNumeroContrato(string nrContrato)
        {
            var numero = Int32.Parse(nrContrato);
            var queryContrato = DbSet;
            var result = queryContrato.FirstOrDefault(x => x.NrAssetSalesForce == numero.ToString());
            return result;
        }

        public FiltroGenericoDto<ContratoDto> Filtrar(FiltroGenericoDto<ContratoDto> filtro)
        {
             var query = DbSet.AsNoTracking()
                .Include(x => x.ServicoContratados)
                .AsQueryable();

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                query = query.Where(x => x.DescContrato.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));

                //query = query.Where(x =>
                     //x.Id.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                     //x.NrAssetSalesForce.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                     //(x.DescContrato != null ? x.DescContrato.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : false) ||
                     //x.DtInicial.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                     //(x.DtFinalizacao.HasValue ? x.DtFinalizacao.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : false) ||
                     //x.DataAlteracao.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                     //x.Usuario.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));
            }

            var dados = query.Select(p => new ContratoDto
            {
                Id = p.Id,
                NrAssetSalesForce = p.NrAssetSalesForce,
                DescContrato = p.DescContrato,
                Usuario = p.Usuario,
                DataAlteracao = p.DataAlteracao,
                DtFinalizacao = p.DtFinalizacao,
                DtInicial = p.DtInicial,
                IdCelulaComercial = p.ServicoContratados.Any(x => x.DescTipoCelula == "COM") ? p.ServicoContratados.Where(x => x.DescTipoCelula == "COM").OrderByDescending(y => y.Id).FirstOrDefault().IdCelula : 0
            });
            filtro.Total = dados.Count();

            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);

            if (filtro.OrdemOrdenacao == "asc")
            {
                dados = dados.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                dados = dados.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else
            {
                dados = dados.OrderBy(x => x.DtFinalizacao).ThenBy(x => x.Id).ThenBy(x => x.NrAssetSalesForce);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

            return filtro;
        }

        public ICollection<Contrato> BuscarContratos(int id)
        {
            var result = DbSet
                .Include(x => x.ClientesContratos)
                .Where(x => x.ClientesContratos.Any(y => y.IdCliente == id))
                .Select(x => new Contrato {
                    Id = x.Id,
                    NrAssetSalesForce = x.NrAssetSalesForce,
                    DtFinalizacao = x.DtFinalizacao,
                    DtInicial = x.DtInicial,
                    DescContrato = x.DescContrato,
                    DescStatusSalesForce = x.DescStatusSalesForce
                }).ToList();
            return result;
        }


        //verificarrrrrrrrrrrrr
        public ICollection<int> BuscarIdsClientePorIdCelula(int idCelula)
        {
            var result = DbSet
                .Include(x => x.ClientesContratos)
                .Include(x => x.ServicoContratados)
                    .ThenInclude(x => x.VinculoServicoCelulaComercial)
                .Where(x => x.ServicoContratados.Any(y => y.IdCelula == idCelula)
                             || x.ServicoContratados.Any(y => y.VinculoServicoCelulaComercial.Any(z => z.IdCelulaComercial == idCelula))).ToList();
            return result.SelectMany(x => x.ClientesContratos).Select(x => x.IdCliente).Distinct().ToList();
        }

        public Contrato BuscarContratoComServicosContratados(int idContrato)
        {
            var result = DbSet
                .Include(x => x.ServicoContratados)
                .FirstOrDefault(x => x.Id == idContrato);
            return result;
        }

        public Contrato BuscarContratoComVinculos(int idContrato)
        {
            var result = DbSet
                .Include(x => x.ServicoContratados)
                .FirstOrDefault(x => x.Id == idContrato);
            return result;
        }


        public int ObterCelulaComercialVigenteContrato(int idContrato)
        {
            var result = DbSet
                .Include(x => x.ServicoContratados)
                .FirstOrDefault(x => x.Id == idContrato).ServicoContratados.OrderByDescending(x => x.DtInicial).FirstOrDefault(x => x.DescTipoCelula == "COM");
            return result != null ? result.IdCelula : 0;
        }

        public int VerificarContratoDefaultPorIdCliente(int idCliente)
        {
            var result = DbSet.FirstOrDefault(x => x.ClientesContratos.Any(y => y.IdCliente == idCliente) && x.NrAssetSalesForce == "");
            if (result != null)
            {
                return result.Id; 
            }
            return 0;
        }
    }
}
