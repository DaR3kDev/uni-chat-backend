using FluentValidation;

namespace uni_chat_backend.Features.Auth.Register.Validators;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        // Username
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El username es obligatorio")
            .MinimumLength(3).WithMessage("El username debe tener al menos 3 caracteres")
            .MaximumLength(20).WithMessage("El username no puede superar los 20 caracteres");

        // Email
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio")
            .EmailAddress().WithMessage("El formato del email no es válido");

        // Phone
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono es obligatorio")
            .Matches(@"^\+[1-9]\d{6,14}$")
            .WithMessage("El teléfono debe estar en formato internacional, ejemplo: +573001234567");
    }
}

