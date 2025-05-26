using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SimpleBank.Domain.Entities
{
    public class FinancialAccount
    {
        public int Id { get; set; }
        public int UserId { get; private set; }
        public string Name { get; private set; }
        public decimal Balance { get; private set; } // Saldo atual da conta
        public DateTime CreatedAt { get; private set; }

        // Construtor
        public FinancialAccount(int userId, string name, decimal initialBalance = 0)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be valid.", nameof(userId));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Account name cannot be null or empty.", nameof(name));
            if (initialBalance < 0) // Uma conta pode começar com 0, mas não com saldo negativo forçado
                throw new ArgumentException("Initial balance cannot be negative.", nameof(initialBalance));


            UserId = userId;
            Name = name;
            Balance = initialBalance;
            CreatedAt = DateTime.UtcNow;
        }

        private FinancialAccount() { } // Construtor para o EF Core

        // Métodos de comportamento para o domínio
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be positive.", nameof(amount));
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdraw amount must be positive.", nameof(amount));
            // Podemos adicionar regras de negócio aqui, tipo não permitir saldo negativo,
            // ou exigir validação extra se for um saque grande.
            // Por simplicidade, vamos permitir saque maior que o saldo por enquanto.
            // if (Balance < amount) throw new InvalidOperationException("Insufficient funds.");
            Balance -= amount;
        }

        // Navegação
        public User? User { get; private set; } // Uma conta pertence a um usuário
        public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();
    }
}
