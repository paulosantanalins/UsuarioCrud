using FluentValidation;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Validators
{
    public class TipoServicoValidator : AbstractValidator<TipoServico>
    {
        public TipoServicoValidator()
        {
            RuleFor(p => p.DescTipoServico)
                .NotEmpty().WithMessage("PERSISTIR_TIPO_SERVICO_CAMPO_OBRIGATORIO_DESCRICAO");
        }
    }
}
