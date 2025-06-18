using SimpleBank.Domain.Entities;
using SimpleBank.Domain.Enums;
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

        // Este método pode retornar uma tupla, um tipo anônimo ou um DTO interno para o relatório
        Task<IEnumerable<(Transaction transaction, string categoryName)>> GetTransactionsForReportAsync(int userId, DateTime startDate, DateTime endDate, TransactionType? type = null);
        // Ou se você preferir um DTO específico que já inclua o CategoryName:
        // Task<IEnumerable<TransactionWithCategoryNameDTO>> GetTransactionsForReportAsync(int userId, DateTime startDate, DateTime endDate, TransactionType? type = null);

        Task<IEnumerable<Transaction>> GetByFinancialAccountIdAsync(int financialAccountId, TransactionType? type = null, DateTime? startDate = null, DateTime? endDate = null);
        // Adicionar para o relatório
    }
}
