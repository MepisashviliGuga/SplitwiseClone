# SplitwiseClone

A production-grade, event-driven expense splitting backend — built like a real fintech product and fully deployed on Microsoft Azure.

🌐 **Live API:** https://splitwise-api.thankfulwater-515390ce.northeurope.azurecontainerapps.io/swagger

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        CLIENT                               │
│                    (Swagger / API)                          │
└─────────────────────┬───────────────────────────────────────┘
                      │ HTTP
                      ▼
┌─────────────────────────────────────────────────────────────┐
│              Azure Container Apps                           │
│                  .NET 8 REST API                            │
│                                                             │
│  POST /api/expenses                                         │
│    1. Save expense header → Azure SQL                       │
│    2. Publish event → Azure Service Bus                     │
│    3. Return 200 instantly                                  │
└────────────┬────────────────────────┬───────────────────────┘
             │                        │
             ▼                        ▼
┌────────────────────┐   ┌────────────────────────────────────┐
│    Azure SQL       │   │       Azure Service Bus            │
│  SplitwiseCloneDb  │   │   Topic: expensecreatedevent       │
│                    │   │   Subscription: ExpenseCreated     │
│  - Users           │   └───────────────┬────────────────────┘
│  - Groups          │                   │ async consumer
│  - Expenses        │                   ▼
│  - LedgerEntries   │   ┌────────────────────────────────────┐
│  - GroupMembers    │◄──│     ExpenseCreatedConsumer         │
│                    │   │  - Calculates splits               │
└────────────────────┘   │  - Writes ledger entries           │
                         │  - ACID transactions               │
                         └────────────────────────────────────┘
                                        │
                                        ▼
                         ┌────────────────────────────────────┐
                         │      Application Insights          │
                         │  - Request tracking                │
                         │  - Response times                  │
                         │  - Error monitoring                │
                         └────────────────────────────────────┘
```

---

## Azure Services Used

| Service | Purpose |
|---|---|
| **Azure Container Apps** | Hosts the .NET 8 API with auto-scaling |
| **Azure Container Registry** | Stores Docker images with versioned tags |
| **Azure SQL** | Cloud database with double-entry ledger schema |
| **Azure Service Bus** | Topics & Subscriptions for async event processing |
| **Application Insights** | Full observability — requests, errors, response times |

---

## Key Design Decisions

### Event-Driven Architecture
When a user creates an expense, the API saves the expense header and immediately returns a `200 OK`. The heavy financial calculations (ledger entries) are handled asynchronously by a background consumer via Azure Service Bus. This keeps API response times in milliseconds regardless of group size.

### Double-Entry Ledger
Every expense creates multiple `LedgerEntry` records — one positive entry for the payer and negative entries for each participant. This approach (inspired by accounting) ensures no money is ever "lost" due to race conditions and makes balance calculations simple aggregations.

### Clean Architecture
The solution is structured into four layers:
- **Api** — Controllers, HTTP concerns
- **Application** — Business logic, services, DTOs, MassTransit consumers
- **Core (Domain)** — Entities, no external dependencies
- **Persistence** — EF Core DbContext, migrations

---

## Tech Stack

- **.NET 8** — ASP.NET Core Web API
- **Entity Framework Core** — ORM with SQL Server provider
- **MassTransit** — Message bus abstraction (Azure Service Bus in production, in-memory locally)
- **Mapster** — Fast object mapping
- **Docker** — Containerization
- **Azure** — Full cloud deployment

---

## Local Development

### Prerequisites
- .NET 8 SDK
- SQL Server (local) or Azure SQL
- Docker Desktop (for deployment)

### Run Locally

```bash
git clone https://github.com/MepisashviliGuga/SplitwiseClone
cd SplitwiseClone
dotnet run --project SplitwiseClone.Api
```

Navigate to `http://localhost:5047/swagger`

### Configuration

`appsettings.json` uses local SQL Server by default. Azure connection strings are injected via environment variables in production — secrets never live inside the Docker image.

---

## Deployment

All deployments are handled by a single PowerShell script:

```powershell
.\deploy.ps1
```

The script:
1. Checks Docker is running
2. Checks Azure CLI authentication
3. Builds a fresh Docker image (no cache)
4. Tags with a timestamp-based version (e.g. `v20260315-1953`)
5. Pushes to Azure Container Registry
6. Creates a new Container App revision
7. Prints the live URL

---

## API Endpoints

### POST /api/Expenses
Create a new expense and split it among participants.

```json
{
  "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "payerId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "description": "Dinner",
  "totalAmount": 90.00,
  "participantIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    "3fa85f64-5717-4562-b3fc-2c963f66afa8",
    "3fa85f64-5717-4562-b3fc-2c963f66afa9"
  ]
}
```

### GET /api/Balances/group/{groupId}
Get the current balance for every member in a group.

---

## What I Would Add Next

- **Azure Key Vault** — move connection strings from environment variables into a managed secrets vault
- **GitHub Actions CI/CD** — trigger deployments automatically on push, tag images with Git commit SHA
- **Debt Simplification Algorithm** — greedy graph algorithm to reduce N payments to the minimum number of transactions
- **Authentication** — Azure AD B2C for user identity
- **Integration Tests** — test the full expense → Service Bus → ledger pipeline
