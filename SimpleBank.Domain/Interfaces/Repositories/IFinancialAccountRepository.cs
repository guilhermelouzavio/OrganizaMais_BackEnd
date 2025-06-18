using SimpleBank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Domain.Interfaces.Repositories
{
    public interface IFinancialAccountRepository
    {
        Task<FinancialAccount?> GetByIdAsync(int id);
        Task<IEnumerable<FinancialAccount>> GetByUserIdAsync(int userId);
        Task<long> AddAsync(FinancialAccount account);
        void Update(FinancialAccount account);
        void Delete(FinancialAccount account);
    }
}
