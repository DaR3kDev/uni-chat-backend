using MediatR;
using uni_chat_backend.Infrastructure.Services;

namespace uni_chat_backend.Features.Messages.UploadFile;

public class UploadFileHandler(CloudinaryService cloudinaryService)
    : IRequestHandler<UploadFileCommand, UploadFileResult>
{
    private readonly CloudinaryService _cloudinaryService = cloudinaryService;

    public async Task<UploadFileResult> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        if (request.File == null || request.File.Length == 0)
            throw new ArgumentException("Archivo inválido");

        var fileUrl = await _cloudinaryService.UploadFileAsync(request.File);
        return new UploadFileResult(fileUrl);
    }
}