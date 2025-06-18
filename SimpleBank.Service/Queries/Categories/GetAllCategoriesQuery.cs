using MediatR;
using SimpleBank.Application.Dtos;
using SimpleBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries.Categories
{
    public class GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDTO>>
    {
        public int? UserId { get; set; } // Pode buscar todas, ou só as do usuário, ou só as padrão
        public TransactionType? Type { get; set; } // Opcional: filtrar por tipo (receita/despesa)
    }
}
