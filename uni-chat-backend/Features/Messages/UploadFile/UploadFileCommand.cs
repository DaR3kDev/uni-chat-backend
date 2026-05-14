using MediatR;

namespace uni_chat_backend.Features.Messages.UploadFile;

public record UploadFileCommand(IFormFile File) : IRequest<UploadFileResult>;