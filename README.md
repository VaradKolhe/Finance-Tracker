# Finance Tracker

## Introduction

Finance Tracker is a full-stack personal finance web application that helps users manage income, expenses, budgets, and reports from one place. It is designed as a smart budgeting platform with analytics, alerts, and AI-powered spending analysis.

The project is built with:

- Frontend: React.js, TypeScript, Material UI, React Router, Axios, Recharts
- Backend: ASP.NET Core Web API, C#, Entity Framework Core, Serilog
- Database: MySQL
- Authentication: JWT-based authentication with bcrypt password hashing
- AI Integration: OpenAI Responses API with a built-in fallback mock analyzer when no API key is configured

---

## Core Features

### User Features

- Register, login, and logout securely using JWT authentication
- Manage profile details
  - first name and last name
  - monthly income
  - financial goal
  - currency code
  - dark mode preference
- Add, edit, and delete transactions
  - amount
  - type: income or expense
  - category
  - date
  - notes
  - recurring transaction fields
- Use predefined categories and create custom personal categories
- View a modern dashboard with
  - total income
  - total expenses
  - net savings
  - savings rate
  - category-wise spending breakdown
  - monthly trends
  - recent transactions
  - budget progress
  - notifications
- Create and manage monthly category budgets
- Receive alerts for
  - budget exceeded
  - unusual spending spikes
- Generate weekly and monthly reports
- Export reports to PDF
- View AI-generated spending insights and saving recommendations

### Admin Features

- Access admin-only dashboard
- View all registered users
- View system-wide analytics
  - total users
  - total transactions
  - total categories
  - total income
  - total expenses
  - active alerts
- Create and manage system-wide categories

---

## AI Spending Analyzer

The application supports two AI modes:

### 1. Live OpenAI Integration

If an OpenAI API key is configured, the backend sends the user's last 30 days of transactions to the OpenAI Responses API and asks for:

- spending summary
- spending behavior insights
- saving recommendations

Default configured model:

- `gpt-5.4-mini`

### 2. Mock AI Fallback

If no API key is provided, the system automatically falls back to a built-in deterministic analyzer. This is not random text. It uses transaction patterns such as:

- top spending categories
- category share percentages
- expense-to-income ratio
- food-heavy spending
- overspending concentration

This ensures the AI section still works even without an OpenAI key.

---

## Project Structure

```text
Finance-Tracker/
├── backend/
│   ├── src/
│   │   ├── AiBudgetSpendingAnalyzer.Domain/
│   │   ├── AiBudgetSpendingAnalyzer.Application/
│   │   ├── AiBudgetSpendingAnalyzer.Infrastructure/
│   │   └── AiBudgetSpendingAnalyzer.API/
│   └── tests/
├── frontend/
├── database/
│   └── schema.sql
└── README.md
```

---

## Backend Architecture

The backend follows a clean architecture-inspired structure:

- Controllers
  - define REST endpoints
- Services
  - contain business logic
- Repositories
  - handle database access
- DTOs
  - define API request and response contracts
- Domain
  - contains entities and enums

Additional backend features:

- JWT authentication and role authorization
- global exception handling middleware
- FluentValidation-based input validation
- Serilog request and application logging
- MySQL database access through EF Core

---

## Database Design

Main tables:

- Users
- Transactions
- Categories
- Budgets
- Notifications

Relationships:

- User -> Transactions: 1:N
- User -> Budgets: 1:N
- Category -> Transactions: 1:N
- User -> Notifications: 1:N

SQL schema file:

- `database/schema.sql`

---

## API Highlights

### Authentication

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/logout`

### User

- `GET /api/profile`
- `PUT /api/profile`

### Transactions

- `GET /api/transactions`
- `POST /api/transactions`
- `PUT /api/transactions/{id}`
- `DELETE /api/transactions/{id}`

### Categories

- `GET /api/categories`
- `POST /api/categories`
- `PUT /api/categories/{id}`
- `DELETE /api/categories/{id}`

### Budgets

- `GET /api/budgets`
- `POST /api/budgets`
- `PUT /api/budgets/{id}`
- `DELETE /api/budgets/{id}`

### Dashboard & AI

- `GET /api/dashboard`
- `GET /api/ai-analysis`
- `GET /api/notifications`
- `PUT /api/notifications/{id}/read`

### Reports

- `POST /api/reports/generate`

### Admin

- `GET /api/admin/dashboard`

Swagger is available after backend startup at:

- `http://localhost:5067/swagger`

