using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarAPI.Models;

namespace CalendarAPI.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}