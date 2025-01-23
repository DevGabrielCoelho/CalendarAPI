using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendarAPI.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PassHash { get; set; } = string.Empty;

        [NotMapped]
        public List<Event> Events { get; set; } = new();

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public bool Validated { get; set; } = false;
    }
}
