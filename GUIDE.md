# How I Built the Fraud Rule Engine — Step by Step Guide

This guide explains every single step used to build this project from scratch.
It is written so that anyone — even someone with no prior experience — can follow along and understand what was done and why.

---

## What Are We Building?

We are building a **Fraud Rule Engine Service** — a backend system that:
1. Accepts a bank transaction (e.g. "Account ACC123 spent R75,000 at Unknown Vendor")
2. Runs that transaction through a set of **fraud rules** (like a checklist)
3. Flags it as suspicious if any rules are triggered
4. Saves everything to a database
5. Lets you retrieve flagged transactions via an API

Think of it like a security guard at a door checking IDs against a list of rules.

---

## Tools & Technologies Used

| Tool | What it does |
|---|---|
| **.NET 9 / C#** | The programming language and framework we use to write the code |
| **ASP.NET Core Web API** | The part of .NET that lets us build a REST API (endpoints you can call via HTTP) |
| **Entity Framework Core (EF Core)** | Lets us talk to the database using C# instead of writing raw SQL |
| **PostgreSQL** | The database where we store transactions and fraud results |
| **xUnit** | A testing framework — lets us write automated tests to verify our rules work |
| **Swagger** | Automatically generates a web UI to test your API endpoints |
| **Docker** | Packages the app into a container so it runs the same anywhere |
| **Git + GitHub** | Version control — saves your code history and lets you share it online |

---

## Prerequisites (What You Need Installed)

- .NET 9 SDK — https://dotnet.microsoft.com/download
- PostgreSQL (already installed on this machine)
- Visual Studio or VS Code
- Git
- Docker Desktop (optional, for running with Docker)
- DBeaver (to view the database visually)

---

## Part 1: Creating the Solution Structure

### What is a "Solution"?
A solution (`.sln` file) is like a folder that holds multiple related projects together. Instead of having one giant project, we split responsibilities into smaller projects.

### Step 1.1 — Create the solution

Open a terminal (PowerShell) and navigate to where you want your project:

```
cd "C:\Users\YourName\Desktop\Projects"
```

Then run:

```
dotnet new sln -n FraudRuleEngine -o FraudRuleEngine
```

- `dotnet new sln` — creates a new solution file
- `-n FraudRuleEngine` — names it FraudRuleEngine
- `-o FraudRuleEngine` — creates it inside a new folder called FraudRuleEngine

### Step 1.2 — Navigate into the project folder

```
cd FraudRuleEngine
```

### Step 1.3 — Create the 4 projects + 1 test project

```
dotnet new classlib -n FraudRuleEngine.Core -o src/FraudRuleEngine.Core
dotnet new classlib -n FraudRuleEngine.Application -o src/FraudRuleEngine.Application
dotnet new classlib -n FraudRuleEngine.Infrastructure -o src/FraudRuleEngine.Infrastructure
dotnet new webapi -n FraudRuleEngine.API -o src/FraudRuleEngine.API
dotnet new xunit -n FraudRuleEngine.UnitTests -o tests/FraudRuleEngine.UnitTests
```

**What each project does:**

| Project | Responsibility |
|---|---|
| `Core` | The heart of the app — domain models (Transaction, FraudEvaluation) and interfaces. Has ZERO dependencies on other projects. |
| `Application` | Business logic — the fraud rules and services that process transactions. Depends on Core only. |
| `Infrastructure` | Database layer — EF Core setup, repositories (how we save/read data). Depends on Core only. |
| `API` | Web layer — the HTTP controllers that receive requests. Depends on Application + Infrastructure. |
| `UnitTests` | Tests — verifies each fraud rule works correctly. |

This is called **Clean Architecture** — each layer only knows about the layers below it, not above.

### Step 1.4 — Register all projects in the solution

```
dotnet sln add src/FraudRuleEngine.Core/FraudRuleEngine.Core.csproj
dotnet sln add src/FraudRuleEngine.Application/FraudRuleEngine.Application.csproj
dotnet sln add src/FraudRuleEngine.Infrastructure/FraudRuleEngine.Infrastructure.csproj
dotnet sln add src/FraudRuleEngine.API/FraudRuleEngine.API.csproj
dotnet sln add tests/FraudRuleEngine.UnitTests/FraudRuleEngine.UnitTests.csproj
```

### Step 1.5 — Wire up project references (who depends on who)

