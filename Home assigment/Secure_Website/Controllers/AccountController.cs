using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Secure_Website.Data;
using Secure_Website.Models;

namespace Secure_Website.Controllers
{
    //https://medium.com/streamwriter/generating-random-passwords-in-asp-net-core-c24449f7c877
    [Authorize(Roles = "Teacher")]
    public class AccountController : Controller
    {
        private ApplicationDbContext _db;
        RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;


        public AccountController(ApplicationDbContext db, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //[HttpGet]
        private string GeneratePassword()
        {
            var options = _userManager.Options.Password;

            int length = options.RequiredLength;

            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));

            return password.ToString();
        }

        private void sendMail(StudentModel model, string password)
        {
            using (MailMessage mm = new MailMessage("asecuring@gmail.com", model.Email))
            {
                mm.Subject = "Your Student Account Has been Created";
                mm.Body = "Use you email as the username: "+model.Email+" and "+password;
                mm.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential("asecuring@gmail.com", "!secur123APPS");
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                    ViewBag.Message = "Email sent.";
                }
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(StudentModel model)
        {
            var exists = _db.Users.SingleOrDefault(u => u.Email.Equals(model.Email));
            if (exists != null)
                return View();

            var student = new ApplicationUser {UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName};
            var password = GeneratePassword();
            var result = await _userManager.CreateAsync(student, password);
           
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(student, "Student");
                sendMail(model, password);

                TempData["message"] = "Successfully Added Student";
                return RedirectToAction("Index", "Home");
            }

            TempData["message"] = "Failed to added Student";
            return View();
        }
    }
}
