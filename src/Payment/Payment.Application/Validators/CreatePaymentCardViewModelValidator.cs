using FluentValidation;
using Payment.Application.ViewModels;

namespace Payment.Application.Validators
{
    public class CreatePaymentCardViewModelValidator : AbstractValidator<CreatePaymentCardViewModel>
    {
        public CreatePaymentCardViewModelValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().NotNull()
               .WithMessage("O UserId não pode ser nulo");

            RuleFor(x => x.ClientName)
                .NotEmpty().NotNull()
               .WithMessage("O ClientName não pode ser nulo");

            RuleFor(x => x.Number)
                .NotEmpty().NotNull()
               .WithMessage("O Number não pode ser nulo");

            RuleFor(x => x.DateValidate)
                .NotEmpty().NotNull()
               .WithMessage("A DateValidate não pode ser nula");

            RuleFor(x => x.SecurityCode)
                .GreaterThan(0)
                .WithMessage("A SecurityCode deve ser maior que 0");
        }
    }
}
