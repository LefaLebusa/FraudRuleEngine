# Fraud Detection System - Explained Simply

**A guide to explain this project to non-technical people**

---

## The Big Picture (Simplest Version)

Imagine you're a **bank manager** watching customer transactions. Your job is to:
1. **Watch** for suspicious transactions
2. **Flag** ones that look risky
3. **Show** the flagged transactions on a dashboard
4. **Alert** someone when something is really risky

That's exactly what this system does, but **automatically 24/7**.

---

## How It Works (Real-World Analogy)

Think of it like a **security system at an airport:**

```
Customer submits a transaction
        ↓
System checks: "Is this suspicious?"
        ↓
System compares against rules:
  - Is the amount too high?
  - Are there too many transactions?
  - Is it happening at a weird time?
        ↓
System decides: LOW RISK / MEDIUM RISK / HIGH RISK
        ↓
If HIGH RISK → Send alert email
        ↓
Show on dashboard for person to review
```

---

## The Files Explained Simply

### **BACKEND (The Brain - Makes Decisions)**

#### **Program.cs** - The Startup Script
- **What it does:** Sets up the entire system when it starts
- **Analogy:** Like a restaurant opening for the day - you turn on lights, unlock doors, prepare the kitchen
- **What it sets up:**
  - Starts the database connection
  - Tells the system which rules to use
  - Allows React dashboard to communicate with this server
  - Prepares email sending system

---

#### **TransactionsController.cs** - The Front Desk (Receives Orders)
- **What it does:** Listens for incoming transactions and accepts them
- **Analogy:** Like a front desk at a hotel - receives check-in requests
- **Key functions:**
  - Receives transaction from bank → "Customer ACC123 is sending 50,000 ZAR"
  - Passes it to the brain to check if it's risky
  - Sends back: "Accepted! This transaction is HIGH RISK"

---

#### **DashboardController.cs** - The Information Booth
- **What it does:** Provides information to the React dashboard
- **Analogy:** Like a museum information desk - answers questions
- **Questions it answers:**
  - "How many total transactions today?" → Returns count
  - "Which transactions are risky?" → Returns list
  - "Show me the trend for last 7 days" → Returns chart data
  - "Which merchants are risky?" → Returns top risky stores

---

#### **TransactionService.cs** - The Organizer
- **What it does:** Takes a transaction and organizes it properly
- **Analogy:** Like a post office worker - takes incoming mail, sorts it, routes it correctly
- **Steps it takes:**
  1. Accepts new transaction from TransactionsController
  2. Saves it to the database
  3. Sends it to the fraud detection engine
  4. Gets back the risk score
  5. Sends high-risk alerts
  6. Returns everything to the controller

---

#### **FraudEvaluationService.cs** - The Risk Analyzer
- **What it does:** Decides if a transaction is risky
- **Analogy:** Like a security officer checking if someone looks suspicious
- **What it does:**
  1. Takes the transaction details
  2. Checks it against all 5 fraud rules
  3. Counts how many rules were triggered
  4. Decides: LOW (0 rules), MEDIUM (1 rule), or HIGH (2+ rules)
  5. Records this decision

---

#### **The 5 Fraud Rules** - Security Checklist
Each rule is like a **security guard with one job**:

**1. HIGH_AMOUNT Rule** 🚨
- **Job:** Flag large transactions
- **Rule:** "If amount > 50,000 ZAR → SUSPICIOUS"
- **Why:** Criminals often withdraw large amounts
- **Analogy:** Like flagging customers withdrawing lots of cash

**2. ROUND_AMOUNT Rule** 🔢
- **Job:** Flag round number amounts
- **Rule:** "If amount >= 5,000 AND divisible by 1,000 → SUSPICIOUS"
- **Why:** Real people rarely withdraw exactly 50,000 or 100,000. Criminals do.
- **Example:** 50,000 ✗ (suspicious), 50,500 ✓ (normal)
- **Analogy:** Like flagging ATM withdrawals of exact round numbers

