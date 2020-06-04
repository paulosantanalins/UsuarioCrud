using AutoMapper;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Api.AutoMapper.Resolvers
{
    public class CustomResolver : IValueResolver<EAcessoRepasse, Repasse, int?>
    {
        public int? Resolve(EAcessoRepasse source, Repasse destination, int? destMember, ResolutionContext context)
        {
            return 2;
            try
            {
                var nrParcela = 1;
                if (source.IdRepasseMae != 0)
                {
                    if (source.DescProjeto.Split("- PARCELA ").Length < 2 && source.DescProjeto.Contains("/"))
                    {
                        if (int.TryParse(source.DescProjeto.Split("- PARCELA ")[1].Split("/")[0], out int result))
                        {
                            nrParcela = Convert.ToInt32(source.DescProjeto.Split("- PARCELA ")[1].Split("/")[0]);
                            return nrParcela;
                        }
                        else
                            return nrParcela;
                    }
                    else
                        return nrParcela;
                }
                else
                    return nrParcela;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