---

## Seeded Admin Account

When the backend starts for the first time, it seeds an admin user:

- Email: `admin@budgetai.local`
- Password: `Admin@12345`

It also seeds default system categories like Salary, Food, Housing, Transport, Utilities, Entertainment, Healthcare, Savings, Shopping, and Travel.

---

## Prerequisites

Before running the project, install:

- .NET SDK 7 or newer
- Node.js 22 or newer
- npm 11 or newer
- MySQL Server 8
- Git

---

## Complete Setup And Run Guide

These steps assume a new user has cloned the repository.

### 1. Clone The Repository

```powershell
git clone https://github.com/om9494/Finance-Tracker.git
cd Finance-Tracker
```

### 2. Create The Database

Open PowerShell in the project root:

```powershell
cd C:\path\to\Finance-Tracker
mysql -u root -proot -e "CREATE DATABASE IF NOT EXISTS ai_budget_spending_analyzer_dev;"
```

If your MySQL username or password is different, update:

- `backend/src/AiBudgetSpendingAnalyzer.API/appsettings.Development.json`

You can also import the schema manually if needed:

```powershell
mysql -u root -proot ai_budget_spending_analyzer_dev < .\database\schema.sql
```

### 3. Run The Backend

Open a new PowerShell terminal:

```powershell
cd C:\path\to\Finance-Tracker\backend
dotnet restore
dotnet run --launch-profile http --project .\src\AiBudgetSpendingAnalyzer.API\AiBudgetSpendingAnalyzer.API.csproj
```

Backend URLs:

- API: `http://localhost:5067`
- Swagger: `http://localhost:5067/swagger`

### 4. Run The Frontend

Open another PowerShell terminal:

```powershell
cd C:\path\to\Finance-Tracker\frontend
npm install
npm run dev -- --host 127.0.0.1
```

Frontend URL:

- `http://127.0.0.1:5173`

### 5. Login

Use the seeded admin account:

- Email: `admin@budgetai.local`
- Password: `Admin@12345`

Or register a new normal user from the Register page.

---

## Environment Notes

### Backend Configuration

Main backend config files:

- `backend/src/AiBudgetSpendingAnalyzer.API/appsettings.json`
- `backend/src/AiBudgetSpendingAnalyzer.API/appsettings.Development.json`

Things you may want to change:

- MySQL connection string
- JWT secret key
- OpenAI API key
- allowed frontend origins

### Frontend Configuration

Frontend environment example:

- `frontend/.env.example`

Example:

```env
VITE_API_BASE_URL=http://localhost:5067/api
```

---

## Running Tests

### Backend Tests

```powershell
cd C:\path\to\Finance-Tracker\backend
dotnet test
```

### Frontend Production Build

```powershell
cd C:\path\to\Finance-Tracker\frontend
npm run build
```

---

## Manual API Testing

A sample HTTP request file is included:

- `backend/src/AiBudgetSpendingAnalyzer.API/AiBudgetSpendingAnalyzer.API.http`

You can use it with VS Code or Visual Studio HTTP client support.

---

## Current Limitations

- Recurring transactions are stored, but automatic scheduled generation is not implemented yet
- Email alerts are not implemented
- Google login is not implemented
- Reports are exported as PDF from the frontend, not generated server-side
- OpenAI integration requires an API key; otherwise the app uses the fallback analyzer

---

## Summary

Finance Tracker is a production-style full-stack budgeting application that demonstrates:

- secure authentication
- scalable backend architecture
- responsive React frontend
- financial dashboarding
- budget planning
- reporting
- admin analytics
- AI-powered finance insights

It is suitable as a portfolio project, academic project, or base product for further extension.
