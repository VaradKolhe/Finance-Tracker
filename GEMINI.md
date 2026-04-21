# GEMINI.md

This file provides project-specific context and instructions for the Finance Tracker application, ensuring the AI agent operates effectively within the workspace conventions.

## Project Overview

Finance Tracker is a full-stack personal finance application that enables users to manage transactions, budgets, and categories. It features a smart budgeting platform with AI-powered spending analysis.

### Tech Stack
- **Frontend**: React (TypeScript), Vite, Material UI, Recharts.
- **Backend**: ASP.NET Core Web API (C#), Entity Framework Core.
- **Database**: MySQL.
- **AI**: OpenAI API (with a mock fallback analyzer).

### Architecture
The backend follows a clean architecture pattern:
- **API**: Controllers and Middleware.
- **Application**: Business logic, Services, DTOs, and Validators.
- **Domain**: Entities and Enums.
- **Infrastructure**: Repositories, DB context, and External service integrations (AI, Security).

---

## Building and Running

### Prerequisites
- .NET SDK 7+
- Node.js 22+
- MySQL Server 8

### 1. Database Setup
```powershell
mysql -u root -p -e "CREATE DATABASE IF NOT EXISTS ai_budget_spending_analyzer_dev;"
# Optional: Manual schema import
mysql -u root -p ai_budget_spending_analyzer_dev < ./database/schema.sql
```

### 2. Backend Execution
```powershell
cd backend
dotnet restore
dotnet run --launch-profile http --project ./src/AiBudgetSpendingAnalyzer.API/AiBudgetSpendingAnalyzer.API.csproj
```
- **API URL**: `http://localhost:5067`
- **Swagger**: `http://localhost:5067/swagger`

### 3. Frontend Execution
```powershell
cd frontend
npm install
npm run dev -- --host 127.0.0.1
```
- **Frontend URL**: `http://127.0.0.1:5173`

---

## Development Conventions

### Coding Standards
- **Naming**: PascalCase for C# classes/methods; camelCase for TypeScript variables/functions and JSON responses.
- **Validation**: Use FluentValidation in the backend and appropriate hooks in the frontend.
- **Error Handling**: Centralized exception handling via `ExceptionHandlingMiddleware` in the API.

### Repository Pattern
All database operations must go through the Repository layer defined in `AiBudgetSpendingAnalyzer.Infrastructure/Repositories`.

### Testing
- **Backend**: Run `dotnet test` from the `backend` directory. Tests are located in `backend/tests/AiBudgetSpendingAnalyzer.Application.Tests`.
- **Frontend**: Use `npm run build` to verify the production build.

---

## AI Analyzer Details
- The AI analyzer sends the last 30 days of transactions to OpenAI (default model: `gpt-5.4-mini`).
- If no `OpenAiSettings:ApiKey` is provided in `appsettings.json`, it defaults to the `MockAiInsightsProvider` for deterministic analysis.

---

## Key Files
- `backend/src/AiBudgetSpendingAnalyzer.API/appsettings.Development.json`: Connection strings and JWT settings.
- `frontend/src/services/apiClient.ts`: Base API configuration.
- `database/schema.sql`: Database structure definition.
- `backend/src/AiBudgetSpendingAnalyzer.Infrastructure/Data/AppDbContextSeed.cs`: Initial data seeding (Admin user: `admin@budgetai.local` / `Admin@12345`).
