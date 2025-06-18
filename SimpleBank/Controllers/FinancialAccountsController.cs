using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleBank.Application.Comands.FinancialAccounts;
using SimpleBank.Application.Dtos;
using SimpleBank.Application.Queries.FinancialAccounts;
using SimpleBank.Application.Queries.Handlers.FinancialAccounts;
using System.Net;

namespace SimpleBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialAccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FinancialAccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria uma nova conta financeira para um usuário.
        /// </summary>
        /// <param name="command">Dados da conta financeira a ser criada.</param>
        /// <returns>O DTO da conta financeira criada.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(FinancialAccountDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)] // Para usuário não encontrado
        public async Task<IActionResult> CreateFinancialAccount([FromBody] CreateFinancialAccountCommand command)
        {
            try
            {
                var accountDto = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetFinancialAccountById), new { id = accountDto.Id, userId = accountDto.UserId }, accountDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }));
            }
            catch (InvalidOperationException ex)
            {
                // Ex: Usuário não encontrado
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = "Ocorreu um erro interno no servidor.", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma conta financeira específica de um usuário.
        /// </summary>
        /// <param name="id">ID da conta financeira.</param>
        /// <param name="userId">ID do usuário proprietário da conta.</param>
        /// <returns>O DTO da conta financeira ou 404 Not Found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FinancialAccountDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetFinancialAccountById(int id, [FromQuery] int userId)
        {
            // Note: Em um sistema real, o userId viria do token JWT do usuário autenticado.
            // Aqui, estamos passando via query param para simplificar e focar na arquitetura.
            var accountDto = await _mediator.Send(new GetFinancialAccountByIdQuery { Id = id, UserId = userId });

            if (accountDto == null)
            {
                return NotFound(new { message = $"Conta financeira com ID {id} para o usuário {userId} não encontrada ou não pertence a ele." });
            }

            return Ok(accountDto);
        }

        /// <summary>
        /// Lista todas as contas financeiras de um usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <returns>Uma lista de DTOs de contas financeiras.</returns>
        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<FinancialAccountDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllFinancialAccountsByUserId(int userId)
        {
            // Note: Em um sistema real, o userId viria do token JWT do usuário autenticado.
            // Aqui, estamos passando via rota para simplificar e focar na arquitetura.
            var accounts = await _mediator.Send(new GetAllFinancialAccountsByUserIdQuery { UserId = userId });

            if (!accounts.Any())
            {
                // Se não encontrar nenhuma conta, pode retornar 200 OK com lista vazia ou 404
                // Depende da sua preferência. 200 OK com lista vazia é comum para listagens.
                return Ok(new List<FinancialAccountDTO>());
            }

            return Ok(accounts);
        }

        // TODO: Implementar UpdateFinancialAccount e DeleteFinancialAccount
    }
}
