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
    [Produces("application/json")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("daily/{date}")]
        [SwaggerOperation(Summary = "Obter relatórios por data", Description = "Retorna todos os relatórios para uma data específica")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Relatórios obtidos com sucesso", typeof(IEnumerable<ReportDTO>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Data inválida")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erro interno no servidor")]
        public async Task<ActionResult<IEnumerable<ReportDTO>>> GetByDate(DateTime date)
        {
            try
            {
                var reports = await _reportService.GetReportsByDateAsync(date);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter relatórios para a data {Date}", date);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação");
            }
        }

        [HttpGet("consolidated/{date}")]
        [SwaggerOperation(Summary = "Obter relatório consolidado", Description = "Retorna o relatório consolidado para uma data específica")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Relatório consolidado obtido com sucesso", typeof(ConsolidatedReportDto))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Data inválida")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erro interno no servidor")]
        public async Task<ActionResult<ConsolidatedReportDto>> GetConsolidatedReport(DateTime date)
        {
            try
            {
                var report = await _reportService.GenerateConsolidatedReportAsync(date);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter relatório consolidado para a data {Date}", date);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação");
            }
        }

        [HttpGet("period")]
        [SwaggerOperation(Summary = "Obter relatório por período", Description = "Retorna o relatório consolidado para um período específico")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Relatório de período obtido com sucesso", typeof(ConsolidatedReportDto))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Datas inválidas")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erro interno no servidor")]
        public async Task<ActionResult<ConsolidatedReportDto>> GetPeriodReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest("A data inicial deve ser menor ou igual à data final");
                }

                var report = await _reportService.GeneratePeriodReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter relatório de período de {StartDate} a {EndDate}", startDate, endDate);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação");
            }
        }

        [HttpPost("process-daily/{date}")]
        [SwaggerOperation(Summary = "Processar consolidado diário", Description = "Processa o consolidado diário para uma data específica")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Consolidado diário processado com sucesso")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Data inválida")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erro interno no servidor")]
        public async Task<ActionResult> ProcessDailyConsolidation(DateTime date)
        {
            try
            {
                await _reportService.ProcessDailyConsolidationAsync(date);
                return Ok("Consolidado diário processado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar consolidado diário para a data {Date}", date);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação");
            }
        }
    }
} 