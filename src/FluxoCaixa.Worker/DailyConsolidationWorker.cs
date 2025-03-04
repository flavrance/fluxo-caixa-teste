using FluxoCaixa.Application.Core.Interfaces.Services;
using FluxoCaixa.Infrastructure.Data.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using System.Text.Json;

namespace FluxoCaixa.Worker
{
    public class DailyConsolidationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMessageService _messageService;
        private readonly ILogger<DailyConsolidationWorker> _logger;
        private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
        private const string QueueName = "daily-consolidation";

        public DailyConsolidationWorker(
            IServiceScopeFactory serviceScopeFactory,
            IMessageService messageService,
            ILogger<DailyConsolidationWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Configurar o Circuit Breaker
            _circuitBreaker = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromMinutes(1),
                    onBreak: (ex, breakDelay) =>
                    {
                        _logger.LogWarning(ex, "Circuit Breaker aberto por {BreakDelay}. Processamento de consolidados pausado.", breakDelay);
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("Circuit Breaker fechado. Processamento de consolidados retomado.");
                    },
                    onHalfOpen: () =>
                    {
                        _logger.LogInformation("Circuit Breaker em estado semi-aberto. Testando processamento de consolidados.");
                    });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de processamento de consolidado diário iniciado");

            // Configurar a fila para receber mensagens
            _messageService.SubscribeToQueue<ConsolidationMessage>(QueueName, async message =>
            {
                try
                {
                    await ProcessConsolidationMessage(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem de consolidação");
                }
            });

            // Processar o consolidado do dia anterior ao iniciar
            await ProcessPreviousDayConsolidation(stoppingToken);

            // Agendar o processamento diário
            using var timer = new PeriodicTimer(TimeSpan.FromHours(24));
            
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ProcessDailyConsolidation(stoppingToken);
            }
        }

        private async Task ProcessPreviousDayConsolidation(CancellationToken cancellationToken)
        {
            var yesterday = DateTime.Today.AddDays(-1);
            _logger.LogInformation("Processando consolidado do dia anterior: {Date}", yesterday);
            
            await ProcessConsolidationForDate(yesterday, cancellationToken);
        }

        private async Task ProcessDailyConsolidation(CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            _logger.LogInformation("Processando consolidado diário: {Date}", today);
            
            await ProcessConsolidationForDate(today, cancellationToken);
        }

        private async Task ProcessConsolidationForDate(DateTime date, CancellationToken cancellationToken)
        {
            try
            {
                // Usar Circuit Breaker para evitar falhas em cascata
                await _circuitBreaker.ExecuteAsync(async () =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();
                        await reportService.ProcessDailyConsolidationAsync(date);
                        _logger.LogInformation("Consolidado diário processado com sucesso para a data {Date}", date);
                    }
                });
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(ex, "Circuit Breaker aberto. Não é possível processar o consolidado para a data {Date}", date);
                
                // Publicar mensagem para reprocessamento posterior
                PublishForReprocessing(date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar consolidado diário para a data {Date}", date);
                
                // Publicar mensagem para reprocessamento posterior
                PublishForReprocessing(date);
            }
        }

        private async Task ProcessConsolidationMessage(ConsolidationMessage message)
        {
            _logger.LogInformation("Processando mensagem de consolidação para a data {Date}", message.Date);
            
            try
            {
                // Usar Circuit Breaker para evitar falhas em cascata
                await _circuitBreaker.ExecuteAsync(async () =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();
                        await reportService.ProcessDailyConsolidationAsync(message.Date);
                        _logger.LogInformation("Consolidado diário processado com sucesso para a data {Date} a partir da mensagem", message.Date);
                    }
                });
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(ex, "Circuit Breaker aberto. Não é possível processar o consolidado para a data {Date} a partir da mensagem", message.Date);
                
                // Se o número de tentativas for menor que o máximo, publicar para reprocessamento
                if (message.RetryCount < 3)
                {
                    PublishForReprocessing(message.Date, message.RetryCount + 1);
                }
                else
                {
                    _logger.LogError("Número máximo de tentativas excedido para a data {Date}. Consolidado não processado.", message.Date);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar consolidado diário para a data {Date} a partir da mensagem", message.Date);
                
                // Se o número de tentativas for menor que o máximo, publicar para reprocessamento
                if (message.RetryCount < 3)
                {
                    PublishForReprocessing(message.Date, message.RetryCount + 1);
                }
                else
                {
                    _logger.LogError("Número máximo de tentativas excedido para a data {Date}. Consolidado não processado.", message.Date);
                }
            }
        }

        private void PublishForReprocessing(DateTime date, int retryCount = 0)
        {
            var message = new ConsolidationMessage
            {
                Date = date,
                RetryCount = retryCount
            };
            
            _messageService.PublishMessage(QueueName, message);
            _logger.LogInformation("Mensagem publicada para reprocessamento do consolidado da data {Date}. Tentativa: {RetryCount}", date, retryCount);
        }
    }

    public class ConsolidationMessage
    {
        public DateTime Date { get; set; }
        public int RetryCount { get; set; }
    }
} 