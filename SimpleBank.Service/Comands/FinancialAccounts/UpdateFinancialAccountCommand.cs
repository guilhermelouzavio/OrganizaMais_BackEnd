using MediatR;
using SimpleBank.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.FinancialAccounts
{
    public class UpdateFinancialAccountCommand : IRequest<FinancialAccountDTO>
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Para verificar se a conta pertence ao usuário
        public string Name { get; set; } = string.Empty;
    }
}
