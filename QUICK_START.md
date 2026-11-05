# Fraud Rule Engine - Quick Start Guide

**Get the whole system running in 5 minutes**

---

## Prerequisites

- ✅ .NET 9 installed
- ✅ Node.js 18+ installed
- ✅ PostgreSQL running (or Docker)
- ✅ API code at `~/Desktop/Projects/FraudRuleEngine`
- ✅ React frontend at `~/Desktop/Projects/FraudRuleEngine/fraud-rule-engine-web`

---

## Step 1: Start PostgreSQL (if not running)

**Option A: Docker (Recommended)**
```bash
docker-compose up -d
# PostgreSQL will start on localhost:5432
```

**Option B: Local PostgreSQL**
```bash
psql -h localhost -U postgres
# Enter password (default: postgres)
```

---

## Step 2: Start the API (Terminal 1)

```bash
cd ~/Library/CloudStorage/OneDrive-CapitecBankLtd/Desktop/Projects/FraudRuleEngine
dotnet run --project src/FraudRuleEngine.API
```

**Wait for:**
```
Now listening on: http://localhost:5016
```

✅ API is ready

---

## Step 3: Start React Dashboard (Terminal 2)

```bash
cd ~/Library/CloudStorage/OneDrive-CapitecBankLtd/Desktop/Projects/FraudRuleEngine/fraud-rule-engine-web
npm start
```

**Wait for:**
```
Compiled successfully!
You can now view fraud-rule-engine-web in the browser.
Local: http://localhost:3000
```

✅ React is ready

---

## Step 4: Open Dashboard

**Go to:** `http://localhost:3000`

You should see:
- 📊 Dashboard with real-time data
- 💎 KPI cards (Total Transactions, Flagged, High Risk)
- 📈 Charts (Trends, Risk Distribution, Merchant Risk)
- 🔍 Search & filters
- 📥 CSV export button

---

## Step 5: Test with Thunder Client

1. **Open Thunder Client** (or Postman)
2. **Create new request:**
   - Method: `POST`
   - URL: `http://localhost:5016/api/transactions`

3. **Headers:**
   ```
   Content-Type: application/json
   ```

4. **Body (JSON):**
   ```json
   {
     "accountId": "ACC12345",
     "amount": 55000,
     "currency": "ZAR",
     "merchantName": "Test Merchant",
     "merchantCategory": "GENERAL",
     "country": "ZA",
     "transactionType": "DEBIT",
     "referenceNumber": "TEST-001"
   }
   ```

5. **Send** → Check response in Thunder Client

6. **Refresh React dashboard** → Transaction appears with alerts!

---

## System Status Check

### ✅ API Running?
```bash
curl http://localhost:5016/swagger
```
Should show Swagger UI

### ✅ Database Connected?
```bash
curl http://localhost:5016/api/transactions/flagged
```
Should return JSON array (even if empty)

### ✅ React Dashboard?
Go to `http://localhost:3000` → Should display dashboard

### ✅ All 3 Components?
```
API ✓ | React ✓ | Database ✓ = Full System Running! 🎉
```

---

## Email Alerts (Optional Setup)

If you want **email alerts for HIGH risk transactions**:

1. **Get Gmail App Password:**
   - Go to myaccount.google.com/apppasswords
   - Select Mail + Device
   - Copy the 16-char password

2. **Update `appsettings.json`:**
   ```json
   "Email": {
     "SmtpServer": "smtp.gmail.com",
     "SmtpPort": "587",
     "SenderEmail": "your-email@gmail.com",
     "SenderPassword": "your-16-char-password"
   }
   ```

3. **Restart API**

4. **Register account email** (in AlertService):
   - HIGH risk transactions will now trigger email alerts

---

## Common Issues

### ❌ "Can't reach localhost:5016"
- Check API terminal for errors
- Restart: Press `Ctrl+C`, then `dotnet run --project src/FraudRuleEngine.API`

### ❌ "Connection refused" on React
- Ensure both API and React are running
- Hard refresh browser: `Cmd + Shift + R`

### ❌ "Database connection failed"
- Verify PostgreSQL is running: `docker ps` (should show postgres container)
- Or check local PostgreSQL: `psql -h localhost -U postgres`

### ❌ Port 5016 already in use
```bash
lsof -i :5016 | grep -v COMMAND | awk '{print $2}' | xargs kill -9
```

### ❌ Port 3000 already in use
```bash
lsof -i :3000 | grep -v COMMAND | awk '{print $2}' | xargs kill -9
```

---

## Dashboard Tour

### Top Section
- **Header:** "Fraud Rule Engine Dashboard"
- **Last Updated:** Shows when data was last refreshed
- **Loading Indicator:** Spinner while updating

### KPI Cards
- **Total Transactions:** Count of all transactions
- **Flagged Transactions:** Count + percentage
- **High Risk Alert:** Count of HIGH risk transactions

