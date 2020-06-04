using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.DominioRoot.Repository;
using Cadastro.Domain.DominioRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using Cadastro.Domain.DominioRoot.Dto;
using Utils;
using Utils.Extensions;
using Utils.Http;
using Cadastro.Domain.PrestadorRoot.Dto;
using System.Net.Http;
using Utils.Base;

namespace Cadastro.Domain.DominioRoot.Service
{
    public class DominioService : IDominioService
    {
        private readonly IDominioRepository _dominioRepository;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVariablesToken _variables;

        public DominioService(
            IDominioRepository dominioRepository,
            MicroServicosUrls microServicosUrls,
            IUnitOfWork unitOfWork, IVariablesToken variables)
        {
            _dominioRepository = dominioRepository;
            _microServicosUrls = microServicosUrls;
            _unitOfWork = unitOfWork;
            _variables = variables;
        }

        public void Persistir(Dominio dominio)
        {
            dominio.DescricaoValor = dominio.DescricaoValor.ToUpper();
            dominio.IdValor = _dominioRepository.BuscarDominiosPorTipo(dominio.ValorTipoDominio,false).Count()+1;
            _dominioRepository.Adicionar(dominio);

            _unitOfWork.Commit();            
        }

        public void Editar(Dominio dominio)
        {
            _dominioRepository.Update(dominio);

            _unitOfWork.Commit();
        }

        public void MudarStatusDominio(int id)
        {
            var dominio = _dominioRepository.BuscarDominioPorId(id);

            if (dominio == null) return;

            dominio.Ativo = !dominio.Ativo;

            _dominioRepository.MudarStatusDominio(dominio);

            _unitOfWork.Commit();
        }

        public bool ValidarDominio(DominioDto dominioDto)
        {
            var valido = _dominioRepository.ValidarDominio(dominioDto);

            return valido;
        }

        public void AddTrackingNoDominio(Dominio dominio)
        {
            if (dominio.IdValor == 0)
            {
                int idValor = _dominioRepository.ObterPeloVlTipoDominio(dominio.ValorTipoDominio)?.LastOrDefault()?.IdValor + 1 ?? 1;
                dominio.IdValor = idValor;
            }
            _dominioRepository.Adicionar(dominio);
        }
         

        public ICollection<ComboDefaultDto> BuscarItens(string tipoDominio)
        {
            tipoDominio = tipoDominio.Trim().ToUpper();

            var result = _dominioRepository.BuscarDominiosPorTipo(tipoDominio, true);

            result = TratarOrdenacao(tipoDominio, result);

            return result.ToList();
        }

        public ICollection<ComboDefaultDto> BuscarItensPropriedade(string tipoDominio)
        {
            tipoDominio = tipoDominio.Trim().ToUpper();

            var result = _dominioRepository.BuscarDominiosPropriedade(tipoDominio, true);

            result = TratarOrdenacao(tipoDominio, result);

            return result.ToList();
        }

        public List<Dominio> BuscarDominios(string tipoDominio)
        {
            var result = _dominioRepository.Buscar(x => x.ValorTipoDominio.Equals(tipoDominio.Trim(), StringComparison.InvariantCultureIgnoreCase))
                .Where(x => x.Ativo).ToList();
            
            return result;
        }

        public FiltroGenericoDto<DominioDto> FiltrarDominios(FiltroGenericoDto<DominioDto> filtro)
        {
            var result = _dominioRepository.FiltrarDominios(filtro);
            return result;
        }

        public DominioDto BuscarDominioPorId(int id)
        {
            var dominio = _dominioRepository.BuscarDominioPorId(id);

            if (dominio == null) return null;

            var dominioDto = new DominioDto
            {
                Id = dominio.Id,
                GrupoDominio = dominio.ValorTipoDominio,
                ValorDominio = dominio.DescricaoValor,
                IdValor = dominio.IdValor
            };

            return dominioDto;
        }

        public List<string> BuscarCombosGruposDominios()
        {   
            var client = new GenericHttpRequests<UsuarioPerfilDto>(_microServicosUrls.UrlApiControle, _variables.Token);
            var funcsUsuario = client.Send($"api/usuarioPerfil/obter-funcionalidades/{_variables.UsuarioToken}", HttpMethod.Get);
            return _dominioRepository.BuscarCombosGruposDominios()
                .Where(x => funcsUsuario.Funcionalidades.Any(y => y == x))
                .ToList();           
        }

        public ICollection<ComboDefaultDto> BuscarTodosItens(string tipoDominio)
        {
            tipoDominio = tipoDominio.Trim().ToUpper();

            var result = _dominioRepository.BuscarDominiosPorTipo(tipoDominio, false);

            result = TratarOrdenacao(tipoDominio, result);

            return result.ToList();
        }

        private static IEnumerable<ComboDefaultDto> TratarOrdenacao(string tipoDominio, IEnumerable<ComboDefaultDto> result)
        {
            if (tipoDominio.ToUpper().Equals("CARGO"))
            {
                result = result.Where(x => x.Ativo).OrderBy(x => x.Descricao);
            }

            if (tipoDominio.ToUpper().Equals("DIA_PAGAMENTO"))
            {
                result = result.OrderBy(x => Int32.Parse(x.Descricao));
            }

            if (tipoDominio.ToUpper().Equals("ESCOLARIDADE"))
            {
                result = result.OrderBy(x => x.Descricao);
            }

            if (tipoDominio.ToUpper().Equals("CONTRATACAO"))
            {
                result = result.OrderByDescending(x => x.Ativo);
            }

            if (tipoDominio.ToUpper().Equals("DESCONTO"))
            {
                result = result.Where(x => x.Ativo).OrderBy(x => x.Descricao);
            }

            if (tipoDominio.ToUpper().Equals("OCORRENCIA_OBS"))
            {
                result = result.OrderByDescending(x => x.Ativo);
            }

            return result;
        }


    }
}
