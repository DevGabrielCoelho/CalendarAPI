using CalendarAPI.Models;

namespace CalendarAPI.Interfaces
{
    public interface IEventRepository
    {
        public Task AddEventAsync(Event @event);
        public Task<List<Event>> GetAllAsync();
        public Task<Event> GetByIdAsync(string id);
        public Task UpdateAsync(Event @event);
        public Task RemoveByIdAsync(string id);
    }
}