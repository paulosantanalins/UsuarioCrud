using AutoMapper;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class EpmRepasseToRepasseCustomProfile : Profile
    {
        public EpmRepasseToRepasseCustomProfile()
        {
            var dataAtual = new DateTime(2019, 05, 21);//DateTime.Now;

            CreateMap<EpmRepasse, Repasse>()
                .ForMember(src => src.Id, opt => opt.MapFrom(x => 0))
                .ForMember(src => src.IdEpm, opt => opt.MapFrom(x => x.Id))
                //.ForMember(src => src.DtRepasse, opt => opt.MapFrom(x => DateTime.Now.Date))
                .ForMember(src => src.DtRepasse, opt => opt.MapFrom(x => new DateTime(dataAtual.Year, dataAtual.Month, dataAtual.Day)))
                .ForMember(src => src.DataAlteracao, opt => opt.MapFrom(x => DateTime.Now.Date))
                .ForMember(src => src.IdRepasseEacesso, opt => opt.MapFrom(x => x.IdRepasse ?? x.IdRepasseInterno ?? 0))
                .ForMember(src => src.DescProjeto, opt => opt.MapFrom(x => x.Comments))
                .ForMember(src => src.FlStatus, opt => opt.MapFrom(x => x.FlStatus))
                .ForMember(src => src.IdServicoContratadoDestino, opt => opt.MapFrom(x => x.IdServicoCom))
                .ForMember(src => src.IdServicoContratadoOrigem, opt => opt.MapFrom(x => x.IdServicoTec))
                .ForMember(src => src.IdProfissional, opt => opt.MapFrom(x => x.IDProfissional))
                //.ForMember(src => src.IdTipoDespesa, opt => opt.MapFrom(x => 1))
                .ForMember(src => src.Usuario, opt => opt.MapFrom(x => "EPM"))
                //duvida
                //.ForMember(src => src.IdRepasseMae, null)
                .ForMember(src => src.QtdRepasse, opt => opt.MapFrom(x => x.TransferWork))
                //.ForMember(src => src.VlCustoProfissional, opt => opt.MapFrom(x => x.ResourceRate))
                .ForMember(src => src.VlTotal, opt => opt.MapFrom(x => x.TransferCost))
                .ForMember(src => src.VlUnitario, opt => opt.MapFrom(x => x.TransferRate))
                //.ForMember(src => src.FlRepasseInterno, opt => opt.MapFrom(x => !x.Checked))


                //sei la
                //.ForMember(src => src.DtRepasseMae, opt => opt.MapFrom(x => DateTime.Now))
                //.ForMember(src => src.DescMotivoNegacao, opt => opt.MapFrom(x => DateTime.Now))
                //.ForMember(src => src.DescJustificativa, opt => opt.MapFrom(x => DateTime.Now))
                ;
        }
    }
}