**3. VELOCITY Rule** ⚡
- **Job:** Flag if someone is rushing
- **Rule:** "If 5+ transactions in 10 minutes → SUSPICIOUS"
- **Why:** Normal people don't send 5 payments in 10 minutes. Hackers do.
- **Analogy:** Like a cashier noticing someone buying 5 items in 10 seconds

**4. DUPLICATE_TRANSACTION Rule** 🔄
- **Job:** Flag repeated identical transactions
- **Rule:** "Same amount + same merchant within 2 minutes → SUSPICIOUS"
- **Why:** Same transaction twice = mistake or fraud
- **Analogy:** Like charging someone twice by accident

**5. UNUSUAL_HOUR Rule** 🌙
- **Job:** Flag transactions at weird times
- **Rule:** "If transaction between 00:00–04:00 SAST (midnight-4am) → SUSPICIOUS"
- **Why:** Normal people don't bank at 2am. Criminals do.
- **Analogy:** Like a store noticing a massive purchase at 3am

---

#### **AlertService.cs** - The Notifier
- **What it does:** Decides if someone should be notified
- **Analogy:** Like a smoke detector - if it detects smoke, it alerts you
- **Logic:**
  - If transaction is HIGH RISK → Send email alert
  - If transaction is MEDIUM/LOW → No alert needed
  - Knows which email to send to (accountId → email mapping)

---

#### **EmailService.cs** - The Messenger
- **What it does:** Sends emails
- **Analogy:** Like a postal service
- **What it sends:**
  - Beautiful HTML email (not plain text)
  - Transaction details
  - Which fraud rules were triggered
  - Color-coded by risk level

---

#### **TransactionRepository.cs** - The Librarian
- **What it does:** Stores and retrieves transactions from the database
- **Analogy:** Like a library - you ask it "Do you have this book?" or "Store this new book"
- **Key tasks:**
  - Saves new transactions to the database
  - Retrieves transactions for analysis
  - Gets transaction history (for VELOCITY rule)

---

#### **FraudDbContext.cs** - The Database Blueprint
- **What it does:** Defines how the database is organized
- **Analogy:** Like a filing cabinet system - it says "This drawer has these folders"
- **What it organizes:**
  - Transactions table (stores all transactions)
  - FraudEvaluations table (stores risk assessments)

---

#### **appsettings.json** - The Configuration File
- **What it does:** Stores settings (like options you can change)
- **Analogy:** Like a restaurant menu board - you can change prices
- **What it stores:**
  - Database connection details (where to connect)
  - Email configuration (SMTP server, username, password)
  - Logging preferences

---

### **FRONTEND (The Display - Shows Results)**

#### **App.js** - The Home Page
- **What it does:** Main container that loads the dashboard
- **Analogy:** Like a website homepage that links to all pages
- **Simple job:** Shows the Dashboard component

---

#### **Dashboard.js** - The Monitoring Screen
- **What it does:** The entire display that shows fraud data
- **Analogy:** Like a security guard's monitoring screen at a mall
- **What it shows:**

**1. Header Section**
- Big title: "Fraud Rule Engine Dashboard"
- Shows: "Real-time transaction monitoring"
- Shows: "Last updated: 3:45 PM"

**2. KPI Cards (Key Performance Indicators)**
- Like a scoreboard at a sports game
- Shows:
  - Total Transactions: 42 (total count)
  - Flagged Transactions: 12 (28%)
  - High Risk Count: 3

**3. Charts (Visual Displays)**
- **Line Chart:** Shows transactions over 7 days
  - X-axis: Days (Mon, Tue, Wed...)
  - Y-axis: Transaction count
  - Shows both total and flagged lines
  - Analogy: Like a stock market chart

- **Pie Chart:** Risk distribution
  - Shows: 30 Low-risk, 8 Medium-risk, 4 High-risk
  - Colors: Green (safe), Yellow (caution), Red (danger)
  - Analogy: Like a weather pie chart

