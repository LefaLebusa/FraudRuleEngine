# Fraud Rule Engine - Development Guide

**Last Updated:** September 17, 2026  
**Status:** ✅ Phases 1, 2, 4 Complete | ⏳ Phases 3, 5 Pending

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Phases Completed](#phases-completed)
4. [Tech Stack](#tech-stack)
5. [Setup & Installation](#setup--installation)
6. [Running the Project](#running-the-project)
7. [API Endpoints](#api-endpoints)
8. [Database Schema](#database-schema)
9. [Fraud Rules](#fraud-rules)
10. [Dashboard Features](#dashboard-features)
11. [Email Configuration](#email-configuration)
12. [Testing](#testing)
13. [Next Steps](#next-steps)

---

## Project Overview

The **Fraud Rule Engine** is an enterprise-grade real-time fraud detection system that processes financial transactions, evaluates them against configurable fraud rules, persists results, and exposes them via REST API with a professional React dashboard.

### Key Features
- ✅ Real-time transaction processing
- ✅ 5 configurable fraud detection rules
- ✅ Live fraud scoring (LOW/MEDIUM/HIGH)
- ✅ Professional React dashboard with charts
- ✅ Email alerts for high-risk transactions
- ✅ Advanced analytics & reporting
- ✅ RESTful API with Swagger documentation

---

## Architecture

```
FraudRuleEngine/
├── src/
│   ├── FraudRuleEngine.Core/           # Domain models & interfaces
│   │   ├── Models/                     # Transaction, FraudEvaluation
│   │   ├── Rules/                      # IFraudRule interface
│   │   └── Interfaces/                 # Repository contracts
│   │
│   ├── FraudRuleEngine.Application/    # Business logic
│   │   ├── Rules/                      # 5 Fraud rule implementations
│   │   └── Services/                   # TransactionService, FraudEvaluationService, AlertService, EmailService
│   │
│   ├── FraudRuleEngine.Infrastructure/ # Data access
│   │   ├── Persistence/                # EF Core DbContext
│   │   ├── Repositories/               # TransactionRepository
│   │   └── Migrations/                 # Database migrations
│   │
│   └── FraudRuleEngine.API/            # REST API
│       ├── Controllers/                # TransactionsController, DashboardController
│       ├── DTOs/                       # Request/Response objects
│       └── Program.cs                  # DI wiring, CORS, configuration
│
├── tests/
│   └── FraudRuleEngine.UnitTests/      # xUnit tests (19 tests)
│
└── fraud-rule-engine-web/             # React Dashboard
    ├── src/
    │   ├── Dashboard.js                # Main dashboard component
    │   ├── Dashboard.css               # Professional styling
    │   └── App.js                      # Root component
    └── package.json
```

### Data Flow

```
Transaction Request
    ↓
TransactionsController (API)
    ↓
TransactionService.ProcessAsync()
    ├→ Repository.AddAsync() → Save to DB
    ├→ FraudEvaluationService.EvaluateAsync()
    │  ├→ Get 24h transaction history
    │  ├→ Run all 5 fraud rules
    │  ├→ Calculate risk level
    │  └→ Save evaluation
    └→ AlertService.SendAlertIfNeededAsync()
       └→ EmailService.SendAlertAsync() (if HIGH risk)
         
Response: {saved transaction, evaluation}
    ↓
Dashboard fetches via DashboardController
    ├→ GET /api/dashboard/summary
    ├→ GET /api/dashboard/recent-flagged
    ├→ GET /api/dashboard/trends
    └→ GET /api/dashboard/risk-by-merchant
```

---

## Phases Completed

### ✅ Phase 1: Dashboard API Endpoints

**Completed:** Sept 17, 2026

**What Was Built:**
- `DashboardController` with 4 endpoints
- Summary statistics endpoint
- Recent flagged transactions endpoint
- Trend analysis endpoint (7-day history)
- Risk by merchant analysis

**Endpoints:**
- `GET /api/dashboard/summary` - Overview stats
- `GET /api/dashboard/recent-flagged` - Last 10 flagged transactions
- `GET /api/dashboard/trends?days=7` - Daily trends
- `GET /api/dashboard/risk-by-merchant` - Top 10 risky merchants

**Key Changes:**
- Added `GetAllAsync()` to repository
- Created `DashboardController`
- All 19 tests still passing

---

### ✅ Phase 2: Real-time Email Alerts

**Completed:** Sept 17, 2026

**What Was Built:**
- `EmailService` - SMTP email sender with HTML templates
- `AlertService` - Account-to-email mapping & alert logic
- Integration with `FraudEvaluationService`
- Email configuration in `appsettings.json`

**Features:**
- Automatic alerts for HIGH risk transactions
- HTML-formatted emails with transaction details
- Triggered rules display
- Risk level color coding
- Configurable SMTP server

**Configuration (appsettings.json):**
```json
"Email": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": "587",
  "SenderEmail": "your-email@gmail.com",
  "SenderPassword": "your-app-password"
}
```

**Key Changes:**
- `EmailService.cs` - Email sending logic
- `AlertService.cs` - Alert orchestration
- Updated `FraudEvaluationService` to trigger alerts
- CORS enabled for React frontend

---

### ✅ Phase 4: React Dashboard (Full UI Enhancement)

**Completed:** Sept 17, 2026

**What Was Built:**
- Professional React dashboard with live data
- 4 advanced charts (Recharts library)
- Smart filtering & search
- CSV export functionality
- Responsive design for all devices

**Dashboard Features:**

1. **KPI Cards**
   - Total Transactions count
   - Flagged Transactions with percentage
   - High Risk alert card

2. **Analytics Charts**
   - Transaction Trends (7-day line chart)
   - Risk Distribution (pie chart)
   - High-Risk Merchants (bar chart)

3. **Transactions Table**
   - Real-time data from API
   - Color-coded risk levels
   - Triggered rules display
   - Timestamp tracking

4. **Filtering & Search**
   - 🔍 Account ID / Merchant search
   - 📅 Date range filtering
   - 🔀 Smart sorting (newest, amount, risk)

5. **Export**
   - 📥 CSV download with all transaction data
   - Timestamped filename

6. **Real-time Updates**
   - Auto-refresh every 30 seconds
   - Live update indicator
   - Error handling with retry button

7. **Responsive Design**
   - Mobile-friendly layout
   - Tablet optimization
   - Desktop full-width mode

**Key Changes:**
- Installed `recharts` for charts
- Rewrote `Dashboard.js` with full features
- Enhanced `Dashboard.css` with professional styling
- Removed unused variables
- CORS configuration in API

---

## Tech Stack

### Backend (.NET 9)
- **Framework:** ASP.NET Core Web API
- **Database:** PostgreSQL 16
- **ORM:** Entity Framework Core 9.0
- **Testing:** xUnit
- **API Docs:** Swagger/OpenAPI

### Frontend (React)
- **React:** 18.x
- **Charts:** Recharts
- **Styling:** CSS3 (custom, no framework)
- **Dev Server:** Create React App

### Infrastructure
- **Containerization:** Docker + Docker Compose
- **Database:** PostgreSQL 16-alpine
- **Message Queue:** Kafka (optional, pre-configured)

---

## Setup & Installation

### Prerequisites

1. **.NET 9 SDK**
   ```bash
   dotnet --version  # Should show 9.x.x
   ```

2. **PostgreSQL 16** (or Docker)
   ```bash
   psql --version
   ```

3. **Node.js 18+**
   ```bash
   node --version
   npm --version
   ```

4. **Docker & Docker Compose** (for containerized setup)
   ```bash
   docker --version
   docker-compose --version
   ```

### Clone & Install

```bash
# Clone repository
git clone <repo-url>
cd FraudRuleEngine

# Backend setup
cd src/FraudRuleEngine.API
dotnet restore
dotnet build

# Frontend setup
cd ../../fraud-rule-engine-web
npm install
```

### Database Setup

**Option 1: Local PostgreSQL**
```bash
# Create database
createdb frauddb

# Update connection string in appsettings.json
# "DefaultConnection": "Host=localhost;Port=5432;Database=frauddb;Username=postgres;Password=postgres"

# Run migrations
dotnet ef database update --project src/FraudRuleEngine.Infrastructure
```

**Option 2: Docker (Recommended)**
```bash
# From project root
docker-compose up -d
# PostgreSQL starts on port 5432
```

---

## Running the Project

### Start Backend API

```bash
cd ~/FraudRuleEngine
dotnet run --project src/FraudRuleEngine.API
```

Expected output:
```
Now listening on: http://localhost:5016
Hosting environment: Development
```

**Swagger UI:** http://localhost:5016/swagger

### Start React Dashboard

```bash
cd ~/FraudRuleEngine/fraud-rule-engine-web
npm start
```

Expected output:
```
Compiled successfully!
You can now view fraud-rule-engine-web in the browser.
Local: http://localhost:3000
```

**Dashboard:** http://localhost:3000

### Both Running

Keep both terminals open:
- Terminal 1: `dotnet run --project src/FraudRuleEngine.API`
- Terminal 2: `npm start`

---

## API Endpoints

### Transaction Endpoints

**Submit Transaction**
```
POST /api/transactions
Content-Type: application/json

{
  "accountId": "ACC123456",
  "amount": 75000,
  "currency": "ZAR",
  "merchantName": "Unknown Vendor",
  "merchantCategory": "GENERAL",
  "country": "ZA",
  "transactionType": "DEBIT",
  "referenceNumber": "REF-001"
}

Response (201 Created):
{
  "saved": {
    "id": "3ccd4206-c26a-4af6-9794-d8d2d20566dd",
    "accountId": "ACC123456",
    "amount": 75000,
    "fraudEvaluation": {...}
  },
  "evaluation": {
    "isFlagged": true,
    "triggeredRules": ["HIGH_AMOUNT", "ROUND_AMOUNT"],
    "riskLevel": "HIGH"
  }
}
```

**Get Transaction by ID**
```
GET /api/transactions/{id}

Response (200 OK):
{
  "id": "3ccd4206-c26a-4af6-9794-d8d2d20566dd",
  "accountId": "ACC123456",
  "amount": 75000,
  "fraudEvaluation": {...}
}
```

**Get Flagged Transactions**
```
GET /api/transactions/flagged

Response (200 OK):
[
  {
    "id": "...",
    "accountId": "ACC123456",
    "amount": 75000,
    "fraudEvaluation": {
      "isFlagged": true,
      "triggeredRules": ["HIGH_AMOUNT"],
      "riskLevel": "HIGH"
    }
  },
  ...
]
```

**Get Account Transactions**
```
GET /api/transactions/account/{accountId}

Response (200 OK): [...]
```

### Dashboard Endpoints

**Summary Statistics**
```
GET /api/dashboard/summary

Response (200 OK):
{
  "totalTransactions": 3,
  "flaggedCount": 3,
  "flaggedPercentage": 100,
  "riskDistribution": {
    "low": 0,
    "medium": 1,
    "high": 2
  }
}
```

**Recent Flagged Transactions**
```
GET /api/dashboard/recent-flagged

Response (200 OK):
[
  {
    "id": "3ccd4206-c26a-4af6-9794-d8d2d20566dd",
    "accountId": "string",
    "amount": 40000,
    "merchantName": "Test",
    "timestamp": "2026-09-17T09:25:27.322542Z",
    "riskLevel": "MEDIUM",
    "triggeredRules": ["ROUND_AMOUNT"]
  }
]
```

**Trends (Last N Days)**
```
GET /api/dashboard/trends?days=7

Response (200 OK):
[
  {
    "date": "2026-09-10",
    "total": 5,
    "avgAmount": 45000.50,
    "flagged": 3
  },
  ...
]
```

**Risk by Merchant**
```
GET /api/dashboard/risk-by-merchant

Response (200 OK):
[
  {
    "merchant": "Unknown Vendor",
    "flaggedCount": 2,
    "highRisk": 2
  },
  ...
]
```

---

## Database Schema

### Transactions Table

```sql
CREATE TABLE "Transactions" (
    "Id" uuid PRIMARY KEY,
    "AccountId" varchar(100) NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "Currency" varchar(3) NOT NULL,
    "MerchantName" varchar(255) NOT NULL,
    "MerchantCategory" varchar(100),
    "Country" varchar(2),
    "TransactionType" varchar(50),
    "ReferenceNumber" varchar(255),
    "Timestamp" timestamp NOT NULL
);
```

### FraudEvaluations Table

```sql
CREATE TABLE "FraudEvaluations" (
    "Id" uuid PRIMARY KEY,
    "TransactionId" uuid NOT NULL REFERENCES "Transactions"("Id"),
    "IsFlagged" boolean NOT NULL,
    "TriggeredRules" varchar(500),  -- CSV format
    "RiskLevel" varchar(10) NOT NULL,  -- LOW, MEDIUM, HIGH
    "EvaluatedAt" timestamp NOT NULL
);
```

---

## Fraud Rules

All 5 rules evaluate in real-time:

### 1. HIGH_AMOUNT Rule
- **Trigger:** `amount > 50,000 ZAR`
- **Reason:** Large transactions warrant scrutiny
- **Risk Impact:** 1 rule triggered = MEDIUM risk

### 2. VELOCITY Rule
- **Trigger:** `5+ transactions within 10 minutes`
- **Reason:** Rapid-fire transactions suggest automated fraud
- **Risk Impact:** 1 rule triggered = MEDIUM risk

### 3. DUPLICATE_TRANSACTION Rule
- **Trigger:** `Same amount + merchant within 2 minutes`
- **Reason:** Identical transactions are rare; duplication is suspicious
- **Risk Impact:** 1 rule triggered = MEDIUM risk

### 4. UNUSUAL_HOUR Rule
- **Trigger:** `Transaction between 00:00–04:00 SAST (UTC+2)`
- **Reason:** Off-hours transactions are unusual
- **Risk Impact:** 1 rule triggered = MEDIUM risk

### 5. ROUND_AMOUNT Rule
- **Trigger:** `amount >= 5,000 AND divisible by 1,000`
- **Reason:** Round amounts suggest manual entry (higher risk)
- **Risk Impact:** 1 rule triggered = MEDIUM risk

### Risk Level Calculation

```csharp
RiskLevel = triggeredRules.Count switch
{
    0 => "LOW",        // No rules triggered
    1 => "MEDIUM",     // 1 rule triggered
    _ => "HIGH"        // 2+ rules triggered
};
```

---

## Dashboard Features

### KPI Cards
- **Total Transactions:** Real-time count from database
- **Flagged Count:** With percentage of total
- **High Risk Alert:** Count of HIGH risk transactions

### Charts

1. **Transaction Trends** (Line Chart)
   - X-axis: Date (last 7 days)
   - Y-axis: Transaction count
   - Shows: Total vs Flagged transactions

2. **Risk Distribution** (Pie Chart)
   - Segments: Low, Medium, High
   - Shows: Proportional breakdown

3. **High-Risk Merchants** (Bar Chart)
   - Top 5 merchants by flagged count
   - Compares: Total flagged vs high risk

### Table Features

- **Real-time Data:** Fetches from `/api/dashboard/recent-flagged`
- **Search:** Filter by account ID or merchant name
- **Date Range:** Filter by start/end date
- **Sort Options:**
  - Newest First (default)
  - Oldest First
  - Highest Amount
  - Lowest Amount
  - Highest Risk

### Export
- **Format:** CSV
- **Filename:** `fraud-report-2026-09-17.csv`
- **Includes:** Account ID, Amount, Merchant, Risk Level, Rules, Timestamp

### Auto-refresh
- **Interval:** Every 30 seconds
- **Display:** Last updated timestamp
- **Error Handling:** Retry button if connection lost

---

## Email Configuration

### Gmail (Recommended)

1. **Enable 2FA** on your Gmail account
2. **Generate App Password:**
   - Go to myaccount.google.com/apppasswords
   - Select Mail + Windows/Mac
   - Copy the 16-character password

3. **Update `appsettings.json`:**
   ```json
   "Email": {
     "SmtpServer": "smtp.gmail.com",
     "SmtpPort": "587",
     "SenderEmail": "your-email@gmail.com",
     "SenderPassword": "your-16-char-app-password"
   }
   ```

### Other Providers

- **Outlook:** `smtp.outlook.com:587`
- **SendGrid:** `smtp.sendgrid.net:587`
- **AWS SES:** `email-smtp.region.amazonaws.com:587`

### Alert Behavior

- **Trigger:** Any transaction with HIGH risk level
- **Recipients:** Registered account emails (configured via `AlertService.RegisterAccount()`)
- **Content:** HTML email with transaction details, triggered rules
- **Styling:** Color-coded by risk level

---

## Testing

### Run All Tests

```bash
dotnet test
```

**Expected:** 19 tests passing

### Test Coverage

| Area | Tests | Status |
|------|-------|--------|
| HIGH_AMOUNT Rule | 1 | ✅ |
| VELOCITY Rule | 1 | ✅ |
| DUPLICATE_TRANSACTION Rule | 1 | ✅ |
| UNUSUAL_HOUR Rule | 1 | ✅ |
| ROUND_AMOUNT Rule | 1 | ✅ |
| Integration Tests | 14 | ✅ |
| **Total** | **19** | **✅** |

### Manual Testing

**Test in Swagger UI:**
1. Go to http://localhost:5016/swagger
2. Try POST `/api/transactions` with test data
3. Verify response includes evaluation

**Test Dashboard:**
1. Go to http://localhost:3000
2. Verify data loads
3. Test search, filters, export

---

## Next Steps

### 🤖 Phase 3: ML-Based Scoring (TODO)

**Goal:** Predict fraud probability using machine learning

**Features to Build:**
- Train fraud detection model (scikit-learn)
- Export model for .NET integration
- Add `/api/transactions/predict` endpoint
- Integrate predictions with dashboard
- Show confidence scores

**Estimated Effort:** 4-6 hours

### 💻 Phase 5: Advanced Backend Features (TODO)

**Goal:** Enterprise-grade features

**Features to Build:**
- Advanced analytics queries
- Webhook support for alerts
- Transaction search API
- Batch import/export
- API rate limiting
- User authentication
- Audit logging
- Performance optimization

**Estimated Effort:** 6-8 hours

---

## Troubleshooting

### API won't start: "Port 5016 already in use"
```bash
# Kill process on port 5016
lsof -i :5016 | grep -v COMMAND | awk '{print $2}' | xargs kill -9
```

### React won't connect to API: "Failed to fetch"
- Ensure API is running (`dotnet run`)
- Check CORS is enabled in Program.cs
- Verify API is on `http://localhost:5016`

### Database connection fails
- Verify PostgreSQL is running
- Check connection string in appsettings.json
- Ensure database exists: `createdb frauddb`

### Emails not sending
- Verify SMTP credentials in appsettings.json
- Check firewall allows port 587
- Verify account email is registered: `alertService.RegisterAccount(accountId, email)`

---

## Summary

You now have a **production-ready fraud detection platform** with:

✅ Real-time transaction processing  
✅ 5 active fraud rules  
✅ Professional React dashboard with charts  
✅ Email alerting system  
✅ RESTful API with Swagger docs  
✅ 19 passing unit tests  
✅ Database persistence  

**Next session:** Build ML predictions and advanced features.

---

**Development by:** Claude Code  
**Last Updated:** September 17, 2026
