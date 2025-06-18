using MediatR;
using SimpleBank.Application.Comands.Transactions;
using SimpleBank.Application.Dtos;
using SimpleBank.Application.Interfaces;
using SimpleBank.Domain.Entities;
using SimpleBank.Domain.Enums;
using SimpleBank.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Handlers.Transactions
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnumerable<ITransactionFeeStrategy> _feeStrategies; // Injetamos TODAS as estratégias

        public CreateTransactionCommandHandler(IUnitOfWork unitOfWork, IEnumerable<ITransactionFeeStrategy> feeStrategies)
        {
            _unitOfWork = unitOfWork;
            _feeStrategies = feeStrategies;
        }

        public async Task<TransactionDTO> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            // 1. Validações de existência (usuário, conta, categoria)
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null) throw new InvalidOperationException($"Usuário com ID {request.UserId} não encontrado.");

            var financialAccount = await _unitOfWork.FinancialAccounts.GetByIdAsync(request.FinancialAccountId);
            if (financialAccount == null || financialAccount.UserId != request.UserId)
            {
                throw new InvalidOperationException($"Conta financeira com ID {request.FinancialAccountId} não encontrada ou não pertence ao usuário.");
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            if (category == null || (category.UserId.HasValue && category.UserId.Value != request.UserId))
            {
                throw new InvalidOperationException($"Categoria com ID {request.CategoryId} não encontrada ou não pertence ao usuário.");
            }
            // Assegurar que o tipo da categoria é compatível com o tipo da transação
            if (category.Type != request.Type)
            {
                throw new InvalidOperationException($"O tipo da categoria '{category.Type}' não corresponde ao tipo da transação '{request.Type}'.");
            }

            // 2. Cálculo da Taxa (Strategy Pattern em ação!)
            decimal calculatedFee = 0;
            var feeStrategy = _feeStrategies.FirstOrDefault(s => s.AppliesTo == request.Type);
            if (feeStrategy != null)
            {
                calculatedFee = await feeStrategy.CalculateFee(request.Value);
            }

            // 3. Criar a entidade Transação
            var transaction = new Transaction(
                request.UserId,
                request.FinancialAccountId,
                request.CategoryId,
                request.Description,
                request.Value, // Valor original da transação
                request.Type,
                request.TransactionDate
            );

            // 4. Atualizar o saldo da conta financeira (Lógica de Domínio)
            if (request.Type == TransactionType.Income)
            {
                financialAccount.Deposit(request.Value - calculatedFee); // Adiciona o valor líquido
            }
            else // Expense
            {
                financialAccount.Withdraw(request.Value + calculatedFee); // Subtrai o valor bruto + taxa
            }

            // 5. Adicionar ao repositório e atualizar a conta
            await _unitOfWork.Transactions.AddAsync(transaction);
            _unitOfWork.FinancialAccounts.Update(financialAccount); // O Dapper fará o update quando CompleteAsync for chamado

            // 6. Persistir as mudanças (Isso garante que a transação e o update do saldo sejam atômicos)
            await _unitOfWork.CompleteAsync();

            // 7. Retornar DTO
            return new TransactionDTO
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                FinancialAccountId = transaction.FinancialAccountId,
                CategoryId = transaction.CategoryId,
                Description = transaction.Description,
                Value = transaction.Value,
                Type = transaction.Type,
                TransactionDate = transaction.TransactionDate,
                CreatedAt = transaction.CreatedAt
            };
        }
    }
}
