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
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext appDbContext){
            _context = appDbContext;
        }

        public async Task AddEventAsync(Event @event)
        {
            if(_context.Events == null)throw new Exception();
            await _context.Events.AddAsync(@event);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Event>> GetAllAsync()
        {
            if(_context.Events == null)throw new Exception();
            return await _context.Events.Include(x => x.User).Include(x => x.Reminders).ToListAsync();
        }

        public async Task<Event> GetByIdAsync(string id)
        {
            if(_context.Events == null)throw new Exception();
            var @events = await _context.Events.Include(x => x.User).Include(x => x.Reminders).FirstOrDefaultAsync(x => x.Id == id);
            if(@events == null)throw new Exception();
            return @events;
        }

        public async Task RemoveByIdAsync(string id)
        {
            if(_context.Events == null)throw new Exception();
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            if(@event == null)throw new Exception();
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event @event)
        {
            if(_context.Events == null)throw new Exception();
            _context.Events.Update(@event);
            await _context.SaveChangesAsync();
        }
    }
}