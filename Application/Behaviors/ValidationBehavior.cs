using Application.Abstractions.Messaging;
using Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using ValidationException = Domain.Exceptions.ValidationException;

namespace Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, ICommand
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        List<ValidationResult> validationResults = [];
        foreach (var validatior in _validators)
        {
            var result = await validatior.ValidateAsync(context, cancellationToken);
            validationResults.Add(result);
        }

        bool isNotFound = false;
        foreach (var validationResult in validationResults)
        {
            var notFoundErrorCodeExists = validationResult.Errors.Exists(x => x.ErrorCode == "404");
            if (notFoundErrorCodeExists)
            {
                isNotFound = true;
                break;
            }
        }

        var errorsDictionary = validationResults
            .SelectMany(x => x.ToDictionary())
            .ToDictionary();

        if (errorsDictionary.Count > 0)
        {
            if (isNotFound)
            {
                throw new NotFoundException(errorsDictionary);
            }

            throw new ValidationException(errorsDictionary);
        }

        return await next();
    }
}
