using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using CalendarAPI.Interfaces;

namespace CalendarAPI.Services
{
    class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 128 / 8;
        private const int KeySize = 256 / 8;
        private const int MemorySize = 15 * 1024;
        private const int Interactions = 2;
        private const int DegreeOfParallelism = 1;
        private const char Delimiter = ';';

        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using (var argon2 = new Argon2id(passwordBytes))
            {

                argon2.MemorySize = MemorySize;
                argon2.Iterations = Interactions;
                argon2.DegreeOfParallelism = DegreeOfParallelism;
                argon2.Salt = salt;

                byte[] hash = argon2.GetBytes(KeySize);
                return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
            }
        }

        public string Hash(string password, byte[] salt)
        {

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using (var argon2 = new Argon2id(passwordBytes))
            {

                argon2.MemorySize = MemorySize;
                argon2.Iterations = Interactions;
                argon2.DegreeOfParallelism = DegreeOfParallelism;
                argon2.Salt = salt;

                byte[] hash = argon2.GetBytes(32);
                return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
            }
        }

        bool IPasswordHasher.Verify(string passwordHash, string inputPassword)
        {
            string[] elements = passwordHash.Split(Delimiter);
            byte[] salt = Convert.FromBase64String(elements[0]);
            byte[] hash = Convert.FromBase64String(elements[1]);

            var hashInput = Hash(inputPassword, salt);
            return CryptographicOperations.FixedTimeEquals(hash, Convert.FromBase64String(hashInput.Split(Delimiter)[1]));

        }
    }
}