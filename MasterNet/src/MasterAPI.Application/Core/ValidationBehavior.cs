using System.ComponentModel.DataAnnotations;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

namespace MasterAPI.Application.Core
{
    public class ValidationBehavior<TRequest, TResponse>(
        IEnumerable<IValidator<TRequest>> validators
    ) : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommandBase
    {
    
        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            
            var contextValidation = new ValidationContext<TRequest>(request);

            var validationFailures = await Task.WhenAll(
                validators.Select(validator => validator.ValidateAsync(contextValidation))
            );
            
            var errors = validationFailures
                        .Where(validationResult => !validationResult.IsValid)
                        .SelectMany(validationResult => validationResult.Errors)
                        .Select(validationFailures => new ValidationError(
                            validationFailures.PropertyName,
                            validationFailures.ErrorMessage
                        )).ToList();

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }

            return await next();
        }
    }
}