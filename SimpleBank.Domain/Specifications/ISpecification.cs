using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Domain.Specifications
{
    public interface ISpecification<T> where T : class
    {
        // Critério da especificação: uma expressão LINQ que será usada para filtrar.
        Expression<Func<T, bool>> Criteria { get; }

        // Inclui as propriedades de navegação que devem ser carregadas (para eager loading no EF Core).
        // Isso é uma lista de expressões para otimizar as consultas.
        List<Expression<Func<T, object>>> Includes { get; }

        // Critérios de ordenação (OrderBy)
        Expression<Func<T, object>>? OrderBy { get; }
        Expression<Func<T, object>>? OrderByDescending { get; }

        // Critérios para paginação
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }
}
