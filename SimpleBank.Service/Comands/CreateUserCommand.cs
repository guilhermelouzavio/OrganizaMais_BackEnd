using MediatR;
using SimpleBank.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands
{
    public class CreateUserCommand : IRequest<UserDTO>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // A senha em texto puro, antes do hash
    }
}
