using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HelpDesk.Mvc.Models;

namespace HelpDesk.Mvc.Services
{
    /// <summary>
    /// Service Layer that consumes the HelpDesk.Api TicketController endpoints using HttpClient.
    /// MVC Controllers must talk only to this service — no direct database access happens here
    /// or anywhere else in this project.
    /// </summary>
    public class TicketService : ITicketService
    {
        private const string BaseRoute = "api/tickets";
        private readonly HttpClient _httpClient;

        public TicketService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TicketViewModel>> GetAllTicketsAsync()
        {
            var tickets = await _httpClient.GetFromJsonAsync<List<TicketViewModel>>(BaseRoute);
            return tickets ?? new List<TicketViewModel>();
        }

        public async Task<TicketViewModel> GetTicketByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseRoute}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<TicketViewModel>();
        }

        public async Task<TicketViewModel> CreateTicketAsync(TicketViewModel ticket)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseRoute, ticket);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TicketViewModel>();
        }

        public async Task<bool> UpdateTicketAsync(TicketViewModel ticket)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseRoute}/{ticket.Id}", ticket);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseRoute}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<TicketViewModel>> GetTicketsByStatusAsync(string status)
        {
            var response = await _httpClient.GetAsync($"{BaseRoute}/status/{Uri.EscapeDataString(status)}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<TicketViewModel>();
            }

            var tickets = await response.Content.ReadFromJsonAsync<List<TicketViewModel>>();
            return tickets ?? new List<TicketViewModel>();
        }
    }
}
