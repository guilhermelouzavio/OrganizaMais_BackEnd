using MediatR;
using SimpleBank.Application.Dtos;
using SimpleBank.Application.Queries.Transactions;
using SimpleBank.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries.Handlers.Transactions
{
    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDTO?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTransactionByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TransactionDTO?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(request.Id);

            // Validação de posse
            if (transaction == null || transaction.UserId != request.UserId)
            {
                return null; // Não encontrada ou não pertence ao usuário
            }

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
