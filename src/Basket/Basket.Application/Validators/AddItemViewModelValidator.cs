using Basket.Application.ViewModels;
using FluentValidation;

namespace Basket.Application.Validators
{
    public class AddItemViewModelValidator : AbstractValidator<AddItemViewModel>
    {
        public AddItemViewModelValidator()
        {
            RuleFor(x => x.Quantity)   
               .NotNull().NotEmpty()
               .WithMessage("A Quantity não pode ser nula ou vazia")
               .GreaterThan(0)
               .WithMessage("A Quantity deve ser maior que 0");

            RuleFor(x => x.ProductId)
                .NotNull().NotEmpty()
                .WithMessage("A Quantity não pode ser nula ou vazia")
                .GreaterThan(0)
                .WithMessage("O ProductId deve ser maior que 0");
        }
    }
}
