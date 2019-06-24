using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace TokenService.Models
{
    public class ApplicationUserRole
    {
        public ApplicationUserRole() : base()
        {

        }

        public ApplicationUser User { get; set; }
        public IEnumerable<IdentityRole> Role
        {
            get; set;
        }
        public IEnumerable<ApplicationRole> UserRole
        {
            get; set;
        }
    }
}