namespace uni_chat_backend.Features.Contacts.GetContacts;

public record ContactResponse(
    Guid Id,
    Guid ContactUserId,
    string? Username,
    string? Phone,
    string? Alias
);