namespace FluxoCaixa.WebApi.UseCases.Debit
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using FluxoCaixa.Application.Commands.Debit;

    [Route("api/[controller]")]
    public sealed class CashFlowsController : Controller
    {
        private readonly IDebitUseCase debitService;

        public CashFlowsController(IDebitUseCase debitService)
        {
            this.debitService = debitService;
        }

        /// <summary>
        /// Debit from an cash flow
        /// </summary>
        [HttpPatch("Debit")]
        public async Task<IActionResult> Debit([FromBody]DebitRequest request)
        {
            DebitResult depositResult = await debitService.Execute(
                request.CashFlowId,
                request.Amount);

            if (depositResult == null)
            {
                return new NoContentResult();
            }

            Model model = new Model(
                depositResult.Entry.Amount,
                depositResult.Entry.Description,
                depositResult.Entry.EntryDate,
                depositResult.UpdatedBalance
            );

            return new ObjectResult(model);
        }
    }
}
