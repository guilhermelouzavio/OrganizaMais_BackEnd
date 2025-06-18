using MediatR;
using SimpleBank.Application.Dtos;
using SimpleBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Transactions
{
    public class UpdateTransactionCommand : IRequest<TransactionDTO>
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Para verificar posse da transação
        public int FinancialAccountId { get; set; } // Se a conta foi alterada
        public int CategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public TransactionType Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
