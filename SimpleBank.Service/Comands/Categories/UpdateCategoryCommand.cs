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
    public class UpdateCategoryCommand : IRequest<CategoryDTO>
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // Para garantir que só edite as próprias ou categorias padrão (se for admin)
        public string Name { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
    }
}