- **Bar Chart:** Risky merchants
  - Shows: Which stores have most fraud
  - Analogy: Like a "Most dangerous locations" chart

**4. Search & Filter Section**
- Search box: "Find by account ID or merchant name"
- Date range: "Show me transactions from Jan 1 to Jan 31"
- Sort dropdown: "Sort by newest, oldest, highest amount, highest risk"
- Analogy: Like searching a spreadsheet

**5. Results Table**
- Shows columns: Account ID, Amount, Merchant, Risk Level, Rules Triggered, Timestamp
- Color-coded rows (green, yellow, red borders)
- Analogy: Like a spreadsheet of suspicious transactions

**6. Export Button**
- Downloads all data as CSV (Excel file)
- Analogy: Like "Save to file" in Word

---

#### **Dashboard.css** - The Styling (Design/Colors)
- **What it does:** Makes the dashboard look professional
- **Analogy:** Like interior design - it makes things pretty
- **What it controls:**
  - Colors (purple header, green/yellow/red risk levels)
  - Fonts and sizes
  - Spacing between elements
  - Animation effects (like a loading spinner)
  - Mobile layout (how it looks on phones)

---

### **DATABASE (The Storage - Saves Everything)**

#### **PostgreSQL Database** - The Filing Cabinet
- **What it does:** Stores all transactions permanently
- **Analogy:** Like a filing cabinet at a bank
- **What it stores:**
  - Every transaction that comes in
  - Risk assessment for each transaction
  - Timestamp (when it happened)

**Two Main Tables:**
1. **Transactions Table**
   - Stores: Account ID, Amount, Merchant, Timestamp
   - Like: A bank ledger

2. **FraudEvaluations Table**
   - Stores: Risk Level, Rules Triggered, Timestamp
   - Like: A supervisor's notes on the ledger

---

## How Everything Works Together

### **The Flow (Step-by-Step)**

```
1. BANK SUBMITS TRANSACTION
   ↓
2. TransactionsController receives it
   "You have a new transaction from ACC123"
   ↓
3. TransactionService takes charge
   - Saves to database ✓
   - Passes to FraudEvaluationService
   ↓
4. FraudEvaluationService analyzes
   - Checks 5 rules
   - Decides: HIGH RISK (2 rules triggered)
   ↓
5. AlertService checks
   - "Is it HIGH RISK?" Yes!
   - "Send alert email"
   ↓
6. EmailService sends
   - Sends beautiful HTML email
   - "Transaction flagged as HIGH RISK"
   ↓
7. React Dashboard updates
   - New transaction appears in table
   - Chart updates
   - KPI cards update
   ↓
8. Security person sees alert
   - Email inbox: New fraud alert
   - Logs into dashboard
   - Reviews the transaction
```

---

## The Two Parts of the System

### **Backend (The Brain)**
- **Lives on:** A server (localhost:5016)
- **Built with:** .NET (Microsoft technology)
- **Does:** Analyzes transactions, makes decisions, sends emails
- **Analogy:** Like a bank's decision-making computer in the back office

### **Frontend (The Face)**
- **Lives on:** Browser (localhost:3000)
- **Built with:** React (Facebook technology)
- **Does:** Shows beautiful dashboard, accepts user input
- **Analogy:** Like the teller window at a bank - customers see this

### **How They Talk**
- React asks: "Hey, give me today's flagged transactions"
- API responds: "Here's the data" (JSON format)
- React displays it beautifully
- Analogy: Like customer calling the bank and asking "What's my balance?"

---

## Real-World Scenario

**Scenario: A customer tries to withdraw 100,000 ZAR at 2am**

