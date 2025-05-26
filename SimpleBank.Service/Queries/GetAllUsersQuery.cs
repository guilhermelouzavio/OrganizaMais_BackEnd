using MediatR;
using SimpleBank.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries
{
    public class GetAllUsersQuery : IRequest<IEnumerable<UserDTO>> // Retorna uma coleção de UserDTOs
    {
        // Por enquanto, sem parâmetros, mas poderíamos ter paginação, filtros, etc.
    }
}