### Charts
- **Transaction Trends:** 7-day line chart
- **Risk Distribution:** Pie chart (Low/Medium/High)
- **High-Risk Merchants:** Top merchants by flagged count

### Transactions Table
- **Search:** By account ID or merchant name
- **Date Range:** Filter by start and end date
- **Sort:** By newest, oldest, highest amount, lowest amount, highest risk
- **Risk Badges:** Color-coded (Green=Low, Orange=Medium, Red=High)
- **Rules Column:** Shows which rules were triggered
- **Export:** Download as CSV

---

## Features Overview

### ✅ Phases Completed

1. **Dashboard API** ✓
   - Summary statistics
   - Recent flagged transactions
   - 7-day trends
   - Risk by merchant

2. **Email Alerts** ✓
   - Automatic alerts for HIGH risk
   - HTML-formatted emails
   - Configurable SMTP

3. **React Dashboard** ✓
   - Real-time data
   - 4 analytics charts
   - Search & filtering
   - CSV export
   - Responsive design

### 📋 Fraud Rules (All Active)

1. **HIGH_AMOUNT** → amount > 50,000 ZAR
2. **ROUND_AMOUNT** → amount >= 5,000 AND divisible by 1,000
3. **VELOCITY** → 5+ transactions in 10 minutes
4. **DUPLICATE_TRANSACTION** → Same amount + merchant within 2 minutes
5. **UNUSUAL_HOUR** → Transaction between 00:00–04:00 SAST

### 🎯 Risk Levels

- **LOW:** 0 rules triggered
- **MEDIUM:** 1 rule triggered
- **HIGH:** 2+ rules triggered

---

## API Endpoints Quick Reference

| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/transactions` | Submit transaction |
| GET | `/api/transactions/flagged` | Get all flagged |
| GET | `/api/transactions/{id}` | Get by ID |
| GET | `/api/transactions/account/{id}` | Get account txns |
| GET | `/api/dashboard/summary` | Dashboard stats |
| GET | `/api/dashboard/recent-flagged` | Recent flagged txns |
| GET | `/api/dashboard/trends?days=7` | 7-day trends |
| GET | `/api/dashboard/risk-by-merchant` | Merchant risk |

---

## File Structure

```
FraudRuleEngine/
├── src/
│   ├── FraudRuleEngine.API/
│   │   ├── Controllers/
│   │   │   ├── TransactionsController.cs
│   │   │   └── DashboardController.cs
│   │   ├── Program.cs (CORS, DI wiring)
│   │   └── appsettings.json (Email config)
│   │
│   ├── FraudRuleEngine.Application/
│   │   ├── Rules/ (5 fraud rules)
│   │   └── Services/ (Transaction, Fraud, Alert, Email)
│   │
│   ├── FraudRuleEngine.Core/
│   │   ├── Models/ (Transaction, FraudEvaluation)
│   │   └── Interfaces/
│   │
│   └── FraudRuleEngine.Infrastructure/
│       ├── Persistence/ (DbContext)
│       └── Repositories/ (TransactionRepository)
│
├── fraud-rule-engine-web/
│   ├── src/
│   │   ├── Dashboard.js (Main component)
│   │   ├── Dashboard.css (Professional styling)
│   │   └── App.js (Root)
│   └── package.json
│
├── DEVELOPMENT_GUIDE.md (Complete documentation)
├── API_REFERENCE.md (API endpoints)
├── QUICK_START.md (This file)
└── docker-compose.yml (PostgreSQL + Kafka)
```

---

## Testing Checklist

- [ ] API running on localhost:5016
- [ ] React dashboard on localhost:3000
- [ ] Dashboard shows data
- [ ] Can submit transaction via Thunder Client
- [ ] Transaction appears in dashboard
- [ ] Can search & filter transactions
- [ ] Can export to CSV
- [ ] Chart data renders
- [ ] Auto-refresh works (30-second interval)

---

## Next Steps (Tomorrow)

### 🤖 Phase 3: ML-Based Scoring
- Train fraud detection model
- Integrate predictions with API
- Show confidence scores in dashboard

### 💻 Phase 5: Advanced Backend
- Advanced search queries
- Webhook support
- API rate limiting
- User authentication
- Performance optimization

---

## Documentation Files

1. **DEVELOPMENT_GUIDE.md** → Complete detailed reference
2. **API_REFERENCE.md** → API endpoints + examples
3. **QUICK_START.md** → This file (5-minute setup)

---

## Support

- **API Issues:** Check `/swagger` for documentation
- **React Issues:** Open browser console (F12) for errors
- **Database Issues:** Check Docker containers `docker ps`
- **Port Issues:** Kill process: `lsof -i :[port] | grep -v COMMAND | awk '{print $2}' | xargs kill -9`

---

**You're all set! 🚀**

Start the 2 terminals and visit `http://localhost:3000`

