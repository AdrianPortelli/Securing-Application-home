using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Secure_Website.Data;
using Secure_Website.Models;

namespace Secure_Website.Controllers
{
    [Authorize(Roles = "Teacher, Student")]
    public class TaskController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        public TaskController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _db = db;
            
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentView()
        {

            List<ScheduleTaskModel> taskList = new List<ScheduleTaskModel>();

            var  loggedstudent = await _userManager.GetUserAsync(HttpContext.User);
            StudentModel studentdb = _db.Student.Where(b => b.StudentId == loggedstudent.Id).FirstOrDefault();
            taskList = _db.ScheduleTask.Where(b => b.TeacherId == studentdb.TeacherId).ToList();


            TaskViewModel model = new TaskViewModel();
            model.ScheduleTasks = taskList;

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Student")]
        public IActionResult TaskSubmission()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public IActionResult TaskSubmission(IFormFile file)
        {

            if (ModelState.IsValid)
            {
                string filename;

                if(file == null)
                {
                    ModelState.AddModelError("file", "File is not valid and acceptable");
                    return View();
                }

                if(Path.GetExtension(file.FileName) == ".pdf" )
                {
                    byte[] whitelist = new byte[] {37,80,68,70,45};

                    if(file != null)
                    {
                        MemoryStream userFile = new MemoryStream();

                        using(var f = file.OpenReadStream())
                        {
                            byte[] buffer = new byte[5];
                            f.Read(buffer, 0, 5);

                            for(int i = 0; i < whitelist.Length; i++)
                            {
                                if(whitelist[i] != buffer[i])
                                {
                                    ModelState.AddModelError("file", "File is not valid and acceptable");
                                    return View();
                                }
                            }

                            f.Position = 0;

                            filename = Guid.NewGuid() + Path.GetExtension(file.FileName);

                            string absolutePath = @"StudentFiles\" + filename;

                            try
                            {
                                using(FileStream fsOut = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write))
                                {
                                    f.CopyTo(fsOut);
                                }
                                f.Close();
                            }catch(Exception ex)
                            {
                                //logger
                                //error page
                                return View();
                            }

                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("file", "File is not valid and acceptable or size is greater than 10Mb");
                    return View();
                }
            }
            return View();
        }



        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult TaskCreation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TaskCreation (ScheduleTaskModel model)
        {
            int result = DateTime.Compare(model.Date, DateTime.Now.Date);

            if(result < 0)
            {
                TempData["Error"] = "Date Is Less Then Current Date";
                return View();
            }

            var teacher = await _userManager.GetUserAsync(HttpContext.User);
            var task = new ScheduleTaskModel { Date = model.Date, Description = model.Description, TaskName = model.TaskName, TeacherId = teacher.Id };

            _db.Add(task);
            await _db.SaveChangesAsync();

            /*if (_db.ScheduleTask.Find(task) != null)
            {
                TempData["Success"] = "Successfully Added Task";
                return View();
            }
            TempData["Error"] = "Error Occured While Creating Task";*/

            return View();
        }
    }
}
