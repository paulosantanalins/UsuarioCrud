using RepasseEAcesso.Domain.DominioRoot.Entity;
using RepasseEAcesso.Domain.DominioRoot.Repository;
using RepasseEAcesso.Domain.DominioRoot.Service.Interface;
using RepasseEAcesso.Domain.SharedRoot;
using RepasseEAcesso.Domain.SharedRoot.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Extensions;

namespace RepasseEAcesso.Domain.DominioRoot.Service
{
    public class DominioService : IDominioService
    {
        private readonly IDominioRepository _dominioRepository;        
        
        public const string APROVADO_NIVEL1 = "A1";
        public const string APROVADO_NIVEL_DOIS = "AP";
        public const string CANCELADO = "CC";
        public const string NEGADO = "NG";
        public const string NAO_ANALISADO = "NA";
        public const string REPASSE_CADASTRADO = "Repasse Cadastrado";
        public const string REPASSE_EDITADO = "Repasse Editado";

        public DominioService(
            IDominioRepository dominioRepository)
        {
            _dominioRepository = dominioRepository;
        }

        public ICollection<ComboDefaultDto> BuscarItens(string tipoDominio)
        {
            var result = _dominioRepository.Buscar(x => x.ValorTipoDominio.Equals(tipoDominio.Trim(), StringComparison.InvariantCultureIgnoreCase))
                .Where(x => x.Ativo).Select(item =>
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
            if (tipoDominio.ToUpper().Equals("MOEDA"))
            {
                result = result.Where(x => x.Ativo).OrderBy(x => x.Descricao);
            }

            if (tipoDominio.ToUpper().Equals("STATUS_REPASSE"))
            {
                result = result.Where(x => x.Ativo).OrderBy(x => x.Descricao);
            }

            return result;
        }

        public string  ObterDescricaoStatus(int IdStatusRepasse)
        {
            string status = _dominioRepository.BuscarPorId(IdStatusRepasse).DescricaoValor;
            switch (status)
            {
                case APROVADO_NIVEL1:
                    return SharedEnuns.StatusRepasseEacessoDescricao.A1.GetDescription();
                case APROVADO_NIVEL_DOIS:
                    return SharedEnuns.StatusRepasseEacessoDescricao.AP.GetDescription(); 
                case NEGADO:
                    return SharedEnuns.StatusRepasseEacessoDescricao.NG.GetDescription(); 
                case CANCELADO:
                    return SharedEnuns.StatusRepasseEacessoDescricao.CC.GetDescription();
                case NAO_ANALISADO:
                    return SharedEnuns.StatusRepasseEacessoDescricao.NA.GetDescription();
                case REPASSE_CADASTRADO:
                    return SharedEnuns.StatusRepasseEacessoDescricao.REPASSE_CADASTRADO.GetDescription();
                case REPASSE_EDITADO:
                    return SharedEnuns.StatusRepasseEacessoDescricao.REPASSE_EDITADO.GetDescription();
                default:
                    return null;
            }
        }
        
    }
}
