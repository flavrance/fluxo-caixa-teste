using FluxoCaixa.Application.Core.DTOs;
using FluxoCaixa.Application.Core.Interfaces.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluxoCaixa.Application.Core.Queries.CashFlow
{
    public class GetCashFlowByIdQuery : IRequest<CashFlowDto?>
    {
        public Guid Id { get; set; }

        public GetCashFlowByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetCashFlowByIdQueryHandler : IRequestHandler<GetCashFlowByIdQuery, CashFlowDto?>
    {
        private readonly ICashFlowService _cashFlowService;

        public GetCashFlowByIdQueryHandler(ICashFlowService cashFlowService)
        {
            _cashFlowService = cashFlowService;
        }

        public async Task<CashFlowDto?> Handle(GetCashFlowByIdQuery request, CancellationToken cancellationToken)
        {
            return await _cashFlowService.GetByIdAsync(request.Id);
        }
    }
} 