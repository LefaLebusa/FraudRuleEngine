using System.Threading.RateLimiting;
using FluentValidation;
using FraudRuleEngine.Application;
using FraudRuleEngine.Core;
using FraudRuleEngine.Infrastructure;
using FraudRuleEngine.API.Observability;
using FraudRuleEngine.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        // Logs: human-readable in Development, one JSON object per line everywhere else (for log shippers)
        if (!builder.Environment.IsDevelopment())
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddJsonConsole();
        }

        // Traces, metrics and logs exported over OTLP (added after ClearProviders so it is kept)
        builder.AddFraudEngineObservability();

        // CORS: allowed frontend origins come from configuration (Cors:AllowedOrigins)
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"];
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
                policy.WithOrigins(allowedOrigins)
                      .WithMethods("GET", "POST", "PUT")
                      .WithHeaders("Content-Type", "Authorization"));
        });

        // Authentication: JWT bearer tokens issued by the identity provider (e.g. Dex)
        var authority = configuration["Authentication:Authority"]
            ?? throw new InvalidOperationException("Authentication:Authority is not configured.");
        var audience = configuration["Authentication:Audience"]
            ?? throw new InvalidOperationException("Authentication:Audience is not configured.");

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
            });

        // Secure by default: every endpoint requires an authenticated caller unless explicitly marked [AllowAnonymous]
        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        // Rate limiting: per client IP, applied to every endpoint
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = (context, _) =>
            {
                context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>().LogWarning(
                    "Rate limit exceeded for client {ClientIp} on {Path}",
                    context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    context.HttpContext.Request.Path);
                return ValueTask.CompletedTask;
            };
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 120,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
        });

        builder.Services.AddControllers().AddJsonOptions(o =>
            o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Fraud Detection API",
                Version = "v1",
                Description = "API for evaluating transactions for potential fraud."
            });

            // Adds the "Authorize" button and lock icons in Swagger UI
            options.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter a valid JWT access token to access this API."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "BearerAuth" }
                    },
                    Array.Empty<string>()
                }
            });

            options.OperationFilter<FraudRuleEngine.API.Swagger.UnauthorizedOperationFilter>();

            // Include the XML doc comments on controllers as operation summaries
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile), includeControllerXmlComments: true);
        });

        // Each layer registers its own types
        builder.Services
            .AddCore()
            .AddApplication()
            .AddInfrastructure(configuration);

        // Request validators defined in the API project (AccountIdRequest, AlertEmailRequest, query requests)
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        // Listen address comes from ASPNETCORE_URLS / launchSettings (e.g. http://+:8080 in Docker)
        var app = builder.Build();
        app.Logger.LogInformation("Fraud Rule Engine API starting in {Environment}", app.Environment.EnvironmentName);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            if (!app.Environment.IsDevelopment())
            {
                context.Response.Headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";
            }
            await next();
        });

        // Correlation: every log line written while handling a request carries its RequestId
        app.Use(async (context, next) =>
        {
            using (app.Logger.BeginScope(new Dictionary<string, object> { ["RequestId"] = context.TraceIdentifier }))
            {
                await next();
            }
        });

        if (configuration.GetValue<bool>("Security:RequireHttps"))
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }

        app.UseCors("AllowFrontend");
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FraudDbContext>();
            db.Database.Migrate();
        }

        app.Run();
    }
}
