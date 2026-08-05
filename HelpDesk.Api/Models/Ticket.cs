using System;

namespace HelpDesk.Api.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        // Valid values: Low, Medium, High
        public string Priority { get; set; }

        // Valid values: Open, In Progress, Closed
        public string Status { get; set; }

        public string RaisedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
