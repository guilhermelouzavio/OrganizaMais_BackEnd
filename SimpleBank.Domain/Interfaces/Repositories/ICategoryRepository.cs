using SimpleBank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Domain.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetByUserIdAsync(int userId); // Para categorias customizadas
        Task<IEnumerable<Category>> GetAllStandardCategoriesAsync(); // Para categorias padrão do sistema
        Task AddAsync(Category category);
        void Update(Category category);
        void Delete(Category category);
    }
}
