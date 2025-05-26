using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Behaviors
{
    // Este Behavior vai interceptar todas as requisições (Commands ou Queries)
    // e executar os validadores FluentValidation associados a elas.
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Se houver validadores para esta requisição...
            if (_validators.Any())
            {
                // Executa todos os validadores encontrados para a requisição.
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                // Coleta todas as falhas de validação.
                var failures = validationResults
                    .Where(r => r.Errors.Any())
                    .SelectMany(r => r.Errors)
                    .ToList();

                // Se houver falhas, lança uma exceção.
                if (failures.Any())
                {
                    // Lançar ValidationException é o padrão do FluentValidation
                    throw new ValidationException(failures);
                }
            }

            // Se não houver validadores ou se todas as validações passarem,
            // passa a requisição para o próximo item na cadeia (ou para o Handler final).
            return await next();
        }
    }
}
