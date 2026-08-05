using System;
using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Mvc.Models
{
    public class TicketViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Priority is required.")]
        public string Priority { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Raised By is required.")]
        [StringLength(100)]
        public string RaisedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
