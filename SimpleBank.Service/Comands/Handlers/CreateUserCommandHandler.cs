using MediatR;
using SimpleBank.Application.Dtos;
using SimpleBank.Domain.Entities;
using SimpleBank.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDTO>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Lógica de Validação (poderia estar em um Behavior ou Validador separado)
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Name, Email, and Password cannot be empty.");
            }

            // 2. Hash da Senha (IMPORTANTE: Use uma biblioteca mais robusta como BCrypt.Net em produção!)
            // Este é APENAS UM EXEMPLO SIMPLES E INSEGURO para hash.
            // Para produção, instale o pacote BCrypt.Net.Core e use:
            // string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
                request.Password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
            }

            // 3. Verificar se o e-mail já existe (usando repositório e talvez uma Specification)
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with email '{request.Email}' already exists.");
            }

            // 4. Criar a entidade de domínio
            var user = new User(request.Name, request.Email, request.Password);

            // 5. Adicionar ao repositório
            await _unitOfWork.Users.AddAsync(user);

            // 6. Persistir as mudanças (com Dapper, geralmente cada operação já é salva,
            // mas o UoW mantém a abstração e seria usado para transações complexas).
            await _unitOfWork.CompleteAsync();

            // 7. Retornar um DTO
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
