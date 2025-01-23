namespace CalendarAPI.Interfaces
{
    public interface IEmailVerificationService
    {
        public string GenerateCode(string email);
        public bool ValidateVerificationCode(string email, string inputCode);
    }
}