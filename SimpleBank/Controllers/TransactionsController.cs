using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleBank.Application.Comands.Transactions;
using SimpleBank.Application.Dtos;
using SimpleBank.Application.Queries.Transactions;
using SimpleBank.Domain.Enums;
using System.Net;

namespace SimpleBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria uma nova transação (receita ou despesa) em uma conta financeira.
        /// </summary>
        /// <param name="command">Dados da transação a ser criada.</param>
        /// <returns>O DTO da transação criada.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TransactionDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)] // Para entidades relacionadas não encontradas
        [ProducesResponseType((int)HttpStatusCode.Conflict)] // Para erro de tipo de categoria incompatível
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand command)
        {
            try
            {
                var transactionDto = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetTransactionById), new { id = transactionDto.Id, userId = transactionDto.UserId }, transactionDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }));
            }
            catch (InvalidOperationException ex)
            {
                // Ex: Usuário, conta ou categoria não encontrada, ou tipo incompatível
                return NotFound(new { error = ex.Message }); // Ou Conflict para tipo incompatível
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = "Ocorreu um erro interno no servidor.", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma transação pelo seu ID para um usuário específico.
        /// </summary>
        /// <param name="id">ID da transação.</param>
        /// <param name="userId">ID do usuário proprietário da transação.</param>
        /// <returns>O DTO da transação ou 404 Not Found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetTransactionById(int id, [FromQuery] int userId)
        {
            var transactionDto = await _mediator.Send(new GetTransactionByIdQuery { Id = id, UserId = userId });

            if (transactionDto == null)
            {
                return NotFound(new { message = $"Transação com ID {id} não encontrada para o usuário {userId} ou não pertence a ele." });
            }

            return Ok(transactionDto);
        }

        /// <summary>
        /// Lista as transações de uma conta financeira, com filtros opcionais.
        /// </summary>
        /// <param name="financialAccountId">ID da conta financeira.</param>
        /// <param name="userId">ID do usuário proprietário da conta.</param>
        /// <param name="type">Tipo opcional da transação (Income/Expense).</param>
        /// <param name="startDate">Data inicial opcional para filtro.</param>
        /// <param name="endDate">Data final opcional para filtro.</param>
        /// <returns>Uma lista de DTOs de transações.</returns>
        [HttpGet("by-account/{financialAccountId}")]
        [ProducesResponseType(typeof(IEnumerable<TransactionDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)] // Se a conta não for encontrada ou não pertencer ao usuário
        public async Task<IActionResult> GetAllTransactionsByFinancialAccountId(
            int financialAccountId,
            [FromQuery] int userId,
            [FromQuery] TransactionType? type = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = new GetAllTransactionsByFinancialAccountIdQuery
            {
                UserId = userId,
                FinancialAccountId = financialAccountId,
                Type = type,
                StartDate = startDate,
                EndDate = endDate
            };
            var transactions = await _mediator.Send(query);

            if (!transactions.Any())
            {
                // Pode retornar 404 se a conta não existe para o usuário ou 200 OK com lista vazia se não há transações
                // Para este endpoint, 200 OK com lista vazia é mais apropriado se a conta existe mas não tem transações.
                // O handler já verifica a posse da conta.
                return Ok(new List<TransactionDTO>());
            }

            return Ok(transactions);
        }

        /// <summary>
        /// Gera um relatório sumarizado de transações por período para um usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <param name="startDate">Data de início do relatório.</param>
        /// <param name="endDate">Data de fim do relatório.</param>
        /// <param name="type">Tipo opcional da transação (Income/Expense) para filtro.</param>
        /// <returns>Um relatório sumarizado de transações.</returns>
        [HttpGet("report")]
        [ProducesResponseType(typeof(IEnumerable<TransactionReportItemDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTransactionsReport(
            [FromQuery] int userId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] TransactionType? type = null)
        {
            var query = new GetTransactionsReportQuery
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                Type = type
            };
            var report = await _mediator.Send(query);
            return Ok(report);
        }

        // TODO: Implementar UpdateTransaction e DeleteTransaction
    }
}
