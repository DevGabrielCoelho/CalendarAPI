using CalendarAPI.Data;
using CalendarAPI.Interfaces;
using CalendarAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task AddUserAsync(User user)
        {
            if (_context.Users == null) throw new InvalidOperationException("User DbSet is null.");
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (_context.Users == null) throw new InvalidOperationException("User DbSet is null.");
            var user = await _context.Users
                .Include(x => x.Events)
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null) throw new KeyNotFoundException("User not found.");
            return user;
        }

        public async Task UpdateTokenAsync(string id, string token)
        {
            if (_context.Users == null) throw new InvalidOperationException("User DbSet is null.");
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) throw new KeyNotFoundException("User not found.");

            user.Token = token;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            if (_context.Users == null) throw new InvalidOperationException("User DbSet is null.");
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
