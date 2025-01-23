namespace CalendarAPI.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body);
        public bool IsValidEmail(string email);
        public bool IsDomainValid(string email);
        
    }
}