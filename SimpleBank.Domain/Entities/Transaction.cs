using SimpleBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; private set; }
        public int FinancialAccountId { get; private set; }
        public int CategoryId { get; private set; }
        public string Description { get; private set; }
        public decimal Value { get; private set; }
        public TransactionType Type { get; private set; } // Herdado da categoria
        public DateTime TransactionDate { get; private set; } // Data real da transação
        public DateTime CreatedAt { get; private set; }

        // Construtor
        public Transaction(int userId, int financialAccountId, int categoryId,
                           string description, decimal value, TransactionType type, DateTime transactionDate)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be valid.", nameof(userId));
            if (financialAccountId <= 0)
                throw new ArgumentException("Financial Account ID must be valid.", nameof(financialAccountId));
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be valid.", nameof(categoryId));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));
            if (value <= 0)
                throw new ArgumentException("Transaction value must be positive.", nameof(value));
            // A validação de TransactionDate pode ser mais elaborada se precisar, ex: não permitir datas futuras

            UserId = userId;
            FinancialAccountId = financialAccountId;
            CategoryId = categoryId;
            Description = description;
            Value = value;
            Type = type;
            TransactionDate = transactionDate;
            CreatedAt = DateTime.UtcNow;
        }

        private Transaction() { } // Construtor para o EF Core

        // Métodos de comportamento para o domínio
        public void UpdateDetails(string newDescription, decimal newValue, DateTime newTransactionDate)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
                throw new ArgumentException("New description cannot be null or empty.", nameof(newDescription));
            if (newValue <= 0)
                throw new ArgumentException("New value must be positive.", nameof(newValue));

            Description = newDescription;
            Value = newValue;
            TransactionDate = newTransactionDate;
        }

        // Navegação
        public User? User { get; private set; }
        public FinancialAccount? FinancialAccount { get; private set; }
        public Category? Category { get; private set; }
    }
}
