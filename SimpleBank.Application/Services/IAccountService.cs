using SimpleBank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Services
{
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(Guid customerId);
        Task<Account> GetByIdAsync(Guid accountId);
        Task DepositAsync(Guid accountId, decimal amount);
        Task WithdrawAsync(Guid accountId, decimal amount);
        Task<decimal> GetBalanceAsync(Guid accountId);
    }
}
