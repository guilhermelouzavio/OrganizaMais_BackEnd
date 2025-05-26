using SimpleBank.Application.Interfaces;
using SimpleBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Strategies
{
    public class PixFeeStrategy : ITransactionFeeStrategy
    {
        public TransactionType AppliesTo => TransactionType.Income; // Exemplo: Sem taxa para Pix recebido

        public Task<decimal> CalculateFee(decimal transactionValue)
        {
            // Exemplo de lógica: Pix geralmente não tem taxa
            return Task.FromResult(0m);
        }
    }
}
