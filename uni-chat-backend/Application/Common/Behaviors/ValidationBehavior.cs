using FluentValidation;
using MediatR;
using uni_chat_backend.Application.Common.Exceptions;

namespace uni_chat_backend.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Iniciando validación para la solicitud: {RequestName}", requestName);

        if (!validators.Any())
        {
            logger.LogInformation("No se encontraron validadores para la solicitud: {RequestName}", requestName);

            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        logger.LogInformation("Ejecutando {ValidatorCount} validadores para la solicitud: {RequestName}",
            validators.Count(), requestName);

        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errors = validationResults.SelectMany(r => r.Errors)
            .Where(f => f != null)
            .GroupBy(x => x.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

        if (errors.Count != 0)
        {
            logger.LogWarning("La validación falló para la solicitud: {RequestName}. Errores: {@Errors}", requestName,
                errors);

            throw new BadRequestException(errors);
        }

        logger.LogInformation("La validación fue exitosa para la solicitud: {RequestName}", requestName);

        return await next(cancellationToken);
    }
}
