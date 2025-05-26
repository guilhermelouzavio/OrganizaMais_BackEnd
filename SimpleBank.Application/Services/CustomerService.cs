
using SimpleBank.Application.Dtos.Requests;
using SimpleBank.Application.Dtos.Responses;
using SimpleBank.Application.Services.Interfaces;
using SimpleBank.Domain.Entities;
using SimpleBank.Domain.Interfaces.Repositories;

namespace SimpleBank.Appilcation.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CustomerResponse>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CustomerResponse> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerResponse> UpdateAsync(string id, UpdateCustomerRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
