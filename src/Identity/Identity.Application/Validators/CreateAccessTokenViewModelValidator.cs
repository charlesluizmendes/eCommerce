using FluentValidation;
using Identity.Application.ViewModels;

namespace Identity.Application.Validators
{
    public class CreateAccessTokenViewModelValidator : AbstractValidator<CreateAccessTokenViewModel>
    {
        public CreateAccessTokenViewModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().NotNull()
                .WithMessage("O Email não pode ser nulo")
                .EmailAddress()
                .WithMessage("O Email é inválido");

            RuleFor(x => x.Password)
               .NotEmpty().NotNull()
               .WithMessage("A Senha não pode ser nula")
               .MinimumLength(3)
               .WithMessage("A Senha deve possuir no mínimo 3 caracteres");
        }
    }
}
