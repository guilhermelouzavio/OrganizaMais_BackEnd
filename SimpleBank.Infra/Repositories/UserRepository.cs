using Dapper;
using Npgsql;
using SimpleBank.Domain.Entities;
using SimpleBank.Domain.Interfaces.Repositories;
using SimpleBank.Domain.Specifications;
using Microsoft.Extensions.Configuration; // Pra pegar a string de conexão
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SimpleBank.Infra.Specifications;

namespace SimpleBank.Infra.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        // Injetamos IConfiguration para pegar a string de conexão
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException("DefaultConnection string not found.");
        }

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);

        public async Task<long> AddAsync(User user)
        {
            var sql = "INSERT INTO Users (Name, Email, PasswordHash, CreatedAt) VALUES (@Name, @Email, @PasswordHash, @CreatedAt) RETURNING Id";
            using var db = Connection;
            return await db.ExecuteScalarAsync<int>(sql, user);
        }

        public void Delete(User user)
        {
            // O Dapper não tem um método "Delete" direto para um objeto complexo.
            // A gente executa um SQL DELETE.
            var sql = "DELETE FROM Users WHERE Id = @Id";
            using var db = Connection;
            db.Execute(sql, new { user.Id }); // Executa a query de forma síncrona, ou ExecuteAsync
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var sql = "SELECT Id, Name, Email, PasswordHash, CreatedAt FROM Users";
            using var db = Connection;
            return await db.QueryAsync<User>(sql);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var sql = "SELECT Id, Name, Email, PasswordHash, CreatedAt FROM Users WHERE Id = @Id";
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            // Usando a Specification aqui para "demonstrar" a ideia,
            // mas para um caso simples como este, SQL direto é mais comum.
            var spec = new UserByEmailSpecification(email);
            var (whereClause, parameters) = SpecificationEvaluator.GetWhereClause(spec);

            var sql = $"SELECT Id, Name, Email, PasswordHash, CreatedAt FROM Users WHERE {whereClause}";
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<User>(sql, parameters);
        }

        public void Update(User user)
        {
            var sql = "UPDATE Users SET Name = @Name, Email = @Email, PasswordHash = @PasswordHash WHERE Id = @Id";
            using var db = Connection;
            db.Execute(sql, user);
        }
    }
}
