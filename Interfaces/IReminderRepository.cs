using CalendarAPI.Models;

namespace CalendarAPI.Interfaces
{
    public interface IReminderRepository
    {
        public Task AddAsync(Reminder reminder);
        public Task<Reminder> GetByIdAsync(string id);
        public Task RemoveAsync(Reminder reminder);
    }
}