using CalendarAPI.Dtos;
using CalendarAPI.Models;

namespace CalendarAPI.Mappers
{
    public static class UserMappers
    {
        public static User RegisterUser(this UserDto dto)
        {
            return new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Email = dto.Email,
                PassHash = dto.Pass
            };
        }
    }
}
