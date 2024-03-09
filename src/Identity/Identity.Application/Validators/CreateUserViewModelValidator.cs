using FluentValidation;
using Identity.Application.ViewModels;

namespace Identity.Application.Validators
{
    public class CreateUserViewModelValidator : AbstractValidator<CreateUserViewModel>
    {
        public CreateUserViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().NotNull()
                .WithMessage("O Nome não pode ser nulo")
                .Matches("^[a-zA-Z]+$")
                .WithMessage("O Nome deve possuir somente letras e não pode conter espaços");

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
