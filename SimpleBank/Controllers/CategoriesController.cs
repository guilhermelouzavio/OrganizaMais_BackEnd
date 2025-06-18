using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleBank.Application.Comands.Categories;
using SimpleBank.Application.Dtos;
using SimpleBank.Application.Queries.Categories;
using SimpleBank.Domain.Enums;
using System.Net;

namespace SimpleBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria uma nova categoria (pode ser padrão do sistema ou personalizada por usuário).
        /// </summary>
        /// <param name="command">Dados da categoria a ser criada.</param>
        /// <returns>O DTO da categoria criada.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)] // Para usuário não encontrado
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            try
            {
                var categoryDto = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetCategoryById), new { id = categoryDto.Id }, categoryDto);
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
        /// Obtém uma categoria pelo seu ID, considerando categorias padrão ou personalizadas do usuário.
        /// </summary>
        /// <param name="id">ID da categoria.</param>
        /// <param name="userId">ID opcional do usuário para buscar categorias personalizadas.</param>
        /// <returns>O DTO da categoria ou 404 Not Found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetCategoryById(int id, [FromQuery] int? userId = null)
        {
            var categoryDto = await _mediator.Send(new GetCategoryByIdQuery { Id = id, UserId = userId });

            if (categoryDto == null)
            {
                return NotFound(new { message = $"Categoria com ID {id} não encontrada para o usuário especificado ou como padrão." });
            }

            return Ok(categoryDto);
        }

        /// <summary>
        /// Lista todas as categorias disponíveis para um usuário (padrão e personalizadas) ou apenas as padrão.
        /// </summary>
        /// <param name="userId">ID opcional do usuário para buscar categorias personalizadas.</param>
        /// <param name="type">Tipo opcional da transação (Income/Expense) para filtrar.</param>
        /// <returns>Uma lista de DTOs de categorias.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllCategories([FromQuery] int? userId = null, [FromQuery] TransactionType? type = null)
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery { UserId = userId, Type = type });
            return Ok(categories);
        }

        // TODO: Implementar UpdateCategory e DeleteCategory
    }
}
