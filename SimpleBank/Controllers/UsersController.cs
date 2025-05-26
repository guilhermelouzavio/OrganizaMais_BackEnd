using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleBank.Application.Comands;
using SimpleBank.Application.Dtos;
using SimpleBank.Application.Queries;
using System.Net;
using FluentValidation; // Para capturar a ValidationException
using System.Net; // Para HttpStatusCode

namespace SimpleBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        // O IMediator é injetado automaticamente pelo sistema de DI (configurado no Program.cs).
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria um novo usuário no sistema.
        /// </summary>
        /// <param name="command">Dados do usuário a ser criado (Nome, Email, Senha).</param>
        /// <returns>O DTO do usuário criado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.Created)] // 201 Created
        [ProducesResponseType((int)HttpStatusCode.BadRequest)] // 400 Bad Request
        [ProducesResponseType((int)HttpStatusCode.Conflict)] // 409 Conflict (para email já existente)
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            try
            {
                // Envia o comando para o MediatR. O MediatR vai encontrar o Handler e o ValidationBehavior.
                var userDto = await _mediator.Send(command);
                // Retorna 201 Created com a URL para o novo recurso e o DTO.
                return CreatedAtAction(nameof(GetUserById), new { id = userDto.Id }, userDto);
            }
            catch (ValidationException ex)
            {
                // Captura exceções de validação do FluentValidation (que o ValidationBehavior lança).
                // Retorna 400 Bad Request com os erros de validação.
                return BadRequest(ex.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }));
            }
            catch (InvalidOperationException ex)
            {
                // Captura exceções de lógica de negócio (ex: e-mail já existente).
                // Retorna 409 Conflict para indicar que o recurso já existe ou há um conflito.
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Captura outras exceções inesperadas.
                // Retorna 500 Internal Server Error.
                // Em um ambiente de produção, você logaria isso.
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = "Ocorreu um erro interno no servidor.", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um usuário pelo seu ID.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>O DTO do usuário ou 404 Not Found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)] // 200 OK
        [ProducesResponseType((int)HttpStatusCode.NotFound)] // 404 Not Found
        public async Task<IActionResult> GetUserById(int id)
        {
            // Envia a query para o MediatR.
            var userDto = await _mediator.Send(new GetUserByIdQuery { Id = id });

            if (userDto == null)
            {
                // Se o usuário não for encontrado, retorna 404 Not Found.
                return NotFound(new { message = $"Usuário com ID {id} não encontrado." });
            }

            // Retorna 200 OK com o DTO do usuário.
            return Ok(userDto);
        }

        // TODO: Implementar UpdateUser e DeleteUser como exercícios.
        // A lógica seria similar, enviando UpdateUserCommand e DeleteUserCommand.

        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command) { ... }

        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteUser(int id) { ... }
    }
}
