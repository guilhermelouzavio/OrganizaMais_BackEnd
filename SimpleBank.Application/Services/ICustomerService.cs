using SimpleBank.Application.Dtos.Requests;
using SimpleBank.Application.Dtos.Responses;

namespace SimpleBank.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);
        Task<CustomerResponse> GetByIdAsync(string id);
        Task<IEnumerable<CustomerResponse>> GetAllAsync();
        Task<CustomerResponse> UpdateAsync(string id, UpdateCustomerRequest request);
        Task<bool> DeleteAsync(string id);
    }
}
