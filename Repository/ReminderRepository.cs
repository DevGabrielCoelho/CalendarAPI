using CalendarAPI.Data;
using CalendarAPI.Interfaces;
using CalendarAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI.Repository
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly AppDbContext _context;

        public ReminderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Reminder reminder)
        {
            if (_context.Reminders == null) throw new InvalidOperationException("Reminder DbSet is null.");
            await _context.Reminders.AddAsync(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task<Reminder> GetByIdAsync(string id)
        {
            if (_context.Reminders == null) throw new InvalidOperationException("Reminder DbSet is null.");
            var reminder = await _context.Reminders
                .Include(x => x.Event)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (reminder == null) throw new KeyNotFoundException("Reminder not found.");
            return reminder;
        }

        public async Task RemoveAsync(Reminder reminder)
        {
            if (_context.Reminders == null) throw new InvalidOperationException("Reminder DbSet is null.");
            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();
        }
    }
}
