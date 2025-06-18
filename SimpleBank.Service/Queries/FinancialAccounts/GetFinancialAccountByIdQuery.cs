using MediatR;
using SimpleBank.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries.FinancialAccounts
{
    public class GetFinancialAccountByIdQuery : IRequest<FinancialAccountDTO?>
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Para garantir que só veja as próprias contas
    }
}
