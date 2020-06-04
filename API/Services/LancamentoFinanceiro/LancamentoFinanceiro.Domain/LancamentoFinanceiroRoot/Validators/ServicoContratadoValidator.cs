using FluentValidation;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Validators
{
    public class ServicoContratadoValidator : AbstractValidator<ServicoContratado>
    {
        public ServicoContratadoValidator()
        {
            RuleFor(e => e.IdContrato)
                .NotEmpty().WithMessage("PERSISTIR_SERVICO_CONTRATADO_CAMPO_OBRIGATORIO_CONTRATO");
            //RuleFor(e => e.IdEscopoServico)
            //    .NotEmpty().WithMessage("PERSISTIR_SERVICO_CONTRATADO_CAMPO_OBRIGATORIO_PORTIFOLIO");
            //RuleFor(e => e.VlMarkup)
            //    .NotNull().WithMessage("PERSISTIR_SERVICO_CONTRATADO_CAMPO_OBRIGATORIO_MARKUP");
            RuleFor(e => e.DtInicial)
                .NotEmpty().WithMessage("PERSISTIR_SERVICO_CONTRATADO_CAMPO_OBRIGATORIO_DTINICIAL");
            RuleFor(e => e.DtFinal)
                .NotEmpty().WithMessage("PERSISTIR_SERVICO_CONTRATADO_CAMPO_OBRIGATORIO_DTFINAL");
            //RuleFor(e => e.VlRentabilidade)
            // .NotEmpty().WithMessage("PERSISTIR_SERVICO_CONTRATADO_CAMPO_OBRIGATORIO_RENTABILIDADE");
            //RuleFor(e => e.NmTipoReembolso)
            // .NotEmpty().WithMessage("PERSISTIR_SERVICO_CONTRATADO_CAMPO_OBRIGATORIO_TIPO_REEMBOLSO");
            //RuleFor(e => e.IdEmpresa)
            // .NotEmpty().WithMessage("PERSISTIR_SERVICO_CONTRATADO_CAMPO_OBRIGATORIO_EMPRESA");
            //RuleFor(e => e.IdProdutoRM)
            //.NotEmpty().WithMessage("PERSISTIR_SERVICO_CONTRATADO_CAMPO_OBRIGATORIO_PRODUTORM");
        }
    }
}
