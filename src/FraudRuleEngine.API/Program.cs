using FraudRuleEngine.Application.Rules;
using FraudRuleEngine.Application.Services;
using FraudRuleEngine.Core.Interfaces;
using FraudRuleEngine.Core.Rules;
using FraudRuleEngine.Infrastructure.Persistence;
using FraudRuleEngine.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FraudDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddScoped<IFraudRule, HighAmountRule>();
builder.Services.AddScoped<IFraudRule, VelocityRule>();
builder.Services.AddScoped<IFraudRule, DuplicateTransactionRule>();
builder.Services.AddScoped<IFraudRule, UnusualHourRule>();
builder.Services.AddScoped<IFraudRule, RoundAmountRule>();

builder.Services.AddScoped<FraudEvaluationService>();
builder.Services.AddScoped<TransactionService>();

var emailConfig = builder.Configuration.GetSection("Email");
builder.Services.AddScoped<EmailService>(sp => new EmailService(
    emailConfig["SmtpServer"] ?? "smtp.gmail.com",
    int.Parse(emailConfig["SmtpPort"] ?? "587"),
    emailConfig["SenderEmail"] ?? "noreply@fraudengine.com",
    emailConfig["SenderPassword"] ?? "your-app-password"
));
builder.Services.AddScoped<AlertService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReact");

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FraudDbContext>();
    db.Database.Migrate();
}

app.Run();