```
dotnet add src/FraudRuleEngine.Application reference src/FraudRuleEngine.Core
dotnet add src/FraudRuleEngine.Infrastructure reference src/FraudRuleEngine.Core
dotnet add src/FraudRuleEngine.API reference src/FraudRuleEngine.Application
dotnet add src/FraudRuleEngine.API reference src/FraudRuleEngine.Infrastructure
dotnet add tests/FraudRuleEngine.UnitTests reference src/FraudRuleEngine.Application
dotnet add tests/FraudRuleEngine.UnitTests reference src/FraudRuleEngine.Core
```

---

## Part 2: Installing NuGet Packages

**What is NuGet?** It's like an app store for .NET — you download pre-built libraries instead of writing everything from scratch.

### Step 2.1 — Install EF Core and PostgreSQL driver

```
dotnet add src/FraudRuleEngine.Infrastructure package Microsoft.EntityFrameworkCore --version 9.0.7
dotnet add src/FraudRuleEngine.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.4
dotnet add src/FraudRuleEngine.Infrastructure package Microsoft.Extensions.Configuration.Json --version 9.0.7
dotnet add src/FraudRuleEngine.API package Microsoft.EntityFrameworkCore.Design --version 9.0.7
dotnet add src/FraudRuleEngine.API package Swashbuckle.AspNetCore --version 7.3.1
```

**Why these specific versions?** Our projects target **.NET 9**, so we use EF Core **9.x**. EF Core 10 only works with .NET 10 and would cause a version mismatch error.

---

## Part 3: Building the Core Layer (Domain Models)

The Core layer defines **what things are** — the data structures used throughout the whole app.

### Step 3.1 — Create the Transaction model

File: `src/FraudRuleEngine.Core/Models/Transaction.cs`

```csharp
public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AccountId { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string MerchantName { get; set; } = default!;
    public string MerchantCategory { get; set; } = default!;
    public string Country { get; set; } = "ZA";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string TransactionType { get; set; } = default!;
    public string ReferenceNumber { get; set; } = default!;
    public FraudEvaluation? FraudEvaluation { get; set; }
}
```

**Simple explanation:** This is like a form that captures all the details of one bank transaction. `Guid` is just a unique ID (like a barcode).

### Step 3.2 — Create the FraudEvaluation model

File: `src/FraudRuleEngine.Core/Models/FraudEvaluation.cs`

```csharp
public class FraudEvaluation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TransactionId { get; set; }
    public bool IsFlagged { get; set; }
    public List<string> TriggeredRules { get; set; } = [];
    public string RiskLevel { get; set; } = "LOW";
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    public Transaction Transaction { get; set; } = default!;
}
```

**Simple explanation:** This is the result after checking a transaction — was it flagged? Which rules triggered? How risky is it?

### Step 3.3 — Create the IFraudRule interface

File: `src/FraudRuleEngine.Core/Rules/IFraudRule.cs`

```csharp
public interface IFraudRule
{
    string RuleName { get; }
    bool Evaluate(Transaction transaction, IEnumerable<Transaction> accountHistory);
}
```

**What is an interface?** Think of it as a contract. It says: "Any fraud rule MUST have a name and MUST be able to evaluate a transaction." Every rule we create must follow this contract.

**Why use an interface?** So we can add new rules in the future without changing the engine. Just create a new class that follows the contract and the engine picks it up automatically.

This is called the **Strategy Pattern** — a common design pattern in software.

### Step 3.4 — Create the ITransactionRepository interface

File: `src/FraudRuleEngine.Core/Interfaces/ITransactionRepository.cs`

```csharp
public interface ITransactionRepository
{
    Task<Transaction> AddAsync(Transaction transaction);
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(string accountId);
    Task<IEnumerable<Transaction>> GetRecentByAccountIdAsync(string accountId, TimeSpan window);
    Task<IEnumerable<Transaction>> GetFlaggedAsync();
    Task UpdateAsync(Transaction transaction);
    Task AddEvaluationAsync(FraudEvaluation evaluation);
}
```

**Simple explanation:** This defines what database operations we need — like a menu of what you can do with transactions. The actual database code lives in the Infrastructure layer, keeping the Core layer clean.

---

## Part 4: Building the Application Layer (Fraud Rules + Services)

This is where the actual business logic lives.

### Step 4.1 — Create the 5 fraud rules

Each rule is a separate file that implements `IFraudRule`.

