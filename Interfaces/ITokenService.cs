using CalendarAPI.Models;

namespace CalendarAPI.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}