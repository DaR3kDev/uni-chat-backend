using FluentValidation;

namespace uni_chat_backend.Features.Auth.Login.Validators;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono es obligatorio")
            .Matches(@"^\+[1-9]\d{6,14}$")
            .WithMessage("El teléfono debe estar en formato internacional, ejemplo: +573001234567");
    }
}