using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace FraudRuleEngine.API.Validation;

public static class ValidationExtensions
{
    /// <summary>
    /// Runs the FluentValidation validator for a request. Returns a 400 ValidationProblem with the
    /// field errors when the request is invalid, or null when it is valid.
    /// </summary>
    public static async Task<IActionResult?> ValidateAsync<T>(
        this ControllerBase controller,
        IValidator<T> validator,
        T request,
        CancellationToken cancellationToken = default)
    {
        ValidationResult result = await validator.ValidateAsync(request, cancellationToken);
        if (result.IsValid)
        {
            return null;
        }

        var errors = result.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());

        return controller.ValidationProblem(new ValidationProblemDetails(errors));
    }
}
