using MediatR;
using SimpleBank.Application.Comands.Categories;
using SimpleBank.Domain.Entities;
using SimpleBank.Application.Dtos;
using SimpleBank.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Handlers.Categories
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDTO>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryDTO> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            // O validador (FluentValidation) já vai garantir que nome e tipo não sejam vazios.
            // Se UserId não for nulo, verificar se o usuário existe (boa prática)
            if (request.UserId.HasValue)
            {
                var userExists = await _unitOfWork.Users.GetByIdAsync(request.UserId.Value);
                if (userExists == null)
                {
                    throw new InvalidOperationException($"Usuário com ID {request.UserId.Value} não encontrado.");
                }
            }

            // Criar a entidade de domínio
            var category = new Category(request.UserId,request.Name,  request.Type, request.Description);

            // Adicionar ao repositório
            await _unitOfWork.Categories.AddAsync(category);

            // Persistir as mudanças
            await _unitOfWork.CompleteAsync();

            // Retornar um DTO
            return new CategoryDTO
            {
                Id = category.Id,
                UserId = category.UserId,
                Name = category.Name,
                Type = category.Type
            };
        }
    }
}
