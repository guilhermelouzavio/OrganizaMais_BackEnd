using SimpleBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Dtos
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // Pode ser nulo para categorias padrão
        public string Name { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
    }
}
