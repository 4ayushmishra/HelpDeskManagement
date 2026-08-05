# HelpDeskManagement

Help Desk Ticket Management System built using ASP.NET Core Web API, ASP.NET Core MVC,
Entity Framework Core, SQL Server, xUnit, Moq and GitHub.

## Repository Structure

```
HelpDeskManagement/
├── HelpDeskManagement.sln
├── README.md
├── .gitignore
├── HelpDesk.Api/           # Part 1 - ASP.NET Core Web API
├── HelpDesk.Mvc/           # Part 2 - ASP.NET Core MVC
└── HelpDesk.Tests/         # Part 3 - xUnit + Moq
```

---

## Part 1 — HelpDesk.Api (Web API)

ASP.NET Core Web API using EF Core + SQL Server with the Repository Pattern.

- `Models/Ticket.cs`
- `Data/AppDbContext.cs`
- `Repositories/ITicketRepository.cs`, `Repositories/TicketRepository.cs`
- `Controllers/TicketsController.cs`

### Endpoints

| Method | Route                        | Description                  |
|--------|-------------------------------|-------------------------------|
| GET    | /api/tickets                  | Get all tickets               |
| GET    | /api/tickets/{id}              | Get a ticket by Id             |
| GET    | /api/tickets/status/{status}   | Get tickets filtered by status |
| POST   | /api/tickets                  | Create a new ticket            |
| PUT    | /api/tickets/{id}              | Update an existing ticket      |
| DELETE | /api/tickets/{id}              | Delete a ticket                |

### Setup

```bash
cd HelpDesk.Api
dotnet restore
```

Update `appsettings.json` → `ConnectionStrings:DefaultConnection` to your SQL Server instance, then:

```bash
dotnet tool install --global dotnet-ef   # once, if not already installed
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Swagger UI: `https://localhost:7050/swagger`

---

## Part 2 — HelpDesk.Mvc (MVC Application)

ASP.NET Core MVC app that talks to the API **only** through a Service Layer (`Services/TicketService.cs`,
via `HttpClient`). Controllers never touch the database directly.

### Features

- **Dashboard** — Total / Open / In Progress / Closed ticket counts
- **View All Tickets** — table of all tickets
- **View Ticket Details** — full info for one ticket
- **Raise New Ticket** — Status hardcoded to `Open`, Priority chosen via dropdown
- **Edit Ticket** — update Title, Description, Priority (dropdown), Status (dropdown)
- **Delete Ticket**
- **Filter Tickets by Status** — dropdown of `Open` / `In Progress` / `Closed`, results shown in a table

### Setup

```bash
cd HelpDesk.Mvc
dotnet restore
```

Update `appsettings.json` → `ApiSettings:BaseUrl` to match wherever `HelpDesk.Api` is running
(default assumes `https://localhost:7050/`), then:

```bash
dotnet run
```

> Run `HelpDesk.Api` first (or alongside), since the MVC app depends on it for all ticket data.

---

## Part 3 — HelpDesk.Tests (xUnit + Moq)

Unit tests for `TicketsController` (`HelpDesk.Api`). The repository (`ITicketRepository`) is fully
mocked with Moq — **no test connects to SQL Server**.

### Mandatory test cases (implemented)

1. `GetAllTickets_ReturnsOkResult_WhenTicketsExist`
2. `GetTicketById_ReturnsOkResult_WhenTicketExists`
3. `GetTicketById_ReturnsNotFound_WhenTicketDoesNotExist`
4. `CreateTicket_ReturnsOkResult_WhenTicketIsCreatedSuccessfully`
5. `CreateTicket_ReturnsBadRequest_WhenTicketIsNull`
6. `GetTicketsByStatus_ReturnsOkResult_WhenMatchingTicketsExist`

### Optional test cases (also implemented)

7. `UpdateTicket_ReturnsOkResult_WhenUpdateIsSuccessful`
8. `UpdateTicket_ReturnsNotFound_WhenTicketDoesNotExist`
9. `DeleteTicket_ReturnsOkResult_WhenTicketIsDeletedSuccessfully`
10. `DeleteTicket_ReturnsNotFound_WhenTicketDoesNotExist`
11. `GetAllTickets_ReturnsEmptyList_WhenNoTicketsExist`
12. `GetTicketsByStatus_ReturnsEmptyList_WhenNoMatchingTicketsExist`

### Run tests

```bash
cd HelpDesk.Tests
dotnet restore
dotnet test
```

---

## Part 4 — Git & GitHub

```bash
git init
git add .
git commit -m "Initial Commit"

# Create a new empty repository on GitHub named HelpDeskManagement (don't initialize with README)

git remote add origin <repository-url>
git push -u origin master
```

`.gitignore` excludes `bin/`, `obj/`, and `.vs/` (plus a few other common build/IDE artifacts).

---

## Coding Guidelines Followed

- Proper naming conventions throughout
- Repository Pattern in the API project; Service Layer pattern in the MVC project
- Asynchronous methods (`async`/`await`) everywhere data is fetched or persisted
- Exception handling around repository calls in the API controller
- Clean, consistently indented code

## Running the Full Solution Locally

1. Start `HelpDesk.Api` (`dotnet run` from `HelpDesk.Api/`) — note the HTTPS port shown in the console.
2. Make sure `HelpDesk.Mvc/appsettings.json` → `ApiSettings:BaseUrl` matches that port.
3. Start `HelpDesk.Mvc` (`dotnet run` from `HelpDesk.Mvc/`) and browse to the Dashboard.
4. Run `dotnet test` from `HelpDesk.Tests/` to verify all unit tests pass.
