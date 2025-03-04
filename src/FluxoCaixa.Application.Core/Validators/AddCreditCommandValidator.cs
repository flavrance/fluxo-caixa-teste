using FluentValidation;
using FluxoCaixa.Application.Core.Commands.CashFlow;

namespace FluxoCaixa.Application.Core.Validators
{
    public class AddCreditCommandValidator : AbstractValidator<AddCreditCommand>
    {
        public AddCreditCommandValidator()
        {
            RuleFor(x => x.CashFlowId)
                .NotEmpty()
                .WithMessage("O ID do fluxo de caixa é obrigatório");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("O valor do crédito deve ser maior que zero");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("A descrição é obrigatória")
                .MaximumLength(200)
                .WithMessage("A descrição deve ter no máximo 200 caracteres");
        }
    }
} 