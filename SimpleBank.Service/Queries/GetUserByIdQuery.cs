using MediatR;
using SimpleBank.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries
{
    public class GetUserByIdQuery : IRequest<UserDTO?> // Retorna um UserDTO ou nulo
    {
        public int Id { get; set; }
    }
}
