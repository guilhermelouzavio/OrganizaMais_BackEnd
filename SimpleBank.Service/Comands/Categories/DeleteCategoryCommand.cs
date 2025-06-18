using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Categories
{
    public class DeleteCategoryCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // Para garantir que só delete as próprias ou categorias padrão (se for admin)
    }
}
