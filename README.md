# Fraud Rule Engine Service

A production-grade backend service that processes financial transactions, evaluates them against a configurable set of fraud rules, persists the results, and exposes them via a REST API.

## Tech Stack

- **.NET 9 / C#** — ASP.NET Core Web API
- **PostgreSQL** — data store via EF Core (Npgsql)
- **Docker / docker-compose** — containerised deployment
- **xUnit** — automated unit testing

## Architecture

The solution follows a clean layered architecture:

```
FraudRuleEngine/
├── src/
│   ├── FraudRuleEngine.Core/           # Domain models + IFraudRule interface
│   ├── FraudRuleEngine.Application/    # Fraud rules + business services
│   ├── FraudRuleEngine.Infrastructure/ # EF Core DbContext + repositories
│   └── FraudRuleEngine.API/            # Controllers + DI wiring
└── tests/
    └── FraudRuleEngine.UnitTests/      # xUnit tests for each rule
```

Dependencies flow inward: API → Application → Core ← Infrastructure.

## Fraud Rules

| Rule | Trigger |
|---|---|
| `HIGH_AMOUNT` | Amount > R50 000 |
| `HIGH_VELOCITY` | 5+ transactions within 10 minutes |
| `DUPLICATE_TRANSACTION` | Same amount + merchant within 2 minutes |
| `UNUSUAL_HOUR` | Transaction between 00:00–04:00 SAST |
| `ROUND_AMOUNT` | Amount ≥ R5 000 and divisible by R1 000 |

Risk level is assigned as: 0 rules = LOW, 1 rule = MEDIUM, 2+ rules = HIGH.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/transactions` | Submit a transaction for evaluation |
| `GET` | `/api/transactions/{id}` | Retrieve a transaction by ID |
| `GET` | `/api/transactions/flagged` | List all flagged transactions |
| `GET` | `/api/transactions/account/{accountId}` | List all transactions for an account |

## Build & Run with Docker (Recommended)

**Prerequisites:** Docker Desktop installed and running.

```bash
# 1. Clone the repo
git clone <your-repo-url>
cd FraudRuleEngine

# 2. Build and start all services
docker compose up --build

# 3. Open Swagger UI
# http://localhost:8080/swagger
```

The API and PostgreSQL database start together. The database schema is applied automatically on first run via EF Core migrations.

## Build & Run Locally

**Prerequisites:** .NET 9 SDK, PostgreSQL running locally.

```bash
# 1. Update the connection string in appsettings.json if needed

# 2. Restore and build
dotnet build

# 3. Run the API
dotnet run --project src/FraudRuleEngine.API

# 4. Open Swagger UI
# https://localhost:5001/swagger
```

## Run Tests

```bash
dotnet test
```

Expected output: 19 tests, all passing.

## Example Request

```json
POST /api/transactions
{
  "accountId": "ACC123456",
  "amount": 75000.00,
  "currency": "ZAR",
  "merchantName": "Unknown Vendor",
  "merchantCategory": "GENERAL",
  "country": "ZA",
  "transactionType": "DEBIT",
  "referenceNumber": "REF-001"
}
```

Example response (flagged for HIGH_AMOUNT):

```json
{
  "saved": { "id": "...", "accountId": "ACC123456", "amount": 75000.00, ... },
  "evaluation": {
    "isFlagged": true,
    "triggeredRules": ["HIGH_AMOUNT"],
    "riskLevel": "MEDIUM"
  }
}
```
