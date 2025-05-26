using SimpleBank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        void Update(User user); // Não é assíncrono porque não acessa o banco diretamente, só marca para update
        void Delete(User user);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
