using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpDesk.Api.Data;
using HelpDesk.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _context.Tickets
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<Ticket> GetTicketByIdAsync(int id)
        {
            return await _context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<int> CreateTicketAsync(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            if (ticket.CreatedDate == default)
            {
                ticket.CreatedDate = DateTime.UtcNow;
            }

            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();

            return ticket.Id;
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            var existingTicket = await _context.Tickets.FindAsync(ticket.Id);

            if (existingTicket == null)
            {
                throw new KeyNotFoundException($"Ticket with Id {ticket.Id} was not found.");
            }

            existingTicket.Title = ticket.Title;
            existingTicket.Description = ticket.Description;
            existingTicket.Priority = ticket.Priority;
            existingTicket.Status = ticket.Status;
            existingTicket.RaisedBy = ticket.RaisedBy;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTicketAsync(int id)
        {
            var existingTicket = await _context.Tickets.FindAsync(id);

            if (existingTicket == null)
            {
                throw new KeyNotFoundException($"Ticket with Id {id} was not found.");
            }

            _context.Tickets.Remove(existingTicket);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Ticket>> GetTicketsByStatusAsync(string status)
        {
            var normalizedStatus = status?.Trim().ToLower();

            return await _context.Tickets
                .AsNoTracking()
                .Where(t => t.Status.ToLower() == normalizedStatus)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
    }
}
