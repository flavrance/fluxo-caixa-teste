namespace FluxoCaixa.WebApi.UseCases.Report
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;
    using FluxoCaixa.Application.Queries;
    using FluxoCaixa.WebApi.Model;
    using System.Collections.Generic;

    [Route("api/[controller]")]
    public sealed class ReportController : Controller
    {
        private readonly ICashFlowsQueries cashFlowsQueries;

        public ReportController(
            ICashFlowsQueries cashFlowsQueries)
        {
            this.cashFlowsQueries = cashFlowsQueries;
        }

        /// <summary>
        /// Get an cash flow balance by date
        /// </summary>
        [HttpGet("{cashFlowId}", Name = "GeBalancedEntriesByDate")]
        public async Task<IActionResult> Get(Guid cashFlowId)
        {
            var cashFlow = await cashFlowsQueries.GetCashFlow(cashFlowId);

            List<EntryModel> entries = new List<EntryModel>();

            foreach (var item in cashFlow.Report)
            {
                var entry = new EntryModel(
                    item.Amount,
                    item.Description,
                    item.EntryDate);

                entries.Add(entry);
            }

            return new ObjectResult(new CashFlowDetailsModel(
                cashFlow.CashFlowId,
                cashFlow.Year,
                cashFlow.CurrentBalance,
                entries));
        }
    }
}
