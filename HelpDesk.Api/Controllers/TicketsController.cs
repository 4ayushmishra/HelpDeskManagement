using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpDesk.Api.Models;
using HelpDesk.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private static readonly HashSet<string> ValidPriorities =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Low", "Medium", "High" };

        private static readonly HashSet<string> ValidStatuses =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Open", "In Progress", "Closed" };

        private readonly ITicketRepository _ticketRepository;

        public TicketsController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        // GET: api/tickets
        [HttpGet]
        public async Task<ActionResult<List<Ticket>>> GetAllTickets()
        {
            var tickets = await _ticketRepository.GetAllTicketsAsync();
            return Ok(tickets);
        }

        // GET: api/tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicketById(int id)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound($"Ticket with Id {id} was not found.");
            }

            return Ok(ticket);
        }

        // GET: api/tickets/status/Open
        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<Ticket>>> GetTicketsByStatus(string status)
        {
            if (!ValidStatuses.Contains(status))
            {
                return BadRequest($"Invalid status. Valid values are: {string.Join(", ", ValidStatuses)}");
            }

            // Normalize to the canonical casing (e.g. "open" -> "Open") before hitting the
            // repository, since the DB comparison is case-sensitive but this check isn't.
            var normalizedStatus = ValidStatuses.First(s => string.Equals(s, status, StringComparison.OrdinalIgnoreCase));

            var tickets = await _ticketRepository.GetTicketsByStatusAsync(normalizedStatus);
            return Ok(tickets);
        }

        // POST: api/tickets
        [HttpPost]
        public async Task<ActionResult> CreateTicket([FromBody] Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var validationError = ValidateTicket(ticket);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            try
            {
                var newId = await _ticketRepository.CreateTicketAsync(ticket);
                var createdTicket = await _ticketRepository.GetTicketByIdAsync(newId);
                return Ok(createdTicket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the ticket: {ex.Message}");
            }
        }

        // PUT: api/tickets/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTicket(int id, [FromBody] Ticket ticket)
        {
            if (ticket == null || id != ticket.Id)
            {
                return BadRequest("Ticket Id in the route does not match the Id in the request body.");
            }

            var validationError = ValidateTicket(ticket);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            try
            {
                await _ticketRepository.UpdateTicketAsync(ticket);
                return Ok(ticket);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Ticket with Id {id} was not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the ticket: {ex.Message}");
            }
        }

        // DELETE: api/tickets/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            try
            {
                await _ticketRepository.DeleteTicketAsync(id);
                return Ok($"Ticket with Id {id} was deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Ticket with Id {id} was not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the ticket: {ex.Message}");
            }
        }

        private static string ValidateTicket(Ticket ticket)
        {
            if (ticket == null)
            {
                return "Ticket data is required.";
            }

            if (string.IsNullOrWhiteSpace(ticket.Title))
            {
                return "Title is required.";
            }

            if (string.IsNullOrWhiteSpace(ticket.Priority) || !ValidPriorities.Contains(ticket.Priority))
            {
                return $"Priority must be one of: {string.Join(", ", ValidPriorities)}";
            }

            if (string.IsNullOrWhiteSpace(ticket.Status) || !ValidStatuses.Contains(ticket.Status))
            {
                return $"Status must be one of: {string.Join(", ", ValidStatuses)}";
            }

            if (string.IsNullOrWhiteSpace(ticket.RaisedBy))
            {
                return "RaisedBy is required.";
            }

            return null;
        }
    }
}
