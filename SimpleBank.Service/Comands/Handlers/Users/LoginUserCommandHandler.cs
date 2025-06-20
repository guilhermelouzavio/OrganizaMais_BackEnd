using MediatR;
using SimpleBank.Application.Comands.Users;
using SimpleBank.Application.Dtos;
using SimpleBank.Application.Interfaces;
using SimpleBank.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Comands.Handlers.Users
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponseDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public LoginUserCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Buscar usuário pelo e-mail
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new InvalidOperationException("Credenciais inválidas.");
            }

            // 2. Comparar a senha fornecida com o hash salvo usando BCrypt.Verify
            // BCrypt.Net.BCrypt.Verify compara a senha em texto puro com o hash e o salt.
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) // Use PasswordHash!
            {
                throw new InvalidOperationException("Credenciais inválidas.");
            }

            // 3. Se as credenciais são válidas, gerar o token JWT
            var token = _tokenService.GenerateToken(user);

            return new AuthResponseDTO
            {
                Token = token,
                UserId = user.Id,
                UserName = user.Name,
                UserEmail = user.Email
            };
        }
    }
}
