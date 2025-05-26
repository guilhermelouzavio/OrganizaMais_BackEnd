using SimpleBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SimpleBank.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public int? UserId { get; private set; } // Pode ser nulo para categorias padrão do sistema
        public string Name { get; private set; }
        public TransactionType Type { get; private set; } // Receita ou Despesa
        public DateTime CreatedAt { get; private set; }

        // Construtor para criar uma nova categoria
        public Category(string name, TransactionType type, int? userId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be null or empty.", nameof(name));

            Name = name;
            Type = type;
            UserId = userId; // Pode ser nulo para categorias globais/padrão
            CreatedAt = DateTime.UtcNow;
        }

        private Category() { } // Construtor para o EF Core

        // Métodos de comportamento
        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("New category name cannot be null or empty.", nameof(newName));
            Name = newName;
        }

        // Navegação
        public User? User { get; private set; } // Uma categoria pode pertencer a um usuário (se for customizada)
        public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();
    }
}
