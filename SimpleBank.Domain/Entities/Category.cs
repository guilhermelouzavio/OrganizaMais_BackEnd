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
        public string? Description { get; private set; } // NOVO CAMPO: Descrição da categoria
        public TransactionType Type { get; private set; } // Receita ou Despesa
        public DateTime CreatedAt { get; private set; }

        // Construtor para criar uma nova categoria
        public Category(int? userId, string name, TransactionType type, string? description = null)
        {
            UserId = userId;
            Name = name;
            Description = description; // Atribui a descrição
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }

        // Construtor para reconstrução de entidade do banco de dados (usado pelo Dapper)
        public Category(int id, int? userId, string name, string? description, TransactionType type, DateTime createdAt)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Description = description; // Atribui a descrição
            Type = type;
            CreatedAt = createdAt;
        }


        private Category() { } // Construtor para o EF Core

        // Método para atualizar a categoria (se necessário)
        public void Update(string name, TransactionType type, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Nome da categoria não pode ser vazio.", nameof(name));
            }
            Name = name;
            Type = type;
            Description = description; // Atualiza a descrição
        }

        // Navegação
        public User? User { get; private set; } // Uma categoria pode pertencer a um usuário (se for customizada)
        public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();
    }
}
