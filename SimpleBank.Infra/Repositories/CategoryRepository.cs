using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SimpleBank.Domain.Entities;
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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException("DefaultConnection string not found.");
        }

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);

        public async Task AddAsync(Category category)
        {
            var sql = "INSERT INTO Categories (UserId, Name, Description ,Type, CreatedAt) VALUES (@UserId, @Name, @Description, @Type::text, @CreatedAt) RETURNING Id";
            using var db = Connection;
            // Para enums, o Dapper precisa de um mapeamento ou que você converta para string/int
            category.Id = await db.ExecuteScalarAsync<int>(sql, new { category.UserId, category.Name, category.Description, Type = category.Type.ToString(), category.CreatedAt });
        }

        public void Delete(Category category)
        {
            var sql = "DELETE FROM Categories WHERE Id = @Id";
            using var db = Connection;
            db.Execute(sql, new { category.Id });
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            var sql = "SELECT Id, UserId, Name, Description, Type, CreatedAt FROM Categories WHERE Id = @Id";
            using var db = Connection;
            // O Dapper pode mapear string para enum automaticamente se os nomes baterem
            return await db.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Category>> GetByUserIdAsync(int userId)
        {
            var sql = "SELECT Id, UserId, Name, Description, Type, CreatedAt FROM Categories WHERE UserId = @UserId";
            using var db = Connection;
            return await db.QueryAsync<Category>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<Category>> GetAllStandardCategoriesAsync()
        {
            var sql = "SELECT Id, UserId, Name, Description, Type, CreatedAt FROM Categories WHERE UserId IS NULL";
            using var db = Connection;
            return await db.QueryAsync<Category>(sql);
        }

        public void Update(Category category)
        {
            var sql = "UPDATE Categories SET UserId = @UserId, Name = @Name, Type = @Type::text WHERE Id = @Id";

           // using var db = Connection;

            using (var db = Connection)
            {
                db.Execute(
                    sql, 
                    new { category.Id, category.UserId, category.Name, Type = category.Type.ToString()
                 });
            }
        }

        public async Task<IEnumerable<Category>> GetByUserIdOrStandardAsync(int userId)
        {
            var sql = "SELECT * FROM Categories WHERE UserId = @UserId OR UserId IS NULL";
            using var db = Connection;
            return await db.QueryAsync<Category>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<Category>> GetStandardCategoriesAsync()
        {
            var sql = "SELECT * FROM Categories WHERE UserId IS NULL";
            using var db = Connection;
            return await db.QueryAsync<Category>(sql);
        }
    }
}
