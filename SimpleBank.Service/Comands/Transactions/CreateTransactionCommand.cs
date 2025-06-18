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
    public class CreateTransactionCommand : IRequest<TransactionDTO>
    {
        public int UserId { get; set; }
        public int FinancialAccountId { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public TransactionType Type { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Today; // Data da transação (pode ser padronizada)
    }
}
