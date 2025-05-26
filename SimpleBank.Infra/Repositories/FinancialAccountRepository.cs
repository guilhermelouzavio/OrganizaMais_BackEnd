using Dapper;
using Npgsql;
using SimpleBank.Domain.Entities;
using SimpleBank.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SimpleBank.Infra.Repositories
{
    public class FinancialAccountRepository : IFinancialAccountRepository
    {
        private readonly string _connectionString;

        public FinancialAccountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException("DefaultConnection string not found.");
        }

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);

        public async Task AddAsync(FinancialAccount account)
        {
            var sql = "INSERT INTO FinancialAccounts (UserId, Name, Balance, CreatedAt) VALUES (@UserId, @Name, @Balance, @CreatedAt) RETURNING Id";
            using var db = Connection;
            account.Id = await db.ExecuteScalarAsync<int>(sql, account);
        }

        public void Delete(FinancialAccount account)
        {
            var sql = "DELETE FROM FinancialAccounts WHERE Id = @Id";
            using var db = Connection;
            db.Execute(sql, new { account.Id });
        }

        public async Task<FinancialAccount?> GetByIdAsync(int id)
        {
            var sql = "SELECT Id, UserId, Name, Balance, CreatedAt FROM FinancialAccounts WHERE Id = @Id";
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<FinancialAccount>(sql, new { Id = id });
        }

        public async Task<IEnumerable<FinancialAccount>> GetByUserIdAsync(int userId)
        {
            var sql = "SELECT Id, UserId, Name, Balance, CreatedAt FROM FinancialAccounts WHERE UserId = @UserId";
            using var db = Connection;
            return await db.QueryAsync<FinancialAccount>(sql, new { UserId = userId });
        }

        public void Update(FinancialAccount account)
        {
            var sql = "UPDATE FinancialAccounts SET UserId = @UserId, Name = @Name, Balance = @Balance WHERE Id = @Id";
            using var db = Connection;
            db.Execute(sql, account);
        }
    }
}
