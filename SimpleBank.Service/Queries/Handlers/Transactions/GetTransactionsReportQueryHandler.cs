using MediatR;
using SimpleBank.Application.Queries.Transactions;
using SimpleBank.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries.Handlers.Transactions
{
    public class GetTransactionsReportQueryHandler : IRequestHandler<GetTransactionsReportQuery, IEnumerable<TransactionReportItemDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTransactionsReportQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TransactionReportItemDTO>> Handle(GetTransactionsReportQuery request, CancellationToken cancellationToken)
        {
            // 1. Validação de datas (pode ser movida para um validador específico se a complexidade aumentar)
            if (request.StartDate > request.EndDate)
            {
                throw new ArgumentException("Data de início não pode ser maior que a data de fim para o relatório.");
            }

            // 2. Obter as transações filtradas do repositório, já com o nome da categoria.
            // O método GetTransactionsForReportAsync retorna uma tupla (Transaction, string categoryName)
            var transactionsWithCategoryNames = await _unitOfWork.Transactions.GetTransactionsForReportAsync(
                request.UserId,
                request.StartDate,
                request.EndDate,
                request.Type
            );

            // 3. Agrupar e sumarizar os dados para o relatório, usando o nome da categoria vindo do JOIN.
            var report = transactionsWithCategoryNames
                .GroupBy(t => new { t.transaction.CategoryId, t.transaction.Type, t.categoryName })
                .Select(group => new TransactionReportItemDTO
                {
                    CategoryName = group.Key.categoryName, // Usamos o nome da categoria que veio do banco
                    Type = group.Key.Type,
                    TotalValue = group.Sum(t => t.transaction.Value),
                    Count = group.Count()
                })
                .OrderBy(item => item.CategoryName)
                .ToList();

            return report;
        }
    }
}
