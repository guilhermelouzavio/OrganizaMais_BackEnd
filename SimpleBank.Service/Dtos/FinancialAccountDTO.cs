using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Dtos
{
    public class FinancialAccountDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
