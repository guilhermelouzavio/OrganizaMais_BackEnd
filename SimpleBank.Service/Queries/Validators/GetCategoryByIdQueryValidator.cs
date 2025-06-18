using FluentValidation;
using SimpleBank.Application.Queries.Categories;
using SimpleBank.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Queries.Validators
{
    public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoryByIdQueryValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID da categoria deve ser maior que zero.");

            // Adicionando a regra de validação assíncrona para verificar a existência e posse da categoria
            RuleFor(x => x)
                .MustAsync(BeAValidCategoryForUser)
                .WithMessage("Categoria não encontrada ou não pertence ao usuário especificado.")
                .When(x => x.Id > 0); // Só roda essa validação se o ID for válido inicialmente
        }

        private async Task<bool> BeAValidCategoryForUser(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(query.Id);

            if (category == null)
            {
                return false; // Categoria não existe
            }

            // Lógica de posse:
            // 1. Se a categoria tem UserId nulo (padrão do sistema), qualquer usuário pode vê-la.
            if (!category.UserId.HasValue)
            {
                return true;
            }

            // 2. Se a categoria pertence a um usuário (UserId não nulo)
            //    A query precisa ter um UserId e esse UserId deve bater com o da categoria.
            if (query.UserId.HasValue && category.UserId.HasValue && query.UserId == category.UserId)
            {
                return true;
            }

            // Se chegou aqui, a categoria existe, mas não é padrão nem pertence ao usuário da requisição.
            return false;
        }
    }
}
