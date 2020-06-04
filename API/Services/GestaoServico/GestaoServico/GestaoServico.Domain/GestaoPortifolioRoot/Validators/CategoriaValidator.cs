using FluentValidation;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Validators
{
    public class CategoriaValidator : AbstractValidator<CategoriaContabil>
    {
        public CategoriaValidator()
        {
            RuleFor(p => p.DescCategoria)
                .NotEmpty().WithMessage("PERSISTIR_CATEGORIA_CAMPO_OBRIGATORIO_DESCRICAO");

            RuleFor(p => p.SgCategoriaContabil)
               .NotEmpty().WithMessage("PERSISTIR_CATEGORIA_CAMPO_OBRIGATORIO_SIGLA");

            When(p => !String.IsNullOrEmpty(p.SgCategoriaContabil), () =>
            {
                RuleFor(y => y.SgCategoriaContabil)
                    .MaximumLength(10).WithMessage("PERSISTIR_CATEGORIA_SIGLA_TAMANHO_INVALIDO");
            });
        }
    }
}
