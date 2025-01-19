using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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