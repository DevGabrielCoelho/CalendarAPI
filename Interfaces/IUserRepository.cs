using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarAPI.Models;

namespace CalendarAPI.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetUserByEmailAsync(string email);
        public Task UpdateTokenAsync(string id, string Token);
        public Task AddUserAsync(User user);
        public Task UpdateUserAsync(User user);
    }
}