using MediatR;
using SimpleBank.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries.Transactions
{
    public class GetTransactionByIdQuery : IRequest<TransactionDTO?>
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Para verificar posse da transação
    }
}
