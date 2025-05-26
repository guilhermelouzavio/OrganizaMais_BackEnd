using SimpleBank.Application.Interfaces;
using SimpleBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Strategies
{
    public class CreditCardFeeStrategy : ITransactionFeeStrategy
    {
        public TransactionType AppliesTo => TransactionType.Expense; // Exemplo: Taxa para despesas (compra no cartão)

        public Task<decimal> CalculateFee(decimal transactionValue)
        {
            // Exemplo de lógica: 2.5% de taxa para transações de cartão de crédito
            return Task.FromResult(transactionValue * 0.025m);
        }
    }
}
