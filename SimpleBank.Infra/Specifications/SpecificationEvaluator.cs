using SimpleBank.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Infra.Specifications
{
    // Esta classe é responsável por converter uma ISpecification em uma string SQL WHERE clause.
    // IMPORTANTE: Esta é uma implementação SIMPLIFICADA para fins didáticos.
    // Em um projeto real, você usaria uma biblioteca de construção de SQL mais robusta
    // ou um gerador de expressões LINQ para SQL mais complexo.
    public static class SpecificationEvaluator
    {
        public static (string WhereClause, object Parameters) GetWhereClause<T>(ISpecification<T> spec) where T : class
        {
            if (spec.Criteria == null)
            {
                return (string.Empty, new { });
            }

            // A forma mais simples de "traduzir" a expressão é usando os membros diretos.
            // Isso funciona para comparações simples (==, !=, >, <) e propriedades diretas.
            // Expressões mais complexas (Contains, StartsWith, etc.) ou com métodos
            // precisariam de um parser de expressão LINQ mais avançado.

            var body = spec.Criteria.Body;
            var parameters = new Dictionary<string, object>();
            var whereClause = new StringBuilder();

            // Vamos simplificar o tratamento de expressões para este exemplo.
            // Para casos complexos, seria necessário um ExpressionVisitor.
            // Aqui, assumimos que as expressões são do tipo BinaryExpression (comparação)
            // ou MemberExpression (acesso a propriedade).

            ProcessExpression(body, whereClause, parameters);

            return (whereClause.ToString(), parameters);
        }

        private static void ProcessExpression(Expression expression, StringBuilder whereClause, Dictionary<string, object> parameters)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                ProcessExpression(binaryExpression.Left, whereClause, parameters);
                whereClause.Append($" {GetSqlOperator(binaryExpression.NodeType)} ");
                ProcessExpression(binaryExpression.Right, whereClause, parameters);
            }
            else if (expression is MemberExpression memberExpression)
            {
                // Se for uma constante ou fechamento (closure) que acessa um valor,
                // avalia o valor para adicionar como parâmetro.
                if (memberExpression.Expression is ConstantExpression || memberExpression.Expression is MemberExpression)
                {
                    var objectMember = Expression.Convert(memberExpression, typeof(object));
                    var getter = Expression.Lambda<Func<object>>(objectMember).Compile();
                    var value = getter();
                    var paramName = $"p{parameters.Count}";
                    parameters.Add(paramName, value);
                    whereClause.Append($"@{paramName}");
                }
                else // Assume que é uma coluna no banco de dados
                {
                    whereClause.Append(memberExpression.Member.Name);
                }
            }
            else if (expression is ConstantExpression constantExpression)
            {
                var paramName = $"p{parameters.Count}";
                parameters.Add(paramName, constantExpression.Value);
                whereClause.Append($"@{paramName}");
            }
            else
            {
                // Para expressões mais complexas, você precisaria de um ExpressionVisitor
                // que percorra a árvore de expressão e gere o SQL correspondente.
                throw new NotSupportedException($"Expression type {expression.GetType().Name} is not supported yet for SQL generation. " +
                                                 "This SpecificationEvaluator is a simplified example.");
            }
        }

        private static string GetSqlOperator(ExpressionType type)
        {
            return type switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "!=",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                _ => throw new NotSupportedException($"Operator {type} is not supported.")
            };
        }
    }
}