**Rule 1: High Amount** — `src/FraudRuleEngine.Application/Rules/HighAmountRule.cs`
Triggers when a transaction is over R50,000.

**Rule 2: Velocity** — `src/FraudRuleEngine.Application/Rules/VelocityRule.cs`
Triggers when the same account makes 5 or more transactions within 10 minutes. (Classic card skimming behaviour.)

**Rule 3: Duplicate Transaction** — `src/FraudRuleEngine.Application/Rules/DuplicateTransactionRule.cs`
Triggers when the same amount at the same merchant appears twice within 2 minutes. (Could be a double charge or replay attack.)

**Rule 4: Unusual Hour** — `src/FraudRuleEngine.Application/Rules/UnusualHourRule.cs`
Triggers when a transaction happens between 12am and 4am SAST. (Most legitimate spending doesn't happen at 2am.)

**Rule 5: Round Amount** — `src/FraudRuleEngine.Application/Rules/RoundAmountRule.cs`
Triggers when a round number (e.g. exactly R10,000 or R50,000) is transacted. Fraudsters often test with round numbers.

### Step 4.2 — Create the FraudEvaluationService

File: `src/FraudRuleEngine.Application/Services/FraudEvaluationService.cs`

This is the **engine** — it runs all rules against a transaction and produces a result.

```
For each transaction:
  1. Get the last 24 hours of transactions for that account
  2. Run every fraud rule
  3. Collect which rules triggered
  4. Assign risk: 0 rules = LOW, 1 rule = MEDIUM, 2+ rules = HIGH
  5. Save the result
```

### Step 4.3 — Create the TransactionService

File: `src/FraudRuleEngine.Application/Services/TransactionService.cs`

This is the **orchestrator** — it coordinates the save and evaluation flow:
1. Save the transaction to the database
2. Run the fraud evaluation
3. Return both results

---

## Part 5: Building the Infrastructure Layer (Database)

This layer handles all communication with PostgreSQL.

### Step 5.1 — Create the DbContext

File: `src/FraudRuleEngine.Infrastructure/Persistence/FraudDbContext.cs`

**What is a DbContext?** It's EF Core's gateway to the database. You define your tables here, and EF Core translates your C# objects into SQL automatically.

```csharp
public class FraudDbContext : DbContext
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<FraudEvaluation> FraudEvaluations => Set<FraudEvaluation>();
}
```

We also configure:
- `Amount` to have precision 18,2 (standard for money)
- `Transaction` has one `FraudEvaluation` (one-to-one relationship)
- `TriggeredRules` (a list) is stored as a comma-separated string in the DB

### Step 5.2 — Create the Repository

File: `src/FraudRuleEngine.Infrastructure/Repositories/TransactionRepository.cs`

This implements `ITransactionRepository` using actual EF Core queries. For example:

```csharp
// Get all flagged transactions
return await _context.Transactions
    .Include(t => t.FraudEvaluation)
    .Where(t => t.FraudEvaluation != null && t.FraudEvaluation.IsFlagged)
    .OrderByDescending(t => t.Timestamp)
    .ToListAsync();
```

### Step 5.3 — Create the Design-Time Factory

File: `src/FraudRuleEngine.Infrastructure/Persistence/FraudDbContextFactory.cs`

**Why do we need this?** When EF Core tools (like migration commands) run, they need to create a `DbContext` without starting the full API. This factory tells them how to do it by reading `appsettings.json` directly.

---

## Part 6: Building the API Layer

This layer exposes your application to the outside world via HTTP endpoints.

### Step 6.1 — Create the DTO (Data Transfer Object)

File: `src/FraudRuleEngine.API/DTOs/TransactionRequest.cs`

```csharp
public record TransactionRequest(
    string AccountId,
    decimal Amount,
    string Currency,
    string MerchantName,
    string MerchantCategory,
    string Country,
    string TransactionType,
    string ReferenceNumber
);
```

**What is a DTO?** It defines exactly what data the caller must send in the request body. It's separate from the domain model so you control what comes in and what goes out.

### Step 6.2 — Create the Controller

File: `src/FraudRuleEngine.API/Controllers/TransactionsController.cs`

This creates 4 API endpoints:

| Method | URL | What it does |
|---|---|---|
| POST | `/api/transactions` | Submit a transaction for fraud evaluation |
| GET | `/api/transactions/{id}` | Get a single transaction by its ID |
| GET | `/api/transactions/flagged` | List all flagged transactions |
| GET | `/api/transactions/account/{accountId}` | List all transactions for one account |

### Step 6.3 — Wire everything up in Program.cs

File: `src/FraudRuleEngine.API/Program.cs`

This is the startup file — it registers all services with .NET's **Dependency Injection** container.

**What is Dependency Injection?** Instead of creating objects manually (like `new TransactionService()`), you register them once and .NET automatically provides them wherever they're needed. This makes code easier to test and change.

```csharp
// Register all 5 fraud rules
builder.Services.AddScoped<IFraudRule, HighAmountRule>();
builder.Services.AddScoped<IFraudRule, VelocityRule>();
builder.Services.AddScoped<IFraudRule, DuplicateTransactionRule>();
builder.Services.AddScoped<IFraudRule, UnusualHourRule>();
builder.Services.AddScoped<IFraudRule, RoundAmountRule>();
```

When `FraudEvaluationService` asks for `IEnumerable<IFraudRule>`, .NET automatically injects all 5 rules. Adding a new rule just means registering it here — nothing else changes.

We also add this to fix circular reference issues when serializing JSON:
```csharp
.AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
```

And this to auto-run migrations when the app starts:
```csharp
db.Database.Migrate();
```

---

## Part 7: Database Migration

**What is a migration?** It's a set of instructions that tells the database how to create or update its tables. EF Core generates these based on your models.

We created the migration file manually at:
`src/FraudRuleEngine.Infrastructure/Migrations/20260907000000_InitialCreate.cs`

The file contains an `Up()` method (creates the tables) and a `Down()` method (drops them if you need to undo).

It creates two tables:
- **Transactions** — stores all transaction data
- **FraudEvaluations** — stores the fraud check result, linked to a transaction

The migration runs **automatically** when the app starts (because of `db.Database.Migrate()` in Program.cs).

---

## Part 8: Connection String (Database Configuration)

File: `src/FraudRuleEngine.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=frauddb;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

This tells the app where to find PostgreSQL:
- `Host=localhost` — the database is on this same machine
- `Port=5432` — default PostgreSQL port
- `Database=frauddb` — the database name (EF Core creates it if it doesn't exist)
- `Username=postgres` — the PostgreSQL username
- `Password=YOUR_PASSWORD` — your PostgreSQL password

**Important:** Never commit real passwords to GitHub. Replace the password with an environment variable before sharing.

---

## Part 9: Unit Tests

File location: `tests/FraudRuleEngine.UnitTests/Rules/`

We wrote tests for each of the 5 rules — 19 tests total.

**What is a unit test?** A small piece of code that checks whether one specific thing works correctly. Think of it as asking: "If I give this function THIS input, do I get THAT output?"

Example test:
```csharp
[Fact]
public void Evaluate_ReturnsTrue_WhenAmountExceedsThreshold()
{
    var transaction = new Transaction { Amount = 75000m };
    Assert.True(new HighAmountRule().Evaluate(transaction, []));
}
```

To run all tests:
```
dotnet test
```

Expected result: **Passed! - 19 tests, 0 failures**

---

## Part 10: Docker

Docker lets you run the app in a container — like a sealed box that has everything the app needs to run, on any computer.

### Dockerfile

File: `Dockerfile`

```
Stage 1 (build):  Use the .NET SDK image → restore packages → publish the app
Stage 2 (runtime): Use the smaller ASP.NET runtime image → copy only the published output
```

This two-stage build keeps the final image small (no SDK tools in production).

### docker-compose.yml

File: `docker-compose.yml`

Defines two services:
- `api` — your application (port 8080)
- `db` — PostgreSQL database (with a health check so the API waits until the DB is ready)

To run everything with Docker:
```
docker compose up --build
```

Then open: http://localhost:8080/swagger

---

## Part 11: Running Locally (Without Docker)

**Prerequisites:** PostgreSQL must be running and you must know your password.

**Step 1** — Update the connection string in `appsettings.json` with your actual PostgreSQL password.

**Step 2** — Run the API:
```
dotnet run --project src/FraudRuleEngine.API --urls "http://localhost:5100"
```

**Step 3** — Open the browser:
```
http://localhost:5100/swagger
```

---

## Part 12: Git & GitHub

**What is Git?** A tool that tracks every change you make to your code — like an undo history for your entire project.

**What is GitHub?** A website where you store your Git history online so others can see and download your code.

### Steps we followed:

**1. Create a `.gitignore` file** — tells Git which files to ignore (e.g. build output, secrets)

**2. Initialize Git in the project folder:**
```
git init
```

**3. Stage all files:**
```
git add .
```

**4. Make the first commit:**
```
git commit -m "Initial commit: Fraud Rule Engine Service"
```

**5. Create a new repo on GitHub** at https://github.com/new

**6. Link your local repo to GitHub:**
```
git remote add origin https://github.com/LefaLebusa/FraudRuleEngine.git
git branch -M main
git push -u origin main
```

**7. For future changes**, after making edits:
```
git add .
git commit -m "Describe what you changed"
git push origin main
```

---

## Part 13: How the Full Flow Works (End to End)

Here is what happens when you call `POST /api/transactions`:

```
1. Browser/Swagger sends a POST request with transaction JSON
        ↓
2. TransactionsController receives the request
        ↓
3. Maps the DTO to a Transaction domain object
        ↓
4. Calls TransactionService.ProcessAsync()
        ↓
5. TransactionRepository.AddAsync() saves the transaction to PostgreSQL
        ↓
6. FraudEvaluationService.EvaluateAsync() runs:
        - Gets last 24h transactions for the account
        - Runs all 5 rules (HighAmount, Velocity, Duplicate, UnusualHour, RoundAmount)
        - Collects triggered rules
        - Calculates risk level (LOW / MEDIUM / HIGH)
        ↓
7. TransactionRepository.AddEvaluationAsync() saves the fraud result
        ↓
8. Controller returns 201 Created with the transaction + fraud evaluation
```

---

## Summary: Files Created

```
FraudRuleEngine/
├── src/
│   ├── FraudRuleEngine.Core/
│   │   ├── Models/Transaction.cs
│   │   ├── Models/FraudEvaluation.cs
│   │   ├── Rules/IFraudRule.cs
│   │   └── Interfaces/ITransactionRepository.cs
│   │
│   ├── FraudRuleEngine.Application/
│   │   ├── Rules/HighAmountRule.cs
│   │   ├── Rules/VelocityRule.cs
│   │   ├── Rules/DuplicateTransactionRule.cs
│   │   ├── Rules/UnusualHourRule.cs
│   │   ├── Rules/RoundAmountRule.cs
│   │   ├── Services/FraudEvaluationService.cs
│   │   └── Services/TransactionService.cs
│   │
│   ├── FraudRuleEngine.Infrastructure/
│   │   ├── Migrations/20260907000000_InitialCreate.cs
│   │   ├── Migrations/FraudDbContextModelSnapshot.cs
│   │   ├── Persistence/FraudDbContext.cs
│   │   ├── Persistence/FraudDbContextFactory.cs
│   │   └── Repositories/TransactionRepository.cs
│   │
│   └── FraudRuleEngine.API/
│       ├── Controllers/TransactionsController.cs
│       ├── DTOs/TransactionRequest.cs
│       ├── Program.cs
│       └── appsettings.json
│
├── tests/
│   └── FraudRuleEngine.UnitTests/
│       └── Rules/
│           ├── HighAmountRuleTests.cs
│           ├── VelocityRuleTests.cs
│           ├── DuplicateTransactionRuleTests.cs
│           ├── UnusualHourRuleTests.cs
│           └── RoundAmountRuleTests.cs
│
├── Dockerfile
├── docker-compose.yml
├── FraudRuleEngine.sln
└── README.md
```

---

## Key Concepts to Remember for Your Interview

| Concept | What it means |
|---|---|
| **Clean Architecture** | Split code into layers (Core, Application, Infrastructure, API) so each layer has one job |
| **Strategy Pattern** | Use an interface (`IFraudRule`) so you can add new rules without changing existing code |
| **Repository Pattern** | Hide database code behind an interface so you can swap databases without changing business logic |
| **Dependency Injection** | Register services once; .NET provides them automatically wherever needed |
| **EF Core** | Write C# instead of SQL; EF translates it to SQL for you |
| **Migration** | A versioned script that creates/updates database tables |
| **Unit Test** | Small test that checks one piece of logic with known inputs and expected outputs |
| **Docker** | Package your app so it runs identically on any machine |
| **REST API** | A standard way of communicating between systems using HTTP (GET, POST, PUT, DELETE) |

---

*Good luck with your interview! You built this — you understand every piece of it.*
