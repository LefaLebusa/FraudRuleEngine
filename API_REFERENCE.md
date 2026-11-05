# Fraud Rule Engine - API Reference

**Quick command testing guide for Thunder Client or Postman**

---

## Base URL

```
http://localhost:5016/api
```

---

## 1. Submit Transaction

**Endpoint:** `POST /transactions`

**Request:**
```json
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
```

**Response (201 Created):**
```json
{
  "saved": {
    "id": "3ccd4206-c26a-4af6-9794-d8d2d20566dd",
    "accountId": "ACC123456",
    "amount": 75000,
    "currency": "ZAR",
    "merchantName": "Unknown Vendor",
    "merchantCategory": "GENERAL",
    "country": "ZA",
    "transactionType": "DEBIT",
    "referenceNumber": "REF-001",
    "timestamp": "2026-09-17T09:25:27.322542Z"
  },
  "evaluation": {
    "isFlagged": true,
    "triggeredRules": ["HIGH_AMOUNT", "ROUND_AMOUNT"],
    "riskLevel": "HIGH"
  }
}
```

**Triggered Rules That Might Apply:**
- `HIGH_AMOUNT` → amount > 50,000 ZAR
- `ROUND_AMOUNT` → amount >= 5,000 AND divisible by 1,000
- `VELOCITY` → 5+ transactions in 10 minutes
- `DUPLICATE_TRANSACTION` → Same amount + merchant within 2 minutes
- `UNUSUAL_HOUR` → Transaction between 00:00–04:00 SAST

---

## 2. Get All Flagged Transactions

**Endpoint:** `GET /transactions/flagged`

**Response (200 OK):**
```json
[
  {
    "id": "3ccd4206-c26a-4af6-9794-d8d2d20566dd",
    "accountId": "ACC123456",
    "amount": 75000,
    "currency": "ZAR",
    "merchantName": "Unknown Vendor",
    "merchantCategory": "GENERAL",
    "country": "ZA",
    "transactionType": "DEBIT",
    "referenceNumber": "REF-001",
    "timestamp": "2026-09-17T09:25:27.322542Z",
    "fraudEvaluation": {
      "id": "uuid",
      "isFlagged": true,
      "triggeredRules": "HIGH_AMOUNT,ROUND_AMOUNT",
      "riskLevel": "HIGH",
      "evaluatedAt": "2026-09-17T09:25:27.322542Z"
    }
  }
]
```

---

## 3. Get Transaction by ID

**Endpoint:** `GET /transactions/{id}`

**Example:** `GET /transactions/3ccd4206-c26a-4af6-9794-d8d2d20566dd`

**Response (200 OK):**
```json
{
  "id": "3ccd4206-c26a-4af6-9794-d8d2d20566dd",
  "accountId": "ACC123456",
  "amount": 75000,
  "currency": "ZAR",
  "merchantName": "Unknown Vendor",
  "merchantCategory": "GENERAL",
  "country": "ZA",
  "transactionType": "DEBIT",
  "referenceNumber": "REF-001",
  "timestamp": "2026-09-17T09:25:27.322542Z",
  "fraudEvaluation": {
    "id": "uuid",
    "isFlagged": true,
    "triggeredRules": "HIGH_AMOUNT,ROUND_AMOUNT",
    "riskLevel": "HIGH",
    "evaluatedAt": "2026-09-17T09:25:27.322542Z"
  }
}
```

---

## 4. Get Account Transactions

**Endpoint:** `GET /transactions/account/{accountId}`

**Example:** `GET /transactions/account/ACC123456`

**Response (200 OK):**
```json
[
  {
    "id": "...",
    "accountId": "ACC123456",
    "amount": 75000,
    ...
  }
]
```

---

## 5. Dashboard Summary

**Endpoint:** `GET /api/dashboard/summary`

**Response (200 OK):**
```json
{
  "totalTransactions": 10,
  "flaggedCount": 3,
  "flaggedPercentage": 30,
  "riskDistribution": {
    "low": 7,
    "medium": 2,
    "high": 1
  }
}
```

