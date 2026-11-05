using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FraudRuleEngine.API.Swagger;

// Documents the 401 response, with an example body, on every endpoint that requires a token.
// The API requires authentication by default (fallback policy), so only [AllowAnonymous] endpoints are skipped.
public sealed class UnauthorizedOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var allowsAnonymous = context.MethodInfo.GetCustomAttributes(true).OfType<IAllowAnonymous>().Any();
        if (allowsAnonymous || operation.Responses.ContainsKey("401"))
        {
            return;
        }

        operation.Responses["401"] = new OpenApiResponse
        {
            Description = "Unauthorized",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Example = new OpenApiObject
                    {
                        ["error"] = new OpenApiString("unauthorized"),
                        ["error_description"] = new OpenApiString("A valid access token is required."),
                        ["status"] = new OpenApiInteger(401)
                    }
                }
            }
        };
    }
}
