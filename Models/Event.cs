using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace CalendarAPI.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [NotMapped]
        public User User { get; set; } = new();

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime DateStart { get; set; }

        [Required]
        public DateTime DateEnd { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string GuestsEmailsJson
        {
            get => JsonSerializer.Serialize(GuestsEmails);
            set => GuestsEmails = string.IsNullOrEmpty(value)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(value) ?? new List<string>();
        }

        [NotMapped]
        public List<string> GuestsEmails { get; set; } = new();

        [NotMapped]
        [Required]
        public bool SendedEmail => Reminders.All(x => x.SendedEmail);

        public List<Reminder> Reminders { get; set; } = new();
    }
}
