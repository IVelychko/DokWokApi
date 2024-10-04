using Application.Abstractions.Messaging;
using Domain.Errors;
using Domain.Errors.Base;
using Domain.Exceptions;
using Domain.ResultType;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using ValidationException = Domain.Exceptions.ValidationException;

namespace Application.Behaviors;

public sealed class ValidationWithResponseBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, ICommand<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationWithResponseBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        //var errorsDictionary = _validators
        //    .Select(x => x.Validate(context))
        //    .SelectMany(x => x.Errors)
        //    .Where(x => x != null)
        //    .GroupBy(
        //        x => x.PropertyName,
        //        x => x.ErrorMessage,
        //        (propertyName, errorMessages) => new
        //        {
        //            Key = propertyName,
        //            Values = errorMessages.Distinct().ToArray()
        //        })
        //    .ToDictionary(x => x.Key, x => x.Values);

        //var validationResults = _validators
        //    .Select(x => x.ValidateAsync(context));

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
            Error error = isNotFound ? new EntityNotFoundError(errorsDictionary)
                : new ValidationError(errorsDictionary);
            var response = CreateFailureResult(error, isNotFound);
            return response;
        }

        return await next();
    }

    private static TResponse CreateFailureResult(Error error, bool isNotFound)
    {
        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            // Get the TValue type argument for Result<TValue>
            var valueType = typeof(TResponse).GetGenericArguments()[0];

            // Create the specific Result<TValue> type
            var resultType = typeof(Result<>).MakeGenericType(valueType);

            // Find the constructor or static Failure method to create a faulted Result<TValue>
            var failureMethod = resultType.GetMethod(nameof(Result<object>.Failure), [typeof(Error)]);

            if (failureMethod is not null)
            {
                // Invoke the Failure method to construct a faulted result
                var resultInstance = failureMethod.Invoke(null, [error]);

                // Cast and return the result as TResponse
                return (TResponse)resultInstance!;
            }
        }

        if (isNotFound)
        {
            throw new NotFoundException(error.Errors);
        }

        throw new ValidationException(error.Errors);
    }
}
