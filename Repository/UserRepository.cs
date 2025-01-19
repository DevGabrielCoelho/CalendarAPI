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
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext appDbContext){
            _context = appDbContext;
            
        }

        public async Task AddUserAsync(User user)
        {
            if(_context.Users == null)throw new Exception();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email){
            if(_context.Users == null)throw new Exception();
            var x = await _context.Users.Include(x => x.Events).FirstOrDefaultAsync(x => x.Email == email);
            if(x == null)throw new Exception();
            return x;
        }

        public async Task UpdateTokenAsync(string id, string Token)
        {
            if(_context.Users == null)throw new Exception();
            await _context.Users.Where(x => x.Id == id).ExecuteUpdateAsync( x => x
            .SetProperty(x => x.Token, Token));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            if(_context.Users == null)throw new Exception();
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}