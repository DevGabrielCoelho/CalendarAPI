using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using CalendarAPI.Interfaces;

namespace CalendarAPI.Services
{
    class PasswordHasher : IPasswordHasher
    {
        private readonly int _saltSize;
        private readonly int _keySize;
        private readonly int _memorySize;
        private readonly int _iterations;
        private readonly int _degreeOfParallelism;
        private readonly char _delimiter;

        public PasswordHasher(IConfiguration configuration)
        {
            var settings = configuration.GetSection("PasswordHasherSettings");

            _saltSize = settings.GetValue<int>("SaltSize");
            _keySize = settings.GetValue<int>("KeySize");
            _memorySize = settings.GetValue<int>("MemorySize");
            _iterations = settings.GetValue<int>("Iterations");
            _degreeOfParallelism = settings.GetValue<int>("DegreeOfParallelism");
            _delimiter = settings.GetValue<string>("Delimiter")?[0] ?? throw new Exception();
        }

        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(_saltSize);
            return Hash(password, salt);
        }

        public string Hash(string password, byte[] salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            using var argon2 = new Argon2id(passwordBytes)
            {
                MemorySize = _memorySize,
                Iterations = _iterations,
                DegreeOfParallelism = _degreeOfParallelism,
                Salt = salt
            };

            var hash = argon2.GetBytes(_keySize);
            return string.Join(_delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        bool IPasswordHasher.Verify(string passwordHash, string inputPassword)
        {
            var elements = passwordHash.Split(_delimiter);
            var salt = Convert.FromBase64String(elements[0]);
            var storedHash = Convert.FromBase64String(elements[1]);

            var hashInput = Hash(inputPassword, salt);
            return CryptographicOperations.FixedTimeEquals(storedHash, Convert.FromBase64String(hashInput.Split(_delimiter)[1]));
        }
    }
}
