using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarAPI.Dtos;
using CalendarAPI.Models;
using CalendarAPI.Services;

namespace CalendarAPI.Mappers
{
    public static class UserMappers
    {
        public static User RegisterUser(this UserDto dto){
            User user = new User{
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Email = dto.Email,
                PassHash = dto.Pass
            };
            return user;
        }
    }
}