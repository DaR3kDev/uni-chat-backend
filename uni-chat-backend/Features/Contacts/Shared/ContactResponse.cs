namespace uni_chat_backend.Features.Contacts.Shared;

public sealed record ContactResponse(
    Guid Id,
    Guid ContactUserId,
    string? Username,
    string? Phone,
    string? Alias
);
