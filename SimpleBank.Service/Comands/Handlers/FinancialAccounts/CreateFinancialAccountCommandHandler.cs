using MediatR;
using SimpleBank.Application.Comands.FinancialAccounts;
using SimpleBank.Application.Dtos;
using SimpleBank.Domain.Entities;
using SimpleBank.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Handlers.FinancialAccounts
{
    public class CreateFinancialAccountCommandHandler : IRequestHandler<CreateFinancialAccountCommand, FinancialAccountDTO>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateFinancialAccountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FinancialAccountDTO> Handle(CreateFinancialAccountCommand request, CancellationToken cancellationToken)
        {
            // Validação simples (o FluentValidation faria validações mais complexas)
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Nome da conta financeira é obrigatório.");
            }

            // Verificar se o usuário existe (boa prática)
            var userExists = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (userExists == null)
            {
                throw new InvalidOperationException($"Usuário com ID {request.UserId} não encontrado.");
            }

            // Criar a entidade de domínio
            var financialAccount = new FinancialAccount(request.UserId, request.Name, request.InitialBalance);

            // Adicionar ao repositório
            var financialAccountIDReturn = await _unitOfWork.FinancialAccounts.AddAsync(financialAccount);

            // Persistir as mudanças
            await _unitOfWork.CompleteAsync();

            if (financialAccountIDReturn != 0)
            {
                // Retornar um DTO
                return new FinancialAccountDTO
                {
                    Id = financialAccount.Id,
                    UserId = financialAccount.UserId,
                    Name = financialAccount.Name,
                    Balance = financialAccount.Balance,
                    CreatedAt = financialAccount.CreatedAt
                };
            }
            else
                return null;
           
        }
    }
}
