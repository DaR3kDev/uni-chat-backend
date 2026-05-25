using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using uni_chat_backend.Infrastructure.Configuration;

namespace uni_chat_backend.Infrastructure.Services;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> options)
    {
        var settings = options.Value ??
                       throw new ArgumentNullException(nameof(options),
                           "La configuración de Cloudinary no se encontró.");

        if (string.IsNullOrWhiteSpace(settings.CloudName) || string.IsNullOrWhiteSpace(settings.ApiKey) ||
            string.IsNullOrWhiteSpace(settings.ApiSecret))
            throw new ArgumentException(
                "La configuración de Cloudinary es inválida. Verifica CloudName, ApiKey y ApiSecret.");

        _cloudinary = new Cloudinary(new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret));
    }

    /// <summary>
    ///     Sube cualquier archivo (imagen, video, audio o genérico) a Cloudinary y devuelve la URL segura.
    /// </summary>
    public async Task<string> UploadFileAsync(IFormFile file)
    {
        ValidateFile(file);

        await using var stream = file.OpenReadStream();

        var folder = file.ContentType?.ToLowerInvariant() switch
        {
            var t when t!.StartsWith("image/") => "chat/images",
            var t when t!.StartsWith("video/") => "chat/videos",
            var t when t!.StartsWith("audio/") => "chat/audios",
            _ => "chat/files"
        };

        return await UploadToCloudinary(file, stream, folder);
    }

    private async Task<string> UploadToCloudinary(IFormFile file, Stream stream, string folder)
    {
        dynamic uploadParams = file.ContentType?.ToLowerInvariant() switch
        {
            var t when t!.StartsWith("image/") => new ImageUploadParams(),
            var t when t!.StartsWith("video/") => new VideoUploadParams(),
            var t when t!.StartsWith("audio/") => new RawUploadParams(),
            _ => new RawUploadParams()
        };

        // Configurar propiedades comunes
        uploadParams.File = new FileDescription(file.FileName, stream);
        uploadParams.Folder = folder;
        uploadParams.UseFilename = true;
        uploadParams.UniqueFilename = true;
        uploadParams.Overwrite = false;

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result?.SecureUrl == null)
            throw new Exception(
                $"Error al subir el archivo a Cloudinary: {result?.Error?.Message ?? "Error desconocido"}");

        return result.SecureUrl.ToString();
    }

    private static void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("Archivo inválido o vacío.", nameof(file));
    }
}
