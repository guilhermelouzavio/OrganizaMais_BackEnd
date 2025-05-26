using SimpleBank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Domain.Specifications
{
    public class UserByEmailSpecification : BaseSpecification<User>
    {
        public UserByEmailSpecification(string email)
            : base(user => user.Email == email) // A condição da especificação
        {
            // Se você quisesse incluir outras propriedades do User aqui, faria:
            // AddInclude(user => user.FinancialAccounts);
        }
    }
}
