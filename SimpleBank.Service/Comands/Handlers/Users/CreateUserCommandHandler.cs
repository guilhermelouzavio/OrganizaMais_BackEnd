using MediatR;
using SimpleBank.Application.Comands.Users;
using SimpleBank.Application.Dtos;
using SimpleBank.Domain.Entities;
using SimpleBank.Domain.Interfaces;
using BCrypt.Net; // Adicione este using!

namespace SimpleBank.Application.Comands.Handlers.Users;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDTO>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Verificação se o e-mail já existe (Boa prática, pode ir para validador)
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("E-mail já cadastrado.");
        }

        // 1. Gerar o hash da senha usando BCrypt
        // BCrypt.HashPassword gera um salt aleatório e o incorpora no hash
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 2. Criar a entidade User com o hash da senha
        // Assumindo que sua entidade User tem uma propriedade para o hash (ex: PasswordHash)
        var user = new User(
            request.Name,
            request.Email,
            passwordHash // Salva o HASH, não a senha em texto puro!
        );

        try
        {
            var idCreated = await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync(); // Salva no banco de dados

            if (idCreated != 0)
            {
                return new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt
                };
            }
            else
                throw new InvalidOperationException("Houve um problema ao criar a conta.");
        }
        catch (Exception)
        {
            throw new InvalidOperationException("E-mail já cadastrado.");
        }
    }
}
