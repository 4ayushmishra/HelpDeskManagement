using System.Collections.Generic;
using System.Threading.Tasks;
using HelpDesk.Mvc.Models;

namespace HelpDesk.Mvc.Services
{
    public interface ITicketService
    {
        Task<List<TicketViewModel>> GetAllTicketsAsync();

        Task<TicketViewModel> GetTicketByIdAsync(int id);

        Task<TicketViewModel> CreateTicketAsync(TicketViewModel ticket);

        Task<bool> UpdateTicketAsync(TicketViewModel ticket);

        Task<bool> DeleteTicketAsync(int id);

        Task<List<TicketViewModel>> GetTicketsByStatusAsync(string status);
    }
}
