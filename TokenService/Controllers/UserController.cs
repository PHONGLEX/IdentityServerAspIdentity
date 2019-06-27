using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TokenService.Models;
using TokenService.Services.User;

namespace TokenService.Controllers
{
    [Authorize(Roles = "SUPERADMIN")]
    public class UserController : Controller
    {
        private IUserServices _userServices;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private readonly Dictionary<string, string> _claimTypes;
        private readonly Dictionary<string, string> _roles;

        public UserController(IUserServices userServices, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userServices = userServices;
            _userManager = userManager;
            _roleManager = roleManager;

            // Get Role and Claim
            var fldInfo = typeof(ClaimTypes).GetFields(BindingFlags.Static | BindingFlags.Public);
            _claimTypes = fldInfo.ToDictionary(i => i.Name, i => (string)i.GetValue(null));
            _roles = roleManager.Roles.ToDictionary(r => r.Id, r => r.Name);
        }

        public IActionResult Index()
        {
            ViewBag.ClaimTypes = _claimTypes.Keys.OrderBy(s => s); 
            ViewBag.Roles = _roles;
            return View();
        }

        public IActionResult GetList()
        {
            var user = _userServices.GetAll().Select(u => new
            {
                Id = u.Id,
                Email = u.UserName,
                Roles = u.Roles.Select(r => _roles[r.RoleId]),
                Claims = u.Claims.Select(c => new KeyValuePair<string, string>(_claimTypes.Single(x => x.Value == c.ClaimType).Key, c.ClaimValue))
            }).ToList();

            return Json(new
            {
                data = user
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string[] roles, List<KeyValuePair<string, string>> claims)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return NotFound("User not found.");

                if (user != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    foreach (string role in roles.Except(userRoles))
                        await _userManager.AddToRoleAsync(user, role);

                    foreach (string role in userRoles.Except(roles))
                        await _userManager.RemoveFromRoleAsync(user, role);

                    var userClaims = await _userManager.GetClaimsAsync(user);

                    foreach (var kvp in claims.Where(a => !userClaims.Any(b => _claimTypes[a.Key] == b.Type && a.Value == b.Value)))
                        await _userManager.AddClaimAsync(user, new Claim(_claimTypes[kvp.Key], kvp.Value));

                    foreach (var claim in userClaims.Where(a => !claims.Any(b => a.Type == _claimTypes[b.Key] && a.Value == b.Value)))
                        await _userManager.RemoveClaimAsync(user, claim);

                    return Json(user);
                }
                return BadRequest();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest();
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                    return Json(new { });
                }
                return NotFound();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}