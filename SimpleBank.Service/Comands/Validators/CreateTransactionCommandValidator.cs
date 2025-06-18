using FluentValidation;
using SimpleBank.Application.Comands.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Validators
{
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("ID do Usuário é obrigatório.");

            RuleFor(x => x.FinancialAccountId)
                .GreaterThan(0).WithMessage("ID da Conta Financeira é obrigatório.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("ID da Categoria é obrigatório.");

            RuleFor(x => x.Value)
                .GreaterThan(0).WithMessage("Valor da transação deve ser maior que zero.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Tipo de transação inválido. Deve ser 'Income' ou 'Expense'.");

            RuleFor(x => x.TransactionDate)
                .NotEmpty().WithMessage("Data da transação é obrigatória.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Data da transação não pode ser futura.");
            // Você pode ajustar essa regra se permitir transações futuras (agendadas)
        }
    }
}
