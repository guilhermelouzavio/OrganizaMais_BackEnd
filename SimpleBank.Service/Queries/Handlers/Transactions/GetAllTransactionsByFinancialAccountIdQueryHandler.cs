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
    public class GetAllTransactionsByFinancialAccountIdQueryHandler : IRequestHandler<GetAllTransactionsByFinancialAccountIdQuery, IEnumerable<TransactionDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllTransactionsByFinancialAccountIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TransactionDTO>> Handle(GetAllTransactionsByFinancialAccountIdQuery request, CancellationToken cancellationToken)
        {
            // Verificar se a conta financeira pertence ao usuário
            var financialAccount = await _unitOfWork.FinancialAccounts.GetByIdAsync(request.FinancialAccountId);
            if (financialAccount == null || financialAccount.UserId != request.UserId)
            {
                return Enumerable.Empty<TransactionDTO>(); // Retorna vazio se a conta não existe ou não pertence ao user
            }

            // O repositório deve ter um método para buscar transações por conta, com filtros opcionais
            var transactions = await _unitOfWork.Transactions.GetByFinancialAccountIdAsync(
                request.FinancialAccountId,
                request.Type,
                request.StartDate,
                request.EndDate
            );

            return transactions.Select(t => new TransactionDTO
            {
                Id = t.Id,
                UserId = t.UserId,
                FinancialAccountId = t.FinancialAccountId,
                CategoryId = t.CategoryId,
                Description = t.Description,
                Value = t.Value,
                Type = t.Type,
                TransactionDate = t.TransactionDate,
                CreatedAt = t.CreatedAt
            }).ToList();
        }
    }
}
