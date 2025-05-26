using SimpleBank.Domain.Entities;
using SimpleBank.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
                //if (amount <= 0)
                //    throw new BusinessException("The amount must be greater than zero.");

                //var fromAccount = await _accountRepository.GetByIdAsync(fromAccountId);
                //var toAccount = await _accountRepository.GetByIdAsync(toAccountId);

                //if (fromAccount == null || toAccount == null)
                //    throw new BusinessException("One or both accounts not found.");

                //if (fromAccount.Balance < amount)
                //    throw new BusinessException("Insufficient funds.");

                //fromAccount.Balance -= amount;
                //toAccount.Balance += amount;

                //await _accountRepository.UpdateAsync(fromAccount);
                //await _accountRepository.UpdateAsync(toAccount);

                //var transaction = new Transaction
                //{
                //    Id = Guid.NewGuid(),
                //    FromAccountId = fromAccountId,
                //    ToAccountId = toAccountId,
                //    Amount = amount,
                //};

                //await _transactionRepository.AddAsync(transaction);
        }

        public async Task PayBillAsync(Guid accountId, decimal amount, string billReference)
        {
            ////if (amount <= 0)
            ////    throw new BusinessException("The amount must be greater than zero.");

            ////var account = await _accountRepository.GetByIdAsync(accountId);
            ////if (account == null)
            ////    throw new BusinessException("Account not found.");

            ////if (account.Balance < amount)
            ////    throw new BusinessException("Insufficient funds.");

            //account.Balance -= amount;

            //await _accountRepository.UpdateAsync(account);

            var transaction = new Transaction
            {
                FromAccountId = accountId,
                ToAccountId = Guid.NewGuid(), //rever
                Amount = amount,
                Description = billReference
            };

            await _transactionRepository.AddAsync(transaction);
        }
    }
}