```
TIME 02:15 AM
↓
Transaction arrives: "ACC789, amount: 100,000, merchant: ATM"
↓
System analyzes:
  Rule 1 (HIGH_AMOUNT): 100,000 > 50,000? YES ✓
  Rule 2 (ROUND_AMOUNT): Divisible by 1,000? YES ✓
  Rule 3 (VELOCITY): 5+ in 10 min? NO
  Rule 4 (DUPLICATE): Same as recent? NO
  Rule 5 (UNUSUAL_HOUR): 2am? YES ✓
↓
Score: 3 rules triggered → HIGH RISK
↓
Decision: Send alert email
↓
Email sent to security team:
  "HIGH RISK TRANSACTION DETECTED
   Account: ACC789
   Amount: 100,000 ZAR
   Merchant: ATM
   Rules: HIGH_AMOUNT, ROUND_AMOUNT, UNUSUAL_HOUR
   Action: REVIEW IMMEDIATELY"
↓
Dashboard updates in real-time
  - New row in table (red border)
  - "High Risk" badge
  - Chart updates
↓
Security person sees email and logs into dashboard
  - Reviews transaction details
  - Can approve or block it
```

---

## What Makes This System Special

### ✅ **Automatic**
- No one has to manually review every transaction
- Like: Antivirus software - runs automatically

### ✅ **Real-Time**
- Alerts sent immediately (not hours later)
- Like: Security camera (live, not recorded)

### ✅ **Intelligent**
- Uses 5 smart rules that catch different fraud patterns
- Like: Security guard trained to spot many types of threats

### ✅ **Visual**
- Beautiful dashboard with charts and graphs
- Like: Control room with screens (not boring spreadsheets)

### ✅ **Configurable**
- Can easily change rules, thresholds, email settings
- Like: Security system you can reprogram

---

## The Three Phases (Completed)

### **Phase 1: Dashboard API** ✅
- **What:** Created the backend engine that analyzes transactions
- **Why:** Needed a way to receive and evaluate transactions
- **Result:** System that flags risky transactions automatically

### **Phase 2: Email Alerts** ✅
- **What:** Added automatic email notifications
- **Why:** Security team needs to know immediately
- **Result:** HIGH-risk transactions trigger email alerts

### **Phase 3 & 4 Coming:** ML Predictions & Advanced Features
- **Phase 3:** Add AI/machine learning for better predictions
- **Phase 5:** Add advanced features (search, filtering, webhooks)

---

## Explaining to Your Boss

**Use this simple version:**

> "We built an automated fraud detection system. It monitors all transactions 24/7 against 5 security rules. When it detects suspicious activity, it immediately:
> 1. Flags the transaction (HIGH/MEDIUM/LOW risk)
> 2. Sends an email alert to our team
> 3. Shows it on a dashboard for review
>
> This catches fraud much faster than manual review, and the dashboard gives us real-time visibility into fraud patterns."

---

## Explaining to Non-Technical Friend

**Use this analogy:**

> "Imagine a bouncer at a nightclub. He has a checklist of things that look suspicious:
> - Is the person too drunk?
> - Are they acting weird?
> - Did they just get in and try to leave immediately?
> - Are they with someone who just caused trouble?
>
> If the person checks multiple boxes, he stops them and calls security. That's what our system does - it's an automated bouncer for bank transactions."

---

## Quick Reference

| Thing | Simple Explanation |
|-------|-------------------|
| **Program.cs** | The startup script |
| **TransactionsController** | Front door that receives transactions |
| **FraudEvaluationService** | Brain that decides if transaction is risky |
| **5 Fraud Rules** | 5 security guards, each checking one thing |
| **EmailService** | Sends alerts via email |
| **Dashboard.js** | The visual display |
| **PostgreSQL** | The filing cabinet where everything is stored |
| **React** | The website interface |
| **API** | The communication line between React and backend |

---

## How to Explain Tomorrow

**Start with this:**
"This is a fraud detection system. Let me explain what each part does using everyday examples..."

**Then use:**
1. The airport security analogy (for overall concept)
2. Real-world scenario (100k at 2am example)
3. The two parts: Brain (backend) and Face (frontend)
4. The bouncer analogy (for technical audience)

---

**Good luck with your presentation!** 🎯

