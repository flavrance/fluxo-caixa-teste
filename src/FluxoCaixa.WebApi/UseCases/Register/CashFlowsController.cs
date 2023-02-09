namespace FluxoCaixa.WebApi.UseCases.Register
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using FluxoCaixa.Application.Commands.Register;
    using FluxoCaixa.WebApi.Model;
    using System.Collections.Generic;

    [Route("api/[controller]")]
    public sealed class CashFlowsController : Controller
    {
        private readonly IRegisterUseCase registerService;

        public CashFlowsController(IRegisterUseCase registerService)
        {
            this.registerService = registerService;
        }

        /// <summary>
        /// Register a new CashFlow
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RegisterRequest request)
        {
            RegisterResult result = await registerService.Execute(
                request.Year, request.InitialAmount);

            List<EntryModel> entries = new List<EntryModel>();

            foreach (var item in result.CashFlow.Entries)
            {
                var transaction = new EntryModel(
                    item.Amount,
                    item.Description,
                    item.EntryDate);

                entries.Add(transaction);
            }

            CashFlowDetailsModel cashFlow = new CashFlowDetailsModel(
                result.CashFlow.CashFlowId,
                result.CashFlow.Year,
                result.CashFlow.CurrentBalance,
                entries);           

            

            return CreatedAtRoute("GetCashFlow", new { cashFlowId = cashFlow.CashFlowId }, cashFlow);
        }
    }
}
