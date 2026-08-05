using System.Collections.Generic;

namespace HelpDesk.Mvc.Models
{
    public class FilterByStatusViewModel
    {
        public string SelectedStatus { get; set; }

        public List<TicketViewModel> Tickets { get; set; } = new List<TicketViewModel>();
    }
}
