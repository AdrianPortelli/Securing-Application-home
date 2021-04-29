﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Secure_Website.Models;

namespace Secure_Website.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class RolesManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
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
                    TempData["error"] = "user not found";
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
                    TempData["error"] = "user not found";
                }
            }

            return RedirectToAction("Index");
        }
    }
}
