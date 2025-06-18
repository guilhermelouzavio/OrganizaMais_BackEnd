using MediatR;
using SimpleBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries.Transactions
{
    // DTO para o relatório sumarizado por categoria/mês etc.
    public class TransactionReportItemDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
        public decimal TotalValue { get; set; }
        public int Count { get; set; }
    }

    public class GetTransactionsReportQuery : IRequest<IEnumerable<TransactionReportItemDTO>>
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TransactionType? Type { get; set; } // Opcional: filtrar por tipo
        // Adicionar outros filtros, como FinancialAccountId, etc.
    }
}
