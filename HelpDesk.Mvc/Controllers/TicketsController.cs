using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpDesk.Mvc.Models;
using HelpDesk.Mvc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpDesk.Mvc.Controllers
{
    public class TicketsController : Controller
    {
        private static readonly List<string> Priorities = new List<string> { "Low", "Medium", "High" };
        private static readonly List<string> Statuses = new List<string> { "Open", "In Progress", "Closed" };

        // MVC Controllers communicate only with the Service Layer — never directly with a database.
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: /Tickets/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();

            var dashboard = new DashboardViewModel
            {
                TotalTickets = tickets.Count,
                OpenTickets = tickets.Count(t => t.Status == "Open"),
                InProgressTickets = tickets.Count(t => t.Status == "In Progress"),
                ClosedTickets = tickets.Count(t => t.Status == "Closed")
            };

            return View(dashboard);
        }

        // GET: /Tickets  (View All Tickets)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return View(tickets);
        }

        // GET: /Tickets/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: /Tickets/Create  (Raise New Ticket)
        [HttpGet]
        public IActionResult Create()
        {
            PopulatePriorityDropdown();

            var ticket = new TicketViewModel
            {
                // Status is hardcoded to Open for newly raised tickets
                Status = "Open"
            };

            return View(ticket);
        }

        // POST: /Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketViewModel ticket)
        {
            // Status is always hardcoded to Open when raising a new ticket
            ticket.Status = "Open";

            if (!ModelState.IsValid)
            {
                PopulatePriorityDropdown();
                return View(ticket);
            }

            await _ticketService.CreateTicketAsync(ticket);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tickets/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            PopulatePriorityDropdown();
            PopulateStatusDropdown();

            return View(ticket);
        }

        // POST: /Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketViewModel ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                PopulatePriorityDropdown();
                PopulateStatusDropdown();
                return View(ticket);
            }

            var success = await _ticketService.UpdateTicketAsync(ticket);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Unable to update the ticket. Please try again.");
                PopulatePriorityDropdown();
                PopulateStatusDropdown();
                return View(ticket);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Tickets/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: /Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _ticketService.DeleteTicketAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tickets/FilterByStatus
        [HttpGet]
        public async Task<IActionResult> FilterByStatus(string status)
        {
            PopulateStatusDropdown();

            var model = new FilterByStatusViewModel
            {
                SelectedStatus = status
            };

            if (!string.IsNullOrWhiteSpace(status))
            {
                model.Tickets = await _ticketService.GetTicketsByStatusAsync(status);
            }

            return View(model);
        }

        private void PopulatePriorityDropdown()
        {
            ViewBag.Priorities = new SelectList(Priorities);
        }

        private void PopulateStatusDropdown()
        {
            ViewBag.Statuses = new SelectList(Statuses);
        }
    }
}
