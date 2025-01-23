namespace CalendarAPI.Dtos
{
    public class CreateReminderDto
    {
        public string EventId { get; set; } = string.Empty;
        public double MinutesBefore { get; set; }
    }
}