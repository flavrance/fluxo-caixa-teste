using MediatR;
using System;
using FluxoCaixa.Application.Core.Interfaces.Services;

namespace FluxoCaixa.Application.Core.Commands.CashFlow
{
    public class AddCreditCommand : IRequest<bool>
    {
        public Guid CashFlowId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class AddCreditCommandHandler : IRequestHandler<AddCreditCommand, bool>
    {
        private readonly ICashFlowService _cashFlowService;

        public AddCreditCommandHandler(ICashFlowService cashFlowService)
        {
            _cashFlowService = cashFlowService;
        }

        public async Task<bool> Handle(AddCreditCommand request, CancellationToken cancellationToken)
        {
            return await _cashFlowService.AddCreditAsync(request.CashFlowId, request.Amount, request.Description);
        }
    }
} 