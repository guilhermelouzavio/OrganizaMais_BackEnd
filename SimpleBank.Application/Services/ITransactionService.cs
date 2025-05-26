using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Services
{
    public interface ITransactionService
    {
        Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount);
        Task PayBillAsync(Guid accountId, decimal amount, string billReference);
    }
}
