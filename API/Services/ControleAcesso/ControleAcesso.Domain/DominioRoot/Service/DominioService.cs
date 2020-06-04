using ControleAcesso.Domain.DominioRoot.Repository;
using ControleAcesso.Domain.DominioRoot.Service.Interfaces;
using ControleAcesso.Domain.SharedRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControleAcesso.Domain.DominioRoot.Service
{
    public class DominioService : IDominioService
    {
        private readonly IDominioRepository _dominioRepository;

        public DominioService(
            IDominioRepository dominioRepository)
        {
            _dominioRepository = dominioRepository;
        }

        public ICollection<ComboDefaultDto> BuscarItens(string tipoDominio)
        {         
           var result = _dominioRepository.Buscar(x => x.ValorTipoDominio.Equals(tipoDominio.Trim().ToUpper())).Where(x => x.Ativo).Select(item =>
                     new ComboDefaultDto
                     {
                         Id = item.Id,
                         Descricao = item.DescricaoValor,
                         Ativo = item.Ativo
                     });

            result = TratarOrdenacao(tipoDominio, result);

            return result.ToList();
        }

        public int ObterIdDominio(string tipoDominio, string valorDominio)
        {
            var id = _dominioRepository.ObterIdDominio(tipoDominio, valorDominio);
            return id;
        }

        public int? ObterIdDominio(int idValor, string tipoDominio)
        {
            var id = _dominioRepository.ObterIdPeloCodValor(idValor, tipoDominio);
            return id;
        }



        public ICollection<ComboDefaultDto> BuscarTodosItens(string tipoDominio)
        {
            var result = _dominioRepository.Buscar(x => x.ValorTipoDominio.Equals(tipoDominio.Trim().ToUpper())).Select(item =>
                     new ComboDefaultDto
                     {
                         Id = item.Id,
                         Descricao = item.DescricaoValor,
                         Ativo = item.Ativo
                     });

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

            return result;
        }
    }
}
