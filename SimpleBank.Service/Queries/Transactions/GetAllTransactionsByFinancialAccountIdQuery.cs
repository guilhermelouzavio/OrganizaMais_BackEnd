using MediatR;
using SimpleBank.Application.Dtos;
using SimpleBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries.Transactions
{
    public class GetAllTransactionsByFinancialAccountIdQuery : IRequest<IEnumerable<TransactionDTO>>
    {
        public int UserId { get; set; }
        public int FinancialAccountId { get; set; }
        public TransactionType? Type { get; set; } // Opcional: filtrar por tipo
        public DateTime? StartDate { get; set; } // Opcional: filtro de data inicial
        public DateTime? EndDate { get; set; } // Opcional: filtro de data final
    }
}
