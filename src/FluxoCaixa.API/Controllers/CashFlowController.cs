using FluxoCaixa.Application.Core.DTOs;
using FluxoCaixa.Application.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FluxoCaixa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CashFlowController : ControllerBase
    {
        private readonly ICashFlowService _cashFlowService;
        private readonly ILogger<CashFlowController> _logger;

        public CashFlowController(ICashFlowService cashFlowService, ILogger<CashFlowController> logger)
        {
            _cashFlowService = cashFlowService;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Obter todos os fluxos de caixa", Description = "Retorna uma lista com todos os fluxos de caixa cadastrados")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Lista de fluxos de caixa obtida com sucesso", typeof(IEnumerable<CashFlowDto>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erro interno no servidor")]
        public async Task<ActionResult<IEnumerable<CashFlowDto>>> GetAll()
        {
            try
            {
                var result = await _cashFlowService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os fluxos de caixa");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação");
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obter fluxo de caixa por ID", Description = "Retorna um fluxo de caixa específico pelo seu ID")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Fluxo de caixa obtido com sucesso", typeof(CashFlowDto))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Fluxo de caixa não encontrado")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erro interno no servidor")]
        public async Task<ActionResult<CashFlowDto>> GetById(Guid id)
        {
            try
            {
                var result = await _cashFlowService.GetByIdAsync(id);
                if (result == null)
                    return NotFound($"Fluxo de caixa com ID {id} não encontrado");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter fluxo de caixa com ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação");
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Criar novo fluxo de caixa", Description = "Cria um novo fluxo de caixa com os dados fornecidos")]
        [SwaggerResponse((int)HttpStatusCode.Created, "Fluxo de caixa criado com sucesso", typeof(Guid))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Dados inválidos")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erro interno no servidor")]
        public async Task<ActionResult<Guid>> Create([FromBody] CashFlowCreateDto dto)
        {
            try
            {
                var id = await _cashFlowService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar fluxo de caixa");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação");
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualizar fluxo de caixa", Description = "Atualiza um fluxo de caixa existente com os dados fornecidos")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Fluxo de caixa atualizado com sucesso")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Fluxo de caixa não encontrado")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Dados inválidos")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erro interno no servidor")]
        public async Task<ActionResult> Update(Guid id, [FromBody] CashFlowUpdateDto dto)
        {
            try
            {
                var success = await _cashFlowService.UpdateAsync(id, dto);
                if (!success)
                    return NotFound($"Fluxo de caixa com ID {id} não encontrado");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar fluxo de caixa com ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação");
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Excluir fluxo de caixa", Description = "Exclui um fluxo de caixa existente pelo seu ID")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Fluxo de caixa excluído com sucesso")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Fluxo de caixa não encontrado")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erro interno no servidor")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _cashFlowService.DeleteAsync(id);
                if (!success)
                    return NotFound($"Fluxo de caixa com ID {id} não encontrado");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir fluxo de caixa com ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação");
            }
        }

        [HttpPost("{id}/credit")]
        [SwaggerOperation(Summary = "Adicionar crédito", Description = "Adiciona um crédito a um fluxo de caixa existente")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Crédito adicionado com sucesso")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Fluxo de caixa não encontrado")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Dados inválidos")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erro interno no servidor")]
        public async Task<ActionResult> AddCredit(Guid id, [FromBody] CreditDto dto)
        {
            try
            {
                var success = await _cashFlowService.AddCreditAsync(id, dto.Amount, dto.Description);
                if (!success)
                    return NotFound($"Fluxo de caixa com ID {id} não encontrado");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar crédito ao fluxo de caixa com ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação");
            }
        }

        [HttpPost("{id}/debit")]
        [SwaggerOperation(Summary = "Adicionar débito", Description = "Adiciona um débito a um fluxo de caixa existente")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Débito adicionado com sucesso")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Fluxo de caixa não encontrado")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Dados inválidos")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erro interno no servidor")]
        public async Task<ActionResult> AddDebit(Guid id, [FromBody] DebitDto dto)
        {
            try
            {
                var success = await _cashFlowService.AddDebitAsync(id, dto.Amount, dto.Description);
                if (!success)
                    return NotFound($"Fluxo de caixa com ID {id} não encontrado");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar débito ao fluxo de caixa com ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação");
            }
        }
    }
} 