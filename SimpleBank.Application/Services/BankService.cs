using SimpleBank.Application.Dtos.Responses;
using SimpleBank.Application.Services.Interfaces;
using SimpleBank.Domain.Entities;
using SimpleBank.Domain.Interfaces.Repositories;

namespace SimpleBank.Application.Services;

public class BankService : IBankService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    public BankService(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<decimal> GetBalanceAsync(Guid accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null)
            throw new Exception("Account not found.");

        return account.Balance;
    }

    public async Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        if (amount <= 0)
            throw new Exception("Amount must be greater than zero.");

        var fromAccount = await _accountRepository.GetByIdAsync(fromAccountId);
        var toAccount = await _accountRepository.GetByIdAsync(toAccountId);

        if (fromAccount == null || toAccount == null)
            throw new Exception("Account not found.");

        if (fromAccount.Balance < amount)
            throw new Exception("Insufficient funds.");

        fromAccount.Withdraw(amount);
        toAccount.Deposit(amount);

        await _accountRepository.UpdateAsync(fromAccount);
        await _accountRepository.UpdateAsync(toAccount);

        var transaction = new Transaction(fromAccountId, toAccountId, amount, "Transfer");
        await _transactionRepository.AddAsync(transaction);
    }

    public async Task PayBillAsync(Guid accountId, decimal amount, string description)
    {
        if (amount <= 0)
            throw new Exception("Amount must be greater than zero.");

        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null)
            throw new Exception("Account not found.");

        if (account.Balance < amount)
            throw new Exception("Insufficient funds.");

        account.Withdraw(amount);

        await _accountRepository.UpdateAsync(account);

        var transaction = new Transaction(accountId, null, amount, $"Bill Payment - {description}");
        await _transactionRepository.AddAsync(transaction);
    }
}
