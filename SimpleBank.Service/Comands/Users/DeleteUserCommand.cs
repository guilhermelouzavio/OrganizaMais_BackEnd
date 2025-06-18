using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Users
{
    public class DeleteUserCommand : IRequest<Unit> // Unit indica que não retorna nada, apenas sucesso/falha
    {
        public int Id { get; set; }
    }
}
