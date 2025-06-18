using FluentValidation;
using SimpleBank.Application.Comands.FinancialAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Validators
{
    public class CreateFinancialAccountCommandValidator : AbstractValidator<CreateFinancialAccountCommand>
    {
        public CreateFinancialAccountCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("ID do Usuário é obrigatório.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome da conta é obrigatório.")
                .MaximumLength(100).WithMessage("Nome da conta não pode ter mais de 100 caracteres.");

            RuleFor(x => x.InitialBalance)
                .GreaterThanOrEqualTo(0).WithMessage("Saldo inicial não pode ser negativo.");
        }
    }
}
