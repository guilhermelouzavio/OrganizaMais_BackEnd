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

        // Método para atualizar informações do usuário (excluindo senha)
        public void Update(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Nome do usuário não pode ser vazio.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email do usuário não pode ser vazio.", nameof(email));
            }
            Name = name;
            Email = email;
        }

        // Método para atualizar a senha (opcional, se fizer um comando de "Alterar Senha")
        public void SetPasswordHash(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
            {
                throw new ArgumentException("Hash da senha não pode ser vazio.", nameof(newPasswordHash));
            }
            PasswordHash = newPasswordHash;
        }

        // No DDD, as coleções de navegação geralmente não são expostas publicamente para escrita direta.
        // O acesso e modificação devem ser feitos através de métodos de domínio se houver regras.
        // Para o EF Core, pode ser IList ou ICollection para navegação.
        public ICollection<FinancialAccount> FinancialAccounts { get; private set; } = new List<FinancialAccount>();
        public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();
        public ICollection<Category> Categories { get; private set; } = new List<Category>();
    }
}
