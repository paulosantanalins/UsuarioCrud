using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seguranca.Domain.UsuarioRoot.Validators
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("Por favor informe o nome");

            When(p => !String.IsNullOrEmpty(p.Nome), () =>
            {
                RuleFor(y => y.Nome)
                    .MaximumLength(100).WithMessage("O nome pode ter no máximo 100 caracteres")
                    .MinimumLength(3).WithMessage("O nome deve ter no minimo 3 caracteres");
            });

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("Por favor informe o e-mail");

            When(p => !String.IsNullOrEmpty(p.Email), () =>
            {
                RuleFor(p => p.Email)
                .MaximumLength(100).WithMessage("O e-mail pode ter no máximo 100 caracteres")
                .MinimumLength(3).WithMessage("O e-mail deve ter no minimo 3 caracteres");
            });
        }
    }
}
