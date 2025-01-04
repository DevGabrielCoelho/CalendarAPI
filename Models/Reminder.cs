using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarAPI.Models
{
    public class Reminder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public string Id { get; set; } = string.Empty;
        [Required]
        public string EventId { get; set; } = string.Empty;
        [NotMapped]
        public Event Event { get; set; } = new();
        [Required]
        public TimeSpan TimeBefore { get; set; }
        [Required]
        public string CreatorEmail { get; set; } = string.Empty;
    }
}