---

## 6. Recent Flagged Transactions (Dashboard)

**Endpoint:** `GET /api/dashboard/recent-flagged`

**Response (200 OK):**
```json
[
  {
    "id": "3ccd4206-c26a-4af6-9794-d8d2d20566dd",
    "accountId": "ACC123456",
    "amount": 75000,
    "merchantName": "Unknown Vendor",
    "timestamp": "2026-09-17T09:25:27.322542Z",
    "riskLevel": "HIGH",
    "triggeredRules": ["HIGH_AMOUNT", "ROUND_AMOUNT"]
  }
]
```

---

## 7. Transaction Trends (7-day history)

**Endpoint:** `GET /api/dashboard/trends?days=7`

**Response (200 OK):**
```json
[
  {
    "date": "2026-09-10",
    "total": 5,
    "avgAmount": 45000.50,
    "flagged": 2
  },
  {
    "date": "2026-09-11",
    "total": 3,
    "avgAmount": 50000.00,
    "flagged": 1
  }
]
```

---

## 8. Risk by Merchant (Top merchants)

**Endpoint:** `GET /api/dashboard/risk-by-merchant`

**Response (200 OK):**
```json
[
  {
    "merchant": "Unknown Vendor",
    "flaggedCount": 2,
    "highRisk": 1
  },
  {
    "merchant": "Test Store",
    "flaggedCount": 1,
    "highRisk": 0
  }
]
```

---

## Testing Examples

### Test 1: Simple LOW Risk Transaction

```bash
curl -X POST http://localhost:5016/api/transactions \
  -H "Content-Type: application/json" \
  -d '{
    "accountId": "ACC999",
    "amount": 1000,
    "currency": "ZAR",
    "merchantName": "Safe Store",
    "merchantCategory": "RETAIL"
  }'
```

**Expected:** `"riskLevel": "LOW"`

---

### Test 2: HIGH Risk (High Amount + Round)

```bash
curl -X POST http://localhost:5016/api/transactions \
  -H "Content-Type: application/json" \
  -d '{
    "accountId": "ACC888",
    "amount": 50000,
    "currency": "ZAR",
    "merchantName": "Big Store",
    "merchantCategory": "GENERAL"
  }'
```

**Expected:** `"riskLevel": "HIGH"`, `"triggeredRules": ["HIGH_AMOUNT", "ROUND_AMOUNT"]`

---

### Test 3: MEDIUM Risk (UNUSUAL_HOUR)

Submit a transaction between 00:00–04:00 SAST (UTC+2):

```bash
curl -X POST http://localhost:5016/api/transactions \
  -H "Content-Type: application/json" \
  -d '{
    "accountId": "ACC777",
    "amount": 5000,
    "currency": "ZAR",
    "merchantName": "Night Shop"
  }'
```

**Expected (if time 00:00–04:00):** `"riskLevel": "MEDIUM"`, `"triggeredRules": ["UNUSUAL_HOUR"]`

---

## Swagger UI

**Interactive API docs available at:**
```
http://localhost:5016/swagger
```

Try endpoints directly from your browser!

---

## Error Responses

### 400 Bad Request
```json
{
  "errors": {
    "accountId": ["The AccountId field is required."]
  },
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400
}
```

### 404 Not Found
```json
{
  "message": "Transaction not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "An error occurred processing your request"
}
```

---

## CORS Headers

All requests from React (localhost:3000) automatically include:
```
Access-Control-Allow-Origin: http://localhost:3000
Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS
Access-Control-Allow-Headers: Content-Type, Authorization
```

---

## Environment Variables

See `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=frauddb;Username=postgres;Password=postgres"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password"
  }
}
```

---

## Quick Debug Commands

**Check if API is running:**
```bash
curl http://localhost:5016/swagger
```

**Get all flagged transactions:**
```bash
curl http://localhost:5016/api/transactions/flagged
```

**Get dashboard summary:**
```bash
curl http://localhost:5016/api/dashboard/summary
```

---

**Created:** September 17, 2026  
**Status:** ✅ Complete
