namespace FluxoCaixa.WebApi.UseCases.GetCashFlowDetails
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;
    using FluxoCaixa.Application.Queries;
    using FluxoCaixa.WebApi.Model;
    using System.Collections.Generic;

    [Route("api/[controller]")]
    public sealed class CashFlowsController : Controller
    {
        private readonly ICashFlowsQueries cashFlowsQueries;

        public CashFlowsController(
            ICashFlowsQueries cashFlowsQueries)
        {
            this.cashFlowsQueries = cashFlowsQueries;
        }

        /// <summary>
        /// Get an cash flow balance
        /// </summary>
        [HttpGet("{cashFlowId}", Name = "GetCashFlow")]
        public async Task<IActionResult> Get(Guid cashFlowId)
        {
            var cashFlow = await cashFlowsQueries.GetCashFlow(cashFlowId);

            List<EntryModel> entries = new List<EntryModel>();

            foreach (var item in cashFlow.Entries)
            {
                var entry = new EntryModel(
                    item.Amount,
                    item.Description,
                    item.EntryDate);

                entries.Add(entry);
            }

            return new ObjectResult(new CashFlowDetailsModel(
                cashFlow.CashFlowId,
                cashFlow.CurrentBalance,
                entries));
        }
    }
}
