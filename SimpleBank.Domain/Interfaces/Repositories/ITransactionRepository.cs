using SimpleBank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Domain.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Transaction>> GetByFinancialAccountIdAsync(int financialAccountId);
        Task AddAsync(Transaction transaction);
        void Update(Transaction transaction);
        void Delete(Transaction transaction);
    }
}
