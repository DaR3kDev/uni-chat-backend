using FluentValidation;

namespace uni_chat_backend.Features.Messages.SendMessage.Validators;

public class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("El mensaje no puede estar vacío")
            .Must(c => !string.IsNullOrWhiteSpace(c))
            .WithMessage("El mensaje no puede contener solo espacios en blanco")
            .MaximumLength(2000)
            .WithMessage("El mensaje excede el tamaño máximo permitido");

        RuleFor(x => x.ConversationId)
           .NotEmpty().WithMessage("El ID de la conversación es obligatorio")
           .NotEqual(Guid.Empty).WithMessage("El ID de la conversación no es válido");
    }
}
