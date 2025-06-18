using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SimpleBank.Domain.Entities;
using SimpleBank.Domain.Enums;
using SimpleBank.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Infra.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly string _connectionString;

        public TransactionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException("DefaultConnection string not found.");
        }

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);

        public async Task AddAsync(Transaction transaction)
        {
            var sql = "INSERT INTO Transactions (UserId, FinancialAccountId, CategoryId, Description, Value, Type, TransactionDate, CreatedAt) VALUES (@UserId, @FinancialAccountId, @CategoryId, @Description, @Value, @Type::text, @TransactionDate, @CreatedAt) RETURNING Id";
            using var db = Connection;

            transaction.Id = await db.ExecuteScalarAsync<int>(sql, new
            {
                transaction.UserId,
                transaction.FinancialAccountId,
                transaction.CategoryId,
                transaction.Description,
                transaction.Value,
                Type = transaction.Type.ToString(), // Converte enum para string
                transaction.TransactionDate,
                transaction.CreatedAt
            });
        }

        public void Delete(Transaction transaction)
        {
            var sql = "DELETE FROM Transactions WHERE Id = @Id";
            using var db = Connection;
            db.Execute(sql, new { transaction.Id });
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            var sql = "SELECT Id, UserId, FinancialAccountId, CategoryId, Description, Value, Type, TransactionDate, CreatedAt FROM Transactions WHERE Id = @Id";
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<Transaction>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Transaction>> GetByFinancialAccountIdAsync(int financialAccountId)
        {
            var sql = "SELECT Id, UserId, FinancialAccountId, CategoryId, Description, Value, Type, TransactionDate, CreatedAt FROM Transactions WHERE FinancialAccountId = @FinancialAccountId";
            using var db = Connection;
            return await db.QueryAsync<Transaction>(sql, new { FinancialAccountId = financialAccountId });
        }

        public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId)
        {
            var sql = "SELECT Id, UserId, FinancialAccountId, CategoryId, Description, Value, Type, TransactionDate, CreatedAt FROM Transactions WHERE UserId = @UserId";
            using var db = Connection;
            return await db.QueryAsync<Transaction>(sql, new { UserId = userId });
        }

        public void Update(Transaction transaction)
        {
            var sql = "UPDATE Transactions SET UserId = @UserId, FinancialAccountId = @FinancialAccountId, CategoryId = @CategoryId, Description = @Description, Value = @Value, Type = @Type::text, TransactionDate = @TransactionDate WHERE Id = @Id";
            using var db = Connection;
            db.Execute(sql, new
            {
                transaction.Id,
                transaction.UserId,
                transaction.FinancialAccountId,
                transaction.CategoryId,
                transaction.Description,
                transaction.Value,
                Type = transaction.Type.ToString(), // Converte enum para string
                transaction.TransactionDate
            });
        }

        public async Task<IEnumerable<Transaction>> GetByFinancialAccountIdAsync(int financialAccountId, TransactionType? type = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var sql = "SELECT * FROM Transactions WHERE FinancialAccountId = @FinancialAccountId";
            var parameters = new DynamicParameters();
            parameters.Add("FinancialAccountId", financialAccountId);

            if (type.HasValue)
            {
                sql += " AND Type = @Type";
                parameters.Add("Type", type.Value.ToString());
            }
            if (startDate.HasValue)
            {
                sql += " AND TransactionDate >= @StartDate";
                parameters.Add("StartDate", startDate.Value);
            }
            if (endDate.HasValue)
            {
                sql += " AND TransactionDate <= @EndDate";
                parameters.Add("EndDate", endDate.Value);
            }
            sql += " ORDER BY TransactionDate DESC"; // Ou como preferir

            using var db = Connection;
            return await db.QueryAsync<Transaction>(sql, parameters);
        }

        public async Task<IEnumerable<(Transaction transaction, string categoryName)>> GetTransactionsForReportAsync(int userId, DateTime startDate, DateTime endDate, TransactionType? type = null)
        {
            var sql = @"
                SELECT
                    t.Id, t.UserId, t.FinancialAccountId, t.CategoryId, t.Description,
                    t.Value, t.Type, t.TransactionDate, t.CreatedAt,
                    c.Name as CategoryName -- Alias para o nome da categoria
                FROM Transactions t
                JOIN Categories c ON t.CategoryId = c.Id
                WHERE t.UserId = @UserId
                  AND t.TransactionDate BETWEEN @StartDate AND @EndDate";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("StartDate", startDate.Date); // Usar .Date para ignorar a hora
            parameters.Add("EndDate", endDate.Date); // Usar .Date para ignorar a hora

            if (type.HasValue)
            {
                sql += " AND t.Type = @Type";
                parameters.Add("Type", type.Value.ToString());
            }

            // Dapper pode mapear para múltiplos objetos ou para um tipo anônimo/tupla
            // O mapeamento funciona se as colunas forem nomeadas corretamente no SQL e no mapeamento
            // Neste caso, 'splitOn: "CategoryName"' indica onde o Dapper deve "dividir" o mapeamento
            // para o próximo tipo na tupla.
            using var db = Connection;
            return await db.QueryAsync<Transaction, string, (Transaction transaction, string categoryName)>(
                sql,
                (transaction, categoryName) => (transaction, categoryName), // Mapeia para a tupla
                parameters,
                splitOn: "CategoryName" // Coluna onde começa o mapeamento do segundo tipo (string)
            );
        }
    }
}
