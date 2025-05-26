using SimpleBank.Application.Dtos.Responses;
using SimpleBank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Services
{
    public interface IBankService
    {
        Task<decimal> GetBalanceAsync(Guid accountId);
        Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount);
        Task PayBillAsync(Guid accountId, decimal amount, string description);
    }
}
