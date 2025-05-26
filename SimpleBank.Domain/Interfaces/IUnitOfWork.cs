using SimpleBank.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Propriedades para acessar os repositórios
        IUserRepository Users { get; }
        IFinancialAccountRepository FinancialAccounts { get; }
        ICategoryRepository Categories { get; }
        ITransactionRepository Transactions { get; }

        // Método para salvar todas as mudanças
        Task<int> CompleteAsync();
    }
}
