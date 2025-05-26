using MediatR;
using SimpleBank.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands
{
    public class UpdateUserCommand : IRequest<UserDTO>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // Não vamos atualizar a senha por aqui, teria um comando específico pra isso.
    }
}
