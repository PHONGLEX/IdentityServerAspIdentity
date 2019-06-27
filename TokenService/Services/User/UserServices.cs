using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Data;
using TokenService.Models;

namespace TokenService.Services.User
{
    public class UserServices : IUserServices
    {
        private UserManager<ApplicationUser> _userManager;

        public UserServices(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _userManager.Users.Include(u => u.Roles).Include(u => u.Claims);
        }
    }
}
