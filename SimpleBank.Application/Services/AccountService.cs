using SimpleBank.Application.Services;
using SimpleBank.Domain.Entities;
using SimpleBank.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Appilcation.Services;
public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICustomerRepository _customerRepository;

    public AccountService(IAccountRepository accountRepository, ICustomerRepository customerRepository)
    {
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Account> CreateAccountAsync(Guid customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            throw new Exception("Customer not found.");

        var account = new Account(customerId);

        await _accountRepository.AddAsync(account);
        return account;
    }

    public async Task<Account> GetByIdAsync(Guid accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null)
            throw new Exception("Account not found.");

        return account;
    }

    public async Task DepositAsync(Guid accountId, decimal amount)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null)
            throw new Exception("Account not found.");

        account.Deposit(amount);

        await _accountRepository.UpdateAsync(account);
    }

    public async Task WithdrawAsync(Guid accountId, decimal amount)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null)
            throw new Exception("Account not found.");

        account.Withdraw(amount);

        await _accountRepository.UpdateAsync(account);
    }

    public async Task<decimal> GetBalanceAsync(Guid accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null)
            throw new Exception("Account not found.");

        return account.Balance;
    }
}