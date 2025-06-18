using MediatR;
using SimpleBank.Application.Dtos;
using SimpleBank.Application.Queries.Categories;
using SimpleBank.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries.Handlers.Categories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDTO>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.Category> categories;

            if (request.UserId.HasValue)
            {
                // Buscar categorias do usuário OU categorias padrão (UserId nulo)
                categories = await _unitOfWork.Categories.GetByUserIdOrStandardAsync(request.UserId.Value);
            }
            else
            {
                // Se não especificado UserId, buscar todas as categorias padrão (se houver uma lógica de admin)
                // Para um usuário comum, essa query sem UserId não deveria existir ou buscaria apenas as padrão.
                // Por enquanto, vamos buscar todas as categorias padrão se o UserId não for especificado.
                categories = await _unitOfWork.Categories.GetAllStandardCategoriesAsync(); // Isso pode pegar todas, inclusive as de outros users.
                                                                         // O ideal seria GetStandardCategoriesAsync().
                categories = categories.Where(c => !c.UserId.HasValue); // Filtra apenas as que não tem UserId
            }

            if (request.Type.HasValue)
            {
                categories = categories.Where(c => c.Type == request.Type.Value);
            }

            return categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                UserId = c.UserId,
                Name = c.Name,
                Description = c.Description,
                Type = c.Type
            }).ToList();
        }
    }
}
