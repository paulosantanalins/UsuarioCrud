using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Validators
{
    public class PerfilValidator : AbstractValidator<Perfil>
    {
        public PerfilValidator()
        {
            RuleFor(x => x.NmPerfil)
                .NotEmpty().WithMessage("PERSISTIR_PERFIL_CAMPO_OBRIGATORIO_NOME");

            RuleFor(x => x.VinculoPerfilFuncionalidades)
                .Must(y => y.Count > 0) .WithMessage("PERSISTIR_PERFIL_CAMPO_OBRIGATORIO_FUNCIONALIDADE");
        }
    }
}
