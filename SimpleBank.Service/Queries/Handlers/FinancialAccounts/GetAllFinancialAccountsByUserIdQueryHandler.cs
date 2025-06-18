using MediatR;
using SimpleBank.Application.Dtos;
using SimpleBank.Application.Queries.FinancialAccounts;
using SimpleBank.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries.Handlers.FinancialAccounts
{
    public class GetAllFinancialAccountsByUserIdQueryHandler : IRequestHandler<GetAllFinancialAccountsByUserIdQuery, IEnumerable<FinancialAccountDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllFinancialAccountsByUserIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<FinancialAccountDTO>> Handle(GetAllFinancialAccountsByUserIdQuery request, CancellationToken cancellationToken)
        {
            // O repositório deve ter um método para buscar por UserId
            var accounts = await _unitOfWork.FinancialAccounts.GetByUserIdAsync(request.UserId);

            // Mapear para DTOs
            return accounts.Select(account => new FinancialAccountDTO
            {
                Id = account.Id,
                UserId = account.UserId,
                Name = account.Name,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt
            }).ToList();
        }
    }
}
