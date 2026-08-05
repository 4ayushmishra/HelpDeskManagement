# HelpDeskManagement

Help Desk Ticket Management System built using **ASP.NET Core Web API**, **ASP.NET Core MVC**, **Entity Framework Core**, **SQL Server**, **xUnit** and **Moq**.

---

##  Solution Overview

The **Help Desk Ticket Management System** is a solution designed to handle IT and employee support requests efficiently. It consists of three primary projects:

| Project Name | Project Type | Purpose |
| :--- | :--- | :--- |
| `HelpDesk.Api` | ASP.NET Core Web API | Implements REST APIs, Entity Framework Core, SQL Server database integration, and Repository Pattern. |
| `HelpDesk.Mvc` | ASP.NET Core Web MVC | Consumes the Web API through an asynchronous `HttpClient` Service Layer (`TicketService`). |
| `HelpDesk.Tests` | xUnit Test Project | Unit testing for `TicketsController` endpoints using **Moq** to mock the repository layer. |

---

##  Data Model — `Ticket`

```csharp
public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; } // Valid: Low, Medium, High
    public string Status { get; set; }   // Valid: Open, In Progress, Closed
    public string RaisedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

---

##  Web API Endpoints (`HelpDesk.Api`)

The API exposes the following REST endpoints via `TicketsController`:

| HTTP Method | Endpoint URL | Description |
| :--- | :--- | :--- |
| `GET` | `/api/tickets` | Get all tickets |
| `GET` | `/api/tickets/{id}` | Get ticket by ID |
| `GET` | `/api/tickets/status/{status}` | Get all tickets filtered by status (`Open`, `In Progress`, `Closed`) |
| `POST` | `/api/tickets` | Create a new ticket |
| `PUT` | `/api/tickets/{id}` | Update an existing ticket |
| `DELETE` | `/api/tickets/{id}` | Delete a ticket |

---

##  MVC Application Features (`HelpDesk.Mvc`)

- **Dashboard:** Displays ticket statistics (Total, Open, In Progress, Closed).
- **View All Tickets:** Displays all support requests in a table.
- **View Ticket Details:** View complete details for a selected ticket.
- **Raise New Ticket:** Create form where Status is hardcoded to `Open` and Priority is selected via dropdown.
- **Edit Ticket:** Update Title, Description, Priority (dropdown), and Status (dropdown: `Open`, `In Progress`, `Closed`).
- **Delete Ticket:** Delete confirmation and execution.
- **Filter Tickets by Status:** Select status via dropdown to filter results into a table.

All MVC controllers talk **only** to the Service Layer (`TicketService`, via `HttpClient`) — there is no direct database access anywhere in this project.

---

##  Unit Tests (`HelpDesk.Tests`)

12 unit test cases implemented using **xUnit** and **Moq**, covering `TicketsController` with the repository fully mocked (no test connects to SQL Server):

**Mandatory (6):**
1. `GetAllTickets_ReturnsOkResult_WhenTicketsExist`
2. `GetTicketById_ReturnsOkResult_WhenTicketExists`
3. `GetTicketById_ReturnsNotFound_WhenTicketDoesNotExist`
4. `CreateTicket_ReturnsOkResult_WhenTicketIsCreatedSuccessfully`
5. `CreateTicket_ReturnsBadRequest_WhenTicketIsNull`
6. `GetTicketsByStatus_ReturnsOkResult_WhenMatchingTicketsExist`

**Optional (6):**
7. `UpdateTicket_ReturnsOkResult_WhenUpdateIsSuccessful`
8. `UpdateTicket_ReturnsNotFound_WhenTicketDoesNotExist`
9. `DeleteTicket_ReturnsOkResult_WhenTicketIsDeletedSuccessfully`
10. `DeleteTicket_ReturnsNotFound_WhenTicketDoesNotExist`
11. `GetAllTickets_ReturnsEmptyList_WhenNoTicketsExist`
12. `GetTicketsByStatus_ReturnsEmptyList_WhenNoMatchingTicketsExist`

### Running Unit Tests

```bash
dotnet test HelpDesk.Tests/HelpDesk.Tests.csproj
```

---

##  How to Run the Application

1. **Configure the database.** Update `ConnectionStrings:DefaultConnection` in `HelpDesk.Api/appsettings.json` to point to your SQL Server instance, then apply migrations:
   ```bash
   cd HelpDesk.Api
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

2. **Start the Web API (`HelpDesk.Api`):**
   ```bash
   dotnet run --project HelpDesk.Api/HelpDesk.Api.csproj
   ```
   Note the HTTPS URL printed in the console (e.g. `https://localhost:7050`).

3. **Point the MVC app at the API.** Update `ApiSettings:BaseUrl` in `HelpDesk.Mvc/appsettings.json` to match the URL from step 2.

4. **Start the MVC Web App (`HelpDesk.Mvc`):**
   ```bash
   dotnet run --project HelpDesk.Mvc/HelpDesk.Mvc.csproj
   ```

5. Open your browser and navigate to the URL shown in the console (e.g. `https://localhost:7060/`) to access the Help Desk Dashboard.

---

##  Repository Structure

```
HelpDeskManagement/
├── HelpDeskManagement.sln
├── README.md
├── .gitignore
├── HelpDesk.Api/
├── HelpDesk.Mvc/
└── HelpDesk.Tests/
```

##  Coding Guidelines Followed

- Proper naming conventions throughout
- Repository Pattern (API) and Service Layer pattern (MVC)
- Asynchronous methods (`async`/`await`) for all data operations
- Exception handling around repository calls
- Clean, consistently indented code

