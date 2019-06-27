using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Models;

namespace TokenService.Controllers
{
    [Authorize(Roles = "SUPERADMIN")]
    public class RoleController : Controller
    {
        private RoleManager<ApplicationRole> _roleManager;

        public RoleController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetListRole()
        {
            var roleLst = _roleManager.Roles.Where(r => r.Name != "SUPERADMIN").Select(r => new { r.Id, r.Name }).ToList();
            var result = new
            {
                draw = 1,
                data = roleLst
            };
            return Json(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationRole role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        return Json(result);
                    }
                    return BadRequest(result.Errors.First().Description);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var role = await _roleManager.FindByIdAsync(id);
                    if (role != null)
                    {
                        var result = await _roleManager.DeleteAsync(role);
                        if (result.Succeeded)
                        {
                            return Json(new { });
                        }
                        return BadRequest(result.Errors.First().Description);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}