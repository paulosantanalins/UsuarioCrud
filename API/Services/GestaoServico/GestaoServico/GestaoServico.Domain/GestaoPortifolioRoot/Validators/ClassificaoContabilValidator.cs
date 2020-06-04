using FluentValidation;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Validators
{
    public class ClassificacaoContabilValidator : AbstractValidator<ClassificacaoContabil>
    {
        public ClassificacaoContabilValidator()
        {
            RuleFor(x => x.DescClassificacaoContabil)
                .NotEmpty().WithMessage("PERSISTIR_CLASSIFICACAO_CONTABIL_CAMPO_OBRIGATORIO_DESCRICAO");

            RuleFor(x => x.SgClassificacaoContabil)
                .NotEmpty().WithMessage("PERSISTIR_CLASSIFICACAO_CONTABIL_CAMPO_OBRIGATORIO_SIGLA");

            RuleFor(x => x.IdCategoriaContabil)
                .NotEmpty().WithMessage("PERSISTIR_CLASSIFICACAO_CONTABIL_CAMPO_OBRIGATORIO_CATEGORIA");

            When(p => !String.IsNullOrEmpty(p.SgClassificacaoContabil), () =>
            {
                RuleFor(y => y.SgClassificacaoContabil)
                    .MaximumLength(10).WithMessage("PERSISTIR_CLASSIFICACAO_CONTABIL_SIGLA_TAMANHO_INVALIDO");
            });
        }
    }
}
