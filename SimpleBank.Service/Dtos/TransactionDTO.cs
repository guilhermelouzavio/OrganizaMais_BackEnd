using SimpleBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Dtos
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FinancialAccountId { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public TransactionType Type { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
