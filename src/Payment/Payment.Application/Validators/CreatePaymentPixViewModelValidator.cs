using FluentValidation;
using Payment.Application.ViewModels;

namespace Payment.Application.Validators
{
    public class CreatePaymentPixViewModelValidator : AbstractValidator<CreatePaymentPixViewModel>
    {
        public CreatePaymentPixViewModelValidator()
        {
            RuleFor(x => x.Key)
                .NotEmpty().NotNull()
               .WithMessage("A Key não pode ser nula");
        }
    }
}
