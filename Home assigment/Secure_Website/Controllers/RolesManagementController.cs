using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Secure_Website.Models;

namespace Secure_Website.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class RolesManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RolesManagementController> _logger;
        public RolesManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<RolesManagementController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }


        public async Task<IActionResult>  Index()
        {

            var user = await _userManager.GetUserAsync(HttpContext.User);
            _logger.LogInformation(user.FirstName + " " + user.LastName + "has accessed role management index");

            RolesManagementModel model = new RolesManagementModel();
            model.Roles = _roleManager.Roles.ToList();
            model.Users = _userManager.Users.ToList();


            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateRole(string role, string user, string btnName)
        {

            var returnedUser = await _userManager.FindByNameAsync(user);
            IEnumerable<string> returnedUserRole = await _userManager.GetRolesAsync(returnedUser);
            if (btnName == "Allocate")
            {
                if (returnedUser != null)
                {

                    await _userManager.RemoveFromRolesAsync(returnedUser, returnedUserRole);
                    await _userManager.AddToRoleAsync(returnedUser, role);

                    TempData["message"] = "successfully allocated";
                }
                else
                {
                    _logger.LogError("Error while allocating Role, User was not found");
                    return View("Error", new ErrorViewModel() { Message = "Error while allocating Role, User was not found" });
                }
            }
            else
            {
                //deallocate
                if (returnedUser != null)
                {
                    await _userManager.RemoveFromRoleAsync(returnedUser, role);

                    TempData["message"] = "successfully deallocated";
                }
                else
                {
                    _logger.LogError("Error while dellocating Role, User was not found");
                    return View("Error", new ErrorViewModel() { Message = "Error while dellocating Role, User was not found" });
                }
            }

            return RedirectToAction("Index");
        }
    }
}
