using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public double MinutesBefore { get; set; }

        [Required]
        public bool SendedEmail { get; set; } = false;

        [Required]
        public string CreatorEmail { get; set; } = string.Empty;
    }
}
