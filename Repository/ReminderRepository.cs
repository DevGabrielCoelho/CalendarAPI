using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarAPI.Data;
using CalendarAPI.Interfaces;
using CalendarAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI.Repository
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly AppDbContext _context;

        public ReminderRepository(AppDbContext context){
            _context = context;
        }

        public async Task AddAsync(Reminder reminder)
        {
            if(_context.Reminders == null)throw new Exception();
            await _context.Reminders.AddAsync(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task<Reminder> GetByIdAsync(string id)
        {
            if(_context.Reminders == null)throw new Exception();
            var reminders = await _context.Reminders.Include(x => x.Event).FirstOrDefaultAsync(x => x.Id == id);
            if(reminders == null)throw new Exception();
            return reminders;
        }

        public async Task RemoveAsync(Reminder reminder)
        {
            if(_context.Reminders == null)throw new Exception();
            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();
        }
    }
}