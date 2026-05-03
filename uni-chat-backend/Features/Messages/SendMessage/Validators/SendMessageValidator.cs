using FluentValidation;
using uni_chat_backend.Domain.Enums;

namespace uni_chat_backend.Features.Messages.SendMessage.Validators;

public class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        // Validación del ID de conversación (siempre obligatorio)
        RuleFor(x => x.ConversationId)
            .NotEmpty().WithMessage("El ID de la conversación es obligatorio")
            .NotEqual(Guid.Empty).WithMessage("El ID de la conversación no es válido");

        // Validación para mensajes de texto
        When(x => x.Type == MessageType.TEXT, () =>
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("El mensaje no puede estar vacío")
                .Must(c => !string.IsNullOrWhiteSpace(c))
                .WithMessage("El mensaje no puede contener solo espacios en blanco")
                .MaximumLength(2000)
                .WithMessage("El mensaje excede el tamaño máximo permitido");
        });

        // Validación para archivos (IMAGE, FILE, VIDEO, AUDIO)
        When(x => x.Type == MessageType.IMAGE
               || x.Type == MessageType.FILE
               || x.Type == MessageType.VIDEO
               || x.Type == MessageType.AUDIO, () =>
               {
                   RuleFor(x => x.FileUrl)
                       .NotEmpty().WithMessage("La URL del archivo es obligatoria");

                   RuleFor(x => x.FileName)
                       .NotEmpty().WithMessage("El nombre del archivo es obligatorio");
               });

        // Validación para tipos inválidos
        RuleFor(x => x.Type)
            .Must(t => t == MessageType.TEXT
                      || t == MessageType.IMAGE
                      || t == MessageType.FILE
                      || t == MessageType.VIDEO
                      || t == MessageType.AUDIO)
            .WithMessage("Tipo de mensaje no válido");
    }
}