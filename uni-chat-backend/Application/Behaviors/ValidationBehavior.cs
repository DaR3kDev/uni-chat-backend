using FluentValidation;
using MediatR;
using uni_chat_backend.API.Exceptions;

namespace uni_chat_backend.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

                var results = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var errors = results
                .SelectMany(r => r.Errors)
                .Where(e => e is not null)
                .Select(e => e.ErrorMessage)
                .ToList();

            if (errors.Count > 0)
                throw new BadRequestException(errors);
        }

        return await next(cancellationToken);
    }
}