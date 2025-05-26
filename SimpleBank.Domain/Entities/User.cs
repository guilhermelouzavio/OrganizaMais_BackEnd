using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SimpleBank.Domain.Entities
{
    public class User
    {
        public int Id { get; set; } // O Id é setado internamente ou pelo banco
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; } // Vai guardar o hash da senha
        public DateTime CreatedAt { get; private set; }

        // Construtor para criar um novo usuário
        public User(string name, string email, string passwordHash)
        {
            // Validações básicas no construtor
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));

            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow; // Sempre armazenar datas em UTC
        }

        // Construtor privado para o Entity Framework Core (e outros ORMs)
        // Isso impede que o EF Core ou outras ferramentas tentem usar o construtor público para hidratação
        private User() { }

        // Métodos de comportamento (se houver, por exemplo, para atualizar dados do usuário)
        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("New name cannot be null or empty.", nameof(newName));
            Name = newName;
        }

        public void UpdateEmail(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentException("New email cannot be null or empty.", nameof(newEmail));
            Email = newEmail;
        }

        // No DDD, as coleções de navegação geralmente não são expostas publicamente para escrita direta.
        // O acesso e modificação devem ser feitos através de métodos de domínio se houver regras.
        // Para o EF Core, pode ser IList ou ICollection para navegação.
        public ICollection<FinancialAccount> FinancialAccounts { get; private set; } = new List<FinancialAccount>();
        public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();
        public ICollection<Category> Categories { get; private set; } = new List<Category>();
    }
}
