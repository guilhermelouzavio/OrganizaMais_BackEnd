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
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDTO?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CategoryDTO?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            // A validação (se a categoria existe e se pertence ao usuário) já foi feita pelo ValidationBehavior
            // antes de chegar aqui. Se chegou aqui, a categoria existe e o usuário tem permissão para vê-la.
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);

            // Se por algum motivo o validador falhar (o que não deveria), o handler ainda garante null
            // Embora a ValidationException já seria lançada antes.

            return category is null ? null : 

            new CategoryDTO
            {
                Id = category.Id,
                UserId = category.UserId,
                Name = category.Name,
                Description = category.Description,
                Type = category.Type
            };
        }
    }
}
