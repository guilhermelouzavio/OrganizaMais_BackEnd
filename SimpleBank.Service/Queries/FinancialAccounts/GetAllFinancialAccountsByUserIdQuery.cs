using MediatR;
using SimpleBank.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries.FinancialAccounts
{
    public class GetAllFinancialAccountsByUserIdQuery : IRequest<IEnumerable<FinancialAccountDTO>>
    {
        public int UserId { get; set; }
    }
}
