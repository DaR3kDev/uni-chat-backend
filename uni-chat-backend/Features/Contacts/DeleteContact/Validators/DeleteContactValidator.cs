using FluentValidation;

namespace uni_chat_backend.Features.Contacts.DeleteContact.Validators;

public class DeleteContactValidator : AbstractValidator<DeleteContactCommand>
{
    public DeleteContactValidator()
    {
        RuleFor(x => x.ContactId).NotEmpty().WithMessage("El identificador del contacto es obligatorio");
    }
}
