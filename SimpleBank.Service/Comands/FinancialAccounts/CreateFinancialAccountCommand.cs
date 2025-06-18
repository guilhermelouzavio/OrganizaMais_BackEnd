using MediatR;
using SimpleBank.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.FinancialAccounts
{
    public class CreateFinancialAccountCommand : IRequest<FinancialAccountDTO>
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal InitialBalance { get; set; }
    }
}
