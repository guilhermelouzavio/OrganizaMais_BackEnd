using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleBank.Application.Comands.Users;
using SimpleBank.Application.Dtos;
using System.Net;

namespace SimpleBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Realiza o login do usuário e retorna um token JWT para autenticação.
        /// </summary>
        /// <param name="command">E-mail e senha do usuário.</param>
        /// <returns>Um token JWT e dados do usuário.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)] // Para credenciais inválidas
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            try
            {
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }));
            }
            catch (InvalidOperationException ex)
            {
                // Captura a exceção de credenciais inválidas
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = "Ocorreu um erro interno no servidor.", details = ex.Message });
            }
        }
    }
}
