using CalendarAPI.Interfaces;

namespace CalendarAPI.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly Dictionary<string, string> _codes = new();

        public string GenerateCode(string email)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();

            var code = new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());

            _codes[email] = code;

            return code;
        }

        public bool ValidateVerificationCode(string email, string inputCode)
        {
            if (_codes.TryGetValue(email, out var storedCode) && storedCode == inputCode)
            {
                _codes.Remove(email);
                return true;
            }
            return false;
        }
    }
}
