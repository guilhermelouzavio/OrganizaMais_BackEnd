using SimpleBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Interfaces
{
    // Interface para as diferentes estratégias de cálculo de taxa.
    public interface ITransactionFeeStrategy
    {
        // Propriedade para identificar qual tipo de transação essa estratégia lida.
        TransactionType AppliesTo { get; }

        // Método para calcular a taxa.
        Task<decimal> CalculateFee(decimal transactionValue);
    }
}
