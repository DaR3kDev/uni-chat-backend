using FluentValidation;

namespace uni_chat_backend.Features.Contacts.AddContact.Validators;

public class AddContactValidator : AbstractValidator<AddContactCommand>
{
    public AddContactValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("El teléfono es obligatorio")
            .Matches(@"^\+[1-9]\d{6,14}$")
            .WithMessage("El teléfono debe estar en formato internacional, ejemplo: +573001234567");

        RuleFor(x => x.Alias)
            .MaximumLength(30)
            .WithMessage("El alias no puede superar los 30 caracteres");
    }
}

