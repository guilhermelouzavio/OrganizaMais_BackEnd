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
    public class GetFinancialAccountByIdQueryHandler : IRequestHandler<GetFinancialAccountByIdQuery, FinancialAccountDTO?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFinancialAccountByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FinancialAccountDTO?> Handle(GetFinancialAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.FinancialAccounts.GetByIdAsync(request.Id);

            // Adicionar validação de posse: só pode ver a própria conta
            if (account == null || account.UserId != request.UserId)
            {
                return null;
            }

            return new FinancialAccountDTO
            {
                Id = account.Id,
                UserId = account.UserId,
                Name = account.Name,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt
            };
        }
    }
}
