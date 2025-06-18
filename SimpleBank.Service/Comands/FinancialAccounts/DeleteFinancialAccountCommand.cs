using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.FinancialAccounts
{
    public class DeleteFinancialAccountCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Para verificar se a conta pertence ao usuário
    }
}
