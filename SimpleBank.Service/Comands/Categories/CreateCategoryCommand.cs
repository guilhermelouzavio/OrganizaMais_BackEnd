using MediatR;
using SimpleBank.Application.Dtos;
using SimpleBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Categories
{
    public class CreateCategoryCommand : IRequest<CategoryDTO>
    {
        public int? UserId { get; set; } // Pode ser nulo para categorias padrão do sistema
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public TransactionType Type { get; set; } // Income ou Expense
    }
}
