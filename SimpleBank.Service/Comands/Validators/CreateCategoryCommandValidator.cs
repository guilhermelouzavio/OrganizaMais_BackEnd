using FluentValidation;
using SimpleBank.Application.Comands.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Validators
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome da categoria é obrigatório.")
                .MaximumLength(100).WithMessage("Nome da categoria não pode ter mais de 100 caracteres.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Tipo de transação inválido. Deve ser 'Income' ou 'Expense'.");

            // Se UserId for informado, deve ser maior que zero
            RuleFor(x => x.UserId)
                .GreaterThan(0).When(x => x.UserId.HasValue).WithMessage("ID do Usuário inválido para categoria personalizada.");
        }
    }
}
