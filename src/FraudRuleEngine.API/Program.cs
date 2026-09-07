using FraudRuleEngine.Application.Rules;
using FraudRuleEngine.Application.Services;
using FraudRuleEngine.Core.Interfaces;
using FraudRuleEngine.Core.Rules;
using FraudRuleEngine.Infrastructure.Persistence;
using FraudRuleEngine.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FraudDbContext>();
    db.Database.Migrate();
}

app.Run();
