using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Transactions
{
    public class DeleteTransactionCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Para verificar posse da transação
    }
}
