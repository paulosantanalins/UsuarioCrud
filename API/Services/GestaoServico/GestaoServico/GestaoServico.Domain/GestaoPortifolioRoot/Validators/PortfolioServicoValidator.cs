using FluentValidation;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Validators
{
    public class PortfolioServicoValidator : AbstractValidator<PortfolioServico>
    {
        public PortfolioServicoValidator()
        {
            RuleFor(e => e.NmServico)
                .NotEmpty().WithMessage("PERSISTIR_PORTFOLIO_SERVICO_CAMPO_OBRIGATORIO_NOME");

            RuleFor(e => e.DescServico)
                .NotEmpty().WithMessage("PERSISTIR_PORTFOLIO_SERVICO_CAMPO_OBRIGATORIO_DESCRICAO");
 
            //RuleFor(e => e.IdTipoServico)
            //    .NotEmpty().WithMessage("PERSISTIR_PORTFOLIO_SERVICO_CAMPO_OBRIGATORIO_TIPOSERVICO");

            //RuleFor(e => e.IdCategoria)
            //   .NotEmpty().WithMessage("PERSISTIR_PORTFOLIO_SERVICO_CAMPO_OBRIGATORIO_CATEGORIA");

            //RuleFor(e => e.IdClassificacaoContabil)
            //   .NotEmpty().WithMessage("PERSISTIR_PORTFOLIO_SERVICO_CAMPO_OBRIGATORIO_CLASSIFICACAO_CONTABIL");
        }
    }
}
