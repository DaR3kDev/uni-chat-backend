using FluentValidation;

namespace uni_chat_backend.Features.Auth.Register.Validators;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        // First Name
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio")
            .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres");

        // Last Name
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio")
            .MinimumLength(2).WithMessage("El apellido debe tener al menos 2 caracteres");

        // Email
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio")
            .EmailAddress().WithMessage("El formato del email no es válido");

        // Username
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El username es obligatorio")
            .MinimumLength(3).WithMessage("El username debe tener al menos 3 caracteres")
            .MaximumLength(20).WithMessage("El username no puede superar los 20 caracteres");

        // Phone
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono es obligatorio")
            .Matches(@"^\+[1-9]\d{6,14}$")
            .WithMessage("El teléfono debe estar en formato internacional, ejemplo: +573001234567");

        // Password
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres")
            .Matches(@"[A-Z]").WithMessage("Debe contener al menos una mayúscula")
            .Matches(@"[a-z]").WithMessage("Debe contener al menos una minúscula")
            .Matches(@"[0-9]").WithMessage("Debe contener al menos un número");

        // Confirm Password
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Debes confirmar la contraseña")
            .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden");
    }
}

