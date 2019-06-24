using System.Collections.Generic;
using TokenService.Models;

namespace TokenService.Services.User
{
    public interface IUserServices
    {
        IEnumerable<ApplicationUser> GetAll();
    }
}